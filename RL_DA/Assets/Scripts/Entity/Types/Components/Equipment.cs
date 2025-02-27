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

        if (slot == "Weapon" && newEquip is Weapon newWeapon) // Spaghetti code!
        {
            if (newWeapon.IsTwoHanded)
            {
                unequipFromSlot("PrimarySlot", addMsg);
                unequipFromSlot("SecondarySlot", addMsg);
                primaryWeapon = newWeapon;
                primaryWeapon.EquipmentType = EquipmentType.PrimarySlot;
                SetEquipTag(primaryWeapon, "(R L) (E)");
            }
            else
            {
                if(primaryWeapon is not null && primaryWeapon.IsTwoHanded)
                {
                    unequipFromSlot("PrimarySlot", addMsg);
                    primaryWeapon = newWeapon;
                    primaryWeapon.EquipmentType = EquipmentType.PrimarySlot;
                    SetEquipTag(primaryWeapon, "(R) (E)");
                }
                else
                {
                    if (primaryWeapon == null)
                    {
                        primaryWeapon = newWeapon;
                        primaryWeapon.EquipmentType = EquipmentType.PrimarySlot;
                        SetEquipTag(primaryWeapon, "(R) (E)");
                    }
                    else if (secondaryWeapon == null)
                    {
                        secondaryWeapon = newWeapon;
                        secondaryWeapon.EquipmentType = EquipmentType.SecondarySlot;
                        SetEquipTag(secondaryWeapon, "(L) (E)");
                    }
                    else
                    {
                        unequipFromSlot("SecondarySlot", addMsg);
                        secondaryWeapon = newWeapon;
                        secondaryWeapon.EquipmentType = EquipmentType.SecondarySlot;
                        SetEquipTag(secondaryWeapon, "(L) (E)");
                    }
                }
                
            }
        }
        else if (slot == "Armor")
        {
            unequipFromSlot(slot, addMsg);
            armor = newEquip;
            SetEquipTag(armor,"(E)");
        }
        else if (slot == "Ranged")
        {
            unequipFromSlot(slot, addMsg);
            ranged = newEquip;
            SetEquipTag(ranged, "(E)");
        }
        else if (slot == "Ring")
        {
            unequipFromSlot(slot, addMsg);
            ring = newEquip;
            ring.equip(this.GetComponent<Actor>());
            SetEquipTag(ring, "(E)");
        }

        if (addMsg) equipMsg(item.CurrName);
    }

    public void unequipFromSlot(string slot, bool addMsg)
    {

        Equippable currentItem = getSlot(slot);
        if (currentItem == null) return;

        unequipMsg(currentItem.name);
        RemoveEquipTag(currentItem);

        if (slot == "PrimarySlot") { primaryWeapon.EquipmentType = EquipmentType.Weapon; primaryWeapon = null; }
        else if (slot == "SecondarySlot") { secondaryWeapon.EquipmentType = EquipmentType.Weapon; secondaryWeapon = null; }
        else if (slot == "Armor") armor = null;
        else if (slot == "Ranged") ranged = null;
        else if (slot == "Ring")
        {
            ring.unequip(this.GetComponent<Actor>());
            ring = null;
        }
    }

    private void SetEquipTag(Equippable item, string tag)
    {
        Item temp = item.GetComponent<Item>();
        RemoveEquipTag(item); // Ensure no duplicate tags
        temp.name = $"{temp.name} {tag}";
        temp.CurrName = $"{temp.CurrName} {tag}";
    }

    private void RemoveEquipTag(Equippable item)
    {
        Item temp = item.GetComponent<Item>();
        temp.name = temp.name.Replace(" (E)", "").Replace(" (R)", "").Replace(" (L)", "").Replace("(R L)", "").Trim();
        temp.CurrName = temp.CurrName.Replace(" (E)", "").Replace(" (R)", "").Replace(" (L)", "").Replace("(R L)", "").Trim();
    }

    public void toggleEquip(Item equippableItem, bool addMsg = true)
    {
        Equippable equip = equippableItem.GetEquippable;

        if (equip == null) return;

        string slot = "";

        switch (equip.EquipmentType)
        {
            case EquipmentType.Weapon:
                slot = "Weapon";
                break;
            case EquipmentType.PrimarySlot:
                slot = "PrimarySlot";
                break;
            case EquipmentType.SecondarySlot:
                slot = "SecondarySlot";
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
                unequipFromSlot(slot, addMsg);
            }
            else if (equip.EquipmentType == EquipmentType.SecondarySlot)
            {
                unequipFromSlot(slot, addMsg);
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
            "PrimarySlot" => primaryWeapon,
            "SecondarySlot" => secondaryWeapon,
            "Armor" => armor,
            "Ring" => ring,
            "Ranged" => ranged,
            _ => null
        };
    }

}
