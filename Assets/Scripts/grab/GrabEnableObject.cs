using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabEnableObjects : MonoBehaviour
{
    [System.Serializable]
    public class TargetObject
    {
        public GameObject target;
        public bool enableOnGrab = true;     // Enable this when grabbed?
        public bool disableOnRelease = true; // Disable again when released?
    }

    [Header("Setup")]
    public XRGrabInteractable grabObject;     // The object you grab
    public TargetObject[] targets;            // Multiple GOs to control

    private void Awake()
    {
        if (grabObject == null)
            grabObject = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabObject.selectEntered.AddListener(OnGrabbed);
        grabObject.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabObject.selectEntered.RemoveListener(OnGrabbed);
        grabObject.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        foreach (var t in targets)
        {
            if (t.target != null && t.enableOnGrab)
                t.target.SetActive(true);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        foreach (var t in targets)
        {
            if (t.target != null && t.disableOnRelease)
                t.target.SetActive(false);
        }
    }
}
