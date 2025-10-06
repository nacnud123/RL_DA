using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();

    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> GetItems { get => items; }

    public float GetMaxCarryWeight()
    {
        // Base carry weight is 50, plus 10 per point of strength
        Actor actor = GetComponent<Actor>();
        if (actor != null && actor.Fighter != null)
        {
            int strength = actor.Fighter.BasePower;
            return 50f + (strength * 10f);
        }
        return 50f;
    }

    public float GetCurrentWeight()
    {
        float totalWeight = 0f;
        foreach (Item item in items)
        {
            totalWeight += item.Weight;
        }
        return totalWeight;
    }

    public bool IsOverEncumbered()
    {
        return GetCurrentWeight() > GetMaxCarryWeight();
    }

    public bool CanCarry(float additionalWeight)
    {
        return GetCurrentWeight() + additionalWeight <= GetMaxCarryWeight();
    }

    public void Add(Item item, bool checkWeight = true)
    {
        if (checkWeight && !CanCarry(item.Weight))
        {
            if (GetComponent<Player>())
            {
                UIManager.init.addMsg($"The {item.RealName} is too heavy to carry!", "#808080");
            }
            return;
        }

        items.Add(item);
        item.transform.SetParent(transform);
        GameManager.init.removeEntity(item);

        UpdateWeightUI();
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);

        UpdateWeightUI();
    }

    public void Drop(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
        item.SR.enabled = true;
        item.addToGameManager();
        UIManager.init.addMsg($"You dropped the {item.RealName}.", "#ff0000");

        UpdateWeightUI();
    }

    private void UpdateWeightUI()
    {
        if (GetComponent<Player>())
        {
            UIManager.init.setWeight(GetCurrentWeight(), GetMaxCarryWeight());
        }
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
