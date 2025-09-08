using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleCertificateRainXR : MonoBehaviour
{
    [Header("Textures & Material")]
    public Texture2D[] certificateTextures;    // different certificates
    public Material quadMaterial;              // Unlit Transparent (no texture assigned)

    [Header("Spawn Settings")]
    public float spawnPerSecond = 10f;
    public Vector2 areaHalfSize = new Vector2(5f, 5f); // XZ half extents
    public float spawnHeight = 6f;
    public float baseWidth = 0.28f;            // visual width for a 1:1 image
    public float lifetime = 20f;

    [Header("Aspect Control")]
    public bool overrideAspect = false;        // use custom aspect instead of texture's
    public float customAspect = 1.0f;          // width / height

    [Header("Soft-Fall Physics")]
    public float startDownSpeed = 0.6f;
    public Vector2 randomSideSpeed = new Vector2(0.2f, 0.25f);
    public Vector2 randomAngular = new Vector2(5f, 18f); // deg/sec
    public float mass = 0.05f;
    public float drag = 2.0f;                  // higher drag = softer landing
    public float angularDrag = 2.5f;
    public float maxFallSpeed = 1.2f;          // terminal velocity cap

    [Header("Grab Settings")]
    public bool throwOnDetach = false;         // feels like paper
    public PhysicsMaterial sheetPhysicMaterial; // assign SheetSoft

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

        // material instance + texture
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = new Material(quadMaterial);
        mr.sharedMaterial.mainTexture = tex;

        // aspect control
        float aspect;
        if (overrideAspect && customAspect > 0f)
        {
            aspect = customAspect; // use manual value
        }
        else
        {
            aspect = (float)tex.width / Mathf.Max(1, tex.height); // from texture
        }

        // apply scale
        go.transform.localScale = new Vector3(baseWidth, baseWidth / aspect, 1f);

        // collider (thin box; disable MeshCollider if present)
        var meshCol = go.GetComponent<MeshCollider>();
        if (meshCol) Destroy(meshCol);
        var box = go.AddComponent<BoxCollider>();
        box.size = new Vector3(1f, 1f, 0.01f);
        if (sheetPhysicMaterial) box.sharedMaterial = sheetPhysicMaterial;

        // rigidbody tuned for soft fall
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        // gentle fall + drift + tumble
        rb.linearVelocity = new Vector3(
            Random.Range(-randomSideSpeed.x, randomSideSpeed.x),
            -startDownSpeed,
            Random.Range(-randomSideSpeed.y, randomSideSpeed.y)
        );
        rb.AddTorque(Random.onUnitSphere * Mathf.Deg2Rad * Random.Range(randomAngular.x, randomAngular.y), ForceMode.VelocityChange);

        // cap terminal velocity (soft landing)
        var limiter = go.AddComponent<TerminalVelocityLimiter>();
        limiter.maxDownSpeed = maxFallSpeed;

        // XR grabbable
        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.Kinematic; // smooth grab, no explosions
        grab.throwOnDetach = throwOnDetach;
        grab.trackRotation = true;
        grab.smoothPosition = true;
        grab.smoothRotation = true;

        // cleanup
        Destroy(go, lifetime);
    }

    // helper to clamp fall speed
    private class TerminalVelocityLimiter : MonoBehaviour
    {
        public float maxDownSpeed = 1.2f;
        Rigidbody rb;

        void Awake() { rb = GetComponent<Rigidbody>(); }
        void FixedUpdate()
        {
            if (!rb) return;
            var v = rb.linearVelocity;
            if (v.y < -maxDownSpeed) v.y = -maxDownSpeed;
            rb.linearVelocity = v;
        }
    }
}
