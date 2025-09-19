using UnityEngine;
using UnityEngine.InputSystem;

public class XRMenuOpener : MonoBehaviour
{
    [Header("UI Menu to Toggle")]
    public GameObject menuUI;

    [Header("Input Action (assign from XRI asset)")]
    public InputActionProperty bButtonAction; // Link to RightHand/SecondaryButton

    [Header("Settings")]
    public float holdDuration = 3f;

    private float holdTimer = 0f;
    private bool menuOpen = false;

    void OnEnable()
    {
        bButtonAction.action.Enable();
    }

    void OnDisable()
    {
        bButtonAction.action.Disable();
    }

    void Update()
    {
        if (bButtonAction.action.IsPressed())
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                ToggleMenu();
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private void ToggleMenu()
    {
        if (menuUI != null)
        {
            menuOpen = !menuOpen;
            menuUI.SetActive(menuOpen);
            Debug.Log("XR Menu " + (menuOpen ? "opened" : "closed"));
        }
    }
}
