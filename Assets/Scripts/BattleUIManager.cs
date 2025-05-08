using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

public class BattleUIManager : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int End = Animator.StringToHash("End");
    public GameObject canvasBattleUI;
    public RectTransform messageUI;
    public SelectCommandBattle userSelectInterface;
    public SelectCommandBattle userAttackInterface;
    
    public PlayerBagSplicemons playerBagBagSplicemons;
    
    public Animator animatorCallGround;
    public Animator animatorTransition;
    public Animator animatorBoardOpponent;
    public Animator animatorBoardPlayer;
    
    public AudioSource sourceBattleSound;
    
    public InfoSplicemon opponentInfo;
    public InfoSplicemon playerInfo;

    public enum CurrentInterface
    {
        Nothing = -1,
        UserSelect = 0,
        Attack = 1,
        SelectSlicemon = 2,
        Bag = 3,
    }
    
    public CurrentInterface currentInterface = CurrentInterface.Nothing;
    private int currentSelected = 0;
    [SerializeField] public bool inBattle = false;
    [SerializeField] public bool firstTime = true;
    public AudioSource audioSource;

    public TMP_Text typeAttack;
    public TMP_Text powerPoint;
    public TMP_Text messageText;
    public List<TMP_Text> textsAttacks;
    public event Action<MoveDetails, int> OnAttackSlot1;
    public event Action<MoveDetails, int> OnAttackSlot2;
    public event Action<MoveDetails, int> OnAttackSlot3;
    public event Action<MoveDetails, int> OnAttackSlot4;
    
    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if (inBattle) return;
        if(currentInterface == 0) userSelectInterface.Move(horizontal, vertical, Selected);
        else
        {
            if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
            {
                audioSource.Play();
                userAttackInterface.gameObject.SetActive(false);
                userSelectInterface.gameObject.SetActive(true);
                currentInterface = 0;
                return;
            }
            userAttackInterface.Move(horizontal, vertical, Selected);
        }
        if(Input.GetKeyDown(KeyCode.H)) HideMessage();
        if(Input.GetKeyDown(KeyCode.J)) ShowMessage();
    }

    public void ShowMessage() => messageUI.SetAsLastSibling();
    public void HideMessage() => messageUI.SetSiblingIndex(3);

    public void UpdateTextSelectAttack(int id)
    {
        powerPoint.SetText($"{playerBagBagSplicemons.currentSplicemon.itensMove[id].ppCurrent}/{playerBagBagSplicemons.currentSplicemon.itensMove[id].ppMax}");
        typeAttack.SetText($"TYPE/{playerBagBagSplicemons.currentSplicemon.itensMove[id].typeMove.typeAttack}");
    }
    private void Selected(int id)
    {
        if(currentInterface == CurrentInterface.Nothing) return;
        if(currentSelected == 0 && currentInterface == CurrentInterface.Attack && typeAttack.text != $"TYPE/{playerBagBagSplicemons.currentSplicemon.itensMove[id].typeMove.typeAttack}")
            typeAttack.SetText($"TYPE/{playerBagBagSplicemons.currentSplicemon.itensMove[id].typeMove.typeAttack}");
        if (currentSelected != id)
        {
            currentSelected = id;
            audioSource.Play();
            if (currentInterface == CurrentInterface.Attack)
            {
                typeAttack.SetText($"TYPE/{playerBagBagSplicemons.currentSplicemon.itensMove[id].typeMove.typeAttack}");
                powerPoint.SetText($"{playerBagBagSplicemons.currentSplicemon.itensMove[id].ppCurrent}/{playerBagBagSplicemons.currentSplicemon.itensMove[id].ppMax}");
            }
        }
        if (!inBattle && !firstTime && !Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return)) return;
        switch (currentInterface)
        {
            case CurrentInterface.UserSelect:
                HandleUserInterface(id);
                break;
            case CurrentInterface.Attack:
                HandlerAttackInterface(id);
                break;
        }
        
    }

    private void HandleUserInterface(int id)
    {
        audioSource.Play();
        switch (id)
        {
            case 0:
                powerPoint.SetText($"{playerBagBagSplicemons.currentSplicemon.itensMove[id].powerAttack}/{playerBagBagSplicemons.currentSplicemon.itensMove[0].powerAttack}");
                userAttackInterface.gameObject.SetActive(true);
                userSelectInterface.gameObject.SetActive(false);
                currentInterface = CurrentInterface.Attack;
                break;
            case 1:
                Logger.Log("Apertou no BAG");
                break;
            case 2:
                Logger.Log("Apertou no POKEMON");
                break;
            case 3:
                Logger.Log("Apertou no RUN");
                break;
        }
    }

    public void UpdatePlayer()
    {
        playerInfo.SetName(playerBagBagSplicemons.currentSplicemon.nameSpliceMon);
        playerInfo.SetLevel(playerBagBagSplicemons.currentSplicemon.level);
        playerInfo.expBar.SetMaxExp(playerBagBagSplicemons.currentSplicemon.experienceMax);
        playerInfo.expBar.SetExp(playerBagBagSplicemons.currentSplicemon.experience);
        playerInfo.healthBar.SetMaxHealth(playerBagBagSplicemons.currentSplicemon.hpStats.currentStat);
        playerInfo.healthBar.SetHealth(playerBagBagSplicemons.currentSplicemon.hpStats.currentStat);
        playerInfo.SetGender(false);
        StartCoroutine(ApiManager.GetSprite(playerBagBagSplicemons.currentSplicemon.backSprite, sprite =>
        {
            playerInfo.SetSprite(sprite);
        }));
        UpdateTexts();
    }
    private void UpdateTexts()
    {
        foreach (var txt in textsAttacks)
            txt.SetText("-");
        for (var i = 0; i < textsAttacks.Count; i++)
        {
            textsAttacks[i].SetText($"{playerBagBagSplicemons.currentSplicemon.PokeData.movesAttack[i].move.nameAttack}");
        }
    }
    private void HandlerAttackInterface(int id)
    {
        audioSource.Play();
        switch (id)
        {
            case 0:
                Logger.Log("Apertou no 1");
                OnAttackSlot1?.Invoke(playerBagBagSplicemons.currentSplicemon.itensMove[id], id);
                break;
            case 1:
                Logger.Log("Apertou no 2");
                OnAttackSlot2?.Invoke(playerBagBagSplicemons.currentSplicemon.itensMove[id], id);
                break;
            case 2:
                Logger.Log("Apertou no 3");
                OnAttackSlot3?.Invoke(playerBagBagSplicemons.currentSplicemon.itensMove[id], id);
                break;
            case 3:
                Logger.Log("Apertou no 4");
                OnAttackSlot4?.Invoke(playerBagBagSplicemons.currentSplicemon.itensMove[id], id);
                break;
        }
    }

    public IEnumerator Dialogue(string message, float lettersPerSecond = 30f, bool waitForInput = true, bool hidenMessage = true)
    {
        ShowMessage();
        messageText.text = "";
    
        var delay = 1f / lettersPerSecond;
        var textCompleted = false;
    
        StartCoroutine(TypeText(message, delay, () => textCompleted = true));
    
        if (waitForInput)
        {
            yield return new WaitUntil(() => textCompleted || Input.GetKeyDown(KeyCode.Z));
        
            if (!textCompleted)
            {
                StopAllCoroutines();
                messageText.text = message;
            }
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }
        else
        {
            yield return new WaitUntil(() => textCompleted);
        }
        if(hidenMessage) HideMessage();
    }
    private IEnumerator TypeText(string message, float delay, Action onComplete)
    {
        for (var i = 0; i < message.Length; i++)
        {
            messageText.text += message[i];
            yield return new WaitForSeconds(delay);
        }
        
        onComplete?.Invoke();
    }
    public IEnumerator Transition(Action<bool> onComplete)
    {
        sourceBattleSound.Play();
        animatorTransition.SetTrigger(Start);
        yield return new WaitForSeconds(2.5f);
    
        onComplete?.Invoke(true);
    }
    public IEnumerator TransitionEndBattle(Action<bool> onComplete)
    {
        animatorTransition.SetTrigger(End);
        yield return new WaitForSeconds(1f);
        onComplete?.Invoke(true);
    }
}
