using UnityEngine;

public class DoorStartTransform : MonoBehaviour
{
    [Header("Set desired start position/rotation/scale here")]
    public Transform startTransform;

    [Header("Apply on Start")]
    public bool applyOnAwake = true; // ensures it happens before physics kicks in
    public bool applyOnStart = false;

    private void Awake()
    {
        if (applyOnAwake && startTransform != null)
        {
            ApplyStartTransform();
        }
    }

    private void Start()
    {
        if (applyOnStart && startTransform != null)
        {
            ApplyStartTransform();
        }
    }

    [ContextMenu("Apply Start Transform")]
    public void ApplyStartTransform()
    {
        if (startTransform == null) return;

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
        transform.localScale = startTransform.localScale;
    }
}
