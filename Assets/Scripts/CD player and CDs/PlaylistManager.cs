using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaylistManager : MonoBehaviour
{
    public List<AudioClip> playlist;
    public bool loopPlaylist = true;   // go back to first after last

    private AudioSource src;
    private int index = 0;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;              // important: we advance manually
        src.spatialBlend = 1f;         // 3D
    }

    void Start()
    {
        if (playlist.Count > 0)
        {
            src.clip = playlist[0];
            src.Play();
        }
    }

    void Update()
    {
        // when current song ends, advance
        if (!src.isPlaying && src.clip != null)
            Next();
    }

    public void TogglePlay()
    {
        if (src.isPlaying) src.Pause();
        else src.Play();
    }

    public void Next()
    {
        if (playlist.Count == 0) return;

        int nextIndex;
        if (playlist.Count == 1)
        {
            nextIndex = 0;
        }
        else
        {
            // pick a random index different from the current one
            do
            {
                nextIndex = Random.Range(0, playlist.Count);
            }
            while (nextIndex == index);
        }

        index = nextIndex;
        src.clip = playlist[index];
        src.Play();
    }

}
