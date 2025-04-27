using System;
using System.Collections;
using System.Collections.Generic;
using LenixSO.Logger;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class ApiManager : MonoBehaviour
{
    private const string BaseURL = "https://pokeapi.co/api/v2/";
    private const string Fusion = "https://ifd-spaces.sfo2.digitaloceanspaces.com/sprites/";

    public const uint ValueMax = 1025;

    public SpliceMon current;
    public InventoryObject playerInventory;

    private void Start()
    {
        var pokeID = (int) Random.Range(0, ValueMax);
        StartCoroutine(GetDataWaiter(pokeID));
    }

    public IEnumerator GetDataWaiter(int pokeID = 1)
    {
        PokeData splicemon = null;
        Sprite frontSprite = null;
        Sprite backSprite = null;
        var haveFemale = false;
        var isFemale = false;
        yield return GetPokeData(pokeID,
            pokeData =>
            {
                splicemon = pokeData;
                haveFemale = pokeData.sprites.frontFemale != null ? true : false;
                if (haveFemale)
                    if (Random.Range(0, 100) < 40)
                        isFemale = true;
            });
        yield return GetSprite(isFemale ? splicemon.sprites.frontFemale : splicemon.sprites.frontDefault, spr =>
        {
            frontSprite = spr;
        });
        yield return GetSprite(isFemale ? splicemon.sprites.backFemale : splicemon.sprites.backDefault, spr =>
        {
            backSprite = spr;
        });
        current = ScriptableObject.CreateInstance<SpliceMon>();
        current.UpdateSliceMon(splicemon, isFemale);
        Logger.LogWarning("Atualizou o Scriptable", LogFlags.GET);
        playerInventory.AddItem(current);
    }

    public static IEnumerator GetPokeData(int pokemonId, Action<PokeData> callback)
    {
        Logger.Log($"Getting poke data => { BaseURL }pokemon/{ pokemonId }");
        var uwr = UnityWebRequest.Get( $"{ BaseURL }pokemon/{ pokemonId }");
        yield return uwr.SendWebRequest();
        if(uwr.result == UnityWebRequest.Result.Success)
        {
            //Logger.Log($"json => {uwr.downloadHandler.text}", LogFlags.GET);
            var pokeData = JsonConvert.DeserializeObject<PokeData>(uwr.downloadHandler.text);
            Logger.Log(pokeData.NameSpliceMoon, LogFlags.GET);
            callback.Invoke(pokeData);
        }
        else
        {
            Logger.LogError("Error Ao Pegar pokemon", LogFlags.ERROR);
        }
    }
    
    public static IEnumerator GetSprite(string url, Action<Sprite> callback)
    {
        var uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();
        
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(uwr);
            texture.filterMode = FilterMode.Point;
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0f)
            );
            callback?.Invoke(sprite);
        }
        else
        {
            Logger.LogError($"Failed to get image: {uwr.error}");
        }
    }
    private IEnumerator GetFusionSprite(uint head, uint body)
    {
        var endpoint = "";
        var hasImage = false;

        yield return StartCoroutine(GetImage("custom/" + $"{head}.{body}.png", result =>
        {
            hasImage = result;
            endpoint = result ? "custom/" : "generated/";
        }));

        var fullUrl = $"{Fusion}{endpoint}{head}.{body}.png";

        using var uwr = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(uwr);
            texture.filterMode = FilterMode.Point;
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0f)
            );
        }
        else
        {
            Logger.LogError($"Failed to get image: {uwr.error}");
        }
    }

    public static IEnumerator GetMoveDetails(List<string> urls, Action<List<MoveDetails>> callback)
    {
        var moveDetailsList = new List<MoveDetails>();
        for (var i = 0; i < 4; i++)
        {
            if (string.IsNullOrEmpty(urls[i]))
                continue;
            
            var uwr = UnityWebRequest.Get(urls[i]);
            yield return uwr.SendWebRequest();
        
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var moveDetails = JsonConvert.DeserializeObject<MoveDetails>(uwr.downloadHandler.text);
                moveDetailsList.Add(moveDetails);
            }
            else
            {
                Logger.LogError($"Error ao pegar detalhes do movimento: {urls}", LogFlags.ERROR);
            }
        }
    
        callback.Invoke(moveDetailsList);
    }
    public static IEnumerator GetSound(string url, Action<AudioClip> callback)
    {
        var uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            var clip = DownloadHandlerAudioClip.GetContent(uwr);
            callback?.Invoke(clip);
        }
        else
        {
            Logger.LogError($"Failed to get sound: {uwr.error}");
        }
    }
    private static IEnumerator GetImage(string relativeUrl, Action<bool> callback)
    {
        var fullUrl = $"{Fusion}{relativeUrl}";
        using var uwr = UnityWebRequest.Head(fullUrl);
        yield return uwr.SendWebRequest();
        callback.Invoke(uwr.result == UnityWebRequest.Result.Success);
    }
}
