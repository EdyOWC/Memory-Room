using UnityEngine;

public class FollowDrawerWithoutParenting : MonoBehaviour
{
    public Transform drawerTransform;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    void Start()
    {
        if (drawerTransform == null)
        {
            Debug.LogError("Drawer transform not assigned!");
            enabled = false;
            return;
        }

        // Store initial local offset
        initialLocalPosition = transform.position - drawerTransform.position;
        initialLocalRotation = Quaternion.Inverse(drawerTransform.rotation) * transform.rotation;
    }

    void LateUpdate()
    {
        // Match position & rotation
        transform.position = drawerTransform.position + drawerTransform.rotation * initialLocalPosition;
        transform.rotation = drawerTransform.rotation * initialLocalRotation;
    }
}
