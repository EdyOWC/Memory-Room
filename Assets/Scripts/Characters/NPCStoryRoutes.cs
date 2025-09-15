using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class NPCStoryRoutes : MonoBehaviour
{
    [System.Serializable]
    public class Route
    {
        public string routeName;
        public Transform[] waypoints;
    }

    [Header("Routes")]
    public List<Route> routes = new List<Route>();

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float turnSpeed = 120f;
    public float reachThreshold = 0.2f;

    private Animator animator;
    private Route currentRoute;
    private int currentWaypoint = 0;

    private bool routeComplete = true;

    private enum State { Idle, Walking, TwistingAtEnd }
    private State npcState = State.Idle;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (routeComplete || currentRoute == null) return;

        Transform target = currentRoute.waypoints[currentWaypoint];

        switch (npcState)
        {
            case State.Walking:
                MoveAndRotateTowards(target);
                break;

            case State.TwistingAtEnd:
                RotateTowardsRotation(target.rotation);
                break;
        }
    }

    // ---- MOVEMENT HELPERS ----

    private void MoveAndRotateTowards(Transform target)
    {
        // Rotate smoothly while walking
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                turnSpeed * Time.deltaTime
            );
        }

        // Move forward
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        animator.SetFloat("Speed", 1f); // ✅ walk animation always while moving

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= reachThreshold)
        {
            transform.position = target.position;

            bool isLast = currentWaypoint >= currentRoute.waypoints.Length - 1;

            if (isLast)
            {
                npcState = State.TwistingAtEnd; // walk-while-twist at the end
            }
            else
            {
                currentWaypoint++;
                npcState = State.Walking; // directly continue
            }
        }
    }

    private void RotateTowardsRotation(Quaternion targetRot)
    {
        // rotate while walking animation still plays
        Quaternion flatRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            flatRot,
            turnSpeed * Time.deltaTime
        );

        animator.SetFloat("Speed", 1f); // ✅ keep walk anim during twist

        if (Quaternion.Angle(transform.rotation, flatRot) < 3f)
        {
            routeComplete = true;
            npcState = State.Idle;
            animator.SetFloat("Speed", 0f); // stop walk anim once finished
            Debug.Log("[NPCStoryRoutes] Finished route: " + currentRoute.routeName);
        }
    }

    // ---- PUBLIC API ----

    public void StartRoute(string routeName)
    {
        var route = routes.Find(r => r.routeName == routeName);

        if (route == null || route.waypoints == null || route.waypoints.Length == 0)
        {
            Debug.LogError("[NPCStoryRoutes] Invalid route: " + routeName);
            return;
        }

        currentRoute = route;
        currentWaypoint = 0;

        // Skip WP0 if too close
        float dist = Vector3.Distance(transform.position, currentRoute.waypoints[0].position);
        if (dist < reachThreshold && currentRoute.waypoints.Length > 1)
        {
            Debug.Log("[NPCStoryRoutes] Skipping WP0 (too close), starting at WP1.");
            currentWaypoint = 1;
        }

        routeComplete = false;
        npcState = State.Walking; // ✅ start walking immediately

        animator.SetFloat("Speed", 1f);

        Debug.Log("[NPCStoryRoutes] Starting route: " + routeName);
    }
}
