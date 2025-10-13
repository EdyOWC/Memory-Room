using UnityEngine;
using UnityEngine.Video;
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
    public VideoPlayer openingVideo;    // optional video for opening credits

    [Header("Settings")]
    public CreditType type = CreditType.Opening;
    public float fadeDuration = 2f;     // fade speed for vignette and logos
    public bool autoStart = true;
    public float quitDelay = 3f;        // delay before app quit (closing credits)

    [Header("Player Control")]
    [Tooltip("Assign your locomotion script(s) here (e.g., ContinuousMoveProviderBase, TeleportationProvider, etc.)")]
    public Behaviour[] locomotionScripts;

    [Header("Objects to Hide During Credits")]
    [Tooltip("Assign any GameObjects you want to hide during credits (UI, props, etc.)")]
    public GameObject[] objectsToDisable;

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
        // 🔒 Disable locomotion & other GOs at start
        SetLocomotionEnabled(false);
        SetObjectsEnabled(false);

        // Opening credits → start black
        // Closing credits → start transparent, fade to white
        vignetteGroup.alpha = (type == CreditType.Opening) ? 1f : 0f;
        gameObject.SetActive(true);

        // For closing: fade vignette to full white first
        if (type == CreditType.Closing)
            yield return FadeVignette(0f, 1f);

        // ---- Opening video (optional) ----
        if (type == CreditType.Opening && openingVideo != null)
        {
            openingVideo.gameObject.SetActive(true);
            openingVideo.Play();

            while (openingVideo.isPlaying)
                yield return null;

            openingVideo.gameObject.SetActive(false);
        }

        // ---- Run through logos/credits ----
        foreach (var entry in credits)
        {
            foreach (var c in credits)
                if (c.creditObject != null) c.creditObject.SetActive(false);

            if (entry.creditObject == null) continue;

            entry.creditObject.SetActive(true);
            CanvasGroup cg = entry.creditObject.GetComponent<CanvasGroup>();
            if (cg == null) cg = entry.creditObject.AddComponent<CanvasGroup>();

            yield return FadeCanvas(cg, 0f, 1f);
            yield return new WaitForSeconds(entry.displayTime);
            yield return FadeCanvas(cg, 1f, 0f);

            entry.creditObject.SetActive(false);
        }

        if (type == CreditType.Opening)
        {
            // Opening → fade vignette out to reveal scene
            yield return FadeVignette(1f, 0f);
            gameObject.SetActive(false);

            // 🔓 Re-enable locomotion & GOs
            SetLocomotionEnabled(true);
            SetObjectsEnabled(true);
        }
        else if (type == CreditType.Closing)
        {
            // Closing → vignette stays white, then quit app
            yield return new WaitForSeconds(quitDelay);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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

    private void SetLocomotionEnabled(bool enabled)
    {
        if (locomotionScripts == null) return;
        foreach (var script in locomotionScripts)
            if (script != null) script.enabled = enabled;
    }

    private void SetObjectsEnabled(bool enabled)
    {
        if (objectsToDisable == null) return;
        foreach (var obj in objectsToDisable)
            if (obj != null) obj.SetActive(enabled);
    }
}
