using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : Entity
{
    [SerializeField] private Consumable consumable;
    [SerializeField] private Equippable equippable;
    [SerializeField] private float weight = 1f;

    public Consumable GetConsumable { get => consumable; }
    public Equippable GetEquippable { get => equippable; }
    
    public float Weight { get => weight; set => weight = value; }

    private void OnValidate()
    {
        if (GetComponent<Consumable>())
        {
            consumable = GetComponent<Consumable>();
        }
    }

    private void Start()
    {
        if(!GameManager.init.getEntities.Contains(this))
        {
            addToGameManager();
        }
    }

    public override EntityState SaveState()
    {
        int inAmm = -1;
        if (GetConsumable)
        {
            var type = GetConsumable.GetType();

            if(type == typeof(Wand) || type.BaseType == typeof(Wand))
            {
                inAmm = GetComponent<Wand>().Uses;
            }
        }

        return new ItemState(
        _name: name,
        _currName: currName,
        _realName: realName,
        _blocksMovment: BlocksMovment,
        _isVisible: MapManager.init.VisibleTiles.Contains(MapManager.init.getFloorMap.WorldToCell(transform.position)),
        _pos: transform.position,
        _parent: transform.parent != null ? transform.parent.gameObject.name : "",
        _amount: inAmm,
        _weight: weight
        );
    }

    public virtual void LoadState(ItemState state)
    {
        if (!state.IsVisible)
            SR.enabled = false;
        if(state.Parent != "")
        {
            GameObject parent = GameObject.Find(state.Parent);
            parent.GetComponent<Inventory>().Add(this);

            if(equippable is not null && state.Name.Contains("(E)"))
            {
                parent.GetComponent<Equipment>().equipToSlot(equippable.EquipmentType.ToString(), this, false);
            }
        }
        transform.position = state.Position;
        this.currName = state.CurrName;
        this.realName = state.RealName;
        this.weight = state.Weight;

        if(state.Amount != -1)
        {
            if(this.GetType() != typeof(RangedAmmo))
            {
                ((Wand)GetConsumable).Uses = state.Amount;
            }
            
        }
    }
}

[System.Serializable]
public class ItemState: EntityState
{
    [SerializeField] private string parent;
    [SerializeField] private string currName;
    [SerializeField] private string realName;
    [SerializeField] private int amount;
    [SerializeField] private float weight;

    public string Parent { get => parent; set => parent = value; }
    public string CurrName { get => currName; set => currName = value; }
    public string RealName { get => realName; set => realName = value; }
    public int Amount { get => amount; set => amount = value; }
    public float Weight { get => weight; set => weight = value; }

    public ItemState(EntityType _type = EntityType.Item, string _name = "", string _currName = "", string _realName = "", bool _blocksMovment = false, bool _isVisible = false, Vector3 _pos = new Vector3(), string _parent = "", int _amount = 0, float _weight = 1f) : base(_type, _name, _blocksMovment, _isVisible, _pos)
    {
        parent = _parent;
        currName = _currName;
        realName = _realName;
        amount = _amount;
        weight = _weight;
    }
}
