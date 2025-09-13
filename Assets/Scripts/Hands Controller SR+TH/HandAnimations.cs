
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimations : MonoBehaviour
{
    private Animator anim;
    public InputActionReference GripAnim_ActionMap;
    public InputActionReference TriggerAnim_ActionMap;


    private void Awake()
    {
        anim = this.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        float gripvalue = GripAnim_ActionMap.action.ReadValue<float>();
        float triggervalue = TriggerAnim_ActionMap.action.ReadValue<float>();

        anim.SetFloat("Grip", gripvalue);
        anim.SetFloat("Trigger", triggervalue);
    }
}
