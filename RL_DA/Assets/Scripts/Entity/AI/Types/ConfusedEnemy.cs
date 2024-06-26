using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy will stumble around. If an actor occupies a tile it is randomly in it will attack
/// </summary>
[RequireComponent(typeof(Actor))]
public class ConfusedEnemy : AI
{
    [SerializeField] private AI prevAI;
    [SerializeField] private int turnsRemainig;

    public AI PrevAI { get => prevAI; set => prevAI = value; }
    public int TurnsRem { get => turnsRemainig; set => turnsRemainig = value; }

    public override void RunAI()
    {
        if(turnsRemainig <= 0)
        {
            UIManager.init.addMsg($"The {gameObject.name} is no longer confused.", "#FF0000");
            GetComponent<Actor>().AI = prevAI;
            GetComponent<Actor>().AI.RunAI();
            Destroy(this);
        }
        else
        {
            // Random Dir
            Vector2Int dir = Random.Range(0, 8) switch
            {
                0 => new Vector2Int(0, 1),
                1 => new Vector2Int(0, -1),
                2 => new Vector2Int(1, 0),
                3 => new Vector2Int(-1, 0),
                4 => new Vector2Int(1, 1),
                5 => new Vector2Int(1, -1),
                6 => new Vector2Int(-1, 1),
                7 => new Vector2Int(-1, -1),
                _ => new Vector2Int(0, 0)
            };
            Action.bumpAction(GetComponent<Actor>(), dir);
            turnsRemainig--;
        }
    }


    public override AIState SaveState() => new ConfusedState(
        _type: "ConfusedEnemy",
        _prevAI: prevAI,
        _turnsRemaining: turnsRemainig
        );

    public void LoadState(ConfusedState state)
    {
        if (state.PrevAI == "HostileEnemy")
            prevAI = GetComponent<HostileEnemy>();

        turnsRemainig = state.TurnsRemaining;
    }
}

[System.Serializable]
public class ConfusedState : AIState
{
    [SerializeField] private string prevAI;
    [SerializeField] private int turnsRemaining;

    public string PrevAI { get => prevAI; set => prevAI = value; }
    public int TurnsRemaining { get => turnsRemaining; set => turnsRemaining = value; }

    public ConfusedState(string _type = "", AI _prevAI = null, int _turnsRemaining = 0) : base(_type)
    {
        prevAI = _prevAI.GetType().ToString();
        turnsRemaining = _turnsRemaining;
    }
}
