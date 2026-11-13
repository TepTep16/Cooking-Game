using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
     
        animator = GetComponent<Animator>();
    }

    void Update()
    {
       
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

       
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);

       
        if (isMoving)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
        }
    }
}