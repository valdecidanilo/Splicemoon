using System.Collections.Generic;
using System.Linq;
using Inventory;
using Models;
using UnityEngine;

public class PlayerBagSplicemons : MonoBehaviour
{
    public SpliceMon currentSplicemon;
    public List<SpliceMon> splicemons = new ();
    public bool bagInitialized = false;
    
    [Header("Test")]
    public int myPokemonTest = 1;
    public int level = 1;
    private void Awake()
    {
        StartCoroutine(ApiManager.GetPokeData(myPokemonTest, SetCurrentSplicemon));
    }
    private void SetCurrentSplicemon(PokeData pokeData)
    {

        var child = new GameObject(pokeData.NameSpliceMoon);
        child.transform.SetParent(gameObject.transform);
        currentSplicemon = child.AddComponent<SpliceMon>();
        currentSplicemon.level = level;
        currentSplicemon.Initialize(pokeData);
        bagInitialized = true;
        var fourfirst = currentSplicemon.stats.possiblesMoveAttack.Take(4).ToList();
        StartCoroutine(ApiManager.GetMoveDetails(fourfirst, moveAttackList =>
        {
            
            for (var i = 0; i < 4; i++)
            {
                moveAttackList[i].ppCurrent = moveAttackList[i].ppMax;
            }
            currentSplicemon.stats.movesAttack = new List<MoveDetails>(moveAttackList);
        }));
    }
}
