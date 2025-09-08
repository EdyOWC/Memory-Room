using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NoteGrabTrigger : MonoBehaviour
{
    [Tooltip("The XR Grab Interactable component on this note.")]
    public XRGrabInteractable grabInteractable;

    [Tooltip("Pins to disable kinematic (loosen) when this note is grabbed.")]
    public List<Rigidbody> targetPins;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // Listen to grab event only
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        foreach (var pin in targetPins)
        {
            if (pin != null)
                pin.isKinematic = false;  // loosen pin on grab
        }
    }
}
