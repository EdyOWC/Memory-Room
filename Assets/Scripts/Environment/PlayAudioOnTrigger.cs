using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayAudioOnTrigger : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource audioSource;     // assign in Inspector
    public AudioClip clipToPlay;        // assign in Inspector

    private bool hasPlayed = false;

    private void Start()
    {
        // Make sure the collider is set as trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} collider is not set as trigger — fixing automatically.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed)
        {
            if (audioSource != null && clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
                hasPlayed = true;
                Debug.Log($"Played {clipToPlay.name} once on trigger enter.");
            }
        }
    }
}
