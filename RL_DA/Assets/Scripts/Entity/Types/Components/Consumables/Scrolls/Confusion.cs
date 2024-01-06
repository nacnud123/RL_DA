using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confusion : Consumable
{
    [SerializeField] private int numOfTurns = 10;

    public int GetNumOfTurns { get => numOfTurns; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.init.addMsg($"Select a target to confuse.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        if(target.TryGetComponent(out ConfusedEnemy confusedEnemy))
        {
            if(confusedEnemy.TurnsRem > 0)
            {
                UIManager.init.addMsg($"The {target.name} is already confused.", "#FF0000");
                consumer.GetComponent<Inventory>().SelectedConsumable = null;
                consumer.GetComponent<Player>().ToggleTargetMode();
                return false;
            }
        }
        else
        {
            confusedEnemy = target.gameObject.AddComponent<ConfusedEnemy>();
        }
        confusedEnemy.PrevAI = target.AI;
        confusedEnemy.TurnsRem = numOfTurns;

        UIManager.init.addMsg($"You cast confusion. {target.name} starts to stumble around!", "#ff0000");
        target.AI = confusedEnemy;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
