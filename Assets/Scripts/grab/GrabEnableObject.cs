using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabEnableObjects : MonoBehaviour
{
    [System.Serializable]
    public class TargetObject
    {
        public GameObject target;
        public bool enableOnGrab = true;      // Enable this when grabbed
        public bool disableOnRelease = false; // Optionally disable when released
    }

    [Header("Setup")]
    public XRGrabInteractable grabObject;    // The Will (OdedsWill)

    [Header("Animation Setup")]
    public Animator animator;                // Animator (Environment_1)
    public string animationTrigger = "Will"; // Trigger name in Animator

    [Header("Skybox Setup")]
    public Material newSkybox;               // Skybox to switch to
    private Material originalSkybox;         // Save original for possible revert

    [Header("Target GOs Setup")]
    public TargetObject[] targets;           // GOs to enable/disable

    private void Awake()
    {
        if (grabObject == null)
            grabObject = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabObject != null)
        {
            grabObject.selectEntered.AddListener(OnGrabbed);
            grabObject.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabObject != null)
        {
            grabObject.selectEntered.RemoveListener(OnGrabbed);
            grabObject.selectExited.RemoveListener(OnReleased);
        }
    }

    private void Start()
    {
        // Save the current skybox so we can revert later if needed
        originalSkybox = RenderSettings.skybox;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("📜 Will grabbed — trigger animation, change skybox, enable GOs");

        // 🔹 Trigger Animator
        if (animator != null && !string.IsNullOrEmpty(animationTrigger))
            animator.SetTrigger(animationTrigger);

        // 🔹 Change Skybox
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
        }

        // 🔹 Enable configured GOs
        foreach (var t in targets)
        {
            if (t.target != null && t.enableOnGrab)
                t.target.SetActive(true);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("📜 Will released — disable marked GOs");

        // 🔹 Disable configured GOs
        foreach (var t in targets)
        {
            if (t.target != null && t.disableOnRelease)
                t.target.SetActive(false);
        }
    }
}
