using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Equipment : MonoBehaviour
{
    [SerializeField] private Equippable primaryWeapon;
    [SerializeField] private Equippable secondaryWeapon;

    [SerializeField] private Equippable armor;
    [SerializeField] private Equippable ring;
    [SerializeField] private Equippable ranged;


    public Equippable PrimaryWeapon { get => primaryWeapon; set => primaryWeapon = value; }
    public Equippable SecondaryWeapon { get => secondaryWeapon; set => secondaryWeapon = value; }

    public Equippable Armor { get => armor; set => armor = value; }
    public Equippable Ring { get => ring; set => ring = value; }
    public Equippable Ranged { get => ranged; set => ranged = value; }


    public int DefenseBonus()
    {
        int bonus = 0;

        if (primaryWeapon is not null) bonus += primaryWeapon.DefenseBonus;
        if (secondaryWeapon is not null) bonus += secondaryWeapon.DefenseBonus;
        if (armor is not null) bonus += armor.DefenseBonus;
        if (ring is not null) bonus += ring.DefenseBonus;

        return bonus;
    }

    public int PowerBonus()
    {
        int bonus = 0;

        if (primaryWeapon is not null) bonus += GameManager.init.getDamage(primaryWeapon.Damage);
        if (secondaryWeapon is not null) bonus += GameManager.init.getDamage(secondaryWeapon.Damage);
        if (armor is not null) bonus += armor.PowerBonus;
        if (ring is not null) bonus += ring.PowerBonus;

        return bonus;
    }

    public bool ItemIsEquipped(Item item)
    {
        if (item.GetEquippable is null) return false;

        return item.GetEquippable == primaryWeapon ||
               item.GetEquippable == secondaryWeapon ||
               item.GetEquippable == armor ||
               item.GetEquippable == ring ||
               item.GetEquippable == ranged;
    }

    public void unequipMsg(string name)
    {
        UIManager.init.addMsg($"You remove the {name}.", "#da8ee7");
    }

    public void equipMsg(string name)
    {
        UIManager.init.addMsg($"You equip the {name}.", "#a000c8");
    }

    public void updateName(string slot, Item item, int amount)
    {
        Equippable currentItem = getSlot(slot);
        Item temp = currentItem.GetComponent<Item>();
        currentItem.name = currentItem.name.Replace($" ({amount + 1}) ", $" ({amount}) ");
        temp.CurrName = temp.CurrName.Replace($" ({amount + 1}) ", $" ({amount}) ");
    }

    public void equipToSlot(string slot, Item item, bool addMsg)
    {
        Equippable newEquip = item.GetEquippable;

        if (slot == "Weapon" && newEquip is Weapon newWeapon)
        {
            if (newWeapon.IsTwoHanded)
            {
                unequipFromSlot("Weapon", addMsg);
                primaryWeapon = newWeapon;
                primaryWeapon.EquipmentType = EquipmentType.PrimarySlot;
            }
            else
            {
                if (primaryWeapon == null)
                {
                    primaryWeapon = newWeapon;
                    primaryWeapon.EquipmentType = EquipmentType.PrimarySlot;
                }
                else if (secondaryWeapon == null)
                {
                    secondaryWeapon = newWeapon;
                    secondaryWeapon.EquipmentType = EquipmentType.SecondarySlot;
                }
                else
                {
                    unequipFromSlot("Weapon", addMsg, secondaryWeapon.GetComponent<Weapon>());
                    secondaryWeapon = newWeapon;
                    secondaryWeapon.EquipmentType = EquipmentType.SecondarySlot;
                }
            }
        }
        else if (slot == "Armor")
        {
            unequipFromSlot(slot, addMsg);
            armor = newEquip;
        }
        else if (slot == "Ranged")
        {
            unequipFromSlot(slot, addMsg);
            ranged = newEquip;
        }
        else if (slot == "Ring")
        {
            unequipFromSlot(slot, addMsg);
            ring = newEquip;
            ring.equip(this.GetComponent<Actor>());
        }

        if (addMsg) equipMsg(item.CurrName);

        item.name = $"{item.name} (E)";
        item.CurrName = $"{item.CurrName} (E)";
    }

    public void unequipFromSlot(string slot, bool addMsg, Weapon specificWeapon = null)
    {
        if (slot == "Weapon")
        {
            if (specificWeapon != null)
            {
                if (primaryWeapon != null && ReferenceEquals(primaryWeapon, specificWeapon))
                {
                    unequipMsg(primaryWeapon.name);
                    RemoveEquipTag(primaryWeapon);
                    primaryWeapon.EquipmentType = EquipmentType.Weapon; // Restore type
                    primaryWeapon = null;
                }
                else if (secondaryWeapon != null && ReferenceEquals(secondaryWeapon, specificWeapon))
                {
                    unequipMsg(secondaryWeapon.name);
                    RemoveEquipTag(secondaryWeapon);
                    secondaryWeapon.EquipmentType = EquipmentType.Weapon; // Restore type
                    secondaryWeapon = null;
                }
            }
            else
            {
                if (secondaryWeapon != null)
                {
                    unequipMsg(secondaryWeapon.name);
                    RemoveEquipTag(secondaryWeapon);
                    secondaryWeapon.EquipmentType = EquipmentType.Weapon; // Restore type
                    secondaryWeapon = null;
                }
                else if (primaryWeapon != null)
                {
                    unequipMsg(primaryWeapon.name);
                    RemoveEquipTag(primaryWeapon);
                    primaryWeapon.EquipmentType = EquipmentType.Weapon; // Restore type
                    primaryWeapon = null;
                }
            }
        }
        else
        {
            Equippable currentItem = getSlot(slot);
            if (currentItem == null) return;

            unequipMsg(currentItem.name);
            RemoveEquipTag(currentItem);

            if (slot == "Armor") armor = null;
            else if (slot == "Ranged") ranged = null;
            else if (slot == "Ring")
            {
                ring.unequip(this.GetComponent<Actor>());
                ring = null;
            }
        }
    }

    // Helper function to add correct equip tag
    private void SetEquipTag(Equippable item, string tag)
    {
        Item temp = item.GetComponent<Item>();
        RemoveEquipTag(item); // Ensure no duplicate tags
        temp.name = $"{temp.name} {tag}";
        temp.CurrName = $"{temp.CurrName} {tag}";
    }

    // Helper function to remove "(E)", "(R)", and "(L)" tags
    private void RemoveEquipTag(Equippable item)
    {
        Item temp = item.GetComponent<Item>();
        temp.name = temp.name.Replace(" (E)", "").Replace(" (R)", "").Replace(" (L)", "").Trim();
        temp.CurrName = temp.CurrName.Replace(" (E)", "").Replace(" (R)", "").Replace(" (L)", "").Trim();
    }



    public void toggleEquip(Item equippableItem, bool addMsg = true)
    {
        Equippable equip = equippableItem.GetEquippable;

        if (equip == null) return;

        string slot = "";

        switch (equip.EquipmentType)
        {
            case EquipmentType.Weapon:
            case EquipmentType.PrimarySlot:
            case EquipmentType.SecondarySlot:
                slot = "Weapon";
                break;
            case EquipmentType.Armor:
                slot = "Armor";
                break;
            case EquipmentType.Ring:
                slot = "Ring";
                break;
            case EquipmentType.Ranged:
                slot = "Ranged";
                break;
        }

        if (ItemIsEquipped(equippableItem))
        {
            // Ensure we remove the correct weapon (Primary or Secondary)
            if (equip.EquipmentType == EquipmentType.PrimarySlot)
            {
                unequipFromSlot(slot, addMsg, (Weapon)primaryWeapon);
            }
            else if (equip.EquipmentType == EquipmentType.SecondarySlot)
            {
                unequipFromSlot(slot, addMsg, (Weapon)secondaryWeapon);
            }
            else
            {
                unequipFromSlot(slot, addMsg);
            }
        }
        else
        {
            equipToSlot(slot, equippableItem, addMsg);
        }
    }

    public Equippable getSlot(string inSlot)
    {
        if (inSlot == "Weapon")
        {
            return primaryWeapon ?? secondaryWeapon;
        }
        return inSlot switch
        {
            "Armor" => armor,
            "Ring" => ring,
            "Ranged" => ranged,
            _ => null
        };
    }

}
