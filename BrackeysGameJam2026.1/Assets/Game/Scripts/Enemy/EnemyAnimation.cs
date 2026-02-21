using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator animator;
    [Tooltip("The Transform that holds the Sprite Renderer. Used to prevent the sprite from rotating if the parent rotates.")]
    public Transform graphicTransform;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
            
        if (graphicTransform == null && animator != null)
            graphicTransform = animator.transform;
    }

    public void UpdateMovement(Vector2 facingDir, float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", facingDir.x);
            animator.SetFloat("Vertical", facingDir.y);
            animator.SetFloat("Speed", speed);
        }
    }
    
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void LateUpdate()
    {
        if (graphicTransform != null)
        {
            // If the parent EnemyAI rotates physically to aim the FOV, 
            // this keeps the sprite upright so it doesn't spin wildly.
            graphicTransform.rotation = Quaternion.identity;
        }
    }
}
