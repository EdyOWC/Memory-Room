using UnityEngine;
using UnityEngine.InputSystem;

public class LighterFlameToggle : MonoBehaviour
{
    public InputActionReference lightAction;
    public GameObject flame;

    private void Start()
    {
        if (flame != null)
            flame.SetActive(false);  //  Off at start
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
        Debug.Log(" Flame ON");
        flame.SetActive(true);
    }

    private void TurnOffFlame(InputAction.CallbackContext ctx)
    {
        Debug.Log(" Flame OFF");
        flame.SetActive(false);
    }
}
