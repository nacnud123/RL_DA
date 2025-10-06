using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballWand : Wand
{
    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(true, Radius);
        UIManager.init.addMsg($"Select a location to throw a fireball.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        if (Uses > 0)
        {
            int dmg = GameManager.init.getDamage(Damage) + 1;

            foreach (Actor target in targets)
            {
                UIManager.init.addMsg($"The {target.RealName} is engulfed in a fireball, taking {dmg} damage!", "#ff0000");
                target.GetComponent<Fighter>().Hp -= dmg;
            }

            Uses -= 1;

        }
        else
        {
            if (Random.Range(0, 6) == 3)
            {
                UIManager.init.addMsg($"The wand misfires!", "#ff0000");
                GameManager.init.makeExplosion(consumer.transform.position, 3);
            }
            else
            {
                UIManager.init.addMsg($"The wand is out of uses!", "#ff0000");
            }
        }



        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
