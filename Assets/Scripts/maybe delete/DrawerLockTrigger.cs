using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerLockTrigger : MonoBehaviour
{
    [Header("Drawer Setup")]
    public Transform drawerTransform;
    public XRGrabInteractable grabInteractable;
    public Rigidbody rb;

    [Header("Lock Settings")]
    public float openDistance = 0.25f; // How far the drawer can move along X
    public bool disableGrabAfterLock = true;

    [Header("Trigger Setup")]
    public GameObject activateOnLock;

    private bool locked = false;
    private float startX;

    void Start()
    {
        if (drawerTransform != null)
            startX = drawerTransform.localPosition.x;  // record the closed X position
    }

    void Update()
    {
        if (locked || drawerTransform == null) return;

        float moved = Mathf.Abs(drawerTransform.localPosition.x - startX);

        if (moved >= openDistance)
        {
            LockDrawer();
        }
    }

    void LockDrawer()
    {
        locked = true;

        // Freeze drawer at current position
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Disable grabbing if required
        if (grabInteractable != null && disableGrabAfterLock)
            grabInteractable.enabled = false;

        // Activate event/GO
        if (activateOnLock != null)
            activateOnLock.SetActive(true);

        Debug.Log("Drawer locked at open distance along X-axis.");
    }
}
