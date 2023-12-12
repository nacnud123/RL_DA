using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Entity
{
    [SerializeField] private Consumable consumable;
    [SerializeField] private Equippable equippable;

    public Consumable GetConsumable { get => consumable; }
    public Equippable GetEquippable { get => equippable; }

    private void OnValidate()
    {
        if (GetComponent<Consumable>())
        {
            consumable = GetComponent<Consumable>();
        }
    }

    private void Start() => addToGameManager();

    public override EntityState SaveState() => new ItemState(
        _name: name,
        _blocksMovment: BlocksMovment,
        _isVisible: MapManager.init.VisibleTiles.Contains(MapManager.init.getFloorMap.WorldToCell(transform.position)),
        _pos: transform.position,
        _parent: transform.parent != null ? transform.parent.gameObject.name : ""
        );

    public void LoadState(ItemState state)
    {
        if (!state.IsVisible)
            GetComponent<SpriteRenderer>().enabled = false;
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
    }
}

[System.Serializable]
public class ItemState: EntityState
{
    [SerializeField] private string parent;

    public string Parent { get => parent; set => parent = value; }

    public ItemState(EntityType _type = EntityType.Item, string _name = "", bool _blocksMovment = false, bool _isVisible = false, Vector3 _pos = new Vector3(), string _parent = "") : base(_type, _name, _blocksMovment, _isVisible, _pos)
    {
        parent = _parent;
    }
}
