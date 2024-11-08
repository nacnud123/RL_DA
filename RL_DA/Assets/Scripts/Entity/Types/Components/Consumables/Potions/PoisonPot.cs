using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPot : Potion
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Player>().PoisTurns = Random.Range(2, 6);
        UIManager.init.addMsg("You don't feel good.", "00ff00");
        Consume(actor);
        return true;
    }
}
