using UnityEngine;

public class CandleVoiceExtinguisher : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // XR camera or XR rig
    public GameObject flame;       // flame GameObject to toggle

    [Header("Settings")]
    public float triggerDistance = 1.5f;
    public float voiceThreshold = 0.1f;
    public float checkInterval = 0.1f;

    private AudioClip micClip;
    private string micDevice;
    private float nextCheckTime;

    void OnEnable()
    {
        StartMic();
    }

    void OnDisable()
    {
        StopMic();
    }

    void StartMic()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 1, 44100);
            Debug.Log("🎤 Microphone started for candle.");
        }
        else
        {
            Debug.LogWarning("⚠️ No microphone detected!");
        }
    }

    void StopMic()
    {
        if (!string.IsNullOrEmpty(micDevice))
        {
            Microphone.End(micDevice);
            Debug.Log("🛑 Microphone stopped for candle.");
        }
        micClip = null;
    }

    void Update()
    {
        if (Time.time >= nextCheckTime && micClip != null)
        {
            nextCheckTime = Time.time + checkInterval;

            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= triggerDistance)
            {
                float level = GetMicLevel();
                if (level > voiceThreshold)
                {
                    if (flame != null && flame.activeSelf)
                    {
                        flame.SetActive(false);
                        Debug.Log("🔥 Candle extinguished!");
                    }
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
