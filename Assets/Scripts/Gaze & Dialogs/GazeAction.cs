using UnityEngine;

public class GazeAction : MonoBehaviour, ILookAt
{
    [Header("Dialog")]
    public AudioClip clip;
    public bool onlyOnce = true;

    private bool hasPlayed = false;

    public void OnLook()
    {
        if (onlyOnce && hasPlayed) return;

        if (clip != null && DialogManager.Instance != null)
        {
            // Only mark as played if manager accepted it
            bool started = DialogManager.Instance.PlayClip(clip);
            if (started)
            {
                hasPlayed = true;
            }
        }
    }
}
