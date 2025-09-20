using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerMissionGate : MonoBehaviour
{
    [Header("Drawer Setup")]
    public XRGrabInteractable drawerGrab;
    public bool drawerLockedAtStart = true;

    [Header("Extra Handle Setup")]
    public XRGrabInteractable extraGrabObject;

    [Header("Mission Setup")]
    public List<string> missions = new List<string> { "A", "B", "C" };
    private HashSet<string> completedMissions = new HashSet<string>();

    [Header("Feedback")]
    public AudioSource sfxSource;
    public AudioClip stuckDrawerSFX;
    public AudioSource dialogSource;
    public AudioClip stuckDialogClip;

    [Header("Peek Movement")]
    public Transform moveTarget;               // GO to move (e.g. drawer body)
    public Vector3 startPos;                   // local pos A
    public Vector3 endPos;                     // local pos B
    public float moveDuration = 1.5f;

    private bool dialogPlayed = false;
    private bool isUnlocked = false;
    private Rigidbody rb;
    private bool isMoving = false;
    private float moveTimer = 0f;

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (drawerLockedAtStart)
            LockDrawer();

        if (drawerGrab != null)
            drawerGrab.selectEntered.AddListener(_ => OnGrabAttempt());

        if (extraGrabObject != null)
            extraGrabObject.enabled = false;

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
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeAll;
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
            CheckAllMissions();
        }
    }

    private void CheckAllMissions()
    {
        if (completedMissions.Count == missions.Count)
            UnlockDrawer();
    }

    private void UnlockDrawer()
    {
        if (isUnlocked) return;

        isUnlocked = true;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezeRotation;
        }

        if (extraGrabObject != null)
            extraGrabObject.enabled = true;

        if (moveTarget != null)
        {
            moveTimer = 0f;
            isMoving = true;
        }

        Debug.Log("✅ All missions completed — Drawer unlocked (moving)!");
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
}
