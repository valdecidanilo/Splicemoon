using System;
using System.Collections;
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
    public AudioSource battleSound;
    public AudioClip hitClip;
    public AudioClip victoryClip;
    
    private static readonly int StartAnimation = Animator.StringToHash("Start");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int Death = Animator.StringToHash("Death");
    private bool deathOpponent;
    private bool deathPlayer;
    private void OnEnable()
    {
        uIManager.OnAttackSlot1 += AttackSlot1;
        uIManager.OnAttackSlot2 += AttackSlot2;
        uIManager.OnAttackSlot3 += AttackSlot3;
        uIManager.OnAttackSlot4 += AttackSlot4;

        uIManager.opponentInfo.healthBar.OnDeath += OpponentDeath;
        uIManager.playerInfo.healthBar.OnDeath += PlayerDeath;
    }

    private void OpponentDeath() => deathOpponent = true;
    private void PlayerDeath() => deathPlayer = false;
    public void StartBattle()
    {
        uIManager.canvasBattleUI.SetActive(true);
        StartCoroutine(StartBattleStep());
    }

    private void AttackSlot1(string url)
    {
        StartCoroutine(PerformPlayerMove(url));
    }

    private void AttackSlot2(string url)
    {
        StartCoroutine(PerformPlayerMove(url));
    }

    private void AttackSlot3(string url)
    {
        StartCoroutine(PerformPlayerMove(url));
    }

    private void AttackSlot4(string url)
    {
        StartCoroutine(PerformPlayerMove(url));
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
        uIManager.opponentInfo.healthBar.SetHealth(opponentPokeData.hpStats.currentStat);
        uIManager.opponentInfo.healthBar.SetMaxHealth(opponentPokeData.hpStats.baseStat);
    }
    private IEnumerator StartBattleStep()
    {
        
        yield return new WaitForSeconds(0.7f);
        Logger.Log("Getting... Player Poke Data");
        while (!playerBagSplicemons.bagInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
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
        yield return ApiManager.GetSound(opponentPokeData.PokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
        yield return uIManager.Dialogue($"{opponentPokeData.nameSpliceMon.ToUpper()} Selvagem Apareceu!", 200f, waitForInput: true);
        yield return new WaitForSeconds(1f);
        yield return uIManager.Dialogue($"Vai! {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()}!", 200f, waitForInput: false);
        yield return new WaitForSeconds(.5f);
        uIManager.animatorCallGround.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.06f);
        uIManager.animatorBoardPlayer.SetTrigger(StartAnimation);
        // espera a animação da pokebola
        uIManager.userAttackInterface.gameObject.SetActive(false);
        uIManager.userSelectInterface.gameObject.SetActive(true);
        yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        uIManager.firstTime = false;
        uIManager.currentIDInterface = 0;
    }
    
    private IEnumerator PerformPlayerMove(string urlMoveAttack)
    {
        uIManager.inBattle = true;
        yield return uIManager.Dialogue(
            $"{playerBagSplicemons.currentSplicemon.nameSpliceMon} usou! \n {playerBagSplicemons.currentSplicemon.PokeData.movesAttack[0].move.nameAttack.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
        yield return new WaitForSeconds(0.5f);
        uIManager.playerInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Damage);
        MoveDetails attack = new();
        yield return ApiManager.GetPPMoveAttack(urlMoveAttack, callbackAttack => { attack = callbackAttack; });
        var damage = Maths.CalculateDamage(attack.ppCurrent, 
            playerBagSplicemons.currentSplicemon.level, 
            playerBagSplicemons.currentSplicemon.attackStats.currentStat, 
            opponentPokeData.defenseStats.currentStat);
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
            var exp = playerBagSplicemons.currentSplicemon.GainExperience(opponentPokeData.level);
            uIManager.playerInfo.expBar.SetExp(exp);
            yield return uIManager.Dialogue(
                $"{playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} Ganhou \n {exp} pontos de EXP!",
                200f, waitForInput: true);
            //se subiu nivel mostrar no texto.
            // se subiu mostrar janelinha do oque foi upado 
            //depois disso transição para o jogo
            uIManager.userSelectInterface.gameObject.SetActive(true);
            uIManager.userAttackInterface.gameObject.SetActive(false);
            uIManager.opponentInfo.animationSplicemon.Play("Idle");
            Destroy(opponentPokeData.gameObject);
            uIManager.currentIDInterface = -1;
            uIManager.canvasBattleUI.SetActive(false);
            playerMovement.inBattle = false;
        }
    }

    private IEnumerator PerformOpponentMove()
    {
        // Espera um breve momento antes do oponente atacar
        yield return new WaitForSeconds(1f);
    
        // Mostra mensagem de que o oponente está atacando
        yield return uIManager.Dialogue(
            $"{opponentPokeData.nameSpliceMon} usou! \n{opponentPokeData.PokeData.movesAttack[0].move.nameAttack.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
    
        // Animação de ataque do oponente
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
    
        // Animação de dano do jogador
        uIManager.playerInfo.animationSplicemon.SetTrigger(Damage);
        yield return new WaitForSeconds(0.2f);
        MoveDetails attack = new();
        // Aplica dano ao jogador
        var damage = Maths.CalculateDamage(attack.ppCurrent, 
            opponentPokeData.level, 
            opponentPokeData.attackStats.currentStat, 
            playerBagSplicemons.currentSplicemon.defenseStats.currentStat);
        uIManager.playerInfo.healthBar.ChangeHealth(-damage);
        battleSound.PlayOneShot(hitClip);
    
        // Verifica se o jogador foi derrotado
        if (deathPlayer)
        {
            // Animação de morte do Pokémon do jogador
            yield return new WaitForSeconds(0.5f);
            uIManager.playerInfo.animationSplicemon.SetTrigger(Death);
            battleSound.PlayOneShot(hitClip);
        
            // Mensagem de derrota
            yield return uIManager.Dialogue($"{playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} foi derrotado!", waitForInput: true);
            // Aqui você pode adicionar lógica para trocar de Pokémon ou encerrar a batalha
        }
        else
        {
            // Volta para a seleção de ação do jogador
            yield return new WaitForSeconds(1f);
            uIManager.userAttackInterface.gameObject.SetActive(false);
            uIManager.userSelectInterface.gameObject.SetActive(true);
            uIManager.currentIDInterface = 0;
            yield return uIManager.Dialogue($"O que o {playerBagSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
    
        uIManager.inBattle = false;
    }
}
