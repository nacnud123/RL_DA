using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class WeightUI : MonoBehaviour
{
    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();

        if (GetComponent<Player>())
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (GetComponent<Player>() && inventory != null)
        {
            UIManager.init.setWeight(inventory.GetCurrentWeight(), inventory.GetMaxCarryWeight());
        }
    }
}
