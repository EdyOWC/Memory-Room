using UnityEngine;

public class TriggerActivatorOneTime : MonoBehaviour
{
    [Header("Objects to Enable/Disable")]
    public GameObject[] objectsToEnable;
    public GameObject[] objectsToDisable;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;   // already used once

        // only react if the entering object is tagged "Player"
        if (!other.CompareTag("Player")) return;

        foreach (var go in objectsToEnable)
            if (go != null) go.SetActive(true);

        foreach (var go in objectsToDisable)
            if (go != null) go.SetActive(false);

        hasTriggered = true;
        Debug.Log("Trigger zone activated once by Player → toggled objects.");
    }
}
