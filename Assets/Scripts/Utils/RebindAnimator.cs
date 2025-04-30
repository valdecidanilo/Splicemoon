using UnityEngine;

public class RebindAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void OnEnable()
    {
        animator.Rebind(); // reseta o estado do Animator
        animator.Update(0f); // força ele a atualizar na próxima frame
    }
}
