using System.Collections;
using Models;
using Unity.VisualScripting;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;
using Stats = Schemas.Stats;

public class BattleController : MonoBehaviour
{
    public BattleUIManager uIManager;
    
    public PlayerSplicemons playerSplicemons;
    [SerializeField] private SpliceMon opponentPokeData;
    
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
    private void Start()
    {
        StartCoroutine(StartBattle());
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
        uIManager.opponentInfo.SetLevel(1);
        
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
    }
    private IEnumerator StartBattle()
    {
        Logger.Log("Getting... Player Poke Data");
        yield return playerSplicemons.bagInitialized;
        Logger.Log("Getting Opponent Poke Data");
        var getOpponent = ApiManager.GetPokeData(
            Mathf.FloorToInt(Random.Range(1, ApiManager.ValueMax)), GetOpponentPokeData
            );
        yield return getOpponent;
        Logger.Log("Starting Battle");
        uIManager.animatorTransition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.5f);
        yield return ApiManager.GetSound(opponentPokeData.PokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
        yield return uIManager.Dialogue($"Selvagem {opponentPokeData.nameSpliceMon.ToUpper()} Apareceu!", 200f, waitForInput: true);
        yield return new WaitForSeconds(1f);
        yield return uIManager.Dialogue($"Vai! {playerSplicemons.currentSplicemon.nameSpliceMon.ToUpper()}!", 200f, waitForInput: false);
        yield return new WaitForSeconds(.05f);
        // espera a animação da pokebola
        uIManager.userAttackInterface.gameObject.SetActive(false);
        uIManager.userSelectInterface.gameObject.SetActive(true);
        yield return uIManager.Dialogue($"O que o {playerSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        uIManager.firstTime = false;
        uIManager.currentIDInterface = 0;
    }
    
    private IEnumerator PerformPlayerMove(string urlMoveAttack)
    {
        uIManager.inBattle = true;
        yield return uIManager.Dialogue(
            $"{playerSplicemons.currentSplicemon.nameSpliceMon} usou! \n {playerSplicemons.currentSplicemon.PokeData.movesAttack[0].move.nameAttack.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
        yield return new WaitForSeconds(0.5f);
        uIManager.playerInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Damage);
        MoveDetails attack = new();
        yield return ApiManager.GetPPMoveAttack(urlMoveAttack, callbackAttack => { attack = callbackAttack; });
        uIManager.opponentInfo.healthBar.ChangeHealth(Stats.CalculateDamage(attack.ppCurrent, playerSplicemons.currentSplicemon.level, playerSplicemons.currentSplicemon.attackStats.currentStat, playerSplicemons.currentSplicemon.defenseStats.currentStat));

        battleSound.PlayOneShot(hitClip);
        if(!deathOpponent)
        {
            yield return PerformOpponentMove();
        }
        else
        {
            uIManager.sourceBattleSound.mute = true; 
           
            yield return ApiManager.GetSound(opponentPokeData.crySound, callback => { battleSound.PlayOneShot(callback); });
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Opponent Death");
            yield return new WaitForSeconds(0.3f);
            yield return uIManager.Dialogue($"Selvagem {opponentPokeData.nameSpliceMon.ToUpper()} Desmaiou!", 200f, waitForInput: true);
            battleSound.PlayOneShot(victoryClip);
            yield return uIManager.Dialogue($"{playerSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} Ganhou \n 0 EXP. Points!", 200f, waitForInput: true);
            //se subiu nivel mostrar no texto.
            // se subiu mostrar janelinha do oque foi upado 
            //depois disso transição para o jogo
            
            uIManager.opponentInfo.animationSplicemon.SetTrigger(Death);
            battleSound.PlayOneShot(hitClip);
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
    
        // Aplica dano ao jogador
        int damage = Random.Range(-10, -20);
        uIManager.playerInfo.healthBar.ChangeHealth(damage);
        battleSound.PlayOneShot(hitClip);
    
        // Verifica se o jogador foi derrotado
        if (deathOpponent)
        {
            // Animação de morte do Pokémon do jogador
            yield return new WaitForSeconds(0.5f);
            uIManager.playerInfo.animationSplicemon.SetTrigger(Death);
            battleSound.PlayOneShot(hitClip);
        
            // Mensagem de derrota
            yield return uIManager.Dialogue($"{playerSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} foi derrotado!", waitForInput: true);
        
            // Aqui você pode adicionar lógica para trocar de Pokémon ou encerrar a batalha
        }
        else
        {
            // Volta para a seleção de ação do jogador
            yield return new WaitForSeconds(1f);
            uIManager.userAttackInterface.gameObject.SetActive(false);
            uIManager.userSelectInterface.gameObject.SetActive(true);
            uIManager.currentIDInterface = 0;
            yield return uIManager.Dialogue($"O que o {playerSplicemons.currentSplicemon.nameSpliceMon.ToUpper()} fará? ", waitForInput: false);
        }
    
        uIManager.inBattle = false;
    }
}
