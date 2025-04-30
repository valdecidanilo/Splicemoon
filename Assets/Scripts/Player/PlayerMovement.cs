using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 3f;
        private Vector2 lastDirection = Vector2.down;
        
        [SerializeField] private PlayerAnimatorController animatorController;

        [Header("Grid Settings")]
        public float gridSize = 0.16f;

        public bool inBattle;
        public bool isMoving;
        private Vector2 input;
        private Vector3 targetPos;

        private void Update()
        {
            if (!isMoving && !inBattle)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");

                if (input.x != 0) input.y = 0;

                if (input != Vector2.zero)
                {
                    var direction = new Vector3(input.x, input.y, 0f).normalized;
                    targetPos = transform.position + direction * gridSize;
                    animatorController.spriteRenderer.flipX = direction.x switch
                    {
                        > 0 => false,
                        < 0 => true,
                        _ => animatorController.spriteRenderer.flipX
                    };
                    if (IsWalkable(targetPos))
                        StartCoroutine(Move(targetPos));
                }
                animatorController.SetMoveDirection(input, input != Vector2.zero);
            }
        }

        private IEnumerator Move(Vector3 destination)
        {
            isMoving = true;

            while ((destination - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = destination;
            isMoving = false;

            var battleZone = GetComponentInChildren<BattleTriggerZone>();
            if (battleZone != null)
                battleZone.TryStartBattle(encounterBattle =>
                {
                    if(encounterBattle) inBattle = true;
                });
            yield return new WaitForEndOfFrame();

            animatorController.SetMoveDirection(Vector2.zero, false);
        }

        private bool IsWalkable(Vector3 targetPos)
        {
            return true;
        }
    }
}