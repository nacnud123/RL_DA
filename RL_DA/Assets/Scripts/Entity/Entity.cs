using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;
    
    public bool BlocksMovment { get => blocksMovement; set => blocksMovement = value; }

    public void addToGameManager()
    {
        GameManager.init.addEntity(this);
    }

    public void Move(Vector2 dir)
    {
        transform.position += (Vector3)dir;
    }
}
