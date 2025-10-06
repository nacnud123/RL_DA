using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : IntractableEntity
{
    public override void onInteract()
    {
        Debug.Log("Hey, interacted with a shrine!");
        GameManager.init.getActors[0].GetComponent<Religion>().Pray();
    }
}
