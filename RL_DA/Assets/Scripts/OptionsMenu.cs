using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public TextMeshProUGUI scrambleBtn;
    public bool scrambleNames = false;

    private void OnEnable()
    {
        scrambleNames = PlayerPrefs.GetInt("scrambleNames", 0) != 0;

        if (scrambleNames)
        { scrambleBtn.text = "[O] On"; }
        else
        { scrambleBtn.text = "[O] Off"; }
    }


    public void changeScramble()
    {
        scrambleNames = !scrambleNames;
        if (scrambleNames)
        { scrambleBtn.text = "[O] On"; }
        else 
        { scrambleBtn.text = "[O] Off"; }
    }

    public void saveData()
    {
        PlayerPrefs.SetInt("scrambleNames", (scrambleNames ? 1 : 0));
    }



}
