using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    public enum ConsumableType
    {
        Healing,
    }

    [SerializeField] private ConsumableType consumableType;
    [SerializeField] private int amount = 0;

    public ConsumableType GetConsumableType { get => consumableType; }
    public int GetAmount { get => amount; }

    public bool Activate(Actor actor, Item item)
    {
        switch (consumableType)
        {
            case ConsumableType.Healing:
                return Healing(actor, item);
            default:
                return false;
        }
    }

    private bool Healing(Actor actor, Item item)
    {
        int amountRec = actor.GetComponent<Fighter>().Heal(amount);

        if(amountRec > 0)
        {
            UIManager.init.addMsg($"You consume the {name}, and healed {amountRec} HP.", "#00ff00");
            Consume(actor, item);
            return true;
        }
        else
        {
            UIManager.init.addMsg($"You are already at full health", "#808080");
            return false;
        }
    }

    private void Consume(Actor actor, Item item)
    {
        actor.GetInventory.GetItems.Remove(item);
        Destroy(item.gameObject);
    }
}
