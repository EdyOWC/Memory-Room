using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerLimitLock : MonoBehaviour
{
    [Header("Drawer Setup")]
    public Rigidbody drawerRb; // Rigidbody on the drawer
    public XRGrabInteractable drawerGrab; // XR Grab component

    [Header("Lock Options")]
    public bool unlockOnExit = false; // Do you want it to unlock if trigger is left?

    private bool isLocked = false;

    private void Awake()
    {
        if (drawerRb == null)
            drawerRb = GetComponent<Rigidbody>();

        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DrawerLimit"))
        {
            LockDrawer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (unlockOnExit && other.CompareTag("DrawerLimit"))
        {
            UnlockDrawer();
        }
    }

    private void LockDrawer()
    {
        if (drawerRb != null && !isLocked)
        {
            drawerRb.constraints = RigidbodyConstraints.FreezeAll;
            isLocked = true;
        }
    }

    private void UnlockDrawer()
    {
        if (drawerRb != null && isLocked)
        {
            drawerRb.constraints = RigidbodyConstraints.None;
            isLocked = false;
        }
    }
}
