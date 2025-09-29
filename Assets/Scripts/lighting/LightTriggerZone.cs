using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LightTriggerZone : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float brightIntensity = 5f;
    public float brightRange = 20f;
    public float transitionTime = 1f;

    [Header("Player Detection")]
    public Transform playerRoot;
    public string playerTag = "Player";

    [Header("Closing Credits")]
    public GameObject closingCreditsGO;   // Canvas root with CreditsSequence
    public CreditsSequence closingCredits;
    public float exitDelay = 3f;           // time after credits finish

    private bool triggered = false;        // ensure one-time only

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (closingCreditsGO != null)
            closingCreditsGO.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // run once only

        if (IsPlayer(other) && targetLight != null)
        {
            triggered = true;
            StartCoroutine(RunEndSequence());
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

    private IEnumerator RunEndSequence()
    {
        // fade light to bright and stay there
        yield return ChangeLight(targetLight.intensity, brightIntensity,
                                 targetLight.range, brightRange);

        // enable and play closing credits
        if (closingCreditsGO != null && closingCredits != null)
        {
            closingCreditsGO.SetActive(true);
            closingCredits.StartSequence();
            StartCoroutine(WaitForCreditsToEnd());
        }
    }

    private IEnumerator ChangeLight(float fromIntensity, float toIntensity,
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
        targetLight.intensity = toIntensity;
        targetLight.range = toRange;
    }

    private IEnumerator WaitForCreditsToEnd()
    {
        float sequenceDuration = 0f;
        foreach (var entry in closingCredits.credits)
        {
            if (entry != null) sequenceDuration += entry.displayTime;
            sequenceDuration += closingCredits.fadeDuration * 2; // logo fade in/out
        }

        yield return new WaitForSeconds(sequenceDuration + exitDelay);
        ExitExperience();
    }

    private void ExitExperience()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
