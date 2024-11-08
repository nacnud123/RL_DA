using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealMapScroll : Scroll
{

    public override bool Activate(Actor actor)
    {
        MapManager.init.revealMap();
        UIManager.init.addMsg($"You read the scroll. It turns out to be a map!", "#ffffff");
        actor.updateFOV();
        return true;
    }
}
