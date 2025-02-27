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
            if (gameObject.transform.parent.GetComponent<Equipment>().PrimaryWeapon is null)
                gameObject.transform.parent.GetComponent<Equipment>().PrimaryWeapon = this;
            else if (gameObject.transform.parent.GetComponent<Equipment>().SecondaryWeapon is null)
                gameObject.transform.parent.GetComponent<Equipment>().SecondaryWeapon = this;
            else
                Debug.Log($"Error with equipping {this.name}! Please check it out!");

        }
    }
}
