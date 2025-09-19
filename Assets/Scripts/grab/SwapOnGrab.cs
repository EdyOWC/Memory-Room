using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SwapOnGrab : MonoBehaviour
{
    [Header("Objects to Disable on Grab")]
    public GameObject[] objectsToDisable;

    [Header("Objects to Enable on Grab")]
    public GameObject[] objectsToEnable;

    [Header("Skybox Settings (Optional)")]
    public Material skyboxToSet;   // leave empty if no skybox change

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        // Listen for grab
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDestroy()
    {
        // Clean up listener
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Toggle objects
        foreach (GameObject go in objectsToDisable)
            if (go) go.SetActive(false);

        foreach (GameObject go in objectsToEnable)
            if (go) go.SetActive(true);

        // Change skybox
        if (skyboxToSet != null)
        {
            RenderSettings.skybox = skyboxToSet;
            DynamicGI.UpdateEnvironment();
            Debug.Log("Skybox switched to: " + skyboxToSet.name);
        }
    }
}
