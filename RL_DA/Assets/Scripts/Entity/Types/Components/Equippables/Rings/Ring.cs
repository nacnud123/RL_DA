using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : Equippable
{
    public Ring()
    {
        EquipmentType = EquipmentType.Ring;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;
        }
    }
}
