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
    //private const string BaseSpriteURL = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";
    private const string Fusion = "https://ifd-spaces.sfo2.digitaloceanspaces.com/sprites/";
    
    public static uint valueMax = 1025;

    private void Start()
    {
        //Debug.Log("Initializing");
        //StartCoroutine(GetFusionSprite(1, 3));
        //StartCoroutine(GetSprite(6, true));
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
