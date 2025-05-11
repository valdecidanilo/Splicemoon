using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProfileUI : MonoBehaviour
    {
        public PlayerUI playerUI;
        
        public TMP_Text playerId;
        public TMP_Text namePlayer;
        public TMP_Text levelPlayer;
        public TMP_Text medalsPlayer;
        public TMP_Text amountSplicemon;
        public TMP_Text moneyPlayer;
        public Image avatarPlayer;

        public void SetPlayerStats(string id,string nickname, int level, int medals, int splicemons, int money, Sprite spr)
        {
            playerId.SetText(id);
            namePlayer.SetText($"Nome: {nickname}");
            levelPlayer.SetText(level.ToString());
            medalsPlayer.SetText(medals.ToString());
            amountSplicemon.SetText(splicemons.ToString());
            moneyPlayer.SetText(money.ToString());
            avatarPlayer.sprite = spr;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                playerUI.stopNavigation = false;
                gameObject.SetActive(false);
            }
        }
    }
}
