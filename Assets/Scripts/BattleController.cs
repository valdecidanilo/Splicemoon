using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    public BattleUIManager uIManager;
    
    [FormerlySerializedAs("playerSplicemons")] public PlayerBagSplicemons playerBagSplicemons;
    [SerializeField] private SpliceMon opponentPokeData;
    public PlayerMovement playerMovement;
    public PlayerUIManager playerUIManager;
    public AudioSource battleSound;
    public AudioClip hitClip;
    public AudioClip victoryClip;
    
    private static readonly int StartAnimation = Animator.StringToHash("Start");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int End = Animator.StringToHash("End");
    private bool deathOpponent;
    private bool deathPlayer;
    private void OnEnable()
    {
        uIManager.OnAttackSlot1 += AttackSlot1;
        uIManager.OnAttackSlot2 += AttackSlot2;
        uIManager.OnAttackSlot3 += AttackSlot3;
        uIManager.OnAttackSlot4 += AttackSlot4;
        uIManager.OnCallTryRun += CallTryRun;
        uIManager.opponentInfo.healthBar.OnDeath += OpponentDeath;
        uIManager.playerInfo.healthBar.OnDeath += PlayerDeath;
    }

    private void OpponentDeath() => deathOpponent = true;
    private void PlayerDeath() => deathPlayer = false;
    public void StartBattle()
    {
        
        StartCoroutine(StartBattleStep());
    }

    private void AttackSlot1(MoveDetails move, int id)
    {
        StartCoroutine(PerformPlayerMove(move, id));
    }

    private void AttackSlot2(MoveDetails move, int id)
    {
        StartCoroutine(PerformPlayerMove(move, id));
    }

    private void AttackSlot3(MoveDetails move, int id)
    {
        StartCoroutine(PerformPlayerMove(move, id));
    }

    private void AttackSlot4(MoveDetails move, int id)
    {
        StartCoroutine(PerformPlayerMove(move, id));
    }

    private void CallTryRun()
    {
        StartCoroutine(TryRun());
    }
    private void GetOpponentPokeData(PokeData data)
    {
        uIManager.opponentInfo.SetName(data.NameSpliceMoon);
        var currentLevel = Random.Range(1, 6);
        uIManager.opponentInfo.SetLevel(currentLevel);
        var isFemale = false;
        if (data.sprites.frontFemale != null) isFemale = Random.Range(0,100) <= 40;
        Logger.Log(isFemale ? data.sprites.backFemale : data.sprites.backDefault);
        StartCoroutine(ApiManager.GetSprite(isFemale ? data.sprites.frontFemale : data.sprites.frontDefault, callback =>
        {
            uIManager.opponentInfo.SetSprite(callback);
        }));
        uIManager.opponentInfo.SetGender(isFemale);
        var child = new GameObject(data.NameSpliceMoon);
        child.transform.SetParent(gameObject.transform);
        opponentPokeData = child.AddComponent<SpliceMon>();
        opponentPokeData.Initialize(data, isFemale);
        opponentPokeData.level = currentLevel;
        var fourfirst = opponentPokeData.stats.possiblesMoveAttack.Take(4).ToList();
        StartCoroutine(ApiManager.GetMoveDetails(fourfirst, moveAttackList =>
        {
            for (var i = 0; i < 4; i++)
            {
                moveAttackList[i].ppCurrent = moveAttackList[i].ppMax;
            }
            opponentPokeData.stats.movesAttack = new List<MoveDetails>(moveAttackList);
        }));
        uIManager.opponentInfo.healthBar.SetHealth(opponentPokeData.stats.hpStats.currentStat);
        uIManager.opponentInfo.healthBar.SetMaxHealth(opponentPokeData.stats.hpStats.baseStat);
    }
    private IEnumerator StartBattleStep()
    {
        uIManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        uIManager.UpdatePlayer();
        if(!playerBagSplicemons.bagInitialized) yield break;
        Logger.Log("Getting Opponent Poke Data");
        var getOpponent = ApiManager.GetPokeData(
            Mathf.FloorToInt(Random.Range(1, ApiManager.ValueMax)), GetOpponentPokeData
            );
        yield return getOpponent;
        Logger.Log("Starting Battle");
        uIManager.animatorCallGround.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.5f);
        uIManager.animatorBoardOpponent.SetTrigger(StartAnimation);
        yield return ApiManager.GetSound(opponentPokeData.crySound, callback => { battleSound.PlayOneShot(callback); });
        yield return uIManager.Dialogue($"{opponentPokeData.nameSpliceMon.ToUpper()} Selvagem Apareceu!", 200f, waitForInput: true);
        yield return new WaitForSeconds(1f);
        yield return uIManager.Dialogue($"Vai! {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()}!", 200f, waitForInput: false);
        yield return new WaitForSeconds(.5f);
        uIManager.animatorCallGround.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.06f);
        uIManager.animatorBoardPlayer.SetTrigger(StartAnimation);
        uIManager.userAttackInterface.gameObject.SetActive(false);
        uIManager.userSelectInterface.gameObject.SetActive(true);
        yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        uIManager.firstTime = false;
        uIManager.currentInterface = BattleUIManager.CurrentInterface.UserSelect;
    }
    
    private IEnumerator PerformPlayerMove(MoveDetails moveAttack, int id)
    {
        
        uIManager.inBattle = true;
        yield return uIManager.Dialogue(
            $"{playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} usou! \n{playerBagSplicemons.currentSplicemon.stats.movesAttack[id].nameMove.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
        yield return new WaitForSeconds(0.5f);
        uIManager.playerInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Damage);
        playerBagSplicemons.currentSplicemon.stats.movesAttack[id].ppCurrent--;
        uIManager.UpdateTextSelectAttack(id);
        var damage = Maths.CalculateDamage(moveAttack.ppCurrent, 
            playerBagSplicemons.currentSplicemon.level, 
            playerBagSplicemons.currentSplicemon.stats.attackStats.currentStat, 
            opponentPokeData.stats.defenseStats.currentStat);
        uIManager.opponentInfo.healthBar.ChangeHealth(-damage);
        battleSound.PlayOneShot(hitClip);
        if(!deathOpponent)
            yield return PerformOpponentMove();
        else
        {
            uIManager.sourceBattleSound.mute = true; 
           
            yield return ApiManager.GetSound(opponentPokeData.crySound, callback =>
            {
                battleSound.PlayOneShot(callback);
            });
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Opponent Death");
            battleSound.PlayOneShot(hitClip);
            yield return new WaitForSeconds(0.4f);
            uIManager.opponentInfo.animationSplicemon.SetTrigger(Death);
            yield return new WaitForSeconds(0.4f);
            battleSound.PlayOneShot(victoryClip);
            yield return uIManager.Dialogue($"{opponentPokeData.nameSpliceMon.ToUpper()} Selvagem Desmaiou!",
                200f, waitForInput: true);
            var exp = Maths.GainExperience(opponentPokeData.level, playerBagSplicemons.currentSplicemon.baseExperience,
                playerBagSplicemons.currentSplicemon.experience, playerBagSplicemons.currentSplicemon.experienceMax, 
                playerBagSplicemons.currentSplicemon.stats);
            uIManager.playerInfo.expBar.SetExp(exp);
            yield return uIManager.Dialogue(
                $"{playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} Ganhou \n {exp} pontos de EXP!",
                200f, waitForInput: true);
            yield return uIManager.Dialogue("", waitForInput: false);
            uIManager.ShowMessage();
            //se subiu nivel mostrar no texto.
            // se subiu mostrar janelinha do oque foi upado 
            //depois disso transição para o jogo
            yield return playerUIManager.TransitionEndBattle(onComplete =>
            {
                if (!onComplete) return;
                uIManager.ResetBattleUI();
                Destroy(opponentPokeData.gameObject);
            });
            yield return null;
            uIManager.gameObject.SetActive(false);
            playerMovement.SetIsBattle(false);
            deathOpponent = false;
            deathPlayer = false;
        }
    }

    private IEnumerator PerformOpponentMove()
    {
        yield return new WaitForSeconds(1f);
    
        yield return uIManager.Dialogue(
            $"{opponentPokeData.nameSpliceMon} usou!\n{opponentPokeData.stats.movesAttack[0].nameMove.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
    
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
    
        uIManager.playerInfo.animationSplicemon.SetTrigger(Damage);
        yield return new WaitForSeconds(0.2f);
        MoveDetails attack = new();
        var damage = Maths.CalculateDamage(attack.ppCurrent, 
            opponentPokeData.level, 
            opponentPokeData.stats.attackStats.currentStat, 
            playerBagSplicemons.currentSplicemon.stats.defenseStats.currentStat);
        uIManager.playerInfo.healthBar.ChangeHealth(-damage);
        battleSound.PlayOneShot(hitClip);
    
        if (deathPlayer)
        {
            yield return new WaitForSeconds(0.5f);
            uIManager.playerInfo.animationSplicemon.SetTrigger(Death);
            battleSound.PlayOneShot(hitClip);
        
            yield return uIManager.Dialogue($"{playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} foi derrotado!", waitForInput: true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            uIManager.userAttackInterface.gameObject.SetActive(false);
            uIManager.userSelectInterface.gameObject.SetActive(true);
            uIManager.currentInterface = BattleUIManager.CurrentInterface.UserSelect;
            yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
        uIManager.sourceBattleSound.mute = false; 
        uIManager.sourceBattleSound.Stop();
        uIManager.inBattle = false;
    }

    private IEnumerator TryRun()
    {
        if (uIManager.isTryRunning) yield return null;
        uIManager.isTryRunning = true;
        var percent = Random.Range(0, 100);
        var success = percent > 50;
        
        if(success)
        {
            yield return uIManager.Dialogue($"Escapou em segurança");
            uIManager.ShowMessage();
            yield return playerUIManager.TransitionEndBattle(onComplete =>
            {
                if (!onComplete) return;
                uIManager.ResetBattleUI();
                Destroy(opponentPokeData.gameObject);
            });
            yield return null;
            uIManager.gameObject.SetActive(false);
            playerMovement.SetIsBattle(false);
            uIManager.isTryRunning = false;
            yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
        else
        {
            yield return uIManager.Dialogue($"Não deu para fugir");
            uIManager.isTryRunning = false;
            yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
            
    }
}
