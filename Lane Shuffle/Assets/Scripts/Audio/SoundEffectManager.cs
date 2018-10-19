using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // needed for Array.Find

[RequireComponent(typeof(VolumeControl))]
public class SoundEffectManager : MonoBehaviour
{
    private VolumeControl volumeControl;

    [SerializeField]
    private SoundEffect[] effects;

    private void Awake()
    {
        volumeControl = GetComponent<VolumeControl>();
    }

#if UNITY_ANDROID && !UNITY_EDITOR

    private void Start()
    {
        AndroidNativeAudio.makePool(); // makes 16 streams by default
        foreach (SoundEffect effect in effects) { effect.fileID = AndroidNativeAudio.load(effect.filePath); }
    }

    public void PlayEffect(string effectName)
    {
        SoundEffect s = Array.Find(effects, effect => effect.name == effectName); // this uses a Lambda Expression
        int SoundID = AndroidNativeAudio.play(s.fileID);
        AndroidNativeAudio.setVolume(SoundID, s.volume * volumeControl.EffectsVolume);
    }

    void OnApplicationQuit()
    {
        foreach (SoundEffect effect in effects) { AndroidNativeAudio.unload(effect.fileID); }
        AndroidNativeAudio.releasePool();
    }

#else

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        foreach (SoundEffect effect in effects)
        {
            // This bit is necessary for accessing audio clips from the StreamingAssets folder in non-Android versions.
            // For more info see the ANAMusic guide.
            var www = new WWW("file:" + Application.streamingAssetsPath + "/" + effect.filePath);
            while (!www.isDone) { } // I think this waits until the clip is loaded
            effect.audioClip = www.GetAudioClip();
        }
    }

    public void PlayEffect(string effectName)
    {
        SoundEffect s = Array.Find(effects, effect => effect.name == effectName); // this uses a Lambda Expression
        audioSource.PlayOneShot(s.audioClip, s.volume * volumeControl.EffectsVolume);
    }

    #endif
}
