using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsterScroll : Consumable
{
    public override bool Activate(Actor actor)
    {
        actor.GetComponent<Inventory>().SelectedConsumable = this;

        List<Vector3> validNeigh = new List<Vector3>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                Vector2Int neighPos = new Vector2Int((int)actor.transform.position.x - x, (int)actor.transform.position.y - y);

                if(y != 0 || x != 0)
                {
                    if(MapManager.init.isValidPos(new Vector3(neighPos.x, neighPos.y)))
                    {
                        validNeigh.Add(new Vector3(neighPos.x, neighPos.y));
                    }
                }


            }
        }

        if(validNeigh.Count != 0)
        {
            int randPos = Random.Range(0, validNeigh.Count);

            MapManager.init.createEntity("Monsters/Emu", validNeigh[randPos]);
        }


        return true;
    }


}
