using System.Collections;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    public BattleUIManager uIManager;
    public PokeData opponentPokeData;
    public PokeData playerPokeData;
    
    public AudioSource battleSound;
    public AudioClip hitClip;
    
    private static readonly int StartAnimation = Animator.StringToHash("Start");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int Death = Animator.StringToHash("Death");
    private bool death;
    private void OnEnable()
    {
        uIManager.OnAttackSlot1 += AttackSlot1;
        uIManager.OnAttackSlot2 += AttackSlot2;
        uIManager.OnAttackSlot3 += AttackSlot3;
        uIManager.OnAttackSlot4 += AttackSlot4;

        uIManager.opponentInfo.healthBar.OnDeath += OpponentDeath;
    }
    private void OpponentDeath() => death = true;
    private void Start()
    {
        StartCoroutine(StartBattle());
    }

    private void AttackSlot1()
    {
        StartCoroutine(PerformPlayerMove());
    }

    private void AttackSlot2()
    {
        
    }

    private void AttackSlot3()
    {
        
    }

    private void AttackSlot4()
    {
        
    }
    private void GetOpponentPokeData(PokeData data)
    {
        uIManager.opponentInfo.SetName(data.NameSpliceMoon);
        uIManager.opponentInfo.SetLevel(1);
        
        var isFemale = false;
        if (data.sprites.frontFemale != null) isFemale = Random.Range(0,100) <= 40;
        Logger.Log(isFemale ? data.sprites.backFemale : data.sprites.backDefault);
        StartCoroutine(ApiManager.GetSprite(isFemale ? data.sprites.frontFemale : data.sprites.frontDefault, callback =>
        {
            uIManager.opponentInfo.SetSprite(callback);
        }));
        uIManager.opponentInfo.SetGender(isFemale);
        opponentPokeData = data;
    }

    private void GetPlayerPokeData(PokeData data)
    {
        playerPokeData = data;
    }

    private IEnumerator StartBattle()
    {
        Logger.Log("Getting Opponent Poke Data");
        var getOpponent = ApiManager.GetPokeData(Mathf.FloorToInt(Random.Range(0, ApiManager.valueMax)), GetOpponentPokeData);
        yield return getOpponent;
        Logger.Log("Getting Player Poke Data");
        var getplayer = ApiManager.GetPokeData(Mathf.FloorToInt(Random.Range(0, ApiManager.valueMax)), GetPlayerPokeData);
        yield return getplayer;
        Logger.Log("Starting Battle");
        uIManager.animatorTransition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.5f);
        yield return ApiManager.GetSound(opponentPokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
    }
    
    private IEnumerator PerformPlayerMove()
    {
        uIManager.playerInfo.animationSplicemon.SetTrigger(Attack);
        
        yield return new WaitForSeconds(0.3f);
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Damage);
        //power do player no change health (MoveDetails)
        yield return new WaitForSeconds(0.3f);
        uIManager.opponentInfo.healthBar.ChangeHealth(Random.Range(-10, -20));
        battleSound.PlayOneShot(hitClip);
        if(!death)
        {
            yield return PerformOpponentMove();
        }
        else
        {
            yield return ApiManager.GetSound(opponentPokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Opponent Death");
            uIManager.opponentInfo.animationSplicemon.SetTrigger(Death);
            battleSound.PlayOneShot(hitClip);
        }

    }

    private IEnumerator PerformOpponentMove()
    {
        Logger.Log("Performing Opponent Move");
        yield return null;
    }
}
