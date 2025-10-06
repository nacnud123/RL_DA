using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepScroll : Scroll
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Player>().NumSlTurns = Random.Range(5, 10);
        UIManager.init.addMsg($"You read the scroll, you feel really tired.", "#00ff00");
        Consume(actor);
        return true;
    }
}
