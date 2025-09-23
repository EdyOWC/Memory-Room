using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
public class DrawerLockAndPriority : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable drawerGrab;       // the drawer's XRGrabInteractable
    public Rigidbody drawerRb;                  // the drawer's rigidbody
    [Tooltip("Trigger collider inside drawer interior that detects hands reaching in.")]
    public Collider priorityZoneTrigger;
    [Tooltip("Collider (isTrigger) placed where you want the drawer to lock.")]
    public Collider lockTrigger;

    [Header("Locking settings")]
    public bool freezeAllConstraintsOnLock = true;
    public bool makeKinematicOnLock = false;

    // internal state
    bool lockedByLockTrigger = false;
    int priorityZoneHands = 0;

    void Reset()
    {
        drawerGrab = GetComponent<XRGrabInteractable>();
        drawerRb = GetComponent<Rigidbody>();
    }

    void OnValidate()
    {
        if (drawerGrab == null) drawerGrab = GetComponent<XRGrabInteractable>();
        if (drawerRb == null) drawerRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (drawerGrab == null) Debug.LogError("DrawerLockAndPriority: drawerGrab not set.");
        if (drawerRb == null) Debug.LogError("DrawerLockAndPriority: drawerRb not set.");
        if (priorityZoneTrigger == null) Debug.LogWarning("DrawerLockAndPriority: priorityZoneTrigger not assigned. Priority feature will be disabled.");
        if (lockTrigger == null) Debug.LogWarning("DrawerLockAndPriority: lockTrigger not assigned.");
    }

    // --- Priority zone logic ---
    public void NotifyPriorityZoneEnter(Collider interactorCollider)
    {
        if (lockedByLockTrigger) return;

        var xrInteractor = interactorCollider.GetComponentInParent<XRBaseInteractor>();
        if (xrInteractor is XRDirectInteractor)
        {
            priorityZoneHands++;
            if (priorityZoneHands == 1)
                SetDrawerTemporarilyUngrabable(true);
        }
    }

    public void NotifyPriorityZoneExit(Collider interactorCollider)
    {
        var xrInteractor = interactorCollider.GetComponentInParent<XRBaseInteractor>();
        if (xrInteractor is XRDirectInteractor)
        {
            priorityZoneHands = Mathf.Max(0, priorityZoneHands - 1);
            if (priorityZoneHands == 0)
                SetDrawerTemporarilyUngrabable(false);
        }
    }

    void SetDrawerTemporarilyUngrabable(bool ungrabable)
    {
        if (drawerGrab == null) return;
        drawerGrab.enabled = !ungrabable;
    }

    // --- Lock trigger logic ---
    public void LockDrawer()
    {
        if (lockedByLockTrigger) return;
        lockedByLockTrigger = true;

        SetDrawerTemporarilyUngrabable(true);

        if (makeKinematicOnLock)
            drawerRb.isKinematic = true;

        if (freezeAllConstraintsOnLock)
            drawerRb.constraints = RigidbodyConstraints.FreezeAll;

        TryForceEndSelection();
    }

    void TryForceEndSelection()
    {
        if (drawerGrab == null) return;

        var selecting = drawerGrab.firstInteractorSelecting;
        if (selecting != null && drawerGrab.interactionManager != null)
        {
            drawerGrab.interactionManager.SelectExit(
                (IXRSelectInteractor)selecting,
                (IXRSelectInteractable)drawerGrab
            );
        }
        else
        {
            drawerGrab.enabled = false; // fallback
        }
    }

    public void UnlockDrawer()
    {
        lockedByLockTrigger = false;
        drawerRb.constraints = RigidbodyConstraints.None;
        drawerRb.isKinematic = false;
        drawerGrab.enabled = true;
    }
}
