using UnityEngine;

public class DisableOnMouthParticleHit : MonoBehaviour
{
    // The layer name of the colliding objects
    public string targetLayerName = "Default";

    private int targetLayer;

    void Start()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer == -1)
        {
            Debug.LogError("Layer " + targetLayerName + " not found!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == targetLayer)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            gameObject.SetActive(false);
        }
    }
}
