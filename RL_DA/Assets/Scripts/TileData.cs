using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    [SerializeField] private string name;
    [SerializeField] private bool isExplored, isVisible;

    public string Name { get => name; set => name = value; }
    public bool IsExplored { get => isExplored; set => isExplored = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }


    public TileData(string _name, bool _isExplored, bool _isVisible)
    {
        name = _name;
        isExplored = _isExplored;
        IsVisible = _isVisible;
    }
}
