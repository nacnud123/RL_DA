using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Equipment : MonoBehaviour
{
    [SerializeField] private Equippable weapon;
    [SerializeField] private Equippable armor;
    [SerializeField] private Equippable ring;
    

    public Equippable Weapon { get => weapon; set => weapon = value; }
    public Equippable Armor { get => armor; set => armor = value; }
    public Equippable Ring { get => ring; set => ring = value; }
    

    public int DefenseBonus()
    {
        int bonus = 0;

        if(weapon is not null && weapon.DefenseBonus > 0)
        {
            bonus += weapon.DefenseBonus;
        }

        if(armor is not null && armor.DefenseBonus > 0)
        {
            bonus += armor.DefenseBonus;
        }

        if (ring is not null && ring.DefenseBonus > 0)
        {
            bonus += ring.DefenseBonus;
        }

        return bonus;
    }

    public int PowerBonus()
    {
        int bonus = 0;

        if(weapon is not null && weapon.PowerBonus > 0)
        {
            bonus += weapon.PowerBonus;
        }

        if(armor is not null && armor.PowerBonus > 0)
        {
            bonus += armor.PowerBonus;
        }

        if(ring is not null && ring.PowerBonus > 0)
        {
            bonus += ring.PowerBonus;
        }

        return bonus;
    }

    public bool ItemIsEquipped(Item item)
    {
        if (item.GetEquippable is null)
            return false;

        return item.GetEquippable == weapon || item.GetEquippable == armor || item.GetEquippable == ring;
    }

    public void unequipMsg(string name)
    {
        UIManager.init.addMsg($"You remove the {name}.", "#da8ee7");
    }

    public void equipMsg(string name)
    {
        UIManager.init.addMsg($"You equip the {name}.", "#a000c8");
    }

    public void equipToSlot(string slot, Item item, bool addMsg)
    {
        Equippable currentItem = getSlot(slot);

        if(currentItem is not null)
        {
            unequipFromSlot(slot, addMsg);
        }

        if(slot == "Weapon")
        {
            weapon = item.GetEquippable;
            
        }
        else if(slot == "Armor")
        {
            armor = item.GetEquippable;
        }
        else
        {
            ring = item.GetEquippable;
            ring.equip(this.GetComponent<Actor>());
        }

        if (addMsg)
        {
            equipMsg(item.CurrName);
        }

        item.name = $"{item.name} (E)";
        item.CurrName = $"{item.CurrName} (E)";
        
    }

    public void unequipFromSlot(string slot, bool addMsg)
    {
        Equippable currentItem = getSlot(slot);
        Item temp = currentItem.GetComponent<Item>();
        currentItem.name = currentItem.name.Replace(" (E)", "");
        temp.CurrName = temp.CurrName.Replace(" (E)", "");

        if (addMsg)
        {
            unequipMsg(temp.CurrName);
        }

        if(slot == "Weapon")
        {
            weapon = null;
        }
        else if(slot == "Armor")
        {
            armor = null;
        }
        else
        {
            ring.unequip(this.GetComponent<Actor>());
            ring = null;
        }
    }

    public void toggleEquip(Item equippableItem, bool addMsg = true)
    {
        string slot = "";

        switch (equippableItem.GetEquippable.EquipmentType)
        {
            case EquipmentType.Weapon:
                slot = "Weapon";
                break;
            case EquipmentType.Armor:
                slot = "Armor";
                break;
            case EquipmentType.Ring:
                slot = "Ring";
                break;
        }

        if (ItemIsEquipped(equippableItem))
        {
            unequipFromSlot(slot, addMsg);
        }
        else
        {
            equipToSlot(slot, equippableItem, addMsg);
        }
    }

    public Equippable getSlot(string inSlot)
    {

        switch (inSlot)
        {
            case "Weapon":
                return weapon;
            case "Armor":
                return armor;
            case "Ring":
                return ring;
        }
        return null;
    }

}
