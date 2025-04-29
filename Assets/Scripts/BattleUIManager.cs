using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

public class BattleUIManager : MonoBehaviour
{
    public RectTransform messageUI;
    public SelectCommandBattle userSelectInterface;
    public SelectCommandBattle userAttackInterface;
    
    public PlayerSplicemons playerBagSplicemons;
    
    public Animator animatorTransition;
    
    public AudioSource sourceBattleSound;
    
    public InfoSplicemon opponentInfo;
    public InfoSplicemon playerInfo;
    
    public int currentIDInterface = -1;
    private int currentSelected = 0;
    [SerializeField] public bool inBattle = false;
    [SerializeField] public bool firstTime = true;
    public AudioSource audioSource;

    public TMP_Text typeAttack;
    public TMP_Text powerPoint;
    public TMP_Text messageText;
    public List<TMP_Text> textsAttacks;
    public event Action<string> OnAttackSlot1;
    public event Action<string> OnAttackSlot2;
    public event Action<string> OnAttackSlot3;
    public event Action<string> OnAttackSlot4;
    
    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if (inBattle) return;
        if(currentIDInterface == 0) userSelectInterface.Move(horizontal, vertical, Selected);
        else
        {
            if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
            {
                audioSource.Play();
                userAttackInterface.gameObject.SetActive(false);
                userSelectInterface.gameObject.SetActive(true);
                currentIDInterface = 0;
                return;
            }
            userAttackInterface.Move(horizontal, vertical, Selected);
        }
        if(Input.GetKeyDown(KeyCode.H)) HideMessage();
        if(Input.GetKeyDown(KeyCode.J)) ShowMessage();
    }

    public void ShowMessage() => messageUI.SetAsLastSibling();
    public void HideMessage() => messageUI.SetSiblingIndex(3);
    
    private void Selected(int id)
    {
        if (currentSelected != id)
        {
            currentSelected = id;
            audioSource.Play();
            if (currentIDInterface == 1)
            {
                typeAttack.SetText($"TYPE/{playerBagSplicemons.currentSplicemon.itensMove[id].typeName}");
                powerPoint.SetText($"{playerBagSplicemons.currentSplicemon.itensMove[id].ppCurrent}/{playerBagSplicemons.currentSplicemon.itensMove[id].ppMax}");
            }
        }
        if (!inBattle && !firstTime && !Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return)) return;
        switch (currentIDInterface)
        {
            case 0:
                HandleUserInterface(id);
                break;
            case 1:
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
                powerPoint.SetText($"{playerBagSplicemons.currentSplicemon.itensMove[id].powerAttack}/{playerBagSplicemons.currentSplicemon.itensMove[0].powerAttack}");
                userAttackInterface.gameObject.SetActive(true);
                userSelectInterface.gameObject.SetActive(false);
                currentIDInterface = 1;
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

    private void UpdatePlayer()
    {
        playerInfo.SetName(playerBagSplicemons.currentSplicemon.nameSpliceMon);
        playerInfo.SetLevel(1);
        playerInfo.SetGender(false);
        StartCoroutine(ApiManager.GetSprite(playerBagSplicemons.currentSplicemon.backSprite, sprite =>
        {
            playerInfo.SetSprite(sprite);
        }));
    }
    private void UpdateTexts()
    {
        foreach (var txt in textsAttacks)
            txt.SetText("-");
        for (var i = 0; i < textsAttacks.Count; i++)
            textsAttacks[i].SetText($"{playerBagSplicemons.currentSplicemon.itensMove[i].nameMove}");
    }
    private void HandlerAttackInterface(int id)
    {
        audioSource.Play();
        switch (id)
        {
            case 0:
                Logger.Log("Apertou no 1");
                OnAttackSlot1?.Invoke(playerBagSplicemons.currentSplicemon.PokeData.movesAttack[id].move.url);
                break;
            case 1:
                Logger.Log("Apertou no 2");
                OnAttackSlot2?.Invoke(playerBagSplicemons.currentSplicemon.PokeData.movesAttack[id].move.url);
                break;
            case 2:
                Logger.Log("Apertou no 3");
                OnAttackSlot3.Invoke(playerBagSplicemons.currentSplicemon.PokeData.movesAttack[id].move.url);
                break;
            case 3:
                Logger.Log("Apertou no 4");
                OnAttackSlot4?.Invoke(playerBagSplicemons.currentSplicemon.PokeData.movesAttack[id].move.url);
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
}
