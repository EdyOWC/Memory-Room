using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorPopOffSimpleKeepScale : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable doorGrab;   // Door root
    public XRGrabInteractable handleGrab; // Handle (child)
    public Rigidbody doorRb;

    [Header("Pop Settings")]
    public float popForce = 1.5f;
    public float popTorque = 0.75f;
    public bool disableHandleAfterPop = true;

    private bool popped;
    private XRInteractionManager im;

    private Quaternion initialRot;
    private Vector3 initialScale;

    private void Awake()
    {
        if (doorRb == null) doorRb = GetComponent<Rigidbody>();
        if (doorGrab == null) doorGrab = GetComponent<XRGrabInteractable>();
        if (im == null) im = FindFirstObjectByType<XRInteractionManager>();

        // store clean values
        initialRot = transform.localRotation;
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (handleGrab != null)
            handleGrab.selectEntered.AddListener(OnHandleGrabbed);

        if (doorGrab != null)
        {
            doorGrab.selectEntered.AddListener(OnDoorGrab);
            doorGrab.selectExited.AddListener(OnDoorRelease);
        }
    }

    private void OnDisable()
    {
        if (handleGrab != null)
            handleGrab.selectEntered.RemoveListener(OnHandleGrabbed);

        if (doorGrab != null)
        {
            doorGrab.selectEntered.RemoveListener(OnDoorGrab);
            doorGrab.selectExited.RemoveListener(OnDoorRelease);
        }
    }

    private void LateUpdate()
    {
        // Every frame: force door scale back to original
        if (transform.localScale != initialScale)
            transform.localScale = initialScale;
    }

    private void OnHandleGrabbed(SelectEnterEventArgs args)
    {
        if (popped) return;
        popped = true;

        transform.SetParent(null, true); // detach

        doorRb.isKinematic = false;
        doorRb.useGravity = true;

        // add funny physics nudge
        doorRb.AddForce(transform.right * popForce, ForceMode.VelocityChange);
        doorRb.AddTorque(Random.onUnitSphere * popTorque, ForceMode.VelocityChange);

        // transfer grab from handle → door
        if (im != null && doorGrab != null)
        {
            im.SelectExit(args.interactorObject, handleGrab);
            im.SelectEnter(args.interactorObject, doorGrab);
        }

        if (disableHandleAfterPop)
            handleGrab.enabled = false;
    }

    private void OnDoorGrab(SelectEnterEventArgs args)
    {
        transform.localScale = initialScale;
    }

    private void OnDoorRelease(SelectExitEventArgs args)
    {
        transform.localScale = initialScale;
        transform.localRotation = initialRot;
    }
}
