using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class NextSongButton : MonoBehaviour
{
    public PlaylistManager playlist;

    XRGrabInteractable grab;
    Rigidbody rb;

    void Awake()
    {
        // Rigidbody so it’s an interactable, but lock it down
        rb = gameObject.GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;                 // physics won’t move it
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Grabbable that doesn’t move when held
        grab = gameObject.GetComponent<XRGrabInteractable>();
        if (!grab) grab = gameObject.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        grab.trackPosition = false;            // **key**: stay put
        grab.trackRotation = false;            // **key**: stay put
        grab.throwOnDetach = false;

        grab.selectEntered.AddListener(_ => OnPressed());
    }

    void OnDestroy()
    {
        if (grab) grab.selectEntered.RemoveAllListeners();
    }

    void OnPressed()
    {
        if (playlist) playlist.Next();
        // (Optional) add a little feedback here (click sound, light flash, etc.)
    }
}
