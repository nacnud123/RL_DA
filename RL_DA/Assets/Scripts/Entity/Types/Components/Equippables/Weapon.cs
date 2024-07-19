using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equippable
{
   

    public Weapon()
    {
        EquipmentType = EquipmentType.Weapon;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Weapon = this;
        }
    }
}
