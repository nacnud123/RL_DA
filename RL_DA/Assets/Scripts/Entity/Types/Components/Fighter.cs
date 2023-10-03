using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
sealed class Fighter : MonoBehaviour
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

    public int getDefense { get => defense; }
    public int getPower { get => power; }
    public Actor Target { get => target; set => target = value; }

    private void Start()
    {
        if (GetComponent<Player>())
        {
            UIManager.init.setHealthMax(maxHP);
            UIManager.init.setHealth(hp, maxHP);
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.init.addMsg("You died!", "#ff0000");
        }
        else
        {
            UIManager.init.addMsg($"{name} is dead.", "#ffa500");
        }

        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        sp.sprite = GameManager.init.getDeadSprite;
        sp.color = new Color(191, 0, 0, 1);
        sp.sortingOrder = 0;

        name = $"Remains of {name}";
        GetComponent<Actor>().BlocksMovment = false;
        GetComponent<Actor>().IsAlive = false;
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
}
