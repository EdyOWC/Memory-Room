using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class LevelablePicture : MonoBehaviour
{
    [Header("Target & Start")]
    [Tooltip("Angle considered perfectly level (world/local Z). Usually 0.")]
    public float targetZ = 0f;

    [Tooltip("If true, set the starting Z to startZ on play.")]
    public bool setStartOnPlay = true;

    [Tooltip("Initial Z (degrees) to start at, if setStartOnPlay is true.")]
    public float startZ = 22f;

    [Header("Detection")]
    [Tooltip("How close (degrees) counts as 'level'.")]
    public float tolerance = 2f;

    [Tooltip("Snap exactly to targetZ when within tolerance.")]
    public bool snapWhenLevel = true;

    [Tooltip("Only invoke once (first time it becomes level).")]
    public bool invokeOnce = true;

    [Header("Snap Behaviour")]
    [Tooltip("How fast to smooth-snap to the target.")]
    public float snapSpeed = 10f;

    [Header("Feedback")]
    public UnityEvent onLeveled;           // Hook up sound, VFX, etc.
    public AudioSource clickSound;         // Optional, plays on success

    [Header("Scale Lock")]
    [Tooltip("World-space size the object should maintain.")]
    public Vector3 desiredWorldScale = Vector3.one;

    private bool hasInvoked;
    private XRGrabInteractable grabInteractable;
    private bool isSnapping = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (setStartOnPlay)
        {
            Vector3 e = transform.eulerAngles;
            e.z = startZ;
            transform.eulerAngles = e;
        }

        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void LateUpdate()
    {
        MaintainWorldScale();
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        float currentZ = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentZ, targetZ);

        if (Mathf.Abs(delta) <= tolerance)
        {
            if (snapWhenLevel)
                StartCoroutine(SmoothSnapToTarget());

            if (!hasInvoked || !invokeOnce)
            {
                onLeveled?.Invoke();
                if (clickSound) clickSound.Play();
                if (invokeOnce) hasInvoked = true;
            }
        }
    }

    IEnumerator SmoothSnapToTarget()
    {
        isSnapping = true;
        Vector3 startRot = transform.eulerAngles;
        Vector3 targetRot = new Vector3(startRot.x, startRot.y, targetZ);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * snapSpeed;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, t);
            yield return null;
        }

        isSnapping = false;
    }

    void MaintainWorldScale()
    {
        Vector3 lossy = transform.lossyScale;
        transform.localScale = new Vector3(
            desiredWorldScale.x / lossy.x * transform.localScale.x,
            desiredWorldScale.y / lossy.y * transform.localScale.y,
            desiredWorldScale.z / lossy.z * transform.localScale.z
        );
    }
}
