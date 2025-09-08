using UnityEngine;

public class VRMicParticleTrigger : MonoBehaviour
{
    [Header("Microphone Settings")]
    public string microphoneName = "";
    public float sensitivity = 0.02f;

    [Header("Physics Particle Settings")]
    public GameObject particlePrefab; // Prefab with Rigidbody + Collider
    public Transform mouthTransform; // Mouth position reference
    public float forwardForce = 5f; // Strength of particles moving forward
    public float coneAngle = 15f; // Cone spread angle
    public float spawnRateMultiplier = 10f; // Higher = more particles when loud
    public float particleLifetime = 3f;

    [Header("Talk Unlock Settings")]
    public GameObject targetToEnable;  // The GO to enable after enough talking
    public float talkDuration = 5f;    // Seconds of talking needed (default 5)
    public float loudnessThreshold = 0.01f; // Min loudness to count as "talking"

    private AudioClip micClip;
    private int sampleWindow = 128;
    private bool micActive = false;

    private float talkTimer = 0f;
    private bool unlocked = false;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
            micClip = Microphone.Start(microphoneName, true, 1, 44100);
            micActive = true;
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }

        // Ensure target starts disabled
        if (targetToEnable != null)
            targetToEnable.SetActive(false);
    }

    void Update()
    {
        if (micActive && mouthTransform != null)
        {
            float loudness = GetLoudness();

            // Spawn particles based on loudness
            int spawnCount = Mathf.RoundToInt(loudness / sensitivity * spawnRateMultiplier);
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnPhysicsParticle();
            }

            // Track talking time
            if (!unlocked)
            {
                if (loudness > loudnessThreshold)
                {
                    talkTimer += Time.deltaTime;
                    if (talkTimer >= talkDuration)
                    {
                        unlocked = true;
                        if (targetToEnable != null)
                            targetToEnable.SetActive(true);

                        Debug.Log("Unlocked! Enabled " + targetToEnable.name);
                    }
                }
                else
                {
                    // Optional: reset timer if silence
                    // talkTimer = 0f;
                }
            }
        }
    }

    void SpawnPhysicsParticle()
    {
        if (particlePrefab == null) return;

        GameObject particle = Instantiate(particlePrefab, mouthTransform.position, mouthTransform.rotation);

        Quaternion spreadRot = Quaternion.Euler(
            Random.Range(-coneAngle, coneAngle),
            Random.Range(-coneAngle, coneAngle),
            0
        );

        Rigidbody rb = particle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = spreadRot * mouthTransform.forward;
            rb.AddForce(direction * forwardForce, ForceMode.Impulse);
        }

        Destroy(particle, particleLifetime);
    }

    float GetLoudness()
    {
        float[] data = new float[sampleWindow];
        int position = Microphone.GetPosition(microphoneName) - sampleWindow;
        if (position < 0) return 0;
        micClip.GetData(data, position);

        float sum = 0f;
        foreach (float sample in data)
        {
            sum += Mathf.Abs(sample);
        }
        return sum / sampleWindow;
    }
}
