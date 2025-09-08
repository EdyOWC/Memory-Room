using UnityEngine;

public class CorridorLoop : MonoBehaviour
{
    public Transform player;
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false; // Disable if using CharacterController

            player.position = respawnPoint.position;

            if (cc) cc.enabled = true; // Re-enable
        }
    }
}
