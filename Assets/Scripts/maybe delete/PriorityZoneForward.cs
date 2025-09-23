using UnityEngine;

public class PriorityZoneForward : MonoBehaviour
{
    public DrawerLockAndPriority drawerController;
    void OnTriggerEnter(Collider other) => drawerController?.NotifyPriorityZoneEnter(other);
    void OnTriggerExit(Collider other) => drawerController?.NotifyPriorityZoneExit(other);
}
