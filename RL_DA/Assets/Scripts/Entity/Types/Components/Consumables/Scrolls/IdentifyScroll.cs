using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifyScroll : Scroll
{
    public override bool Activate(Actor actor)
    {
        
        UIManager.init.toggleInv(actor);

        Consume(actor);

        UIManager.init.toggleIdentifyMenu(actor);
        
        return true;
    }

}
