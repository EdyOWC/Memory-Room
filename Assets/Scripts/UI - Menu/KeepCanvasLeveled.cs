using UnityEngine;

public class KeepCanvasLeveled : MonoBehaviour
{
    [Header("Axis Control")]
    public bool freezeX = true;
    public bool freezeZ = true;

    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;

        if (freezeX) euler.x = 0f;   // lock tilt forward/back
        if (freezeZ) euler.z = 0f;   // lock tilt sideways

        transform.eulerAngles = euler;
    }
}
