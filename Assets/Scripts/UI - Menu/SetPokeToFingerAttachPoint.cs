using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SetPokeToFingerAttachPoint : MonoBehaviour
{
    [Header("Assign the finger attach point here")]
    public Transform PokeAttachPoint;

    private XRPokeInteractor _xrPokeInteractor;

    void Start()
    {
        // Look up the XRPokeInteractor in the parent hierarchy
        _xrPokeInteractor = transform.parent.parent.GetComponentInChildren<XRPokeInteractor>();

        SetPokeAttachPoint();
    }

    void SetPokeAttachPoint()
    {
        if (PokeAttachPoint == null)
        {
            Debug.Log("Poke Attach Point is null");
            return;
        }

        if (_xrPokeInteractor == null)
        {
            Debug.Log("XR Poke Interactor is null");
            return;
        }

        // Assign the attach transform
        _xrPokeInteractor.attachTransform = PokeAttachPoint;
    }
}
