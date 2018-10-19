using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VolumeControl))]
public class MusicManager : MonoBehaviour
{
    // TODO Make the music use a start section and a loop section
    // TODO Find a better solution for the game temporarly freezing when the music is stopped. Currently I mute the music instead of stopping it.

    private VolumeControl volumeControl;

    [SerializeField, Tooltip("This should be in the StreamingAssets folder. Don't forget the file extension!")]
    private string filePath;
    [SerializeField, Range(0f, 1f)]
    private float volume = 0.8f;

    private bool hasEnded;

    private void Awake()
    {
        volumeControl = GetComponent<VolumeControl>();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    
    private int musicID;

    private bool isMuted = false;

    void Start()
    {
        musicID = ANAMusic.load(filePath);
        ANAMusic.play(musicID);
        ANAMusic.setVolume(musicID, volume * volumeControl.MusicVolume);
    }

    public void PlayMusic()
    {
        if (hasEnded) return;
        ANAMusic.play(musicID);
    }

    public void PauseMusic()
    {
        ANAMusic.pause(musicID);
    }

    public void EndMusic()
    {
        hasEnded = true;
        //PauseMusic();

        // Muting instead of stopping the music because stopping it can make the app freeze for a fraction of a second
        ANAMusic.setVolume(musicID, 0);
        isMuted = true;
    }

    public void UpdateVolume()
    {
        if (isMuted) { return; }
        ANAMusic.setVolume(musicID, volume * volumeControl.MusicVolume);
    }

    private void OnDestroy()
    {
        ANAMusic.release(musicID);
    }

#else

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // This bit is necessary for accessing audio clips from the StreamingAssets folder in non-Android versions.
        // For more info see the ANAMusic guide.
        var www = new WWW("file:" + Application.streamingAssetsPath + "/" + filePath);
        while (!www.isDone) { } // I think this waits until the clip is loaded
        audioSource.clip = www.GetAudioClip();

        audioSource.volume = volume * volumeControl.MusicVolume;
        audioSource.Play();
    }

    public void PlayMusic()
    {
        if (hasEnded) return;
        audioSource.Play();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void EndMusic()
    {
        hasEnded = true;
        PauseMusic();
    }

    public void UpdateVolume()
    {
        audioSource.volume = volume * volumeControl.MusicVolume;
    }

#endif
}
