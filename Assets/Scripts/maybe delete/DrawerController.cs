using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerController : MonoBehaviour
{
    [Header("Drawer Setup")]
    public Rigidbody drawerRb;
    public XRGrabInteractable drawerGrab;
    public Transform drawerTransform;

    [Header("Movement Limits")]
    public float minX = 0f;   // closed position (local X)
    public float maxX = 0.3f; // max open position before stopper

    [Header("Stopper Settings")]
    public string stopperTag = "DrawerStopper"; // assign this tag to your stopper GO

    [Header("Unlock Settings")]
    public XRGrabInteractable willGrab; // the hidden Will GO
    public GameObject activateOnLock;   // optional GO (e.g. AnimationManager)
    public bool disableDrawerGrabOnLock = true;

    private bool locked = false;

    void Start()
    {
        if (willGrab != null)
            willGrab.enabled = false; // make sure Will is ungrabbable at start
    }

    void Update()
    {
        if (locked || drawerTransform == null) return;

        // Clamp the drawer so it can’t “sink” past bounds
        Vector3 pos = drawerTransform.localPosition;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        drawerTransform.localPosition = pos;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (locked) return;

        if (collision.gameObject.CompareTag(stopperTag))
        {
            LockDrawer();
        }
    }

    void LockDrawer()
    {
        locked = true;

        // Freeze drawer rigidbody completely
        if (drawerRb != null)
        {
            drawerRb.linearVelocity = Vector3.zero;
            drawerRb.angularVelocity = Vector3.zero;
            drawerRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Disable drawer grab if required
        if (drawerGrab != null && disableDrawerGrabOnLock)
            drawerGrab.enabled = false;

        // Unlock Will grab
        if (willGrab != null)
            willGrab.enabled = true;

        // Activate extra GO (dialog/animation/etc.)
        if (activateOnLock != null)
            activateOnLock.SetActive(true);

        Debug.Log("Drawer locked at stopper → frozen + Will unlocked.");
    }
}
