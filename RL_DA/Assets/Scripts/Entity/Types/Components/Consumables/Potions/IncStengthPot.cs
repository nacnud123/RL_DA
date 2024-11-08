using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncStengthPot : Potion
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Fighter>().BasePower += 1;
        UIManager.init.addMsg("You feel stronger!", "#00ff00");
        Consume(actor);
        return true;
    }
}
