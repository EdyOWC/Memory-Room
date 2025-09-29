using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class NPCHugAndWalkSimple : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public string playerTag = "Player";

    [Header("NPC")]
    public Animator npcAnimator;
    public string hug1Trigger = "Hug";   // starts Hug-1
    public string hug2Trigger = "Hug2";  // starts Hug-2

    [Header("Target")]
    public Transform walkTarget;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 5f;

    [Header("Hug Settings")]
    public float hug1Duration = 3f;   // actual Hug-1 clip length
    public float hugBuffer = 2f;      // extra hold time before Hug-2
    public AudioSource audioSource;
    public AudioClip hugClip;

    [Header("Trigger")]
    public bool oneShot = true;

    private bool triggered = false;
    private bool walking = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (IsPlayer(other))
        {
            triggered = true;
            StartCoroutine(HugSequence());
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (playerRoot != null)
        {
            Transform t = other.transform;
            while (t.parent != null) t = t.parent;
            if (t == playerRoot) return true;
        }
        return !string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag);
    }

    private IEnumerator HugSequence()
    {
        // Start Hug-1
        if (npcAnimator) npcAnimator.SetTrigger(hug1Trigger);

        // Play sound
        if (audioSource != null && hugClip != null)
            audioSource.PlayOneShot(hugClip);

        // Wait Hug-1 duration
        yield return new WaitForSeconds(hug1Duration);

        // Wait extra buffer
        yield return new WaitForSeconds(hugBuffer);

        // Now trigger Hug-2 manually
        if (npcAnimator) npcAnimator.SetTrigger(hug2Trigger);

        // After Hug-2/Turn anims, walking will begin automatically
        yield return null;
    }

    private void Update()
    {
        if (!walking || walkTarget == null) return;

        Vector3 toTarget = walkTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            walkTarget.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, walkTarget.position) < 0.1f)
        {
            walking = false;
        }
    }

    // Call this from an Animation Event on "Walk2_com"
    public void BeginWalking()
    {
        walking = true;
    }
}
