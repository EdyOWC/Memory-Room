using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    private AudioSource src;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
    }

    /// <summary>
    /// Tries to play a line. Returns true if it started, false if busy.
    /// </summary>
    public bool PlayLine(AudioClip clip)
    {
        if (clip == null) return false;

        if (src.isPlaying) return false; // busy, reject

        src.clip = clip;
        src.Play();
        return true;
    }

    public bool IsPlaying()
    {
        return src.isPlaying;
    }
}
