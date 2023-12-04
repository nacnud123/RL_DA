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


    public void Drop(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
        item.GetComponent<SpriteRenderer>().enabled = true;
        item.addToGameManager();
        UIManager.init.addMsg($"You dropped the {item.name}.", "#ff0000");
    }

}
