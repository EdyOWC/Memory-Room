using UnityEngine;

public class CandleIgniter : MonoBehaviour
{
    [Header("Setup")]
    public GameObject flameToEnable;  // Assign the root that holds candle flame & light

    [Header("Mission Gate")]
    public DrawerMissionGate missionGate; // Drag your drawer here
    public string missionName = "A";      // Mission ID (default = "A")

    private bool missionCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LighterFlame"))
        {
            // Enable the flame
            if (flameToEnable != null && !flameToEnable.activeSelf)
            {
                flameToEnable.SetActive(true);
                Debug.Log("Candle lit!");
            }

            // Complete the mission (only once)
            if (!missionCompleted && missionGate != null)
            {
                missionGate.CompleteMission(missionName);
                missionCompleted = true;
            }
        }
    }
}
