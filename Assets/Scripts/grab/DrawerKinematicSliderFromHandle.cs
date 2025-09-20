using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class DrawerHybridSliderFromHandle : MonoBehaviour
{
    [Header("Assign the drawer to move")]
    public Rigidbody drawerRb;                      // Rigidbody on Drawer.004
    public Transform drawer;                        // Drawer.004 Transform

    [Header("Slide axis & limits (in drawer local space)")]
    public Vector3 axisLocal = Vector3.right;       // use Vector3.left if opposite
    public float min = 0f;                          // closed offset (meters)
    public float max = 0.30f;                       // open offset (meters)

    [Header("Behavior")]
    public bool snapOnRelease = false;
    [Range(0f, 1f)] public float snapThreshold = 0.5f;
    public float smoothLerp = 12f;

    private XRGrabInteractable grab;
    private IXRSelectInteractor interactor;
    private Vector3 closedLocalPos;
    private Vector3 drawerLocalAtGrab;
    private Vector3 worldAxis;
    private Vector3 interactorWorldAtGrab;
    private bool held;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        closedLocalPos = drawer.localPosition;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        // start closed → kinematic
        drawerRb.isKinematic = true;
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as IXRSelectInteractor;
        interactorWorldAtGrab = args.interactorObject.GetAttachTransform(grab).position;
        drawerLocalAtGrab = drawer.localPosition;
        worldAxis = drawer.TransformDirection(axisLocal.normalized);
        held = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        held = false;
        interactor = null;

        float currentOffset = GetCurrentOffset();

        // if snapping, decide target position
        if (snapOnRelease)
        {
            float t = Mathf.InverseLerp(min, max, currentOffset);
            float target = (t >= snapThreshold) ? max : min;
            drawer.localPosition = closedLocalPos + axisLocal.normalized * target;
            currentOffset = target;
        }

        // after release:
        // - closed → kinematic
        // - open → dynamic (physics reacts to bumps)
        if (Mathf.Approximately(currentOffset, min))
            drawerRb.isKinematic = true;    // locked shut
        else
            drawerRb.isKinematic = false;   // free to bump
    }

    void Update()
    {
        if (held && interactor != null)
        {
            // force kinematic while dragging
            drawerRb.isKinematic = true;

            Vector3 currentAttachPos = interactor.GetAttachTransform(grab).position;
            float deltaOnAxis = Vector3.Dot(currentAttachPos - interactorWorldAtGrab, worldAxis);

            float startOffset = ProjectionAlongAxis(drawerLocalAtGrab - closedLocalPos);
            float unclamped = startOffset + deltaOnAxis;
            float clamped = Mathf.Clamp(unclamped, min, max);

            Vector3 targetLocal = closedLocalPos + axisLocal.normalized * clamped;

            if (smoothLerp > 0f)
                drawer.localPosition = Vector3.Lerp(
                    drawer.localPosition, targetLocal,
                    1f - Mathf.Exp(-smoothLerp * Time.deltaTime)
                );
            else
                drawer.localPosition = targetLocal;
        }

        // 🔒 keep handle stuck to drawer
        if (drawer != null)
        {
            transform.SetParent(drawer, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    float ProjectionAlongAxis(Vector3 localVec)
    {
        var n = axisLocal.normalized;
        return Vector3.Dot(localVec, n);
    }

    float GetCurrentOffset()
    {
        return ProjectionAlongAxis(drawer.localPosition - closedLocalPos);
    }
}
