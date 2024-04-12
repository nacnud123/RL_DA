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

                Actor actor = GetComponent<Actor>();
                float targetDistance;
                Vector3 closestTilePos = transform.position;

                if(actor.Size.x > 1 || actor.Size.y > 1)
                {
                    float closestDistance = float.MaxValue;
                    for(int i = 0; i < actor.OccupiedTiles.Length; i++)
                    {
                        float distance = Vector3.Distance(actor.OccupiedTiles[i], fighter.Target.transform.position);
                        if(distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTilePos = actor.OccupiedTiles[i];
                        }
                    }
                    targetDistance = closestDistance;
                }
                else
                {
                    targetDistance = Vector3.Distance(transform.position, fighter.Target.transform.position);
                }

                if(targetDistance < 1.5f) // If it close to the player attack it.
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
