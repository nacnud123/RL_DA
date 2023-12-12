using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class Dagger : Equippable
{
    public Dagger()
    {
        EquipmentType = EquipmentType.Weapon;
        PowerBonus = 2;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Weapon = this;
        }
    }
}
