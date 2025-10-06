using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthRing : Ring
{
    public override bool equip(Actor actor)
    {
        actor.GetComponent<Fighter>().BasePower += 2;
        UIManager.init.addMsg("You feel stronger!", "#a000c8");
        return true;
    }

    public override bool unequip(Actor actor)
    {
        actor.GetComponent<Fighter>().BasePower -= 2;
        UIManager.init.addMsg("You no longer feel stronger.", "#da8ee7");
        return true;
    }
}
