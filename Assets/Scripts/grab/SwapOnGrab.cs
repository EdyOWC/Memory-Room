using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SwapOnGrab : MonoBehaviour
{
    [Header("Objects to Disable on Grab")]
    public GameObject[] objectsToDisable;

    [Header("Objects to Enable on Grab")]
    public GameObject[] objectsToEnable;

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        // Listen for grab
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        foreach (GameObject go in objectsToDisable)
            if (go) go.SetActive(false);

        foreach (GameObject go in objectsToEnable)
            if (go) go.SetActive(true);
    }
}
