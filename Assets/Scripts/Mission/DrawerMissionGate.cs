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

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        if (drawerGrab != null && drawerLockedAtStart)
            drawerGrab.enabled = false;
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
            Debug.Log("✅ All missions completed — Drawer unlocked!");
        }
    }
}
