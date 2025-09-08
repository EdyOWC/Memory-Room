using UnityEngine;

public class SimpleCertificateRain : MonoBehaviour
{
    [Header("Textures & Material")]
    public Texture2D[] certificateTextures;   // drop different certificates here
    public Material quadMaterial;             // Unlit Transparent

    [Header("Spawn Settings")]
    public float spawnPerSecond = 12f;
    public Vector2 areaHalfSize = new Vector2(5f, 5f); // XZ half extents
    public float spawnHeight = 6f;
    public float baseWidth = 0.3f;            // visual width for a 1:1 image
    public float lifetime = 8f;

    [Header("Physics")]
    public float startDownSpeed = 1.2f;       // initial downward velocity
    public Vector2 randomSideSpeed = new Vector2(0.3f, 0.3f);
    public Vector2 randomAngular = new Vector2(10f, 35f); // deg/sec around random axis
    public float drag = 0.2f;
    public float angularDrag = 0.4f;

    float acc;

    void Update()
    {
        if (certificateTextures == null || certificateTextures.Length == 0 || quadMaterial == null) return;

        acc += spawnPerSecond * Time.deltaTime;
        while (acc >= 1f)
        {
            SpawnOne();
            acc -= 1f;
        }
    }

    void SpawnOne()
    {
        // pick a texture
        var tex = certificateTextures[Random.Range(0, certificateTextures.Length)];

        // make a quad
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "Cert";
        go.transform.position = transform.position + new Vector3(
            Random.Range(-areaHalfSize.x, areaHalfSize.x),
            spawnHeight,
            Random.Range(-areaHalfSize.y, areaHalfSize.y)
        );
        go.transform.rotation = Random.rotationUniform;

        // apply material & texture (instance so we don�t mutate the shared one)
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = new Material(quadMaterial);
        mr.sharedMaterial.mainTexture = tex;

        // preserve aspect by scaling X/Y
        float aspect = (float)tex.width / Mathf.Max(1, tex.height);
        go.transform.localScale = new Vector3(baseWidth, baseWidth / aspect, 1f);

        // add physics
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        // gentle fall + drift + tumble
        rb.linearVelocity = new Vector3(
            Random.Range(-randomSideSpeed.x, randomSideSpeed.x),
            -startDownSpeed,
            Random.Range(-randomSideSpeed.y, randomSideSpeed.y)
        );
        rb.AddTorque(Random.onUnitSphere * Mathf.Deg2Rad * Random.Range(randomAngular.x, randomAngular.y), ForceMode.VelocityChange);

        // thin collider so it doesn�t block too hard
        var col = go.GetComponent<MeshCollider>();
        if (col) Destroy(col); // MeshCollider on Quad is overkill; use BoxCollider
        var box = go.AddComponent<BoxCollider>();
        box.size = new Vector3(1f, 1f, 0.01f);

        // clean up
        Destroy(go, lifetime);
    }
}
