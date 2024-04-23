using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Fighter>().Hp = 0;
        actor.GetComponent<Fighter>().Die();
        UIManager.init.addMsg($"You consume the {name}, and died.", "#00ff00");
        Consume(actor);
        return true;
    }

}
