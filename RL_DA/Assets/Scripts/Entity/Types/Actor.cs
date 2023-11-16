using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;
    [SerializeField] private int fovRange = 8;
    [SerializeField] private List<Vector3Int> fov;
    [SerializeField] private AI ai;
    [SerializeField] private Inventory inv;

    AdamMilVisibility algorithm;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> getFOV { get => fov; }
    public Inventory GetInventory { get => inv; }

    public AI AI { get => ai; set => ai = value; }

    private void OnValidate()
    {
        if (GetComponent<AI>())
        {
            ai = GetComponent<AI>();
        }

        if (GetComponent<Inventory>())
            inv = GetComponent<Inventory>();
    }

    // Start is called before the first frame update
    void Start()
    {
        addToGameManager();
        if (GetComponent<Player>())
            GameManager.init.insertActor(this, 0);
        else
            GameManager.init.addActor(this);
        

        algorithm = new AdamMilVisibility();
        updateFOV();
    }

    public void updateFOV()
    {
        Vector3Int gridPos = MapManager.init.getFloorMap.WorldToCell(transform.position);

        fov.Clear();
        algorithm.Compute(gridPos, fovRange, fov);

        if (GetComponent<Player>())
        {
            MapManager.init.updateFogMap(fov);
            MapManager.init.setEntitiesVisibilities();
        }
    }
}
