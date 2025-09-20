using UnityEngine;

public class LockTriggerForward : MonoBehaviour
{
    public DrawerLockAndPriority drawerController;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == drawerController.transform)
            drawerController.LockDrawer();
    }
}
