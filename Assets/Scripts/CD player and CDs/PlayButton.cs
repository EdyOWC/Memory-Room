using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class PlayButton : MonoBehaviour
{
    public PlaylistManager manager;

    private void Awake()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(_ => OnPressed());
    }

    private void OnPressed()
    {
        if (manager != null)
        {
            Debug.Log("Play Button Pressed");
            manager.TogglePlay();
        }
        else
        {
            Debug.LogWarning("PlayButton has no PlaylistManager assigned!");
        }
    }
}
