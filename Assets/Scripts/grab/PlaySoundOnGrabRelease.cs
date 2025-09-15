using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PlaySoundOnGrabRelease : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource audioSource;   // Assign in Inspector (shared with dialogs)
    public AudioClip grabSound;       // The clip to play when grabbed
    public float fadeDuration = 2f;   // Time to fade out after release

    private XRGrabInteractable grab;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (audioSource != null && grabSound != null)
        {
            // Only play if nothing else is already playing (e.g. dialog)
            if (!audioSource.isPlaying)
            {
                audioSource.clip = grabSound;
                audioSource.volume = 1f; // reset volume
                audioSource.Play();
            }
        }

        // Cancel fade if object is grabbed again
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == grabSound)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration));
        }
    }

    private System.Collections.IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = audioSource.volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        // Only stop if it's still the grabSound playing
        if (audioSource.clip == grabSound)
        {
            audioSource.Stop();
            audioSource.volume = startVolume; // reset for next grab
        }

        fadeCoroutine = null;
    }
}
