using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

public class PlayerCurrentSplicemon : MonoBehaviour
{
    public int myFirstPoke = 1;
    public PokeData currentSplicemonData;
    public List<MoveDetails> listMoves = new ();

    public Action OnShowMoveTexts;
    public Action OnShowSpliceInfo;
    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        listMoves.Clear();
        yield return ApiManager.GetPokeData(myFirstPoke, poke => { currentSplicemonData = poke; });
        OnShowSpliceInfo?.Invoke();
        Logger.Log($"nome: {currentSplicemonData.NameSpliceMoon}");
        var urlList = currentSplicemonData.movesAttack.Select(move => move.move.url).ToList();
        Logger.Log("Getting Moves");
        yield return ApiManager.GetMoveDetails(urlList, m =>
        {
            foreach (var moveDetails in m)
            {
                listMoves.Add(moveDetails);
            }
            OnShowMoveTexts?.Invoke();
        });
    }
}
