using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Religion : MonoBehaviour
{
    [SerializeField] private int currentFavor = 0;
    public int Favor { get => currentFavor; set => currentFavor = value; }

    public void modifyFavor(int mod)
    {
        if (mod >= 10)
            UIManager.init.addMsg($"The God is pleased with your offer!", "#698cf5");
        else if (mod >= 5)
            UIManager.init.addMsg($"The God is likes your offer!", "#698cf5");
        else if (mod >= 1)
            UIManager.init.addMsg($"The God wishes you could given more!", "#698cf5");
        else if (mod == 0)
            UIManager.init.addMsg($"The God does not care about your offer!", "#808080");
        else if (mod <= -10)
            UIManager.init.addMsg($"The God hates your offer!", "#ad0505");
        else if (mod <= -5)
            UIManager.init.addMsg($"The God dislikes your offer!", "#ad0505");
        else if (mod <= -1)
            UIManager.init.addMsg($"The God wishes you did not give them this!", "#ad0505");
    }

    public void Pray()
    {
        if(currentFavor >= 15)
            UIManager.init.addMsg($"BEST!", "#698cf5");
        else if (currentFavor >= 10)
            UIManager.init.addMsg($"GREAT!", "#698cf5");
        else if (currentFavor >= 1)
            UIManager.init.addMsg($"OK!", "#698cf5");
        else if (currentFavor >= 0)
            UIManager.init.addMsg($"MEH!", "#698cf5");
        else if(currentFavor <= -15)
            UIManager.init.addMsg($"REALLY BAD!", "#ad0505");
        else if (currentFavor <= -10)
            UIManager.init.addMsg($"BAD!", "#ad0505");
        else if (currentFavor <= -1)
            UIManager.init.addMsg($"NOT GOOD!", "#ad0505");
    }



}
