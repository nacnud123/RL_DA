using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class HostileEnemy : AI
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private bool isFighting;

    private void OnValidate() // Get components
    {
        fighter = GetComponent<Fighter>();
        AStar = GetComponent<AStar>();
    }

    public override void RunAI()
    {
        if (isFrozen)
        {
            frozenTurns -= 1;
            if(frozenTurns != 0)
            {
                Action.waitAction();
                return;
            }
            else
            {
                isFrozen = false;
            }
        }

        if (isPoisoned)
        {
            poisonedTurns -= 1;
            if(poisonedTurns != 0)
            {
                fighter.Hp -= Random.Range(1, 10);
            }
            else
            {
                isPoisoned = false;
            }
        }

        if (!fighter.Target) // If it does not have a target set that target to the player
        {
            fighter.Target = GameManager.init.getActors[0];
        }
        else if (fighter.Target && !fighter.Target.IsAlive)
        {
            fighter.Target = null;
        }

        if (fighter.Target) // If it does have a target then make it go to the player and attack it
        {
            Vector3Int targetPos = MapManager.init.getFloorMap.WorldToCell(fighter.Target.transform.position);
            if (isFighting || GetComponent<Actor>().getFOV.Contains(targetPos))
            {
                if (!isFighting)
                    isFighting = true;

                float targetDis = Vector3.Distance(transform.position, fighter.Target.transform.position);
                Actor acotr = GetComponent<Actor>();
                Vector3 closestTilePos = transform.position; // Maybe reduntend. Is hold-over from multi-tile enemies.

                if (targetDis < 1.5f) // If it close to the player attack it.
                {
                    Action.meleeAction(GetComponent<Actor>(), fighter.Target);
                    return;
                }
                else // If it is not close to the player continue to walk to it.
                {
                    moveAlongPath(closestTilePos, targetPos);
                    return;
                }
            }
        }

        Action.waitAction();

    }

    public override AIState SaveState() => new AIState(
        _type: "HostileEnemy"
        );
}
