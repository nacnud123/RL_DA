using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : Consumable
{
    [SerializeField] private int uses = 10;
    [SerializeField] private string damage = "";
    [SerializeField] private int radius = 3;

    public string Damage { get => damage; }
    public int Radius { get => radius; }
    public int Uses { get => uses; set => uses = value; }

    public virtual void updateName()
    {

    }

}
