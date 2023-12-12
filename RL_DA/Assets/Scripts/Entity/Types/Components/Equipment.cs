using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Equipment : MonoBehaviour
{
    [SerializeField] private Equippable weapon;
    [SerializeField] private Equippable armor;

    public Equippable Weapon { get => weapon; set => weapon = value; }
    public Equippable Armor { get => armor; set => armor = value; }

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

        return bonus;
    }

    public bool ItemIsEquipped(Item item)
    {
        if (item.GetEquippable is null)
            return false;

        return item.GetEquippable == weapon || item.GetEquippable == armor;
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
        Equippable currentItem = slot == "Weapon" ? weapon : armor;

        if(currentItem is not null)
        {
            unequipFromSlot(slot, addMsg);
        }

        if(slot == "Weapon")
        {
            weapon = item.GetEquippable;
        }
        else
        {
            armor = item.GetEquippable;
        }

        if (addMsg)
        {
            equipMsg(item.name);
        }

        item.name = $"{item.name} (E)";
        
    }

    public void unequipFromSlot(string slot, bool addMsg)
    {
        Equippable currentItem = slot == "Weapon" ? weapon : armor;
        currentItem.name = currentItem.name.Replace(" (E)", "");

        if (addMsg)
        {
            unequipMsg(currentItem.name);
        }

        if(slot == "Weapon")
        {
            weapon = null;
        }
        else
        {
            armor = null;
        }
        // Stopd 2:53
    }

    public void toggleEquip(Item equippableItem, bool addMsg = true)
    {
        string slot = equippableItem.GetEquippable.EquipmentType == EquipmentType.Weapon ? "Weapon" : "Armor";

        if (ItemIsEquipped(equippableItem))
        {
            unequipFromSlot(slot, addMsg);
        }
        else
        {
            equipToSlot(slot, equippableItem, addMsg);
        }
    }

}
