using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(XRGrabInteractable))]
public class GrabToPlayVideo : MonoBehaviour
{
    [Header("Setup")]
    public Texture pictureTexture;      // still image before play
    public VideoClip videoClip;         // video to play
    public bool loop = false;           // loop video

    private VideoPlayer vp;
    private Renderer rend;
    private XRGrabInteractable xri;
    private string texProp = "_BaseMap"; // URP default (_MainTex for Built-in)

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        rend = GetComponent<Renderer>();
        xri = GetComponent<XRGrabInteractable>();

        // Pick the correct texture property automatically (URP vs Built-in)
        if (rend != null && rend.sharedMaterial != null)
        {
            if (!rend.sharedMaterial.HasProperty("_BaseMap") && rend.sharedMaterial.HasProperty("_MainTex"))
                texProp = "_MainTex";
        }

        // Set initial still image
        if (pictureTexture != null && rend != null)
            rend.material.SetTexture(texProp, pictureTexture);

        // Configure VideoPlayer
        vp.playOnAwake = false;
        vp.isLooping = loop;
        vp.renderMode = VideoRenderMode.MaterialOverride;
        vp.targetMaterialRenderer = rend;
        vp.targetMaterialProperty = texProp;
        vp.clip = videoClip;
        vp.aspectRatio = VideoAspectRatio.FitVertically; // tweak if needed

        // If there's an AudioSource, route audio to it
        var audioSrc = GetComponent<AudioSource>();
        if (audioSrc != null)
        {
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
            vp.EnableAudioTrack(0, true);
            vp.SetTargetAudioSource(0, audioSrc);
        }
        else
        {
            vp.audioOutputMode = VideoAudioOutputMode.Direct;
        }

        xri.selectEntered.AddListener(OnGrab);
        xri.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs _)
    {
        if (vp.clip == null) return;
        vp.Play();
    }

    private void OnRelease(SelectExitEventArgs _)
    {
        vp.Stop();
        // restore still image
        if (pictureTexture != null && rend != null)
            rend.material.SetTexture(texProp, pictureTexture);
    }

    void OnDestroy()
    {
        if (xri != null)
        {
            xri.selectEntered.RemoveListener(OnGrab);
            xri.selectExited.RemoveListener(OnRelease);
        }
    }
}
