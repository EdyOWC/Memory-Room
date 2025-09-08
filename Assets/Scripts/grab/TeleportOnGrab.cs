using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportOnGrab : MonoBehaviour
{
    [Header("Destination Point")]
    public Transform targetPoint;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Get the interactor's transform (player rig or hand)
        Transform playerRig = args.interactorObject.transform.root;

        if (targetPoint && playerRig)
            playerRig.position = targetPoint.position;
    }
}
