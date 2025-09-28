using UnityEngine;

public class EnvironmentChain : MonoBehaviour
{
    [Header("Animators")]
    public Animator env1Animator;   // Environment_1 animator
    public Animator env3Animator;   // Environment_3 animator

    [Header("State Names")]
    public string finalStateEnv1 = "AfterWillGrab"; // must match state name
    public string startStateEnv3 = "Finale Anim";   // must match state name

    private bool isChaining = false;

    void Update()
    {
        if (env1Animator == null || env3Animator == null) return;

        // Check if Environment_1 is playing its last state
        AnimatorStateInfo state = env1Animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(finalStateEnv1))
        {
            // Wait until the animation is basically finished
            if (state.normalizedTime >= 1f && !isChaining)
            {
                isChaining = true;
                TriggerEnv3();
            }
        }
    }

    void TriggerEnv3()
    {
        Debug.Log("Chaining: Env1 finished → starting Env3.");
        env3Animator.Play(startStateEnv3, 0, 0f);
    }
}
