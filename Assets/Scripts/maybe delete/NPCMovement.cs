using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 2f;     // how fast NPC walks
    public float turnSpeed = 60f;    // how quickly NPC turns
    public Transform target;         // optional target to walk toward

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null)
        {
            // Rotate toward target
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

            // Move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Trigger walking animation
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // Idle animation
            animator.SetFloat("Speed", 0f);
        }
    }
}
