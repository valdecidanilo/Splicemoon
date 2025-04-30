using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class BattleTriggerZone : MonoBehaviour
    {
        public LayerMask grassLayer;
        public float encounterChance = 0.1f; // 10% chance
        public BattleController battleController;
        public BattleUIManager uIManager;
        public void TryStartBattle(Action<bool> encounterBattle)
        {
            Vector2 position = transform.position;

            var grass = Physics2D.OverlapCircle(position, 0.1f, grassLayer);
            if (grass != null)
            {
                var roll = Random.Range(0f, 1f);
                if (roll < encounterChance)
                {
                    encounterBattle?.Invoke(true);
                    uIManager.StartCoroutine(uIManager.Transition(onComplete =>
                    {
                        if(onComplete) battleController.StartBattle();
                    }));
                }
            }
        }
    }
}
