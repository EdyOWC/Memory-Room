using UnityEngine;

public class CandleVoiceExtinguisher : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // XR camera or XR rig
    public GameObject flame;       // flame GameObject to toggle

    [Header("Settings")]
    public float triggerDistance = 1.5f;   // distance in meters
    public float voiceThreshold = 0.1f;    // how loud you need to be (0-1)
    public float checkInterval = 0.1f;     // how often to sample mic

    private AudioClip micClip;
    private string micDevice;
    private float nextCheckTime;

    void Start()
    {
        // Start microphone recording
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 1, 44100);
        }
        else
        {
            Debug.LogWarning("No microphone detected!");
        }
    }

    void Update()
    {
        if (Time.time >= nextCheckTime && micClip != null)
        {
            nextCheckTime = Time.time + checkInterval;

            // Check distance
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= triggerDistance)
            {
                // Check voice loudness
                float level = GetMicLevel();
                if (level > voiceThreshold)
                {
                    if (flame != null) flame.SetActive(false);
                    Debug.Log("Candle extinguished!");
                }
            }
        }
    }

    float GetMicLevel()
    {
        int sampleSize = 128;
        float[] data = new float[sampleSize];
        int micPos = Microphone.GetPosition(micDevice) - sampleSize;
        if (micPos < 0) return 0;

        micClip.GetData(data, micPos);

        float sum = 0f;
        for (int i = 0; i < sampleSize; i++)
            sum += Mathf.Abs(data[i]);

        return sum / sampleSize;
    }
}
