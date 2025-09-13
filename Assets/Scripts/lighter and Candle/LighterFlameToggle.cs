using UnityEngine;
using UnityEngine.InputSystem;

public class LighterFlameToggle : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference lightAction;

    [Header("Visuals")]
    public GameObject flame;

    [Header("Audio")]
    public AudioSource ignitionSource;   // short ignition "click"
    public AudioSource burningSource;    // looping flame sound

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
            lightAction.action.started += TurnOnFlame;
            lightAction.action.canceled += TurnOffFlame;
        }
    }

    private void OnDisable()
    {
        if (lightAction != null)
        {
            lightAction.action.started -= TurnOnFlame;
            lightAction.action.canceled -= TurnOffFlame;
            lightAction.action.Disable();
        }
    }

    private void TurnOnFlame(InputAction.CallbackContext ctx)
    {
        Debug.Log("Flame ON");
        if (flame != null) flame.SetActive(true);

        // play ignition SFX once
        if (ignitionSource != null)
            ignitionSource.PlayOneShot(ignitionSource.clip);

        // start looping burning sound
        if (burningSource != null && !burningSource.isPlaying)
            burningSource.Play();
    }

    private void TurnOffFlame(InputAction.CallbackContext ctx)
    {
        Debug.Log("Flame OFF");
        if (flame != null) flame.SetActive(false);

        // stop burning sound
        if (burningSource != null && burningSource.isPlaying)
            burningSource.Stop();
    }
}
