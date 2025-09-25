using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LighterFlameToggle : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference lightAction;

    [Header("Visuals")]
    public GameObject flame;

    [Header("Audio")]
    public AudioSource ignitionSource;   // short ignition "click"
    public AudioSource burningSource;    // looping flame sound

    [Header("XR")]
    public XRGrabInteractable grabInteractable;

    private bool isGrabbed = false;

    private void Start()
    {
        if (flame != null)
            flame.SetActive(false);  // Off at start

        if (burningSource != null)
            burningSource.loop = true;   // make sure flame sound loops
    }

    private void OnEnable()
    {
        if (lightAction != null)
        {
            lightAction.action.Enable();
            lightAction.action.started += TryTurnOnFlame;
            lightAction.action.canceled += TryTurnOffFlame;
        }

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (lightAction != null)
        {
            lightAction.action.started -= TryTurnOnFlame;
            lightAction.action.canceled -= TryTurnOffFlame;
            lightAction.action.Disable();
        }

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        // Auto turn off when released
        TurnOffFlame();
    }

    private void TryTurnOnFlame(InputAction.CallbackContext ctx)
    {
        if (isGrabbed) TurnOnFlame();
    }

    private void TryTurnOffFlame(InputAction.CallbackContext ctx)
    {
        if (isGrabbed) TurnOffFlame();
    }

    private void TurnOnFlame()
    {
        Debug.Log("Flame ON");
        if (flame != null) flame.SetActive(true);

        if (ignitionSource != null)
            ignitionSource.PlayOneShot(ignitionSource.clip);

        if (burningSource != null && !burningSource.isPlaying)
            burningSource.Play();
    }

    private void TurnOffFlame()
    {
        Debug.Log("Flame OFF");
        if (flame != null) flame.SetActive(false);

        if (burningSource != null && burningSource.isPlaying)
            burningSource.Stop();
    }
}
