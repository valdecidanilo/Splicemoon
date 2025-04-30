using UnityEngine;

namespace Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private Vector2 lastDirection = Vector2.down;
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
            if (input != Vector2.zero)
                lastDirection = input;

            animator.SetFloat(MoveX, lastDirection.x);
            animator.SetFloat(MoveY, lastDirection.y);
            animator.SetBool(IsMoving, isMoving);
        }
    }
}
