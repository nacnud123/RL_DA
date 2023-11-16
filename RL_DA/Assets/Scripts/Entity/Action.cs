using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Action
{
    public static void pickupAction(Actor actor)
    {
        for(int i = 0; i < GameManager.init.getEntities.Count; i++)
        {
            if (GameManager.init.getEntities[i].GetComponent<Actor>() || actor.transform.position != GameManager.init.getEntities[i].transform.position)
                continue;

            if (actor.GetInventory.GetItems.Count >= actor.GetInventory.GetCapacity)
            {
                UIManager.init.addMsg($"Your inventory is full.", "#808080"); return;
            }

            Item item = GameManager.init.getEntities[i].GetComponent<Item>();
            item.transform.SetParent(actor.transform);
            actor.GetInventory.GetItems.Add(item);

            UIManager.init.addMsg($"You picked up the {item.name}.", "#ffffff");

            GameManager.init.removeEntity(item);
            GameManager.init.endTurn();
        }
    }

    public static void dropAction(Actor actor, Item item)
    {
        actor.GetInventory.Drop(item);

        UIManager.init.toggleDropMenu();
        GameManager.init.endTurn();
    }

    public static void useAction(Actor consumer, Item item)
    {
        bool itemUsed = false;

        if (item.GetComponent<Consumable>())
            itemUsed = item.GetComponent<Consumable>().Activate(consumer);

        UIManager.init.toggleInv();


        if (itemUsed)
        {
            GameManager.init.endTurn();
        }
    }


    public static bool bumpAction(Actor actor, Vector2 dir)
    {
        Actor target = GameManager.init.GetActorAtLocation(actor.transform.position + (Vector3)dir);

        if (target)
        {
            meleeAction(actor, target);
            return false;
        }
        else
        {
            movementAction(actor, dir);
            return true;
        }
    }

    public static void meleeAction(Actor actor, Actor target)
    {
        int dmg = actor.GetComponent<Fighter>().getPower - target.GetComponent<Fighter>().getDefense;

        string attackDesc = $"{actor.name} attacks {target.name}";

        string colorHex = "";

        if (actor.GetComponent<Player>())
            colorHex = "#ffffff";
        else
            colorHex = "#d1a3a4";

        if(dmg > 0)
        {
            UIManager.init.addMsg($"{attackDesc} for {dmg} hit points", colorHex);
            target.GetComponent<Fighter>().Hp -= dmg;
        }
        else
        {
            UIManager.init.addMsg($"{attackDesc} but does not damage.", colorHex);
        }
        GameManager.init.endTurn();
    }

    public static void movementAction(Actor actor, Vector2 dir)
    {
        actor.Move(dir);
        actor.updateFOV();
        GameManager.init.endTurn();
    }

    public static void waitAction()
    {
        GameManager.init.endTurn();
    }

    public static void CastAction(Actor consumer, Actor target, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, target);

        if (castSuccess)
            GameManager.init.endTurn();
    }

    public static void CastAction(Actor consumer, List<Actor> targets, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, targets);

        if (castSuccess)
            GameManager.init.endTurn();
    }
}