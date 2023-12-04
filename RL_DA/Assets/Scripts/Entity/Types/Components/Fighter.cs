using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Fighter : MonoBehaviour
{
    [SerializeField] private int maxHP, hp, defense, power;
    [SerializeField] private Actor target;

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
    public int MaxHp { get => maxHP; set => maxHP = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Power { get => power; set => power = value; }
    public Actor Target { get => target; set => target = value; }

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
                UIManager.init.addMsg($"{name} is dead.", "#ffa500");
            }
            GetComponent<Actor>().IsAlive = false;
        }

        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        sp.sprite = GameManager.init.getDeadSprite;
        sp.color = new Color(191, 0, 0, 1);
        sp.sortingOrder = 0;

        name = $"Remains of {name}";
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
        _defense: defense,
        _power: power,
        _target: target != null ? target.name : null
        );

    public void LoadState(FighterState state)
    {
        maxHP = state.MaxHp;
        hp = state.Hp;
        defense = state.Defense;
        power = state.Power;
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