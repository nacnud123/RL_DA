using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionPot : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Player>().ConfTurns += Random.Range(2, 10);
        UIManager.init.addMsg("You feel dizzy!", "#00ff00");
        Consume(actor);
        return true;

    }
}
