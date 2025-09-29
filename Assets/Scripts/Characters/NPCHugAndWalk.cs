using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class NPCHugAndWalk : MonoBehaviour
{
    [Header("Who is the player?")]
    [Tooltip("Optional: drag your XR Origin (root). If left empty, we'll use the tag below.")]
    public Transform playerRoot;
    [Tooltip("Fallback tag to detect the player collider.")]
    public string playerTag = "Player";

    [Header("Freeze player during hug by disabling these components")]
    [Tooltip("Drag your locomotion components here, e.g. ActionBasedContinuousMoveProvider, ContinuousTurnProvider, TeleportationProvider, etc.")]
    public MonoBehaviour[] locomotionToDisable;

    [Header("NPC")]
    public Animator npcAnimator;
    [Tooltip("Trigger name for hug animation in the NPC Animator.")]
    public string hugTrigger = "Hug";
    [Tooltip("Bool name for walk state in the NPC Animator.")]
    public string walkBool = "IsWalking";

    [Header("Hug")]
    [Tooltip("Duration to wait if you don't use an Animation Event.")]
    public float hugDuration = 3f;

    [Header("Walk Path")]
    public Transform[] waypoints;
    public float npcMoveSpeed = 1.5f;
    public float npcTurnSpeed = 5f;

    [Header("Trigger")]
    [Tooltip("If true, this trigger only fires once.")]
    public bool oneShot = true;

    // runtime
    private bool triggered = false;
    private bool walking = false;
    private bool hugFinished = false;
    private int currentTarget = 0;
    private Collider triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider>();
        triggerCol.isTrigger = true; // ensure it's a trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (!IsPlayer(other)) return;

        triggered = true;
        StartCoroutine(HugThenWalkRoutine());
    }

    private bool IsPlayer(Collider other)
    {
        // Prefer explicit player root reference
        if (playerRoot != null)
        {
            Transform t = other.transform;
            while (t.parent != null) t = t.parent;
            if (t == playerRoot) return true;
        }
        // Fallback to tag
        return !string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag);
    }

    private IEnumerator HugThenWalkRoutine()
    {
        // Freeze player locomotion
        ToggleLocomotion(false);

        // Play hug
        if (npcAnimator) npcAnimator.SetTrigger(hugTrigger);

        // Wait for duration (you can also call OnHugFinished from an Animation Event to end earlier)
        yield return new WaitForSeconds(hugDuration);

        if (!hugFinished) OnHugFinished();
    }

    /// <summary>
    /// Optional: call this from an Animation Event at the end of the hug clip.
    /// </summary>
    public void OnHugFinished()
    {
        if (hugFinished) return;
        hugFinished = true;

        // Unfreeze player
        ToggleLocomotion(true);

        // Start NPC walk
        StartWalk();
    }

    private void ToggleLocomotion(bool enable)
    {
        if (locomotionToDisable == null) return;
        foreach (var comp in locomotionToDisable)
        {
            if (comp == null) continue;
            comp.enabled = enable;
        }
    }

    private void StartWalk()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        walking = true;
        currentTarget = 0;
        if (npcAnimator) npcAnimator.SetBool(walkBool, true);
    }

    void Update()
    {
        if (!walking || waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentTarget];
        Vector3 to = target.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(to.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * npcTurnSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, npcMoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentTarget++;
            if (currentTarget >= waypoints.Length)
            {
                walking = false;
                if (npcAnimator) npcAnimator.SetBool(walkBool, false);
            }
        }
    }

    // Draw path in editor
    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
