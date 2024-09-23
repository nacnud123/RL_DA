using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scrambleBtn;
    [SerializeField] private bool scrambleNames = false;

    [Header("Audio sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private SoundMixerManager sMM;


    private void OnEnable()
    {
        scrambleNames = PlayerPrefs.GetInt("scrambleNames", 0) != 0;

        masterSlider.value = PlayerPrefs.GetFloat("masterVol");
        ambientSlider.value = PlayerPrefs.GetFloat("ambientVol");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol");

        if (scrambleNames)
        { scrambleBtn.text = "On"; }
        else
        { scrambleBtn.text = "Off"; }
    }


    public void changeScramble()
    {
        scrambleNames = !scrambleNames;
        if (scrambleNames)
        { scrambleBtn.text = "On"; }
        else 
        { scrambleBtn.text = "Off"; }
    }

    public void saveData()
    {
        PlayerPrefs.SetInt("scrambleNames", (scrambleNames ? 1 : 0));
        PlayerPrefs.SetFloat("masterVol", masterSlider.value);
        PlayerPrefs.SetFloat("ambientVol", ambientSlider.value);
        PlayerPrefs.SetFloat("sfxVol", sfxSlider.value);
    }



}
