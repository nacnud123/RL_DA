using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private AmmoType requiredAmmo;
    [SerializeField] private string rangedDamage = "";

    public string RangedDamage { get => rangedDamage; set => rangedDamage = value; }
    public AmmoType RequiredAmmo { get => requiredAmmo; set => requiredAmmo = value; }

    public RangedWeapon()
    {
        EquipmentType = EquipmentType.Ranged;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Ranged = this;
        }
    }

    public bool canFire(AmmoType inType)
    {
        if(inType == requiredAmmo)
            return true;
        return false;
    }

    public void fireShot()
    {
        ((RangedAmmo)GameManager.init.getActors[0].GetComponent<Actor>().GetInventory.GetItems.Find(x => x.GetType() == typeof(RangedAmmo))).Amount -= 1;
    }
}

