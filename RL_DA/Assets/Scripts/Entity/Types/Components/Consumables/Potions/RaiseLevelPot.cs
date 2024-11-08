using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseLevelPot : Potion
{
    public override bool Activate(Actor actor)
    {
        int ammountAdd = actor.GetComponent<Level>().XpToNextLevel + 1;
        actor.GetComponent<Level>().AddXP(ammountAdd);
        UIManager.init.addMsg("You feel like you have more experience.", "#00ff00");
        Consume(actor);
        return true;

    }
}
