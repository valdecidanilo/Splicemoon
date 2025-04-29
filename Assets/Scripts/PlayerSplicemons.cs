using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Logger = LenixSO.Logger.Logger;

public class PlayerSplicemons : MonoBehaviour
{
    public SpliceMon currentSplicemon;
    public List<SpliceMon> splicemons = new ();
    public bool bagInitialized = false;
    private void Awake()
    {
        StartCoroutine(ApiManager.GetPokeData(1, SetCurrentSplicemon));
    }
    private void SetCurrentSplicemon(PokeData pokeData)
    {
        var child = new GameObject(pokeData.NameSpliceMoon);
        child.transform.SetParent(gameObject.transform);
        currentSplicemon = child.AddComponent<SpliceMon>();
        currentSplicemon.Initialize(pokeData);
        bagInitialized = true;
        StartCoroutine(ApiManager.GetMoveDetails(currentSplicemon.moveAttack, callback =>
        {
            for (var i = 0; i < 4; i++)
            {
                Logger.Log($"move : {callback[i].nameMove}");
                currentSplicemon.itensMove.Add(callback[i]);
            }
        }));
    }
}
