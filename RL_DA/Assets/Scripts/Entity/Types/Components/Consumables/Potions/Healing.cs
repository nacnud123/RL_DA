using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Consumable
{
    [SerializeField] private int maxAmmount = 0, minAmmount = 0;
    [SerializeField] private int amount = 0;


    public int GetAmount { get => amount; }

    public override bool Activate(Actor consumer)
    {
        amount = Random.Range(minAmmount, maxAmmount);

        int amountRec = consumer.GetComponent<Fighter>().Heal(amount);

        if (amountRec > 0)
        {
            UIManager.init.addMsg($"You consume the {this.GetComponent<Item>().RealName}, and healed {amountRec} HP.", "#00ff00");
            Consume(consumer);
            return true;
        }
        else
        {
            UIManager.init.addMsg($"You are already at full health", "#808080");
            return false;
        }
    }

}
