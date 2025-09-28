using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float fallThresholdY = -5f;     // If object goes below this Y, respawn
    public Transform respawnPoint;         // Where to respawn (can be same GO's start position)

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        // If no respawn point is set, use the current transform as the default
        if (respawnPoint == null)
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }
        else
        {
            startPosition = respawnPoint.position;
            startRotation = respawnPoint.rotation;
        }
    }

    void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Reset position & rotation
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Also reset velocity if the GO has a Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
