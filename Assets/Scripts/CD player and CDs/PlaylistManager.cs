using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaylistManager : MonoBehaviour
{
    public List<AudioClip> playlist;
    public bool loopPlaylist = true;

    private AudioSource src;
    private int index = 0;

    public bool IsPlaying => src != null && src.isPlaying;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 1f;
    }

    // 🚫 Removed auto-start in Start()
    // Music will only play if PlayFirst() / TogglePlay() is called

    void Update()
    {
        // Only auto-advance if music is already playing
        if (src.clip != null && !src.isPlaying && src.time > 0f)
        {
            Next();
        }
    }

    public void TogglePlay()
    {
        if (src.isPlaying) src.Pause();
        else if (src.clip != null) src.Play();
        else PlayFirst();
    }

    public void PlayFirst()
    {
        if (playlist.Count == 0) return;
        index = 0;
        src.clip = playlist[index];
        src.Play();
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
