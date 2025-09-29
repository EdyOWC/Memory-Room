using UnityEngine;
using System.Collections;

public class CreditsSequence : MonoBehaviour
{
    [System.Serializable]
    public class CreditEntry
    {
        public GameObject creditObject;  // logo/text object
        public float displayTime = 3f;   // how long to stay visible
    }

    public enum CreditType { Opening, Closing }

    [Header("Setup")]
    public CanvasGroup vignetteGroup;   // the black/white background vignette
    public CreditEntry[] credits;       // list of logos/credits in order

    [Header("Settings")]
    public CreditType type = CreditType.Opening;
    public float fadeDuration = 2f;     // fade speed for vignette and logos
    public bool autoStart = true;

    void OnEnable()
    {
        if (autoStart) StartSequence();
    }

    public void StartSequence()
    {
        StartCoroutine(RunCredits());
    }

    IEnumerator RunCredits()
    {
        // Opening credits → start black
        // Closing credits → start transparent, fade to white
        vignetteGroup.alpha = (type == CreditType.Opening) ? 1f : 0f;
        gameObject.SetActive(true);

        // For closing: fade vignette to full white first
        if (type == CreditType.Closing)
            yield return FadeVignette(0f, 1f);

        // ---- Run through logos/credits ----
        foreach (var entry in credits)
        {
            // hide all first
            foreach (var c in credits)
                if (c.creditObject != null) c.creditObject.SetActive(false);

            if (entry.creditObject == null) continue;

            entry.creditObject.SetActive(true);

            // fade logo in
            CanvasGroup cg = entry.creditObject.GetComponent<CanvasGroup>();
            if (cg == null) cg = entry.creditObject.AddComponent<CanvasGroup>();
            yield return FadeCanvas(cg, 0f, 1f);

            // hold
            yield return new WaitForSeconds(entry.displayTime);

            // fade logo out
            yield return FadeCanvas(cg, 1f, 0f);
            entry.creditObject.SetActive(false);
        }

        // For opening → fade vignette out to reveal scene
        if (type == CreditType.Opening)
        {
            yield return FadeVignette(1f, 0f);
            gameObject.SetActive(false); // done
        }
    }

    IEnumerator FadeVignette(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            vignetteGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        vignetteGroup.alpha = to;
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = to;
    }
}
