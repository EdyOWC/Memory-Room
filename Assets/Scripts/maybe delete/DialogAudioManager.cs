using UnityEngine;
using System.Collections.Generic;

public class DialogAudioManager : MonoBehaviour
{
    [Header("Dialog AudioSources (only)")]
    public List<AudioSource> dialogSources = new List<AudioSource>();

    /// <summary>
    /// Plays a dialog clip on the given source.
    /// Stops all other dialog sources first.
    /// </summary>
    public void PlayDialog(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;

        // Stop all other dialogs
        foreach (var s in dialogSources)
        {
            if (s != null && s.isPlaying)
                s.Stop();
        }

        // Play the requested one
        source.loop = false;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Stop all dialog audio sources.
    /// </summary>
    public void StopAllDialogs()
    {
        foreach (var s in dialogSources)
        {
            if (s != null && s.isPlaying)
                s.Stop();
        }
    }

    /// <summary>
    /// Returns true if any dialog is playing.
    /// </summary>
    public bool IsDialogPlaying()
    {
        foreach (var s in dialogSources)
        {
            if (s != null && s.isPlaying)
                return true;
        }
        return false;
    }
}
