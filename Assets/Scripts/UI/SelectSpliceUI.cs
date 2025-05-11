using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class SelectSpliceUI : MonoBehaviour
    {
        [Header("Referências")]
        public PlayerUI playerUI;
        public PlayerBag playerBag;
        public List<CardSlicemon> cards;
        public Sprite[] selected;
        public Sprite[] gender;
        public ButtonClose buttonClose;

        [Header("Estado de Seleção")]
        public CardSlicemon selectedCard;
        [SerializeField] private int currentSelect;
        private int TotalSlots => playerBag.splicemons.Count + 1 + 1;

        private void OnEnable()
        {
            currentSelect = 0;
            UpdateList();
            UpdateCurrentSplicemon();
        }

        private void Update()
        {
            HandleNavigation();
        }

        private void HandleNavigation()
        {
            int previousSelect = currentSelect;

            if (Input.GetKeyDown(KeyCode.W)) currentSelect--;
            if (Input.GetKeyDown(KeyCode.S)) currentSelect++;

            currentSelect = Mathf.Clamp(currentSelect, 0, TotalSlots - 1);

            if (previousSelect != currentSelect)
                UpdateSelectionVisual();

            if (Input.GetKeyDown(KeyCode.Return))
                HandleSelect();
        }

        private void HandleSelect()
        {
            if (currentSelect == 0)
            {
                Debug.Log("Selecionado: CURRENT");
            }
            else if (currentSelect == TotalSlots - 1)
            {
                buttonClose.button.onClick?.Invoke();
                gameObject.SetActive(false);
                playerUI.stopNavigation = false;
            }
            else
            {
                var index = currentSelect - 1;
                var oldSplicemon = playerBag.currentSplicemon;
                var temp = playerBag.splicemons[index];
            
                playerBag.currentSplicemon = temp;
                playerBag.splicemons[index] = oldSplicemon;
            
                Debug.Log($"Trocado com: {temp.nameSpliceMon}");
            
                UpdateCurrentSplicemon();
                UpdateList();
            }
        }

        private void UpdateCurrentSplicemon()
        {
            var splicemon = playerBag.currentSplicemon;
            selectedCard.SetSplicemon(splicemon.nameSpliceMon, splicemon.level,
                splicemon.stats.hpStats.currentStat, splicemon.stats.hpStats.baseStat);
            selectedCard.SelectGender(splicemon.isFemale ? gender[1] : gender[0]);
            UpdateSelectionVisual();
        }
        private void UpdateList()
        {
        
            foreach (var card in cards)
            {
                card.gameObject.SetActive(false);
                card.SetSelectedContainer(false);
            }

            for (var i = 0; i < playerBag.splicemons.Count && i + 1 < cards.Count; i++)
            {
                var splicemon = playerBag.splicemons[i];
                var card = cards[i + 1];

                card.gameObject.SetActive(true);
                card.SetSplicemon(splicemon.nameSpliceMon, splicemon.level,
                    splicemon.stats.hpStats.currentStat, splicemon.stats.hpStats.baseStat);
                card.SelectGender(splicemon.isFemale ? gender[1] : gender[0]);
            }

            UpdateSelectionVisual();
        }

        private void UpdateSelectionVisual()
        {
            for (var i = 0; i < cards.Count; i++)
            {
                cards[i].SetSelectedContainer(i == currentSelect);
                cards[i].SetIconSelected(selected[i == currentSelect ? 1 : 0]);
            }

            selectedCard.SetSelectedContainer(currentSelect == 0);
            selectedCard.SetIconSelected(selected[currentSelect == 0 ? 1 : 0]);

            buttonClose.SetContainerSelected(currentSelect == TotalSlots - 1 ? 0 : 1);
            buttonClose.SetIconSelected(currentSelect == TotalSlots - 1 ? selected[1] : selected[0]);
        }
    }
}
