using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 0;
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();

    public int GetCapacity { get => capacity; }
    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> GetItems { get => items; }

    public void Add(Item item)
    {
        items.Add(item);
        item.transform.SetParent(transform);
        GameManager.init.removeEntity(item);
    }
    
    public void Remove(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
    }

    public void Drop(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
        item.SR.enabled = true;
        item.addToGameManager();
        UIManager.init.addMsg($"You dropped the {item.RealName}.", "#ff0000");
    }

    public void identifyItem(Item item)
    {
        item.CurrName = item.RealName;
        UIManager.init.addMsg($"Thats a {item.RealName}", "#0da2ff");
    }

    public bool hasAmmoType(AmmoType inType)
    {
        if(items.Exists(x => x.GetType() == typeof(RangedAmmo)))
        {
            return ((RangedAmmo)items.Find(x => x.GetType() == typeof(RangedAmmo))).Type == inType;
        }

        return false;
    }

    public void addToAmmo(Item item, AmmoType inType, int inAmount)
    {
        if (items.Exists(x => x.GetType() == typeof(RangedAmmo)))
        {
            ((RangedAmmo)items.Find(x => x.GetType() == typeof(RangedAmmo))).Amount += inAmount;
        }
        else
        {
            this.Add(item);
        }
    }

}
