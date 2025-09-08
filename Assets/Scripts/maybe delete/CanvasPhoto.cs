using UnityEngine;

public class CanvasPhoto : MonoBehaviour
{
    public Material front;
    public Material sides;

    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var materials = new Material[mesh.subMeshCount];

        for (int i = 0; i < materials.Length; i++)
            materials[i] = sides;

        materials[2] = front; // Set front material (Z+)
        GetComponent<MeshRenderer>().materials = materials;
    }
}
