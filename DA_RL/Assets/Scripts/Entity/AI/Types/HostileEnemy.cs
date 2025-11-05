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

        if (fighter.Target)
        {
            Actor actor = GetComponent<Actor>();
            Vector3 currentPos = transform.position;
            Vector3 targetPosition = fighter.Target.transform.position;
            Vector3Int targetPos = MapManager.init.getFloorMap.WorldToCell(targetPosition);

            if (isFighting || actor.getFOV.Contains(targetPos))
            {
                if (!isFighting)
                    isFighting = true;

                float targetDis = Vector3.Distance(currentPos, targetPosition);
                Vector3 closestTilePos = currentPos;

                if (targetDis < 1.5f)
                {
                    Action.attackAction(actor, fighter.Target);
                    return;
                }
                else
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
