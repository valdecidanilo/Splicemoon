using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        // Configurações
        public float moveSpeed = 3f;
        [SerializeField] private float turnDelay = 0.15f; // Ajuste esse valor no Inspector
        
        private Vector2 currentDirection;
        private Vector2 inputBuffer;
        private bool isMoving;
        public bool isInBattle;
        private bool isWaitingToMove = false;
        private bool isTurning;
        
        [Header("References")]
        [SerializeField] private PlayerAnimatorController animatorController;
        
        [Header("Grid Settings")]
        public float gridSize = 0.16f;
        [SerializeField] private float radiusGrass = 0.16f;
        [SerializeField] private float radius = 0.16f;
        public float originY = 0.2f;

        private void Update()
        {
            if (isMoving || isInBattle) return;

            inputBuffer.x = Input.GetAxisRaw("Horizontal");
            inputBuffer.y = Input.GetAxisRaw("Vertical");

            if (inputBuffer.x != 0) inputBuffer.y = 0;

            if (inputBuffer != Vector2.zero)
            {
                if (inputBuffer != currentDirection || !isWaitingToMove)
                {
                    currentDirection = inputBuffer;
                    isWaitingToMove = false;
                    StartCoroutine(HandleTurn());
                }
                else if (inputBuffer == currentDirection && !isTurning)
                {
                    MoveToTarget();
                }
            }
            else
            {
                isWaitingToMove = false;
                animatorController.SetMoveDirection(Vector2.zero, false);
            }
        }
        private IEnumerator HandleTurn()
        {
            animatorController.SetMoveDirection(currentDirection, false);
            UpdateSpriteFlip();
            yield return new WaitForSeconds(turnDelay);
            isWaitingToMove = true;
        }

        private void UpdateSpriteFlip()
        {
            if (currentDirection.x != 0)
            {
                animatorController.spriteRenderer.flipX = currentDirection.x < 0;
            }
        }

        private void MoveToTarget()
        {
            Vector3 targetPos = transform.position + (Vector3)currentDirection * gridSize;
            
            if (IsWalkable(targetPos))
            {
                StartCoroutine(ExecuteMovement(targetPos));
                animatorController.SetMoveDirection(currentDirection, true);
            }
        }

        private IEnumerator ExecuteMovement(Vector3 destination)
        {
            isMoving = true;
    
            while (Vector3.Distance(transform.position, destination) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    destination, 
                    moveSpeed * Time.deltaTime
                );
        
                UpdateGrassDetection();
                yield return null;
            }

            transform.position = destination;
            isMoving = false;
    
            CheckForBattles();
    
            if (inputBuffer == Vector2.zero)
                animatorController.SetMoveDirection(Vector2.zero, false);
        }

        private void UpdateGrassDetection()
        {
            var origin = new Vector2(transform.position.x, transform.position.y - originY);
            GrassManager.UpdateNearbyGrass(origin, radius, radiusGrass);
        }

        private void CheckForBattles()
        {
            var battleZone = GetComponentInChildren<BattleTriggerZone>();
            battleZone?.TryStartBattle(encounter =>
            {
                isInBattle = encounter;
                inputBuffer = Vector2.zero;
            });
        }

        private bool IsWalkable(Vector3 targetPos)
        {
            return true;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            var origin = new Vector2(transform.position.x, transform.position.y - originY);
            Gizmos.DrawWireSphere(origin, radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin, radiusGrass);
        }
        #endif
    }
}