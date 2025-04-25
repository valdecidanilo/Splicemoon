using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ApiManager : MonoBehaviour
{
    private const string BaseURL = "https://pokeapi.co/api/v2/";
    private const string BaseSpriteURL = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";
    private const string Fusion = "https://ifd-spaces.sfo2.digitaloceanspaces.com/sprites/";
    
    private const uint ValueMax = 1018;

    public Image test;
    public Image test2;

    private void Start()
    {
        Debug.Log("Initializing");
        StartCoroutine(GetFusionSprite(1, 3));
        StartCoroutine(GetSprite(6, true));
    }

    private IEnumerator GetSprite(uint spriteID, bool isBack = false)
    {
        var uwr = UnityWebRequestTexture.GetTexture(BaseSpriteURL + (isBack ? "back/" : "") + spriteID + ".png");
        yield return uwr.SendWebRequest();
        
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(uwr);
            test2.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0f)
            );
            Debug.Log("Get Image succeeded");
        }
        else
        {
            Debug.LogError($"Failed to get image: {uwr.error}");
        }
    }
    private IEnumerator GetFusionSprite(uint head, uint body)
    {
        Debug.Log("Getting Images...");
        var endpoint = "";
        var hasImage = false;

        yield return StartCoroutine(GetImage("custom/" + $"{head}.{body}.png", result =>
        {
            hasImage = result;
            endpoint = result ? "custom/" : "generated/";
        }));

        var fullUrl = $"{Fusion}{endpoint}{head}.{body}.png";
        Debug.Log($"Endpoint is => {fullUrl}");

        using var uwr = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(uwr);
            test.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0f)
            );
            Debug.Log("Get Image succeeded");
        }
        else
        {
            Debug.LogError($"Failed to get image: {uwr.error}");
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
