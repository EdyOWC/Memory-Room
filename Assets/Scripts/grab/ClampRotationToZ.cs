using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ClampRotationToZ : MonoBehaviour
{
    XRGrabInteractable grab;
    Vector3 baseEuler; // locked X/Y
    public float minZ = -30f;
    public float maxZ = 40f;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        baseEuler = transform.eulerAngles; // e.g., your X=0, Y=180

        // Update baseEuler whenever grabbed
        grab.selectEntered.AddListener(_ => baseEuler = transform.eulerAngles);
    }

    void LateUpdate()
    {
        // Get current Z from object
        float currentZ = transform.eulerAngles.z;

        // Convert to delta from "zero" reference (baseEuler.z)
        float deltaZ = Mathf.DeltaAngle(baseEuler.z, currentZ);

        // Clamp the delta
        deltaZ = Mathf.Clamp(deltaZ, minZ, maxZ);

        // Apply back, locking X/Y
        float clampedZ = baseEuler.z + deltaZ;
        transform.rotation = Quaternion.Euler(baseEuler.x, baseEuler.y, clampedZ);
    }
}
