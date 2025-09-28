using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

    [Header("UI Feedback")]
    public GameObject lighterUI;   // parent object for UI ("A for Flame")
    public Color normalColor = Color.white;
    public Color activeColor = Color.red;
    public float fadeDuration = 0.3f;

    private Image buttonImage;        // circle outline
    private TMP_Text buttonText;      // letter "A"
    private bool isGrabbed = false;
    private bool flameOn = false;
    private Coroutine fadeRoutine;

    private void Start()
    {
        // Ensure flame is off at start
        if (flame != null)
            flame.SetActive(false);

        if (burningSource != null)
            burningSource.loop = true;

        // Hide UI initially
        if (lighterUI != null)
        {
            lighterUI.SetActive(false);
            buttonImage = lighterUI.GetComponent<Image>();
            if (buttonImage == null)
                buttonImage = lighterUI.GetComponentInChildren<Image>();

            buttonText = lighterUI.GetComponentInChildren<TMP_Text>();
        }
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
        if (lighterUI != null)
            lighterUI.SetActive(true);

        UpdateUIButton();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        TurnOffFlame(); // auto shut off when dropped
        if (lighterUI != null)
            lighterUI.SetActive(false);
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
        flameOn = true;

        if (flame != null)
            flame.SetActive(true);

        if (ignitionSource != null)
            ignitionSource.PlayOneShot(ignitionSource.clip);

        if (burningSource != null && !burningSource.isPlaying)
            burningSource.Play();

        UpdateUIButton();
    }

    private void TurnOffFlame()
    {
        flameOn = false;

        if (flame != null)
            flame.SetActive(false);

        if (burningSource != null && burningSource.isPlaying)
            burningSource.Stop();

        UpdateUIButton();
    }

    private void UpdateUIButton()
    {
        Color targetColor = flameOn ? activeColor : normalColor;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeUI(targetColor));
    }

    private System.Collections.IEnumerator FadeUI(Color targetColor)
    {
        float elapsed = 0f;
        Color startColorImg = buttonImage != null ? buttonImage.color : Color.white;
        Color startColorTxt = buttonText != null ? buttonText.color : Color.white;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(startColorImg, targetColor, t);

            if (buttonText != null)
                buttonText.color = Color.Lerp(startColorTxt, targetColor, t);

            yield return null;
        }

        if (buttonImage != null)
            buttonImage.color = targetColor;

        if (buttonText != null)
            buttonText.color = targetColor;
    }
}
