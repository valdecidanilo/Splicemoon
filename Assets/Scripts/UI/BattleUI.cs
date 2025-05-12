using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

namespace UI
{
    public class BattleUI : MonoBehaviour
    {
        private static readonly int Start = Animator.StringToHash("Start");
        private static readonly int End = Animator.StringToHash("End");
        public RectTransform messageUI;
        public SelectCommandBattle userSelectInterface;
        public SelectCommandBattle userAttackInterface;
    
        public PlayerBag playerBag;
    
        public Animator animatorCallGround;
        public Animator animatorTransition;
        public Animator animatorBoardOpponent;
        public Animator animatorBoardPlayer;
    
        public InfoSplicemon opponentInfo;
        public InfoSplicemon playerInfo;
        public bool isBagOrSelect;

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
        public bool isTryRunning;
        [SerializeField] public bool firstTime = true;

        public TMP_Text typeAttack;
        public TMP_Text powerPoint;
        public TMP_Text messageText;
        public List<TMP_Text> textsAttacks;
        public event Action<MoveDetails, int> OnAttackSlot1;
        public event Action<MoveDetails, int> OnAttackSlot2;
        public event Action<MoveDetails, int> OnAttackSlot3;
        public event Action<MoveDetails, int> OnAttackSlot4;
        public event Action OnCallTryRun;
    
        private void Update()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            if (inBattle || isBagOrSelect || isTryRunning) return;
            if(currentInterface == 0) userSelectInterface.Move(horizontal, vertical, Selected);
            else
            {
                if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
                {
                    AudioManager.Instance.Play("ClickUI");
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
            powerPoint.SetText($"{playerBag.currentSplicemon.stats.movesAttack[id].ppCurrent}/{playerBag.currentSplicemon.stats.movesAttack[id].ppMax}");
            typeAttack.SetText($"TYPE/{playerBag.currentSplicemon.stats.movesAttack[id].typeMove.TypeAttack}");

        }
        private void Selected(int id)
        {
            if(currentInterface == CurrentInterface.Nothing) return;
            if(currentSelected == 0 && currentInterface == CurrentInterface.Attack && typeAttack.text != $"TYPE/{playerBag.currentSplicemon.stats.movesAttack[id].TypeMove.TypeAttack}")
                typeAttack.SetText($"TYPE/{playerBag.currentSplicemon.stats.movesAttack[id].TypeMove.TypeAttack}");
            if (currentSelected != id)
            {
                currentSelected = id;
                AudioManager.Instance.Play("ClickUI");
                if (currentInterface == CurrentInterface.Attack)
                {
                    typeAttack.SetText($"TYPE/{playerBag.currentSplicemon.stats.movesAttack[id].TypeMove.TypeAttack}");
                    powerPoint.SetText($"{playerBag.currentSplicemon.stats.movesAttack[id].ppCurrent}/{playerBag.currentSplicemon.stats.movesAttack[id].ppMax}");
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
                case CurrentInterface.SelectSlicemon:
                case CurrentInterface.Bag:
                
                    break;
                case CurrentInterface.Nothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
        }

        private void HandleUserInterface(int id)
        {
            AudioManager.Instance.Play("ClickUI");
            switch (id)
            {
                case 0:
                    powerPoint.SetText($"{playerBag.currentSplicemon.stats.movesAttack[id].powerAttack}/{playerBag.currentSplicemon.stats.movesAttack[0].powerAttack}");
                    userAttackInterface.gameObject.SetActive(true);
                    userSelectInterface.gameObject.SetActive(false);
                    currentInterface = CurrentInterface.Attack;
                    break;
                case 1:
                    Logger.Log("Apertou no BAG");
                    isBagOrSelect = true;
                    break;
                case 2:
                    Logger.Log("Apertou no POKEMON");
                    isBagOrSelect = true;
                    break;
                case 3:
                    OnCallTryRun?.Invoke();
                    break;
            }
        }

        public void UpdatePlayer()
        {
            playerInfo.SetName(playerBag.currentSplicemon.nameSpliceMon);
            playerInfo.SetLevel(playerBag.currentSplicemon.level);
            playerInfo.expBar.SetMaxExp(playerBag.currentSplicemon.experienceMax);
            playerInfo.expBar.SetExp(playerBag.currentSplicemon.experience);
            playerInfo.healthBar.SetMaxHealth(playerBag.currentSplicemon.stats.hpStats.currentStat);
            playerInfo.healthBar.SetHealth(playerBag.currentSplicemon.stats.hpStats.currentStat);
            playerInfo.SetGender(false);
            StartCoroutine(ApiManager.GetSprite(playerBag.currentSplicemon.backSprite, sprite =>
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
                textsAttacks[i].SetText($"{playerBag.currentSplicemon.stats.movesAttack[i].NameMove}");
            }
        }

        public void ResetBattleUI()
        {
            animatorCallGround.CrossFade("Idle", 0f);
            animatorCallGround.Update(0f);
            opponentInfo.animationSplicemon.Play("Idle");
            playerInfo.animationSplicemon.Play("Idle");
            playerInfo.animationSplicemon.Update(0f);
            animatorCallGround.Play("Idle");
            userSelectInterface.gameObject.SetActive(false);
            userAttackInterface.gameObject.SetActive(false);
            inBattle = false;
            currentInterface = CurrentInterface.Nothing;
        }
        private void HandlerAttackInterface(int id)
        {
            AudioManager.Instance.Play("ClickUI");
            switch (id)
            {
                case 0:
                    OnAttackSlot1?.Invoke(playerBag.currentSplicemon.stats.movesAttack[id], id);
                    break;
                case 1:
                    OnAttackSlot2?.Invoke(playerBag.currentSplicemon.stats.movesAttack[id], id);
                    break;
                case 2:
                    OnAttackSlot3?.Invoke(playerBag.currentSplicemon.stats.movesAttack[id], id);
                    break;
                case 3:
                    OnAttackSlot4?.Invoke(playerBag.currentSplicemon.stats.movesAttack[id], id);
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
}
