using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeatherArmor : Equippable
{
    public LeatherArmor()
    {
        EquipmentType = EquipmentType.Armor;
        DefenseBonus = 1;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;
        }
    }
}
