using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Environment References")]
    public GameObject environment3;

    public void EnableFinale()
    {
        environment3.SetActive(true);
    }
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
