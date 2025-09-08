using UnityEngine;

public class GazeInteractor : MonoBehaviour
{
    [Header("Ray Settings")]
    public float maxDistance = 10f;           // how far the ray can reach
    public LayerMask interactableLayer;       // which layers to check

    [Header("Gaze Settings")]
    public float gazeTime = 2f;               // how long to look before triggering
    private float gazeTimer;

    private GameObject currentTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            if (hit.collider.gameObject != currentTarget)
            {
                // reset if we look at a new object
                gazeTimer = 0f;
                currentTarget = hit.collider.gameObject;
            }

            gazeTimer += Time.deltaTime;

            if (gazeTimer >= gazeTime)
            {
                // trigger action on the object
                ILookAt interactable = currentTarget.GetComponent<ILookAt>();
                if (interactable != null)
                {
                    interactable.OnLook();
                }

                gazeTimer = 0f; // reset to allow repeat
            }
        }
        else
        {
            // not looking at anything
            currentTarget = null;
            gazeTimer = 0f;
        }
    }
}

// 👁️ Interface for objects that react to being looked at
public interface ILookAt
{
    void OnLook();
}
