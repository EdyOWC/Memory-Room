using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShirtSwapOnGrab : MonoBehaviour
{
    public GameObject unfoldedShirt; // Assign in Inspector
    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Cast to new interfaces
        var interactor = (IXRSelectInteractor)args.interactorObject;
        var unfoldedGrab = unfoldedShirt.GetComponent<XRGrabInteractable>();

        // Match position/rotation
        unfoldedShirt.transform.position = transform.position;
        unfoldedShirt.transform.rotation = transform.rotation;
        unfoldedShirt.SetActive(true);

        // Force attach to the interactor
        unfoldedGrab.interactionManager.SelectEnter(interactor, unfoldedGrab);

        // Disable this shirt
        gameObject.SetActive(false);
    }
}
