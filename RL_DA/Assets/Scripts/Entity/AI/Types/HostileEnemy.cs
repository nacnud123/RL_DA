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

    public void RunAI()
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

                float targetDis = Vector3.Distance(transform.position, fighter.Target.transform.position);

                if(targetDis < 1.5f) // If it close to the player attack it.
                {
                    Action.meleeAction(GetComponent<Actor>(), fighter.Target);
                    return;
                }
                else // If it is not close to the player continue to walk to it.
                {
                    moveAlongPath(targetPos);
                    return;
                }
            }
        }

        Action.skipAction();

    }

}
