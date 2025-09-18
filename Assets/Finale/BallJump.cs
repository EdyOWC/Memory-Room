using UnityEngine;

public class BallJump : MonoBehaviour
{
    public float forceAmount = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(3f, 0f, 7f), ForceMode.Impulse);
    }
}
