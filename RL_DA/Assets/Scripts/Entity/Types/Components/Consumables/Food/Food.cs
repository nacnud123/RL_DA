using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Consumable
{
    [SerializeField] private int nutrition = 200;

    public int Nutrition { get => nutrition; }

    public override bool Activate(Actor consumer)
    {
        if (consumer.Hunger == null)
        {
            UIManager.init.addMsg("You don't need to eat.", "#808080");
            return false;
        }

        consumer.Hunger.Eat(nutrition);
        UIManager.init.addMsg($"You eat the {consumer.GetInventory.GetItems.Find(x => x.GetConsumable == this).RealName}.", "#00ff00");
        Consume(consumer);
        return true;
    }
}
