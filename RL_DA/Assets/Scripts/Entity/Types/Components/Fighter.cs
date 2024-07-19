using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Fighter : MonoBehaviour
{
    [SerializeField] private int maxHP, hp, baseDefense, basePower;
    [SerializeField] private Actor target;
    [SerializeField] private string damage = "";

    public string Damage { get => damage; }

    public int Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Max(0, Mathf.Min(value, maxHP));

            if (GetComponent<Player>())
                UIManager.init.setHealth(hp, maxHP);

            if (hp == 0)
                Die();
        }
    }
    public int MaxHp
    {
        get => maxHP;
        set
        {
            maxHP = value;
            if (GetComponent<Player>())
            {
                UIManager.init.setHealthMax(maxHP);
            }
        }
    }

    public int BaseDefense { get => baseDefense; set => baseDefense = value; }
    public int BasePower { get => basePower; set => basePower = value; }
    public Actor Target { get => target; set => target = value; }


    public int Power()
    {
        if (this.GetComponent<HostileEnemy>())
        {
            return GameManager.init.getDamage(Damage);
        }
        else
        {
            return basePower + powerBonus();
        }
    }

    public int Defense()
    {
        return baseDefense + defenseBonus();
    }

    public int defenseBonus()
    {
        if(GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().DefenseBonus();
        }
        return 0;
    }

    public int powerBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            
            return GetComponent<Equipment>().PowerBonus();
        }
        return 0;
    }

    private void Start()
    {
        if (GetComponent<Player>())
        {
            UIManager.init.setHealthMax(maxHP);
            UIManager.init.setHealth(hp, maxHP);
        }
    }

    public void Die()
    {
        if (GetComponent<Actor>().IsAlive) {
            if (GetComponent<Player>())
            {
                UIManager.init.addMsg("You died!", "#ff0000");
            }
            else
            {
                GameManager.init.getActors[0].GetComponent<Level>().AddXP(GetComponent<Level>().XPGiven);
                UIManager.init.addMsg($"{this.GetComponent<Actor>().RealName} is dead.", "#ffa500");
            }
            GetComponent<Actor>().IsAlive = false;
        }

        SpriteRenderer sp = GetComponent<Actor>().SR;
        sp.sprite = GameManager.init.getDeadSprite;
        sp.color = new Color(191, 0, 0, 1);
        sp.sortingOrder = 0;

        name = $"Remains of {this.GetComponent<Actor>().RealName}";
        GetComponent<Actor>().BlocksMovment = false;
        if (!GetComponent<Player>())
        {
            GameManager.init.removeActor(this.GetComponent<Actor>());
        }

    }

    public int Heal(int amount)
    {
        if (hp == maxHP)
            return 0;

        int newHpVal = hp + amount;

        if (newHpVal > maxHP)
            newHpVal = maxHP;

        int amountRec = newHpVal - hp;
        Hp = newHpVal;
        return amountRec;
    }

    public FighterState SaveState() => new FighterState(
        _maxHp: maxHP,
        _hp: hp,
        _defense: baseDefense,
        _power: basePower,
        _target: target != null ? target.name : null
        );

    public void LoadState(FighterState state)
    {
        maxHP = state.MaxHp;
        hp = state.Hp;
        baseDefense = state.Defense;
        basePower = state.Power;
        target = GameManager.init.getActors.Find(a => a.name == state.Target);
    }
}

public class FighterState
{
    [SerializeField] private int maxHp, hp, defense, power;
    [SerializeField] private string target;

    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Hp { get => hp; set => hp = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Power { get => power; set => power = value; }
    public string Target { get => target; set => target = value; }

    public FighterState(int _maxHp, int _hp, int _defense, int _power, string _target)
    {
        maxHp = _maxHp;
        hp = _hp;
        defense = _defense;
        power = _power;
        target = _target;
    }
}
