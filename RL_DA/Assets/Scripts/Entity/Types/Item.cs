using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Entity
{
    private Consumable consumable;

    public Consumable GetConsumable { get => consumable; }

    private void OnValidate()
    {
        if (GetComponent<Consumable>())
        {
            consumable = GetComponent<Consumable>();
        }
    }

    private void Start()
    {
        addToGameManager();
    }

}
