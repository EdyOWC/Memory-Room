using UnityEngine;
using TMPro;
using System.Collections;

public class VignetteCredits : MonoBehaviour
{
    public enum FadeMode { FadeIn, FadeOut } // Choose opening or closing

    [Header("Setup")]
    public CanvasGroup vignetteGroup;   // assign CanvasGroup on vignette image
    public TextMeshProUGUI creditsText; // optional TMP text (can be null)

    [Header("Timing")]
    public FadeMode mode = FadeMode.FadeOut;
    public float fadeDuration = 2f;    // fade-in/out duration
    public float holdDuration = 3f;    // time credits stay fully visible

    void OnEnable()
    {
        // Start with correct initial alpha depending on mode
        if (mode == FadeMode.FadeIn)
            vignetteGroup.alpha = 0f;
        else
            vignetteGroup.alpha = 1f;

        StartCoroutine(RunCredits());
    }

    IEnumerator RunCredits()
    {
        float elapsed = 0f;

        if (mode == FadeMode.FadeIn)
        {
            // Fade in
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                vignetteGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            vignetteGroup.alpha = 1f;
        }

        // Hold credits
        yield return new WaitForSeconds(holdDuration);

        // Reset timer
        elapsed = 0f;

        if (mode == FadeMode.FadeOut)
        {
            // Fade out
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                vignetteGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            vignetteGroup.alpha = 0f;
            vignetteGroup.gameObject.SetActive(false);
        }
    }
}
