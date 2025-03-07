using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Item))]
public class Equippable : MonoBehaviour
{
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private int powerBonus = 0;
    [SerializeField] private int defenseBonus = 0;
    [SerializeField] private string damage = "";
    [SerializeField] private bool isTwoHanded = false;


    public string Damage { get => damage; }
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public int PowerBonus { get => powerBonus; set => powerBonus = value; }
    public int DefenseBonus { get => defenseBonus; set => defenseBonus = value; }
    public bool IsTwoHanded { get => isTwoHanded; }


    public virtual bool equip(Actor actor) => false;
    public virtual bool unequip(Actor actor) => false;

}
