using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(-50)] // Garante inicialização antecipada
public class AudioManager : MonoBehaviour
{
    private const int MAX_CONCURRENT_SFX = 10;
    private const float MIN_VOLUME_DB = -80f;

    public enum SoundCategory
    {
        Music,
        SFX,
        UI,
        Ambient
    }

    [System.Serializable]
    public class SoundConfig
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
        public int priority = 128;
        public SoundCategory category;
    }

    [Header("Core Configuration")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private int _audioSourcePoolSize = 15;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _musicGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    [SerializeField] private AudioMixerGroup _uiGroup;

    [Header("Sound Library")]
    [SerializeField] private List<SoundConfig> _soundLibrary = new();

    private Dictionary<string, SoundConfig> _soundRegistry;
    private Dictionary<SoundCategory, List<AudioSource>> _activeSources;
    private Queue<AudioSource> _sourcePool;
    private AudioSource _currentMusicSource;
    private Coroutine _musicFadeRoutine;

    #region Singleton Pattern
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        _soundRegistry = new();
        _activeSources = new();
        _sourcePool = new();

        // Setup category tracking
        foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
        {
            _activeSources[category] = new();
        }

        // Create audio source pool
        for (int i = 0; i < _audioSourcePoolSize; i++)
        {
            CreatePooledSource();
        }

        // Register all sounds
        foreach (var config in _soundLibrary)
        {
            if (_soundRegistry.ContainsKey(config.id))
            {
                Debug.LogWarning($"Duplicate sound ID detected: {config.id}");
                continue;
            }
            _soundRegistry[config.id] = config;
        }

        Debug.Log($"AudioManager initialized with {_soundRegistry.Count} sounds registered");
    }

    private AudioSource CreatePooledSource()
    {
        GameObject sourceObj = new("PooledAudioSource");
        sourceObj.transform.SetParent(transform);
        var source = sourceObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        _sourcePool.Enqueue(source);
        return source;
    }
    #endregion

    #region Playback Methods
    public void Play(string soundId, float volumeMultiplier = 1f)
    {
        if (!_soundRegistry.TryGetValue(soundId, out SoundConfig config))
        {
            Debug.LogWarning($"Sound not found: {soundId}");
            return;
        }

        switch (config.category)
        {
            case SoundCategory.Music:
                PlayMusic(soundId);
                break;
            default:
                PlaySoundEffect(config, volumeMultiplier);
                break;
        }
    }

    public void PlayAudioClipExtern(AudioClip clip, float volumeMultiplier = 1f)
    {
        var config = new SoundConfig
        {
            id = clip.name,
            clip = clip,
            volume = 1f,
            loop = false,
            priority = 128,
            category = SoundCategory.SFX
        };
        PlaySoundEffect(config, volumeMultiplier);
    }

    private void PlayMusic(string musicId)
    {
        if (!_soundRegistry.TryGetValue(musicId, out SoundConfig config))
            return;

        // Stop current music with fade out
        if (_currentMusicSource != null && _currentMusicSource.isPlaying)
        {
            if (_musicFadeRoutine != null)
                StopCoroutine(_musicFadeRoutine);
            
            StartCoroutine(FadeOutSource(_currentMusicSource, 0.5f, () => 
            {
                _currentMusicSource.Stop();
                StartNewMusic(config);
            }));
        }
        else
        {
            StartNewMusic(config);
        }
    }

    private void StartNewMusic(SoundConfig config)
    {
        var source = GetAvailableSource();
        SetupSource(source, config, _musicGroup);
        source.Play();
        _currentMusicSource = source;
        
        // Optional fade in
        StartCoroutine(FadeInSource(source, 1f, config.volume));
    }

    private void PlaySoundEffect(SoundConfig config, float volumeMultiplier)
    {
        // Limit concurrent SFX
        if (config.category == SoundCategory.SFX && 
            _activeSources[SoundCategory.SFX].Count >= MAX_CONCURRENT_SFX)
        {
            Debug.Log("Max concurrent SFX reached");
            return;
        }

        var source = GetAvailableSource();
        SetupSource(source, config, GetMixerGroup(config.category), volumeMultiplier);
        source.Play();
        _activeSources[config.category].Add(source);
        
        if (!config.loop)
        {
            StartCoroutine(ReturnSourceToPoolWhenFinished(source, config.category));
        }
    }

    private IEnumerator ReturnSourceToPoolWhenFinished(AudioSource source, SoundCategory category)
    {
        yield return new WaitWhile(() => source.isPlaying);
        ReturnSourceToPool(source, category);
    }
    #endregion

    #region Source Management
    private AudioSource GetAvailableSource()
    {
        if (_sourcePool.Count > 0)
        {
            return _sourcePool.Dequeue();
        }

        Debug.Log("AudioSource pool exhausted - creating new one");
        return CreatePooledSource();
    }

    private void ReturnSourceToPool(AudioSource source, SoundCategory category)
    {
        source.Stop();
        source.clip = null;
        _activeSources[category].Remove(source);
        _sourcePool.Enqueue(source);
    }

    private void SetupSource(AudioSource source, SoundConfig config, AudioMixerGroup group, 
                           float volumeMultiplier = 1f)
    {
        source.clip = config.clip;
        source.volume = config.volume * volumeMultiplier;
        source.loop = config.loop;
        source.priority = config.priority;
        source.outputAudioMixerGroup = group;
    }
    #endregion

    #region Volume Control
    public void SetMasterVolume(float linearVolume)
    {
        SetMixerVolume("MasterVolume", linearVolume);
    }

    public void SetMusicVolume(float linearVolume)
    {
        SetMixerVolume("MusicVolume", linearVolume);
    }

    public void SetSFXVolume(float linearVolume)
    {
        SetMixerVolume("SFXVolume", linearVolume);
    }

    public void SetUIVolume(float linearVolume)
    {
        SetMixerVolume("UIVolume", linearVolume);
    }

    private void SetMixerVolume(string parameterName, float linearVolume)
    {
        if (_mixer == null) return;
        
        float dB = linearVolume > 0.0001f ? 
                20f * Mathf.Log10(linearVolume) : 
                MIN_VOLUME_DB;
                
        _mixer.SetFloat(parameterName, dB);
    }
    #endregion

    #region Fade Effects
    private IEnumerator FadeOutSource(AudioSource source, float duration, System.Action onComplete = null)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        source.volume = 0f;
        onComplete?.Invoke();
    }

    private IEnumerator FadeInSource(AudioSource source, float duration, float targetVolume)
    {
        source.volume = 0f;
        float timer = 0f;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume;
    }
    #endregion

    #region Utility Methods
    private AudioMixerGroup GetMixerGroup(SoundCategory category)
    {
        return category switch
        {
            SoundCategory.Music => _musicGroup,
            SoundCategory.SFX => _sfxGroup,
            SoundCategory.UI => _uiGroup,
            _ => null
        };
    }

    public void StopAllSounds(bool includeMusic = false)
    {
        foreach (var category in _activeSources.Keys)
        {
            if (!includeMusic && category == SoundCategory.Music)
                continue;

            foreach (var source in _activeSources[category])
            {
                source.Stop();
                ReturnSourceToPool(source, category);
            }
        }
    }

    public void StopSound(string soundId)
    {
        if (!_soundRegistry.TryGetValue(soundId, out SoundConfig config))
            return;

        foreach (var source in _activeSources[config.category])
        {
            if (source.clip == config.clip && source.isPlaying)
            {
                source.Stop();
                ReturnSourceToPool(source, config.category);
                break;
            }
        }
    }

    public bool IsSoundPlaying(string soundId)
    {
        if (!_soundRegistry.TryGetValue(soundId, out SoundConfig config))
            return false;

        foreach (var source in _activeSources[config.category])
        {
            if (source.clip == config.clip && source.isPlaying)
                return true;
        }

        return false;
    }
    #endregion
}