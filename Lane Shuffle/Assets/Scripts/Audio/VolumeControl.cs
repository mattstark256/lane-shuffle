using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MusicManager))]
public class VolumeControl : MonoBehaviour
{
    // TODO Maybe make the volume sliders exponential

    private MusicManager musicManager;

    [SerializeField]
    private Slider effectsVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private float defaultEffectsVolume = 0.7f;
    [SerializeField]
    private float defaultMusicVolume = 0.7f;

    public float EffectsVolume { get; private set; }
    public float MusicVolume { get; private set; }

    private bool slidersHaveBeenInitialized = false;

    private void Awake()
    {
        musicManager = GetComponent<MusicManager>();

        if (!PlayerPrefs.HasKey("Effects Volume")) { PlayerPrefs.SetFloat("Effects Volume", defaultEffectsVolume); }
        if (!PlayerPrefs.HasKey("Music Volume")) { PlayerPrefs.SetFloat("Music Volume", defaultMusicVolume); }

        EffectsVolume = PlayerPrefs.GetFloat("Effects Volume");
        MusicVolume = PlayerPrefs.GetFloat("Music Volume");
    }

    private void Start()
    {
        effectsVolumeSlider.value = EffectsVolume;
        musicVolumeSlider.value = MusicVolume;
        slidersHaveBeenInitialized = true;
    }

    public void UpdateEffectsVolume()
    {
        if (!slidersHaveBeenInitialized) { return; }
        EffectsVolume = effectsVolumeSlider.value;
        PlayerPrefs.SetFloat("Effects Volume", EffectsVolume);
    }

    public void UpdateMusicVolume()
    {
        if (!slidersHaveBeenInitialized) { return; }
        MusicVolume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("Music Volume", MusicVolume);
        musicManager.UpdateVolume();
    }
}
