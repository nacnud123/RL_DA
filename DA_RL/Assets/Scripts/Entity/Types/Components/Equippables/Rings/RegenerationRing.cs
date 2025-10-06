using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenerationRing : Ring
{
    public override bool equip(Actor actor)
    {
        actor.GetComponent<Player>().HealthRegen = true;
        UIManager.init.addMsg("You feel your health regenerate.", "#a000c8");
        return true;
    }
    public override bool unequip(Actor actor)
    {
        actor.GetComponent<Player>().HealthRegen = false;
        UIManager.init.addMsg("You feel worse.", "#da8ee7");
        return false;
    }
}
