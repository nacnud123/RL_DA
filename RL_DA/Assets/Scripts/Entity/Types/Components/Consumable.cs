using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    public virtual bool Activate(Actor actor) => false;
    public virtual bool Cast(Actor actor, Actor target) => false;
    public virtual bool Cast(Actor actor, List<Actor> targets) => false;

    public void Consume(Actor consumer)
    {
        if(consumer.GetComponent<Inventory>().SelectedConsumable == this)
        {
            consumer.GetComponent<Inventory>().SelectedConsumable = null;
        }

        consumer.GetInventory.GetItems.Remove(GetComponent<Item>());
        GameManager.init.removeEntity(GetComponent<Item>());
        Destroy(gameObject);
    }
}
