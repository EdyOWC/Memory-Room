using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class XRLocomotionLock : MonoBehaviour
{
    [Header("Providers to lock/unlock")]
    public LocomotionProvider[] providersToLock;

    [Header("Extra components to lock")]
    public Behaviour[] extraComponentsToLock;

    public bool isLocked = true;

    void OnEnable()
    {
        ApplyLock();
    }

    void OnDisable()
    {
        Unlock();
    }

    public void ApplyLock()
    {
        foreach (var p in providersToLock)
            if (p) p.enabled = false;

        foreach (var c in extraComponentsToLock)
            if (c) c.enabled = false;
    }

    public void Unlock()
    {
        foreach (var p in providersToLock)
            if (p) p.enabled = true;

        foreach (var c in extraComponentsToLock)
            if (c) c.enabled = true;
    }
}
