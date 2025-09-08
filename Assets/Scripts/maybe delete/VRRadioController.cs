using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRRadioController : MonoBehaviour
{
    public List<AudioClip> playlist;
    public AudioSource audioSource;

    public InputActionProperty playPauseAction;   // A button
    public InputActionProperty nextSongAction;    // B button
    public InputActionProperty joystickAction;    // Thumbstick (Vector2)

    private XRBaseInteractor currentInteractor;
    private bool aPressed = false;
    private bool bPressed = false;
    private bool inputLocked = false;
    private int currentTrack = 0;

    private void OnEnable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnGrabbed);
        GetComponent<XRGrabInteractable>().selectExited.AddListener(OnReleased);

        playPauseAction.action.Enable();
        nextSongAction.action.Enable();
        joystickAction.action.Enable();
    }

    private void OnDisable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnGrabbed);
        GetComponent<XRGrabInteractable>().selectExited.RemoveListener(OnReleased);

        playPauseAction.action.Disable();
        nextSongAction.action.Disable();
        joystickAction.action.Disable();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseInteractor;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void Update()
    {
        if (currentInteractor == null) return;

        // A = Play/Pause
        if (playPauseAction.action.ReadValue<float>() > 0.5f)
        {
            if (!aPressed)
            {
                TogglePlay();
                aPressed = true;
                Debug.Log ("YES!");
            }
        }
        else aPressed = false;

        // B = Next Song
        if (nextSongAction.action.ReadValue<float>() > 0.5f)
        {
            if (!bPressed)
            {
                NextSong();
                bPressed = true;
            }
        }
        else bPressed = false;

        // Joystick left = Restart
        Vector2 joystick = joystickAction.action.ReadValue<Vector2>();

        if (!inputLocked && joystick.x < -0.6f)
        {
            RestartSong();
            inputLocked = true;
        }

        if (Mathf.Abs(joystick.x) < 0.2f)
        {
            inputLocked = false;
        }
    }

    void TogglePlay()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.Play();
    }

    void NextSong()
    {
        if (playlist.Count == 0) return;

        currentTrack = (currentTrack + 1) % playlist.Count;
        audioSource.clip = playlist[currentTrack];
        audioSource.Play();
    }

    void RestartSong()
    {
        audioSource.time = 0;
    }
}
