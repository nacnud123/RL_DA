using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Player>().NumSlTurns = Random.Range(5, 10);
        Consume(actor);
        return true;
    }
}
