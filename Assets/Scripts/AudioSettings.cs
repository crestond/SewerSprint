using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders (-80 to 0 dB)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY   = "SFXVolume";

    private void Start()
    {
        // Load saved dB values (default to -10 dB)
        float master = PlayerPrefs.GetFloat(MASTER_KEY, -10f);
        float music  = PlayerPrefs.GetFloat(MUSIC_KEY, -10f);
        float sfx    = PlayerPrefs.GetFloat(SFX_KEY, -10f);

        // Update sliders without triggering callbacks
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(master);
        if (musicSlider  != null) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider    != null) sfxSlider.SetValueWithoutNotify(sfx);

        // Apply to mixer
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Hook up listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider  != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider    != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Slider now passes dB directly (-80 to 0)
    public void SetMasterVolume(float dB)
    {
        audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat(MASTER_KEY, dB);
    }

    public void SetMusicVolume(float dB)
    {
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat(MUSIC_KEY, dB);
    }

    public void SetSFXVolume(float dB)
    {
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat(SFX_KEY, dB);
    }
}
