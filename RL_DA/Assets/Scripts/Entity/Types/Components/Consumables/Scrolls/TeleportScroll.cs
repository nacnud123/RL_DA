using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Inventory>().SelectedConsumable = this;
        actor.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg("Select a tile to teleport to.", "#63ffff");
        actor.GetComponent<Player>().Teleporting = true;
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.init.addMsg($"You teleport!", "#ffffff");

        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
