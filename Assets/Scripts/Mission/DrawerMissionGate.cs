



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
    public XRGrabInteractable extraGrabObject; // <- assign separate GO here

    [Header("Mission Setup")]
    public List<string> missions = new List<string> { "A", "B", "C" };
    private HashSet<string> completedMissions = new HashSet<string>();

    [Header("Feedback")]
    public AudioSource sfxSource;
    public AudioClip stuckDrawerSFX;
    public AudioSource dialogSource;
    public AudioClip stuckDialogClip;

    private bool dialogPlayed = false;
    private bool isUnlocked = false;
    private Rigidbody rb;

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();

        if (drawerGrab != null)
        {
            if (drawerLockedAtStart)
                LockDrawer();

            drawerGrab.selectEntered.AddListener(_ => OnGrabAttempt());
        }

        if (extraGrabObject != null)
        {
            // Keep it disabled until drawer is unlocked
            extraGrabObject.enabled = false;
        }
    }

    private void OnGrabAttempt()
    {
        if (!isUnlocked)
        {
            // Play SFX every time
            if (sfxSource != null && stuckDrawerSFX != null)
                sfxSource.PlayOneShot(stuckDrawerSFX);

            // Play dialog only first time
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

    // Call this from mission scripts
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
        if (!isUnlocked)
        {
            isUnlocked = true;

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX |
                                 RigidbodyConstraints.FreezePositionY |
                                 RigidbodyConstraints.FreezeRotation;
            }

            if (extraGrabObject != null)
                extraGrabObject.enabled = true;

            Debug.Log("✅ All missions completed — Drawer unlocked!");
        }
    }
}


