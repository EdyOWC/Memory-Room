using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MissionOnGrab : MonoBehaviour
{
    [Header("Mission Gate")]
    public DrawerMissionGate missionGate;  // Drag your drawer (gate) here
    public string missionName = "X";       // Mission ID (A, B, C, etc.)

    private XRGrabInteractable grab;
    private bool missionCompleted = false;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
        }
    }

    private void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!missionCompleted && missionGate != null)
        {
            missionGate.CompleteMission(missionName);
            missionCompleted = true;
            Debug.Log($"Mission {missionName} completed by grabbing {gameObject.name}!");
        }
    }
}
