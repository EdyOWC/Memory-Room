using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabAttachSwitcher : MonoBehaviour
{
    [Header("Attach Points")]
    public Transform leftHandAttach;
    public Transform rightHandAttach;

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
        if (interactor == null) return;

        XRController controller = interactor.GetComponent<XRController>();
        if (controller == null) return;

        if (controller.controllerNode == XRNode.LeftHand && leftHandAttach != null)
        {
            grab.attachTransform = leftHandAttach;
            Debug.Log("Using LEFT attach point");
        }
        else if (controller.controllerNode == XRNode.RightHand && rightHandAttach != null)
        {
            grab.attachTransform = rightHandAttach;
            Debug.Log("Using RIGHT attach point");
        }
    }
}
