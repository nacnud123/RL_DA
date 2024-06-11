using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Inventory>().SelectedConsumable = this;
        actor.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg("Select a target to freeze.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.init.addMsg($"{target.name} seems to be frozen!", "#ffffff");
        target.AI.isFrozen = true;
        target.AI.frozenTurns = Random.Range(5, 10);
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
