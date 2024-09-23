using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(PlayerPrefs.GetFloat("masterVol", 1f)) * 20f);
        audioMixer.SetFloat("Ambient Volume", Mathf.Log10(PlayerPrefs.GetFloat("ambientVol", .4f)) * 20f);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVol", .6f)) * 20f);
    }

    public void setMasterVolume(float level)
    {
        audioMixer.SetFloat("Master Volume", Mathf.Log10(level) * 20f);
    }

    public void setAmbientVolume(float level)
    {
        audioMixer.SetFloat("Ambient Volume", Mathf.Log10(level) * 20f);
    }

    public void setSFXVolume(float level)
    {
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(level) * 20f);
    }

}
