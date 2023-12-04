using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class AI : MonoBehaviour
{
    [SerializeField] private AStar aStar;

    public AStar AStar { get => aStar; set => aStar = value; }

    private void OnValidate() => aStar = GetComponent<AStar>();

    public virtual void RunAI()
    {

    }

    public void moveAlongPath(Vector3Int targetPos)
    {
        Vector3Int gridPosition = MapManager.init.getFloorMap.WorldToCell(transform.position);
        Vector2 dir = aStar.Compute((Vector2Int)gridPosition, (Vector2Int)targetPos);
        Action.movementAction(GetComponent<Actor>(), dir);
    }

    public virtual AIState SaveState() => new AIState();
}

[System.Serializable]
public class AIState
{
    [SerializeField] private string type;

    public string Type { get => type; set => type = value; }

    public AIState(string _type = "")
    {
        type = _type;
    }
}
