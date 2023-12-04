using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Level : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1, currentXP, xpToNextLevel, levelUpBase = 200, levelUpFactor = 150, xpGiven;

    public int CurrentLevel { get => currentLevel; }
    public int CurrentXP { get => currentXP; }
    public int XpToNextLevel { get => xpToNextLevel; }
    public int XPGiven { get => xpGiven; set => xpGiven = value; }

    private void OnValidate() => xpToNextLevel = ExpToNextLevel();

    private int ExpToNextLevel() => levelUpBase + currentLevel * levelUpFactor;

    private bool RequiresLevelUp() => currentXP >= xpToNextLevel;

    public void AddXP(int xp)
    {
        if (xp == 0 || levelUpBase == 0) return;

        currentXP += xp;

        UIManager.init.addMsg($"You gain {xp} xp.", "#FFFFFF");

        if (RequiresLevelUp())
        {
            UIManager.init.toggleLevelUpMenu(GetComponent<Actor>());
            UIManager.init.addMsg($"You gained a levle {currentLevel + 1}!", "#00FF00");
        }
    }

    private void IncrLevel()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = ExpToNextLevel();
    }

    public void IncrMaxXP(int amount = 20)
    {
        GetComponent<Actor>().Fighter.MaxHp += amount;
        GetComponent<Actor>().Fighter.Hp += amount;

        UIManager.init.addMsg($"Your health increased!", "#00ff00");
        IncrLevel();
    }
    

    public void IncrPower(int amount = 20)
    {
        GetComponent<Actor>().Fighter.Power += amount;

        UIManager.init.addMsg($"You feel stronger!", "#00ff00");
        IncrLevel();
    }

    public void IncrDefense(int amount = 1)
    {
        GetComponent<Actor>().Fighter.Defense += amount;

        UIManager.init.addMsg($"Your defense improves!", "#00ff00");
        IncrLevel();
    }

    public LevelState SaveState() => new LevelState(
        _currLevel: currentLevel,
        _currXP: currentXP,
        _xpToNextLevel: xpToNextLevel
        );

    public void LoadState(LevelState state)
    {
        currentLevel = state.CurrentLevel;
        currentXP = state.CurrentXp;
        xpToNextLevel = state.XpToNextLevel;
    }
}

public class LevelState
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CurrentXp { get => currentXp; set => currentXp = value; }
    public int XpToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

    public LevelState(int _currLevel, int _currXP, int _xpToNextLevel)
    {
        currentLevel = _currLevel;
        currentXp = _currXP;
        xpToNextLevel = _xpToNextLevel;
    }
}
