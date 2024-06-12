using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealMapScroll : Consumable
{

    public override bool Activate(Actor actor)
    {
        MapManager.init.revealMap();
        actor.updateFOV();
        return true;
    }
}
