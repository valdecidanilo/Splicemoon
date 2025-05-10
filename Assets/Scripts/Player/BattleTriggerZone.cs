using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class BattleTriggerZone : MonoBehaviour
    {
        public LayerMask grassLayer;
        public float encounterChance = 0.1f;
        public float originY;
        public float radius = 0.05f;
        public BattleController battleController;
        public BattleUIManager uIManager;
        public void TryStartBattle(Action<bool> encounterBattle)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y - originY);

            var grass = Physics2D.OverlapCircle(position, radius, grassLayer);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            var origin = new Vector2(transform.position.x, transform.position.y - originY);
            Gizmos.DrawWireSphere(origin, radius);
        }
    }
}
