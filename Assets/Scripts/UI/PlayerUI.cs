using System;
using System.Collections;
using System.Collections.Generic;
using CustomButton;
using Player;
using UnityEngine;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Referências")] 
        public PlayerMovement player;

        public GameObject uiSplicemon;
        public GameObject uiBag;
        public GameObject uiProfile;
        public GameObject menu;
        public AudioSource sourceBattleSound;
        public Animator animatorTransition;
        public List<CustomButtonClass> buttons;

        [Header("Estado do Menu")]
        public int currentSelect;

        public bool stopNavigation;

        private static readonly int Start = Animator.StringToHash("Start");
        private static readonly int End = Animator.StringToHash("End");

        private void Update()
        {
            if(player.inBattleSpr.enabled || stopNavigation) return;
            HandleMenuToggle();
            if (player.InMenu)
            {
                HandleNavigation();
                UpdateVisual();
                if(Input.GetKeyDown(KeyCode.Return)) HandleSelect();
            }
        }

        private void HandleMenuToggle()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                player.SetInMenu(!player.InMenu);
                menu.SetActive(player.InMenu);
                player.ForceIdleAnimation();
                currentSelect = 0;
                UpdateVisual();
            }
        }

        private void HandleSelect()
        {
            switch (currentSelect)
            {
                case 0://Slicemon
                    stopNavigation = true;
                    uiSplicemon.SetActive(true);
                    break;
                case 1://Bolsa
                    //stopNavigation = true;
                    //uiBag.SetActive(true);
                    break;
                case 2://Perfil
                    stopNavigation = true;
                    uiProfile.SetActive(true);
                    break;
                case 3://Sair
                    player.SetInMenu(false);
                    menu.SetActive(player.InMenu);
                    currentSelect = 0;
                    UpdateVisual();
                    break;
            }
        }
        private void HandleNavigation()
        {
            int previousSelect = currentSelect;

            if (Input.GetKeyDown(KeyCode.W)) currentSelect--;
            if (Input.GetKeyDown(KeyCode.S)) currentSelect++;

            currentSelect = Mathf.Clamp(currentSelect, 0, buttons.Count - 1);

            if (previousSelect != currentSelect) return;
            //sourceBattleSound.Play(); // Som opcional ao trocar seleção
        }

        private void UpdateVisual()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var isSelected = (i == currentSelect);
                buttons[i].transform.GetChild(1).gameObject.SetActive(isSelected);
            }
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
}