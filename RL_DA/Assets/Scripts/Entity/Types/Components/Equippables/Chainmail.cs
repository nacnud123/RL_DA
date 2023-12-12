using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainmail : Equippable
{
    public Chainmail()
    {
        EquipmentType = EquipmentType.Armor;
        DefenseBonus = 3;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;
        }
    }
}