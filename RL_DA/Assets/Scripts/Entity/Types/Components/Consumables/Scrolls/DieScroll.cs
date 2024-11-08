using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScroll : Scroll
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Fighter>().Die();
        UIManager.init.addMsg($"You read the scroll, you don't feel so good.", "#00ff00");
        Consume(actor);
        return true;
    }

}
