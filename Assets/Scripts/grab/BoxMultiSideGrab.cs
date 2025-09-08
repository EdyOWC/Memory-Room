using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class BoxMultiSideGrab : MonoBehaviour
{
    public Transform attach;                 // the single Attach transform on the grab interactable
    public List<Transform> grabPoints;       // one per side (GrabPoint_Top, etc.)

    XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (!attach) attach = grab.attachTransform;
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Position of the interactor hand/controller
        var interactor = args.interactorObject as IXRSelectInteractor;
        Transform interactorTf = (interactor as Component)?.transform;
        Vector3 refPos = interactorTf ? interactorTf.position : transform.position;

        // Pick the nearest grab point to that hand
        Transform best = null;
        float bestSqr = float.PositiveInfinity;
        foreach (var gp in grabPoints)
        {
            if (!gp) continue;
            float d = (gp.position - refPos).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = gp; }
        }

        if (best)
        {
            attach.SetPositionAndRotation(best.position, best.rotation);
        }
    }
}
