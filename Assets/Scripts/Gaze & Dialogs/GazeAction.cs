using UnityEngine;

public class GazeAction : MonoBehaviour, ILookAt
{
    [Header("Dialog")]
    public AudioClip clip;
    public bool onlyOnce = true;

    private bool hasPlayed = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
    }

    public void OnLook()
    {
        if (onlyOnce && hasPlayed) return;
        if (clip == null) return;

        bool started = false;

        if (DialogManager.Instance != null)
        {
            // Try to play through DialogManager
            if (!DialogManager.Instance.IsPlaying)
            {
                started = DialogManager.Instance.PlayClip(clip);
            }
            else
            {
                Debug.Log("Dialog skipped: DialogManager busy.");
            }
        }
        else
        {
            // Fallback: play locally if no DialogManager exists
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
                started = true;
            }
        }

        if (started)
        {
            hasPlayed = true; // only mark once it really played
        }
    }
}
