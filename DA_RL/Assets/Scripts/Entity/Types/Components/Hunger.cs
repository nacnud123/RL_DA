using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Hunger : MonoBehaviour
{
    [SerializeField] private int currentHunger = 1000;
    [SerializeField] private int maxHunger = 2000;
    [SerializeField] private int hungerWarningThreshold = 300;
    [SerializeField] private int starvingThreshold = 150;
    [SerializeField] private int criticalThreshold = 50;

    private int lastWarningLevel = -1;

    public int CurrentHunger
    {
        get => currentHunger;
        set
        {
            currentHunger = Mathf.Clamp(value, 0, maxHunger);
            CheckHungerStatus();
        }
    }

    public int MaxHunger { get => maxHunger; }

    public HungerStatus GetHungerState()
    {
        if (currentHunger >= maxHunger * 0.8f)
            return HungerStatus.Full;
        else if (currentHunger >= hungerWarningThreshold)
            return HungerStatus.Normal;
        else if (currentHunger >= starvingThreshold)
            return HungerStatus.Hungry;
        else if (currentHunger >= criticalThreshold)
            return HungerStatus.Starving;
        else
            return HungerStatus.Critical;
    }

    public void DecrementHunger(int amount = 1)
    {
        CurrentHunger -= amount;

        // Take damage if starving
        if (currentHunger <= 0 && GetComponent<Fighter>())
        {
            GetComponent<Fighter>().Hp -= 1;
            if (GetComponent<Player>())
            {
                UIManager.init.addMsg("You are dying from starvation!", "#ff0000");
            }
        }
    }

    public void Eat(int nutrition)
    {
        int oldHunger = currentHunger;
        CurrentHunger += nutrition;

        if (GetComponent<Player>())
        {
            if (oldHunger < hungerWarningThreshold && currentHunger >= hungerWarningThreshold)
            {
                UIManager.init.addMsg("You no longer feel hungry.", "#00ff00");
            }
            else if (currentHunger >= maxHunger)
            {
                UIManager.init.addMsg("You feel satiated.", "#00ff00");
            }
        }
    }

    private void CheckHungerStatus()
    {
        if (!GetComponent<Player>())
            return;

        int currentLevel = (int)GetHungerState();


        Debug.Log("Check hunger status!");

        UIManager.init.setHunger(GetHungerState());

        if (currentLevel != lastWarningLevel)
        {
            switch (GetHungerState())
            {
                case HungerStatus.Hungry:
                    UIManager.init.addMsg("You are getting hungry.", "#ffff00");
                    break;
                case HungerStatus.Starving:
                    UIManager.init.addMsg("You are starving!", "#ff8800");
                    break;
                case HungerStatus.Critical:
                    UIManager.init.addMsg("You are near death from starvation!", "#ff0000");
                    break;
            }
            lastWarningLevel = currentLevel;
        }
    }

    public HungerState SaveState() => new HungerState(currentHunger, maxHunger);

    public void LoadState(HungerState state)
    {
        maxHunger = state.MaxHunger;
        currentHunger = state.CurrentHunger;
    }
}

public enum HungerStatus
{
    Full,
    Normal,
    Hungry,
    Starving,
    Critical
}

[System.Serializable]
public class HungerState
{
    [SerializeField] private int currentHunger;
    [SerializeField] private int maxHunger;

    public int CurrentHunger { get => currentHunger; set => currentHunger = value; }
    public int MaxHunger { get => maxHunger; set => maxHunger = value; }

    public HungerState(int _currentHunger, int _maxHunger)
    {
        currentHunger = _currentHunger;
        maxHunger = _maxHunger;
    }
}
