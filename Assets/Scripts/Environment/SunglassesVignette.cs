using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SunglassesVignette : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable grabInteractable;
    public Canvas vignetteCanvas;
    public Image vignetteImage;
    public Collider headTrigger;     // Sphere collider around XR camera
    public Transform head;           // XR Camera (head)

    [Header("Settings")]
    public Vector3 wornOffset = new Vector3(0f, -0.05f, 0.1f);
    public float fadeSpeed = 5f;

    private bool isGrabbed = false;
    private bool isInsideHeadTrigger = false;
    private bool isWorn = false;
    private float targetAlpha = 0f;

    private Rigidbody rb;
    private Transform originalParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        if (vignetteCanvas != null)
            vignetteCanvas.enabled = true;

        SetAlpha(0f);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void Update()
    {
        targetAlpha = isWorn ? 1f : 0f;

        if (vignetteImage != null)
        {
            Color c = vignetteImage.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            vignetteImage.color = c;
        }
    }

    // ===== Grab + Release =====
    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        if (isWorn)
        {
            // Taking them off
            isWorn = false;
            targetAlpha = 0f;
            RestorePhysics();
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

        if (isInsideHeadTrigger)
            WearGlasses();
        else
            RestorePhysics();
    }

    // ===== Trigger Detection =====
    private void OnTriggerEnter(Collider other)
    {
        if (other == headTrigger)
            isInsideHeadTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == headTrigger)
            isInsideHeadTrigger = false;
    }

    // ===== Wear & Remove =====
    private void WearGlasses()
    {
        isWorn = true;

        // Parent to head so they move with it
        transform.SetParent(head);
        transform.localPosition = wornOffset;
        transform.localRotation = Quaternion.identity;

        if (rb != null)
            rb.isKinematic = true;
    }

    private void RestorePhysics()
    {
        if (rb != null)
            rb.isKinematic = false;

        // Detach from head if still parented
        if (transform.parent == head)
            transform.SetParent(originalParent);
    }

    // ===== Helpers =====
    private void SetAlpha(float value)
    {
        if (vignetteImage == null) return;
        Color c = vignetteImage.color;
        c.a = Mathf.Clamp01(value);
        vignetteImage.color = c;
    }
}
