using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MantrasPlaylistPlayer : MonoBehaviour
{
    [Header("Playlist Settings")]
    public List<AudioClip> playlist;
    public bool shuffle = true;   // set false for sequential play

    private AudioSource src;
    private int index = 0;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;   // we control play manually
        src.loop = false;          // we handle looping ourselves
        src.spatialBlend = 1f;     // 3D audio (set to 0f for 2D everywhere)
    }

    void Start()
    {
        PlayFirst();
    }

    void Update()
    {
        // If clip ended, immediately go to next
        if (!src.isPlaying && src.clip != null)
        {
            Next();
        }
    }

    private void PlayFirst()
    {
        if (playlist.Count == 0) return;
        index = 0;
        src.clip = playlist[index];
        src.Play();
    }

    private void Next()
    {
        if (playlist.Count == 0) return;

        int nextIndex = index;

        if (shuffle && playlist.Count > 1)
        {
            // pick a different random index
            while (nextIndex == index)
            {
                nextIndex = Random.Range(0, playlist.Count);
            }
        }
        else
        {
            // sequential with wrap-around
            nextIndex = (index + 1) % playlist.Count;
        }

        index = nextIndex;
        src.clip = playlist[index];
        src.Play();
    }
}
