using System.Collections;
using DB;
using DB.Data;
using Models;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Button join;
    public Button register;
    public AuthController authController;
    public PlayerBag player;
    public PlayerMovement playerMovement;
    public GameObject authUI;
    public GameObject transitionToGame;
    public TMP_Text loadingText;
    public TMP_Text errorText;
    
    public TMP_InputField email;
    public TMP_InputField nickname;
    public TMP_InputField password;

    private string tempNickname;
    
    private void Awake()
    {
        authController.Database = new Database();
        playerMovement.SetInMenu(true);
    }

    private void Start()
    {
        AudioManager.Instance.Play("Lobby");
        join.onClick.AddListener(StartJoin);
        register.onClick.AddListener(Register);
    }
    private void StartJoin()
    {
        AudioManager.Instance.Play("ClickUI");
        var user = authController.Login(email.text, password.text);
        StartCoroutine(StepLogin(user));
    }

    private void Register()
    {
        var onSucess = authController.Register(email.text, password.text, nickname.text);
        if (onSucess.Item1)
        {
            Logger.Log($"Bem-vindo, {nickname.text}! ID: {onSucess.Item2}");
            var randomPokemonId = Random.Range(1, 151);
            StartCoroutine(GiveRandomSpliceMon(onSucess.Item3, randomPokemonId));
            StartCoroutine(TransitionToGame());
        }

        if(!onSucess.Item1) StartCoroutine(ShowError(onSucess.Item2));
    }
    private IEnumerator GiveRandomSpliceMon(int userId, int pokeId)
    {
        yield return ApiManager.GetPokeData(pokeId, pokeData =>
        {
            var go = new GameObject($"Splicemon_{pokeData.NameSpliceMoon}");
            go.transform.SetParent(player.transform);

            var splicemon = go.AddComponent<SpliceMon>();
            splicemon.level = 2;
            splicemon.Initialize(pokeData);
            
            var data = new SplicemonData
            {
                Name = splicemon.nameSpliceMon,
                Level = splicemon.level,
                Nature = splicemon.nature,
                Experience = splicemon.experience,
                ExperienceMax = splicemon.experienceMax,
                BaseExperience = splicemon.baseExperience,
                FrontSprite = splicemon.frontSprite,
                BackSprite = splicemon.backSprite,
                CrySound = splicemon.crySound,
                GrowthRate = splicemon.growthRate.ToString(),
                UserId = userId,
                
                IvHp = splicemon.stats.hpStats.iv,
                IvAttack = splicemon.stats.attackStats.iv,
                IvSpAttack = splicemon.stats.specialAttackStats.iv,
                IvSpDefense = splicemon.stats.specialDefenseStats.iv,
                IvDefense = splicemon.stats.defenseStats.iv,
                IvSpeed = splicemon.stats.speedStats.iv,
                
                Hp = splicemon.stats.hpStats.currentStat,
                Speed = splicemon.stats.speedStats.currentStat,
                Attack = splicemon.stats.attackStats.currentStat,
                Defense = splicemon.stats.defenseStats.currentStat,
                SpAttack = splicemon.stats.specialAttackStats.currentStat,
                SpDefense = splicemon.stats.specialDefenseStats.currentStat,
                
                EffortHp = splicemon.stats.hpStats.effort,
                EffortSpeed = splicemon.stats.speedStats.effort,
                EffortAttack = splicemon.stats.attackStats.effort,
                EffortDefense = splicemon.stats.defenseStats.effort,
                EffortSpAttack = splicemon.stats.specialAttackStats.effort,
                EffortSpDefense = splicemon.stats.specialDefenseStats.effort,
                
                MovesJson = JsonUtility.ToJson(new MoveListWrapper { moves = splicemon.stats.movesAttack }),
                PossibleMovesJson = JsonUtility.ToJson(new StringListWrapper { items = splicemon.stats.possiblesMoveAttack }),
            };
            player.currentSplicemon = splicemon;
            authController.Database.SaveSplicemonForUser(data, userId);
            StartCoroutine(TransitionToGame());
        });
    }

    private IEnumerator StepLogin(UserData user)
    {
        if (user != null)
        {
            tempNickname = user.Nickname;
            Logger.Log($"Bem-vindo de volta, {user.Nickname}! ID: {user.Id}");

            var splicemonDataList = authController.Database.GetSplicemonsByUser(user.Id);

            player.splicemons.Clear();

            for (int i = 0; i < splicemonDataList.Count; i++)
            {
                var data = splicemonDataList[i];
                Logger.Log($"Carregando Splicemon {i + 1}/{splicemonDataList.Count}: {data.Name}");

                var go = new GameObject($"Splicemon_{data.Name}");
                go.transform.SetParent(player.transform);

                var splicemon = go.AddComponent<SpliceMon>();
                splicemon.InitializeFromData(data);

                if (i == 0)
                    player.currentSplicemon = splicemon;
                else
                {
                    player.splicemons.Add(splicemon);
                }

                yield return null;
            }

            player.bagInitialized = true;

            yield return StartCoroutine(TransitionToGame());
        }
        else
        {
            yield return StartCoroutine(ShowError("Email ou senha incorretos."));
        }
    }
    private IEnumerator TransitionToGame()
    {
        authUI.SetActive(false);
        transitionToGame.SetActive(true);

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        yield return StartCoroutine(ShowLoading("Carregando mundo", 1.5f));
        
        yield return StartCoroutine(ShowLoading("Carregando Splicemons", 1.8f));
        player.currentSplicemon.LoadMoves();
        playerMovement.SetNickname(tempNickname);
        yield return StartCoroutine(ShowLoading("Fazendo fusões", 2f));
        
        yield return new WaitForSeconds(0.5f);

        transitionToGame.SetActive(false);
        playerMovement.SetInMenu(false);
        AudioManager.Instance.Play("Game");
    }

    private IEnumerator ShowLoading(string baseText, float duration)
    {
        var elapsed = 0f;
        var dotCount = 0;

        while (elapsed < duration)
        {
            dotCount = (dotCount + 1) % 4;
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.3f);
            elapsed += 0.3f;
        }
    }

    private IEnumerator ShowError(string message, float duration = 3f)
    {
        errorText.text = message;

        var color = errorText.color;
        color.a = 1f;
        errorText.color = color;

        var elapsed = 0f;
        errorText.gameObject.SetActive(true);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            color.a = alpha;
            errorText.color = color;

            yield return null;
        }

        color.a = 0f;
        errorText.color = color;
        errorText.gameObject.SetActive(false);
    }
    public void Redraw(RectTransform contentTransform)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
    }
}
