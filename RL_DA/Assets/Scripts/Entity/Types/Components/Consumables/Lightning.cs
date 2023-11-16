using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Consumable
{
    [SerializeField] private int damage = 20;
    [SerializeField] private int maximumRange = 5;

    public int Damage { get => damage; }
    public int MaxRangeP { get => maximumRange; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg("Select a target to strike.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.init.addMsg($"A strike of lightning hits {target.name}, for {damage} damage!", "#ffffff");
        target.GetComponent<Fighter>().Hp -= damage;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
