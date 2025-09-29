using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LightTriggerZone : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;           // spotlight to affect
    public float newIntensity = 5f;     // intensity after trigger
    public float newRange = 20f;        // range after trigger
    public float transitionTime = 1f;   // smooth transition time

    [Header("Player Detection")]
    public Transform playerRoot;        // XR Origin or player object
    public string playerTag = "Player"; // fallback

    private float startIntensity;
    private float startRange;
    private bool inTransition = false;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (targetLight != null)
        {
            startIntensity = targetLight.intensity;
            startRange = targetLight.range;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other) && targetLight != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeLight(targetLight.intensity, newIntensity,
                                       targetLight.range, newRange));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other) && targetLight != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeLight(targetLight.intensity, startIntensity,
                                       targetLight.range, startRange));
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (playerRoot != null)
        {
            Transform t = other.transform;
            while (t.parent != null) t = t.parent;
            if (t == playerRoot) return true;
        }
        return !string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag);
    }

    private System.Collections.IEnumerator ChangeLight(float fromIntensity, float toIntensity,
                                                       float fromRange, float toRange)
    {
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);

            targetLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
            targetLight.range = Mathf.Lerp(fromRange, toRange, t);

            yield return null;
        }
    }
}
