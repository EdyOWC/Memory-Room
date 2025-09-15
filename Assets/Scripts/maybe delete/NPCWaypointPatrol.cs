using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class NPCStoryWalk : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;         // Assign in Inspector, in order
    public float reachThreshold = 0.2f;   // Distance considered "arrived"

    [Header("Movement")]
    public float moveSpeed = 2f;          // Walking speed
    public float turnSpeed = 60f;         // Turning speed
    public float idleTimeAtWaypoint = 2f; // Seconds to idle at each point

    private Animator animator;
    private int currentWaypoint = 0;
    private bool walking = false;
    private bool finished = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Start at first waypoint (if exists)
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;

        animator.SetFloat("Speed", 0f); // start idle
        StartCoroutine(StartWalkingAfterDelay(idleTimeAtWaypoint));
    }

    IEnumerator StartWalkingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        walking = true;
    }

    void Update()
    {
        if (!walking || finished || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // keep flat on ground

        float distance = direction.magnitude;

        if (distance > reachThreshold)
        {
            // Rotate smoothly toward target
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

            // Move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Play walk animation
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // Snap exactly to waypoint
            transform.position = target.position;

            // Switch to idle and wait
            StartCoroutine(IdleAtWaypoint());
        }
    }

    IEnumerator IdleAtWaypoint()
    {
        walking = false;
        animator.SetFloat("Speed", 0f); // idle

        yield return new WaitForSeconds(idleTimeAtWaypoint);

        // Next waypoint
        currentWaypoint++;

        if (currentWaypoint >= waypoints.Length)
        {
            finished = true; // stop forever
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            walking = true;
        }
    }
}
