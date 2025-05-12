using System.Collections;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = .5f;
        [SerializeField] private float turnDelay = 0.15f;
        public string nickname;
        public TMP_Text nicknameText;
        
        private Vector2 currentDirection;
        private Vector2 inputBuffer;
        public bool InMenu { get; private set; }
        private bool isMoving;
        private bool isRuning;
        public bool IsInBattle { get; private set; }
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
            if (isMoving || IsInBattle || InMenu) return;

            inputBuffer.x = Input.GetAxisRaw("Horizontal");
            inputBuffer.y = Input.GetAxisRaw("Vertical");
            isRuning = Input.GetKey(KeyCode.LeftShift);
            moveSpeed = isRuning ? 1f : .5f;
            animatorController.SetRunState(isRuning);

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
        public void SetNickname(string currentNickname)
        {
            nickname = currentNickname;
            nicknameText.SetText(nickname);
        }

        public void SetInMenu(bool setInMenu)
        {
            InMenu = setInMenu;
        }
        public void ForceIdleAnimation() => inputBuffer = Vector2.zero;
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
            battleZone.originY = originY;
            battleZone?.TryStartBattle(encounter =>
            {
                SetIsBattle(encounter);
                inputBuffer = Vector2.zero;
            });
        }

        public void SetIsBattle(bool isBattle)
        {
            IsInBattle = isBattle;
            var nicknameInBattle = isBattle ? $"<size=150%><sprite=12></size>\n{nickname}" : $"{nickname}";
            nicknameText.SetText($"{nicknameInBattle}");
        } 
        private bool IsWalkable(Vector3 targetPos)
        {
            return true;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var origin = new Vector2(transform.position.x, transform.position.y - originY);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin, radiusGrass);
        }
        #endif
    }
}