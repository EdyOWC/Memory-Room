using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerMissionGate : MonoBehaviour
{
    [Header("Drawer Setup")]
    public XRGrabInteractable drawerGrab;
    public bool drawerLockedAtStart = true;

    [Header("Mission Setup")]
    public List<string> missions = new List<string> { "A", "B", "C" };
    private HashSet<string> completedMissions = new HashSet<string>();

    private Rigidbody rb;

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();

        if (drawerGrab != null && drawerLockedAtStart)
        {
            drawerGrab.enabled = false;

            // Lock physics so it can’t be pushed before unlocking
            if (rb != null)
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // Call this from mission scripts
    public void CompleteMission(string missionName)
    {
        if (!missions.Contains(missionName))
        {
            Debug.LogWarning($"Mission {missionName} is not part of this drawer’s requirements.");
            return;
        }

        if (!completedMissions.Contains(missionName))
        {
            completedMissions.Add(missionName);
            Debug.Log($"Mission {missionName} completed!");

            CheckAllMissions();
        }
    }

    private void CheckAllMissions()
    {
        if (completedMissions.Count == missions.Count)
        {
            UnlockDrawer();
        }
    }

    private void UnlockDrawer()
    {
        if (drawerGrab != null && !drawerGrab.enabled)
        {
            drawerGrab.enabled = true;

            if (rb != null)
            {
                // Allow movement only on Z (example axis)
                rb.constraints = RigidbodyConstraints.FreezePositionX |
                                 RigidbodyConstraints.FreezePositionY |
                                 RigidbodyConstraints.FreezeRotation;
            }

            Debug.Log("✅ All missions completed — Drawer unlocked!");
        }
    }
}
