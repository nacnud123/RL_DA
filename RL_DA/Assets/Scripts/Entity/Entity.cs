using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;
    
    public bool BlocksMovment { get => blocksMovement; set => blocksMovement = value; }

    public virtual void addToGameManager()
    {
        if (GetComponent<Player>())
        {
            GameManager.init.InsertEntity(this, 0);
        }
        else
        {
            GameManager.init.addEntity(this);
        }
    }

    public void Move(Vector2 dir)
    {
        if(MapManager.init.isValidPos(transform.position + (Vector3)dir))
        {
            transform.position += (Vector3)dir;
        }
    }

    public virtual EntityState SaveState() => new EntityState();
}

[System.Serializable]
public class EntityState
{
    public enum EntityType
    {
        Actor,
        Item,
        Other
    }

    [SerializeField] private EntityType type;
    [SerializeField] private string name;
    [SerializeField] private bool blocksMovement, isVisible;
    [SerializeField] private Vector3 position;

    public EntityType Type { get => type; set => type = value; }
    public string Name { get => name; set => name = value; }
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }
    public Vector3 Position { get => position; set => position = value; }

    public EntityState(EntityType _type = EntityType.Other, string _name = "", bool _blocksMovment = false, bool _isVisible = false, Vector3 _pos = new Vector3())
    {
        type = _type;
        name = _name;
        blocksMovement = _blocksMovment;
        IsVisible = _isVisible;
        position = _pos;
    }

}
