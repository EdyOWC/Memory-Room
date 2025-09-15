using UnityEngine;

public class GazeMusicTrigger : MonoBehaviour, ILookAt
{
    [Header("Dialog")]
    public AudioClip dialogClip;
    public bool onlyOnce = true;

    [Header("Music")]
    public PlaylistManager musicManager;

    private bool hasPlayed = false;

    public void OnLook()
    {
        if (onlyOnce && hasPlayed) return;

        if (DialogManager.Instance != null && dialogClip != null)
        {
            bool started = DialogManager.Instance.PlayClip(dialogClip);
            if (started)
            {
                hasPlayed = true;
                DialogManager.Instance.StartCoroutine(PlayMusicAfterDialog());
            }
        }
        else
        {
            if (musicManager != null && !musicManager.IsPlaying)
            {
                musicManager.gameObject.SetActive(true);
                musicManager.PlayFirst();
            }
        }
    }

    private System.Collections.IEnumerator PlayMusicAfterDialog()
    {
        yield return new WaitWhile(() => DialogManager.Instance.IsPlaying);

        if (musicManager != null && !musicManager.IsPlaying)
        {
            musicManager.gameObject.SetActive(true);
            musicManager.PlayFirst();
        }
    }
}
