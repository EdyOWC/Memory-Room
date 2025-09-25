using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeightResetUI : MonoBehaviour
{
    [Header("XR Rig Setup")]
    public XROrigin xrOrigin;

    [Header("Target Eye Height (meters)")]
    public float targetEyeHeight = 1.7f;

    public void ResetHeight()
    {
        if (xrOrigin == null || xrOrigin.Camera == null) return;

        // Measure current camera (headset) height relative to the XR Origin
        float currentEyeHeight = xrOrigin.CameraInOriginSpaceHeight;

        // How much to shift
        float difference = targetEyeHeight - currentEyeHeight;

        // Apply offset by moving the CameraFloorOffsetObject
        if (xrOrigin.CameraFloorOffsetObject != null)
        {
            Vector3 pos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
            pos.y += difference;
            xrOrigin.CameraFloorOffsetObject.transform.localPosition = pos;
        }

        Debug.Log($"Height adjusted. Current eye height: {currentEyeHeight:F2}, Target: {targetEyeHeight}, Shift: {difference:F2}");
    }
}
