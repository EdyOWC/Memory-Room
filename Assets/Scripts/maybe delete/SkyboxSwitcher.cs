using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [Tooltip("List of skybox materials to switch between")]
    public Material[] skyboxes;

    private int current = 0;

    void Start()
    {
        if (skyboxes != null && skyboxes.Length > 0)
        {
            SetSkybox(0); // start with first
        }
    }

    public void NextSkybox()
    {
        if (skyboxes == null || skyboxes.Length == 0) return;

        current = (current + 1) % skyboxes.Length;
        SetSkybox(current);
    }

    public void SetSkybox(int index)
    {
        if (skyboxes == null || skyboxes.Length == 0) return;
        if (index < 0 || index >= skyboxes.Length) return;

        current = index;
        RenderSettings.skybox = skyboxes[current];
        DynamicGI.UpdateEnvironment();
        Debug.Log("Skybox set to: " + skyboxes[current].name);
    }
}
