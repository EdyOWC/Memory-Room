using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RouteTrigger : MonoBehaviour
{
    [Header("NPC Reference")]
    public NPCStoryRoutes npc;

    [Header("Routes")]
    public string forwardRoute;
    public string backwardRoute;

    private bool goingForward = true;

    private void Reset()
    {
        // Make sure BoxCollider is trigger
        var col = GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // only react to player

        if (npc == null)
        {
            Debug.LogError("[RouteTrigger] NPC reference missing!");
            return;
        }

        if (goingForward)
        {
            if (!string.IsNullOrEmpty(forwardRoute))
            {
                npc.StartRoute(forwardRoute);
                goingForward = false;
                Debug.Log($"[RouteTrigger] Triggered forward route: {forwardRoute}");
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(backwardRoute))
            {
                npc.StartRoute(backwardRoute);
                goingForward = true;
                Debug.Log($"[RouteTrigger] Triggered backward route: {backwardRoute}");
            }
        }
    }
}
