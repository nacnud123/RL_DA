using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileWand : Wand
{
    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg($"Select a location to cast a magic missile.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor actor, Actor target)
    {
        if (Uses > 0)
        {
            int dmg = GameManager.init.getDamage(Damage) + 1;

            Uses -= 1;

            UIManager.init.addMsg($"The {target.name} is hit with a magic missile, it takes {dmg} damage!", "#ff0000");
            target.GetComponent<Fighter>().Hp -= dmg;
        }
        else
        {
            if(Random.Range(0, 6) == 3)
            {
                UIManager.init.addMsg($"The wand misfires!", "#ff0000");
                GameManager.init.makeExplosion(actor.transform.position, 3);
            }
            else
            {
                UIManager.init.addMsg($"The wand is out of uses!", "#ff0000");
            }
        }

        actor.GetComponent<Player>().ToggleTargetMode();
        return true;
    }


}
