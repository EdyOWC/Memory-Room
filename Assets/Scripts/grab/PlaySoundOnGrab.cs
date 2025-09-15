using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PlaySoundOnGrab : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource audioSource;   // Assign in Inspector
    public AudioClip grabSound;       // The clip to play when grabbed

    private XRGrabInteractable grab;
    private static bool isPlaying = false;   // global lock

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!isPlaying && audioSource != null && grabSound != null)
        {
            audioSource.PlayOneShot(grabSound);
            isPlaying = true;
            Invoke(nameof(ResetLock), grabSound.length);
        }
    }

    private void ResetLock()
    {
        isPlaying = false;
    }
}
