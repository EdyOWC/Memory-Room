using UnityEngine;

public class CDPlayerSlot : MonoBehaviour
{
    public string targetTag = "GrabbableCD";
    public int songIndex = 0;                 // which track this CD should trigger
    public PlaylistManager playlistManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag) && playlistManager != null)
        {
            if (songIndex >= 0 && songIndex < playlistManager.playlist.Count)
            {
                playlistManager.StopAllCoroutines(); // cleaner than SendMessage
                var audioSource = playlistManager.GetComponent<AudioSource>();
                audioSource.clip = playlistManager.playlist[songIndex];
                audioSource.Play();
                Debug.Log("CD collided → Playing song index: " + songIndex);
            }
        }
    }
}
