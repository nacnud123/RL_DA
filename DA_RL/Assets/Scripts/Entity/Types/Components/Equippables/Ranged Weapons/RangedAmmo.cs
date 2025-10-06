using System;
using System.Security.Cryptography;
using UnityEngine;

public enum AmmoType
{
    Arrows
}

public class RangedAmmo : Item
{
    [Header("Ammo properties")]
    [SerializeField] private AmmoType type;
    [SerializeField] private int amount, maxAmount;

    public AmmoType Type { get => type; }
    public int Amount { 
        get => amount;
        set
        {
            amount = Mathf.Max(0, Mathf.Min(value, maxAmount));

            if(amount <= 0)
            {
                this.transform.parent.GetComponent<Actor>().GetInventory.Remove(this);

                GameManager.init.removeEntity(this.GetComponent<Entity>());

                UIManager.init.addMsg($"You run out of {RealName}", "#ff0000");

                GameManager.init.DestroyEntity(this.GetComponent<Entity>());
            }
            CurrName = CurrName.Replace($" ({amount + 1})", $" ({amount})");
        }
    }

    public int MaxAmount { get => maxAmount; set => maxAmount = value; }

    public override EntityState SaveState() => new ItemState(
        _name: name,
        _currName: CurrName,
        _realName: RealName,
        _blocksMovment: BlocksMovment,
        _isVisible: MapManager.init.VisibleTiles.Contains(MapManager.init.getFloorMap.WorldToCell(transform.position)),
        _pos: transform.position,
        _parent: transform.parent != null ? transform.parent.gameObject.name : "",
        _amount: amount
        );

    public override void LoadState(ItemState state)
    {
        base.LoadState(state);
        this.amount = state.Amount;
    }

    public bool canFire()
    {
        if (amount - 1 < 0)
            return false;
        return true;
    }

    internal void initName()
    {
        Debug.Log("Ranged Ammo init name!");
        CurrName += $" ({amount})";
    }
}
