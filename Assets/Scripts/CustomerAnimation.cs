using UnityEngine;

public class CustomerAnimation : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPos);

        if (distanceMoved > 0 )
        {
            Vector3 direction = (transform.position - lastPos).normalized;

            animator.SetBool("isMoving", true);
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        lastPos = transform.position;
    }
}
