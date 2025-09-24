using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerMissionGate : MonoBehaviour
{
    [Header("Drawer Setup")]
    public XRGrabInteractable drawerGrab;
    public bool drawerLockedAtStart = true;
    public Rigidbody drawerRb;

    [Header("Will Setup")]
    public XRGrabInteractable willGrabObject;   // The object inside the drawer (Will)

    [Header("Relock Setup")]
    public Collider relockTrigger;              // The "stopper" in the desk
    public Collider drawerBackMarker;           // Small trigger collider at the back of the drawer

    [Header("Mission Setup")]
    public List<string> missions = new List<string> { "A", "B", "C" };
    private HashSet<string> completedMissions = new HashSet<string>();

    [Header("Feedback")]
    public AudioSource sfxSource;
    public AudioClip stuckDrawerSFX;
    public AudioSource dialogSource;
    public AudioClip stuckDialogClip;

    [Header("Peek Movement")]
    public Transform moveTarget;  // GO to move (e.g. drawer body)
    public Vector3 startPos;      // local pos A
    public Vector3 endPos;        // local pos B
    public float moveDuration = 1.5f;

    [Header("Animation Setup")]
    public Animator animator;                  // Animator that controls DARKER/AfterWillGrab
    public string relockTriggerName = "Dark";  // Trigger name for DARKER animation

    private bool dialogPlayed = false;
    private bool isUnlocked = false;
    private bool isRelocked = false;
    private bool isMoving = false;
    private float moveTimer = 0f;

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        if (drawerRb == null)
            drawerRb = GetComponent<Rigidbody>();

        if (drawerLockedAtStart)
            LockDrawer();

        if (drawerGrab != null)
            drawerGrab.selectEntered.AddListener(_ => OnGrabAttempt());

        // Will should NOT be grabbable at start
        if (willGrabObject != null)
            willGrabObject.enabled = false;

        if (moveTarget != null)
            moveTarget.localPosition = startPos;
    }

    private void OnGrabAttempt()
    {
        if (!isUnlocked)
        {
            if (sfxSource != null && stuckDrawerSFX != null)
                sfxSource.PlayOneShot(stuckDrawerSFX);

            if (!dialogPlayed && dialogSource != null && stuckDialogClip != null)
            {
                dialogSource.PlayOneShot(stuckDialogClip);
                dialogPlayed = true;
            }
        }
    }

    private void LockDrawer()
    {
        if (drawerRb != null)
            drawerRb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnlockDrawer()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        isRelocked = false;

        if (drawerRb != null)
        {
            drawerRb.constraints = RigidbodyConstraints.FreezePositionX |
                                   RigidbodyConstraints.FreezePositionY |
                                   RigidbodyConstraints.FreezeRotation;
        }

        Debug.Log("✅ All missions completed — Drawer unlocked!");
        StartPeekMove();
    }

    private void RelockDrawer()
    {
        if (!isUnlocked) return; // ignore if never unlocked

        LockDrawer();
        isRelocked = true;

        // Now the Will becomes grabbable
        if (willGrabObject != null)
            willGrabObject.enabled = true;

        // 🔹 Instead of enabling a GO, play the DARKER animation
        if (animator != null && !string.IsNullOrEmpty(relockTriggerName))
            animator.SetTrigger(relockTriggerName);

        Debug.Log("🔒 Drawer re-locked — DARKER animation triggered, Will enabled!");
    }

    private void StartPeekMove()
    {
        if (moveTarget != null)
        {
            moveTimer = 0f;
            isMoving = true;
        }
    }

    public void CompleteMission(string missionName)
    {
        if (!missions.Contains(missionName))
        {
            Debug.LogWarning($"Mission {missionName} is not part of this drawer’s requirements.");
            return;
        }

        if (completedMissions.Add(missionName))
        {
            Debug.Log($"Mission {missionName} completed!");
            if (completedMissions.Count == missions.Count)
                UnlockDrawer();
        }
    }

    void Update()
    {
        if (isMoving && moveTarget != null)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            float easedT = Mathf.SmoothStep(0, 1, t);
            moveTarget.localPosition = Vector3.Lerp(startPos, endPos, easedT);

            if (t >= 1f)
                isMoving = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Relock only if the back marker touches the stopper
        if (drawerBackMarker != null && relockTrigger != null)
        {
            if (other == relockTrigger)
            {
                RelockDrawer();
            }
        }
    }
}
