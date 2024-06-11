using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Inventory>().SelectedConsumable = this;
        actor.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg("Select a target to poison.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.init.addMsg($"{target.name} is poisoned!", "#ffffff");
        target.AI.isPoisoned = true;
        target.AI.poisonedTurns = Random.Range(3, 8);
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
