using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Vector2Int size = new Vector2Int(1, 1);
    [SerializeField] private Vector3[] occupiedTiles;
    
    public bool BlocksMovment { get => blocksMovement; set => blocksMovement = value; }
    public SpriteRenderer SR { get => spriteRender; set => spriteRender = value; }
    public Vector2Int Size { get => size; set => size = value; }
    public Vector3[] OccupiedTiles { get => occupiedTiles; set => occupiedTiles = value; }

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
        if (!canMove(dir)) { return; }

        transform.position += (Vector3)dir;

        if(size.x > 1 || size.y > 1)
        {
            occupiedTiles = getOccupTiles();
        }

        MapManager.init.UpdateTile(this);
    }

    private bool canMove(Vector2 direction)
    {
        if(size.x > 1 || size.y > 1)
        {
            foreach(Vector3 occTile in OccupiedTiles)
            {
                Vector3 potentialOccupTile = occTile + (Vector3)direction;
                Actor acotr = GameManager.init.GetActorAtLocation(potentialOccupTile);
                if(!MapManager.init.isValidPos(potentialOccupTile) || acotr != null && acotr != this)
                {
                    return false;
                }
            }
        }
        else if(!MapManager.init.isValidPos(transform.position + (Vector3)direction))
        {
            return false;
        }
        return true;
    }

    public Vector3[] getOccupTiles()
    {
        Vector3[] tiles = new Vector3[(int)size.x * (int)size.y];
        for(int i =0; i < tiles.Length; i++)
        {
            tiles[i] = new Vector3(transform.position.x + i % (int)size.x, transform.position.y + i / (int)size.x);
        }
        return tiles;
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
