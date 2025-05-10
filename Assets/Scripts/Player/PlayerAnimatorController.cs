using UnityEngine;

namespace Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private Vector2 lastDirection = Vector2.down;
        private bool isCurrentlyMoving;
        public SpriteRenderer spriteRenderer;
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetMoveDirection(Vector2 input, bool isMoving)
        {
            if (input != lastDirection || isMoving != isCurrentlyMoving)
            {
                lastDirection = input != Vector2.zero ? input : lastDirection;
                isCurrentlyMoving = isMoving;

                animator.SetFloat(MoveX, lastDirection.x);
                animator.SetFloat(MoveY, lastDirection.y);
                animator.SetBool(IsMoving, isCurrentlyMoving);

                if (input.x != 0 && input != Vector2.zero)
                {
                    spriteRenderer.flipX = input.x < 0;
                }
            }
        }
        public void SetRunState(bool isRunning)
        {
            animator.SetBool("IsRunning", isRunning);
        }
    }
}
