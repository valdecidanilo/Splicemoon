using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using Player;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    public BattleUI uI;
    
    [FormerlySerializedAs("playerSplicemons")] public PlayerBag playerBag;
    private SpliceMon opponentPokeData;
    public PlayerMovement playerMovement;
    public PlayerUI playerUI;
    
    private static readonly int StartAnimation = Animator.StringToHash("Start");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int End = Animator.StringToHash("End");
    private bool deathOpponent;
    private bool deathPlayer;
    private void OnEnable()
    {
        uI.OnAttackSlot1 += AttackSlot1;
        uI.OnAttackSlot2 += AttackSlot2;
        uI.OnAttackSlot3 += AttackSlot3;
        uI.OnAttackSlot4 += AttackSlot4;
        uI.OnCallTryRun += CallTryRun;
        uI.opponentInfo.healthBar.OnDeath += OpponentDeath;
        uI.playerInfo.healthBar.OnDeath += PlayerDeath;
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
        uI.opponentInfo.SetName(data.NameSpliceMoon);
        var currentLevel = Random.Range(1, 6);
        uI.opponentInfo.SetLevel(currentLevel);
        var isFemale = false;
        if (data.sprites.frontFemale != null) isFemale = Random.Range(0,100) <= 40;
        Logger.Log(isFemale ? data.sprites.backFemale : data.sprites.backDefault);
        StartCoroutine(ApiManager.GetSprite(isFemale ? data.sprites.frontFemale : data.sprites.frontDefault, callback =>
        {
            uI.opponentInfo.SetSprite(callback);
        }));
        uI.opponentInfo.SetGender(isFemale);
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
        uI.opponentInfo.healthBar.SetHealth(opponentPokeData.stats.hpStats.currentStat);
        uI.opponentInfo.healthBar.SetMaxHealth(opponentPokeData.stats.hpStats.baseStat);
    }
    private IEnumerator StartBattleStep()
    {
        uI.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        uI.UpdatePlayer();
        if(!playerBag.bagInitialized) yield break;
        Logger.Log("Getting Opponent Poke Data");
        var getOpponent = ApiManager.GetPokeData(
            Mathf.FloorToInt(Random.Range(1, ApiManager.ValueMax)), GetOpponentPokeData
            );
        yield return getOpponent;
        Logger.Log("Starting Battle");
        uI.animatorCallGround.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.5f);
        uI.animatorBoardOpponent.SetTrigger(StartAnimation);
        yield return ApiManager.GetSound(opponentPokeData.crySound, callback =>
        {
            AudioManager.Instance.PlayAudioClipExtern(callback);
        });
        yield return uI.Dialogue($"{opponentPokeData.nameSpliceMon.ToUpper()} Selvagem Apareceu!", 200f, waitForInput: true);
        yield return new WaitForSeconds(1f);
        yield return uI.Dialogue($"Vai! {playerBag.currentSplicemon.nameSpliceMon.ToUpper()}!", 200f, waitForInput: false);
        yield return new WaitForSeconds(.5f);
        uI.animatorCallGround.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.06f);
        uI.animatorBoardPlayer.SetTrigger(StartAnimation);
        uI.userAttackInterface.gameObject.SetActive(false);
        uI.userSelectInterface.gameObject.SetActive(true);
        yield return uI.Dialogue($"O que o {playerBag.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        uI.firstTime = false;
        uI.currentInterface = BattleUI.CurrentInterface.UserSelect;
    }
    
    private IEnumerator PerformPlayerMove(MoveDetails moveAttack, int id)
    {
        
        uI.inBattle = true;
        yield return uI.Dialogue(
            $"{playerBag.currentSplicemon.nameSpliceMon.ToUpper()} usou! \n{playerBag.currentSplicemon.stats.movesAttack[id].nameMove.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
        yield return new WaitForSeconds(0.5f);
        uI.playerInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
        uI.opponentInfo.animationSplicemon.SetTrigger(Damage);
        playerBag.currentSplicemon.stats.movesAttack[id].ppCurrent--;
        uI.UpdateTextSelectAttack(id);
        var damage = Maths.CalculateDamage(moveAttack.ppCurrent, 
            playerBag.currentSplicemon.level, 
            playerBag.currentSplicemon.stats.attackStats.currentStat, 
            opponentPokeData.stats.defenseStats.currentStat);
        uI.opponentInfo.healthBar.ChangeHealth(-damage);
        AudioManager.Instance.Play("Hit");
        if(!deathOpponent)
            yield return PerformOpponentMove();
        else
        {
            uI.sourceBattleSound.mute = true; 
           
            yield return ApiManager.GetSound(opponentPokeData.crySound, callback =>
            {
                AudioManager.Instance.PlayAudioClipExtern(callback);
            });
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Opponent Death");
            AudioManager.Instance.Play("Hit");
            yield return new WaitForSeconds(0.4f);
            uI.opponentInfo.animationSplicemon.SetTrigger(Death);
            yield return new WaitForSeconds(0.4f);
            AudioManager.Instance.Play("Victory");
            yield return uI.Dialogue($"{opponentPokeData.nameSpliceMon.ToUpper()} Selvagem Desmaiou!",
                200f, waitForInput: true);
            var exp = Maths.GainExperience(opponentPokeData.level, playerBag.currentSplicemon.baseExperience,
                playerBag.currentSplicemon.experience, playerBag.currentSplicemon.experienceMax, 
                playerBag.currentSplicemon.stats);
            uI.playerInfo.expBar.SetExp(exp);
            yield return uI.Dialogue(
                $"{playerBag.currentSplicemon.nameSpliceMon.ToUpper()} Ganhou \n {exp} pontos de EXP!",
                200f, waitForInput: true);
            yield return uI.Dialogue("", waitForInput: false);
            uI.ShowMessage();
            //se subiu nivel mostrar no texto.
            // se subiu mostrar janelinha do oque foi upado 
            //depois disso transição para o jogo
            yield return playerUI.TransitionEndBattle(onComplete =>
            {
                if (!onComplete) return;
                uI.ResetBattleUI();
                Destroy(opponentPokeData.gameObject);
            });
            yield return null;
            uI.gameObject.SetActive(false);
            playerMovement.SetIsBattle(false);
            deathOpponent = false;
            deathPlayer = false;
        }
    }

    private IEnumerator PerformOpponentMove()
    {
        yield return new WaitForSeconds(1f);
    
        yield return uI.Dialogue(
            $"{opponentPokeData.nameSpliceMon} usou!\n{opponentPokeData.stats.movesAttack[0].NameMove.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
    
        uI.opponentInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
    
        uI.playerInfo.animationSplicemon.SetTrigger(Damage);
        yield return new WaitForSeconds(0.2f);
        MoveDetails attack = new();
        var damage = Maths.CalculateDamage(attack.ppCurrent, 
            opponentPokeData.level, 
            opponentPokeData.stats.attackStats.currentStat, 
            playerBag.currentSplicemon.stats.defenseStats.currentStat);
        uI.playerInfo.healthBar.ChangeHealth(-damage);
        AudioManager.Instance.Play("Hit");
    
        if (deathPlayer)
        {
            yield return new WaitForSeconds(0.5f);
            uI.playerInfo.animationSplicemon.SetTrigger(Death);
            AudioManager.Instance.Play("Hit");
        
            yield return uI.Dialogue($"{playerBag.currentSplicemon.nameSpliceMon.ToUpper()} foi derrotado!", waitForInput: true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            uI.userAttackInterface.gameObject.SetActive(false);
            uI.userSelectInterface.gameObject.SetActive(true);
            uI.currentInterface = BattleUI.CurrentInterface.UserSelect;
            yield return uI.Dialogue($"O que o {playerBag.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
        uI.sourceBattleSound.mute = false; 
        uI.sourceBattleSound.Stop();
        uI.inBattle = false;
    }

    private IEnumerator TryRun()
    {
        if (uI.isTryRunning) yield return null;
        uI.isTryRunning = true;
        var percent = Random.Range(0, 100);
        var success = percent > 50;
        
        if(success)
        {
            yield return uI.Dialogue($"Escapou em segurança");
            uI.ShowMessage();
            yield return playerUI.TransitionEndBattle(onComplete =>
            {
                if (!onComplete) return;
                uI.ResetBattleUI();
                Destroy(opponentPokeData.gameObject);
            });
            yield return null;
            uI.gameObject.SetActive(false);
            playerMovement.SetIsBattle(false);
            uI.isTryRunning = false;
            yield return uI.Dialogue($"O que o {playerBag.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
        else
        {
            yield return uI.Dialogue($"Não deu para fugir");
            uI.isTryRunning = false;
            yield return uI.Dialogue($"O que o {playerBag.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
            
    }
}
