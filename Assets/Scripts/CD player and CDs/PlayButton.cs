using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PlayButton : MonoBehaviour
{
    public PlaylistManager manager;

    private void Awake()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(_ => {
            Debug.Log("Play Button Pressed");
            manager.TogglePlay();
        });
    }
}
