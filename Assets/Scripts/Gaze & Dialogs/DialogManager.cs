using UnityEngine;
using System;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    private AudioSource audioSource;
    private Action onFinished;

    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public bool PlayClip(AudioClip clip, Action finishedCallback = null)
    {
        if (IsPlaying) return false; // Prevent overlapping

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            onFinished = finishedCallback;
            Invoke(nameof(HandleFinished), clip.length);
            return true;
        }
        return false;
    }

    private void HandleFinished()
    {
        if (!audioSource.isPlaying && onFinished != null)
        {
            var callback = onFinished;
            onFinished = null;
            callback.Invoke();
        }
    }
}
