using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;
    [SerializeField] private int fovRange = 8;
    [SerializeField] private List<Vector3Int> fov;
    [SerializeField] private AI ai;
    [SerializeField] private Inventory inv;
    [SerializeField] private Equipment equipment;
    [SerializeField] private Fighter fighter;
    [SerializeField] private Level level;
    [SerializeField] private Hunger hunger;
    

    AdamMilVisibility algorithm;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> getFOV { get => fov; }
    public Inventory GetInventory { get => inv; }
    public Equipment GetEquipment { get => equipment; }

    public AI AI { get => ai; set => ai = value; }

    public Fighter Fighter { get => fighter; set => fighter = value; }
    public Level Level { get => level; set => level = value; }
    public Hunger Hunger { get => hunger; set => hunger = value; }

    private void OnValidate()
    {
        if (GetComponent<AI>())
        {
            ai = GetComponent<AI>();
        }

        if (GetComponent<Inventory>())
            inv = GetComponent<Inventory>();

        if (GetComponent<Fighter>())
            fighter = GetComponent<Fighter>();

        if (GetComponent<Level>())
            level = GetComponent<Level>();

        if (GetComponent<Equipment>())
            equipment = GetComponent<Equipment>();

        if (GetComponent<Hunger>())
            hunger = GetComponent<Hunger>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!GameManager.init.getActors.Contains(this))
        {
            addToGameManager();
        }

        if (isAlive)
        {
            algorithm = new AdamMilVisibility();
            updateFOV();
        }
        else if(fighter != null)
        {
            fighter.Die();
        }
    }

    public override void addToGameManager()
    {
        base.addToGameManager();

        if (GetComponent<Player>())
            GameManager.init.insertActor(this, 0);
        else
            GameManager.init.addActor(this);

    }

    public void updateFOV()
    {
        Vector3Int gridPos = MapManager.init.getFloorMap.WorldToCell(transform.position);

        fov.Clear();
        algorithm.Compute(gridPos, fovRange, fov);

        if (GetComponent<Player>())
        {
            MapManager.init.updateFogMap(fov);
            MapManager.init.setEntitiesVisibilities();
        }
    }

    public override EntityState SaveState() => new ActorState(
        _name: name,
        _blocksMovment: BlocksMovment,
        _isAlive: isAlive,
        _isVisible: MapManager.init.VisibleTiles.Contains(MapManager.init.getFloorMap.WorldToCell(transform.position)),
        _pos: transform.position,
        _currAI: ai != null ? AI.SaveState() : null,
        _fighterState: fighter != null ? fighter.SaveState() : null,
        _levelState: level != null && GetComponent<Player>() ? level.SaveState() : null,
        _hungerState: hunger != null && GetComponent<Player>() ? hunger.SaveState() : null
        );

    public void LoadState(ActorState state)
    {
        transform.position = state.Position;
        isAlive = state.IsAlive;

        if (!isAlive)
            GameManager.init.removeActor(this);

        if (!state.IsVisible)
            SR.enabled = false;

        if(state.CurrentAI != null)
        {
            if (state.CurrentAI.Type == "HostileEnemy")
                ai = GetComponent<HostileEnemy>();
            else if(state.CurrentAI.Type == "ConfusedEnemy")
            {
                ai = gameObject.AddComponent<ConfusedEnemy>();
                ConfusedState confusedState = state.CurrentAI as ConfusedState;
                ((ConfusedEnemy)ai).LoadState(confusedState);
            }
        }

        if(state.FighterState != null)
        {
            fighter.LoadState(state.FighterState);
        }

        if(state.LevelState != null)
        {
            level.LoadState(state.LevelState);
        }

        if(state.HungerState != null)
        {
            hunger.LoadState(state.HungerState);
        }
    }

    public override void OnMouseEnter()
    {
        UIManager.init.ShowTooltip(this.realName);
    }
}

[System.Serializable]
public class ActorState: EntityState
{
    [SerializeField] private bool isAlive;
    [SerializeField] private AIState currentAI;
    [SerializeField] private FighterState fighterState;
    [SerializeField] private LevelState levelState;
    [SerializeField] private HungerState hungerState;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public AIState CurrentAI { get => currentAI; set => currentAI = value; }
    public FighterState FighterState { get => fighterState; set => fighterState = value; }
    public LevelState LevelState { get => levelState; set => levelState = value; }
    public HungerState HungerState { get => hungerState; set => hungerState = value; }

    public ActorState(EntityType _type = EntityType.Actor, string _name = "", bool _blocksMovment = false, bool _isVisible = false, Vector3 _pos = new Vector3(), bool _isAlive = true, AIState _currAI = null, FighterState _fighterState = null, LevelState _levelState = null, HungerState _hungerState = null): base(_type, _name, _blocksMovment, _isVisible, _pos)
    {
        isAlive = _isAlive;
        currentAI = _currAI;
        fighterState = _fighterState;
        levelState = _levelState;
        hungerState = _hungerState;
    }
}
