using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

static public class Action
{
    public static void pickupAction(Actor actor)
    {
        for(int i = 0; i < GameManager.init.getEntities.Count; i++)
        {
            if (GameManager.init.getEntities[i].GetComponent<Actor>() || actor.transform.position != GameManager.init.getEntities[i].transform.position)
                continue;

            if (GameManager.init.getEntities[i].TryGetComponent<IntractableEntity>(out var intractableEntity))
            {
                intractableEntity.onInteract();
                GameManager.init.endTurn();
                continue;
            }

            Item item = GameManager.init.getEntities[i].GetComponent<Item>();

            // Check if we can carry this item
            if (!actor.GetInventory.CanCarry(item.Weight))
            {
                UIManager.init.addMsg($"The {item.CurrName} is too heavy to carry!", "#808080");
                return;
            }

            if(item.GetType() == typeof(RangedAmmo))
            {
                actor.GetInventory.addToAmmo(item, ((RangedAmmo)item).Type, ((RangedAmmo)item).Amount);
            }
            else
            {
                actor.GetInventory.Add(item, false); // Skip weight check since we already did it
            }


            UIManager.init.addMsg($"You picked up the {item.CurrName}.", "#ffffff");

            GameManager.init.endTurn();
        }
    }

    public static void TakeStairsAction(Actor actor)
    {
        Vector3Int pos = MapManager.init.getFloorMap.WorldToCell(actor.transform.position);
        string tileName = MapManager.init.getFloorMap.GetTile(pos).name;

        //Debug.Log($"Curr tile name: {tileName}");

        if(tileName != MapManager.init.UpStairsTile.name && tileName != MapManager.init.DownStairsTile.name)
        {
            UIManager.init.addMsg("There are no stairs here.", "#0da2ff");
            return;
        }
        if(SaveManager.init.CurrentFloor == 1 && tileName == MapManager.init.UpStairsTile.name)
        {
            UIManager.init.addMsg("You cannot go back up.", "#0da2ff");
            return;
        }

        SaveManager.init.SaveGame();
        SaveManager.init.CurrentFloor += tileName == MapManager.init.UpStairsTile.name ? -1 : 1; // Checks if it is and up stairs or down stairs

        if (SaveManager.init.Save.Scenes.Exists(x => x.FloorNumber == SaveManager.init.CurrentFloor)) // Goes up a floor if it exists
        {
            SaveManager.init.LoadScene(false);
        }
        else // If it does not exist then go down a floor and generate a new floor
        {
            GameManager.init.Reset(false);
            MapManager.init.GenerateDungeon();
        }

        UIManager.init.addMsg("You take the stairs", "#0da2ff");
        UIManager.init.setDungeonFloorText(SaveManager.init.CurrentFloor);

    }

    public static void TalkAction(Actor actor) // Not Done, Talk direction up,down,left,right like spell casting. 
    {
        Debug.Log("Talking!");
        for (int i = 0; i < GameManager.init.getActors.Count; i++)
        {
           
            // GameManager.init.GetActorAtLocation(actor.transform.position); maybe use this.
            if (!GameManager.init.getNPCAtLocation(actor.transform.position).GetComponent<NPC>())
                continue;

            GameManager.init.getNPCAtLocation(actor.transform.position).GetComponent<NPC>().onTalk();

            GameManager.init.endTurn();
        }
    }

    public static void dropAction(Actor actor, Item item)
    {
        if (actor.GetEquipment.ItemIsEquipped(item))
        {
            actor.GetEquipment.toggleEquip(item);
        }

        actor.GetInventory.Drop(item);

        UIManager.init.toggleDropMenu();
        GameManager.init.endTurn();
    }

    public static void useAction(Actor consumer, Item item)
    {
        bool itemUsed = false;

        if (item.GetComponent<IdentifyScroll>())
            UIManager.init.skipRest = true;

        if (item.GetConsumable is not null)
            itemUsed = item.GetComponent<Consumable>().Activate(consumer);
        
        if(!item.GetComponent<IdentifyScroll>())
            UIManager.init.toggleInv();


        if (itemUsed)
        {
            GameManager.init.endTurn();
        }
    }



    public static void identifyAction(Actor actor, Item item)
    {
        actor.GetInventory.identifyItem(item);
        UIManager.init.toggleIdentifyMenu();
        GameManager.init.endTurn();
    }

    public static bool bumpAction(Actor actor, Vector2 dir)
    {
        //Debug.Log("Moving!");
        Actor target = GameManager.init.GetActorAtLocation(actor.transform.position + (Vector3)dir);

        if (target && !target.GetComponent<NPC>())
        {
            attackAction(actor, target);
            return false;
        }
        else
        {
            movementAction(actor, dir);
            return true;
        }
    }

    public static void attackAction(Actor actor, Actor target, int _dmg = -1)
    {
        int dmg;
        if (_dmg != -1)
            dmg = _dmg - target.GetComponent<Fighter>().Defense();
        else
            dmg = actor.GetComponent<Fighter>().Power() - target.GetComponent<Fighter>().Defense();

        string attackDesc = $"{actor.RealName} attacks {target.RealName}";

        string colorHex = "";

        if (actor.GetComponent<Player>())
            colorHex = "#ffffff";
        else
            colorHex = "#d1a3a4";

        if(dmg > 0)
        {
            UIManager.init.addMsg($"{attackDesc} for {dmg} hit points", colorHex);
            target.GetComponent<Fighter>().Hp -= dmg;

            if(actor.GetComponent<Player>())
            {
                Camera.main.GetComponent<ScreenShake>().TriggerShake();
                SFXManager.init.playHitSfx();
                DamagePopup.Create(target.transform.position, dmg, false);
            }
        }
        else
        {
            UIManager.init.addMsg($"{attackDesc} but does not damage.", colorHex);
            if (actor.GetComponent<Player>())
            {
                SFXManager.init.playMissSfx();
            }
        }
        

        GameManager.init.endTurn();
    }

    public static void movementAction(Actor actor, Vector2 dir)
    {
        // Check if player is over-encumbered
        if (actor.GetComponent<Player>() && actor.GetInventory.IsOverEncumbered())
        {
            UIManager.init.addMsg("You are carrying too much to move!", "#ff0000");
            GameManager.init.endTurn();
            return;
        }

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

    static public void EquipAction(Actor actor, Item item)
    {
        if(item.GetEquippable is null)
        {
            UIManager.init.addMsg($"The {item.name} cannot be equipped.", "#808080");
            return;
        }

        actor.GetEquipment.toggleEquip(item);

        UIManager.init.toggleInv();
        GameManager.init.endTurn();
    }
}