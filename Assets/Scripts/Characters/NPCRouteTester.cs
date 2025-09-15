using UnityEngine;
using UnityEngine.InputSystem; // for new Input System

[RequireComponent(typeof(NPCStoryRoutes))]
public class NPCRouteTester : MonoBehaviour
{
    [Header("Route to test at runtime")]
    public string routeToTest;

    private NPCStoryRoutes routes;

    void Awake()
    {
        routes = GetComponent<NPCStoryRoutes>();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(routeToTest))
        {
            Debug.Log("[NPCRouteTester] Auto-starting route: " + routeToTest);
            routes.StartRoute(routeToTest);
        }
    }

    // Optional: restart route with a key press
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("[NPCRouteTester] Restarting route: " + routeToTest);
            routes.StartRoute(routeToTest);
        }
    }

    [ContextMenu("Start Route")]
    public void CM_StartRoute()
    {
        routes.StartRoute(routeToTest);
    }
}
