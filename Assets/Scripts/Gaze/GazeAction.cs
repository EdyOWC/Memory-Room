using UnityEngine;
using UnityEngine.Events;

public class GazeAction : MonoBehaviour, ILookAt
{
    [Header("What happens when I look at this?")]
    public UnityEvent onLook;  // You can assign functions here from the Inspector

    public void OnLook()
    {
        onLook?.Invoke();
    }
}
