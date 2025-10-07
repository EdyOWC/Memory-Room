using UnityEngine;
using System.Collections;

public class EnvironmentChain : MonoBehaviour
{
    [Header("Animators")]
    public Animator env1Animator;   // Environment_1 animator
    public Animator env3Animator;   // Environment_3 animator

    [Header("Environment Objects")]
    public GameObject env1Object;   // Root object of Environment_1
    public GameObject env3Object;   // Root object of Environment_3

    [Header("State Names")]
    public string finalStateEnv1 = "AfterWillGrab"; // must match Env1 animator
    public string startStateEnv3 = "Finale Anim";   // must match Env3 animator

    [Header("Timing")]
    public float delayBeforeEnv3 = 1f; // Delay before Env3 starts

    private bool isChaining = false;

    void Update()
    {
        if (env1Animator == null || env3Animator == null) return;

        AnimatorStateInfo state = env1Animator.GetCurrentAnimatorStateInfo(0);

        // Wait for Env1 to finish its final state
        if (state.IsName(finalStateEnv1) && state.normalizedTime >= 1f && !isChaining)
        {
            isChaining = true;
            StartCoroutine(ChainTransition());
        }
    }

    IEnumerator ChainTransition()
    {
        Debug.Log("Env1 finished → preparing Env3...");

        yield return new WaitForSeconds(delayBeforeEnv3);

        // Deactivate Env1
        if (env1Object != null)
        {
            env1Object.SetActive(false);
            Debug.Log("Env1 disabled.");
        }

        // Activate Env3
        if (env3Object != null)
        {
            env3Object.SetActive(true);
            Debug.Log("Env3 enabled.");
        }

        // Wait a frame so Animator initializes properly
        yield return null;

        // Make sure the animator resets before playback
        env3Animator.Rebind();
        env3Animator.Update(0f);

        // Start the finale animation
        env3Animator.Play(startStateEnv3, 0, 0f);
        Debug.Log("Env3 animation started: " + startStateEnv3);
    }
}
