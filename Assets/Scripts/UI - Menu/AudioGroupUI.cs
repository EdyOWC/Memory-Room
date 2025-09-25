using UnityEngine;
using UnityEngine.UI;

public class AudioGroupUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider dialogSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider ambientSlider;

    [Header("Tags")]
    public string dialogTag = "Dialog";
    public string sfxTag = "SFX";
    public string musicTag = "Music";
    public string ambientTag = "Ambient";

    private AudioSource[] dialogSources;
    private AudioSource[] sfxSources;
    private AudioSource[] musicSources;
    private AudioSource[] ambientSources;

    private float masterVolume = 1f; // multiplier for everything

    private void Start()
    {
        dialogSources = FindAudioSources(dialogTag);
        sfxSources = FindAudioSources(sfxTag);
        musicSources = FindAudioSources(musicTag);
        ambientSources = FindAudioSources(ambientTag);

        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (dialogSlider) dialogSlider.onValueChanged.AddListener((v) => SetVolume(dialogSources, v));
        if (sfxSlider) sfxSlider.onValueChanged.AddListener((v) => SetVolume(sfxSources, v));
        if (musicSlider) musicSlider.onValueChanged.AddListener((v) => SetVolume(musicSources, v));
        if (ambientSlider) ambientSlider.onValueChanged.AddListener((v) => SetVolume(ambientSources, v));
    }

    private AudioSource[] FindAudioSources(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        AudioSource[] sources = new AudioSource[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            sources[i] = objs[i].GetComponent<AudioSource>();
        }
        return sources;
    }

    private void SetVolume(AudioSource[] sources, float volume)
    {
        foreach (var src in sources)
        {
            if (src != null)
                src.volume = volume * masterVolume;
        }
    }

    private void SetMasterVolume(float v)
    {
        masterVolume = v;
        // Re-apply all group sliders at current values
        if (dialogSlider) SetVolume(dialogSources, dialogSlider.value);
        if (sfxSlider) SetVolume(sfxSources, sfxSlider.value);
        if (musicSlider) SetVolume(musicSources, musicSlider.value);
        if (ambientSlider) SetVolume(ambientSources, ambientSlider.value);
    }
}
