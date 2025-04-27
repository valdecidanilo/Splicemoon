using System.Collections;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour
{
    public BattleUIManager uIManager;
    public PokeData opponentPokeData;
    public PlayerCurrentSplicemon playerPokeData;
    
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
        //StartCoroutine(StartBattle());
    }

    private void AttackSlot1()
    {
        StartCoroutine(PerformPlayerMove());
    }

    private void AttackSlot2()
    {
        StartCoroutine(PerformPlayerMove());
    }

    private void AttackSlot3()
    {
        StartCoroutine(PerformPlayerMove());
    }

    private void AttackSlot4()
    {
        StartCoroutine(PerformPlayerMove());
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

    private IEnumerator StartBattle()
    {
        Logger.Log("Getting Opponent Poke Data");
        var getOpponent = ApiManager.GetPokeData(Mathf.FloorToInt(Random.Range(0, ApiManager.ValueMax)), GetOpponentPokeData);
        yield return getOpponent;
        Logger.Log("Starting Battle");
        uIManager.animatorTransition.SetTrigger(StartAnimation);
        yield return new WaitForSeconds(1.5f);
        yield return ApiManager.GetSound(opponentPokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
        yield return uIManager.Dialogue($"Selvagem {opponentPokeData.NameSpliceMoon.ToUpper()} Apareceu!", 200f, waitForInput: true);
        yield return new WaitForSeconds(1f);
        yield return uIManager.Dialogue($"Vai! {playerPokeData.currentSplicemonData.NameSpliceMoon.ToUpper()}!", 200f, waitForInput: false);
        yield return new WaitForSeconds(1f);
        // espera a animação da pokebola
        uIManager.userAttackInterface.gameObject.SetActive(false);
        uIManager.userSelectInterface.gameObject.SetActive(true);
        yield return uIManager.Dialogue($"O que o {playerPokeData.currentSplicemonData.NameSpliceMoon.ToUpper()} fará? ", waitForInput: false);
        uIManager.firstTime = false;
        uIManager.currentIDInterface = 0;
    }
    
    private IEnumerator PerformPlayerMove()
    {
        uIManager.inBattle = true;
        yield return uIManager.Dialogue(
            $"{playerPokeData.currentSplicemonData.NameSpliceMoon} usou! \n {playerPokeData.currentSplicemonData.movesAttack[0].move.nameAttack.ToUpper()}", 
            waitForInput: false,
            hidenMessage: false);
        yield return new WaitForSeconds(0.5f);
        uIManager.playerInfo.animationSplicemon.SetTrigger(Attack);
        yield return new WaitForSeconds(0.7f);
        uIManager.opponentInfo.animationSplicemon.SetTrigger(Damage);
        yield return new WaitForSeconds(0.2f);
        uIManager.opponentInfo.healthBar.ChangeHealth(Random.Range(-10, -20));

        battleSound.PlayOneShot(hitClip);
        if(!deathOpponent)
        {
            
            yield return PerformOpponentMove();
        }
        else
        {
            uIManager.sourceBattleSound.mute = true; 
           
            yield return ApiManager.GetSound(opponentPokeData.soundUrl.latest, callback => { battleSound.PlayOneShot(callback); });
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Opponent Death");
            yield return new WaitForSeconds(0.3f);
            yield return uIManager.Dialogue($"Selvagem {opponentPokeData.NameSpliceMoon.ToUpper()} Desmaiou!", 200f, waitForInput: true);
            battleSound.PlayOneShot(victoryClip);
            yield return uIManager.Dialogue($"{playerPokeData.currentSplicemonData.NameSpliceMoon.ToUpper()} Ganhou \n 0 EXP. Points!", 200f, waitForInput: true);
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
            $"{opponentPokeData.NameSpliceMoon} usou! \n{opponentPokeData.movesAttack[0].move.nameAttack.ToUpper()}", 
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
            yield return uIManager.Dialogue($"{playerPokeData.currentSplicemonData.NameSpliceMoon.ToUpper()} foi derrotado!", waitForInput: true);
        
            // Aqui você pode adicionar lógica para trocar de Pokémon ou encerrar a batalha
        }
        else
        {
            // Volta para a seleção de ação do jogador
            yield return new WaitForSeconds(1f);
            uIManager.userAttackInterface.gameObject.SetActive(false);
            uIManager.userSelectInterface.gameObject.SetActive(true);
            uIManager.currentIDInterface = 0;
            yield return uIManager.Dialogue($"O que o {playerPokeData.currentSplicemonData.NameSpliceMoon.ToUpper()} fará? ", waitForInput: false);
        }
    
        uIManager.inBattle = false;
    }
}
