using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class MapManager : MonoBehaviour
{
    public static MapManager init;

    [Header("Map settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;


    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;
    [SerializeField] private TileBase upStairsTile;
    [SerializeField] private TileBase downStairsTile;
    [SerializeField] private TileBase closedDoor;
    [SerializeField] private TileBase openDoor;

    [Header("TileMaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms;
    [SerializeField] private List<Vector3Int> visibleTiles;

    [SerializeField] private Dictionary<Vector3Int, TileData> tiles;
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    // Get methods
    public int getWidth { get => width; }
    public int getHeight { get => height; }

    public TileBase FloorTile { get => floorTile; }
    public TileBase WallTile { get => wallTile; }
    public TileBase UpStairsTile { get => upStairsTile; }
    public TileBase DownStairsTile { get => downStairsTile; }
    public TileBase ClosedDoor { get => closedDoor; }
    public TileBase OpenDoor { get => openDoor; }

    public Tilemap getFloorMap { get => floorMap; }
    public Tilemap getInteractableMap { get => interactableMap; }
    public Tilemap getObstacleMap { get => obstacleMap; }
    public Tilemap getFogMap { get => fogMap; }

    public List<RectangularRoom> getRooms { get => rooms; }
    public List<Vector3Int> VisibleTiles { get => visibleTiles; }

    public Dictionary<Vector2Int, Node> getNodes { get => nodes; set => nodes = value; }

    private void Awake()
    {
        if (init == null) { init = this; }
        else { Destroy(gameObject); }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.init.Save.Scenes.Find(x => x.FloorNumber == SaveManager.init.CurrentFloor);

        if(sceneState is not null)
        {
            LoadState(sceneState.MapState);
        }
        else
        {
            GenerateDungeon(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        //Camera.main.orthographicSize = 27;
    }

    public void GenerateDungeon(bool isNewGame = false)
    {
        if(floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }
        else
        {
            rooms = new List<RectangularRoom>();
            tiles = new Dictionary<Vector3Int, TileData>();
            visibleTiles = new List<Vector3Int>();
        }

        ProcGen procGen = new ProcGen();
        procGen.generateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, rooms, isNewGame);

        addTileMapToDictionary(floorMap);
        addTileMapToDictionary(interactableMap);
        addTileMapToDictionary(obstacleMap);

        setupFogMap();

        if (!isNewGame)
        {
            GameManager.init.RefreshPlayer();
        }
    }

    public bool inBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public GameObject createEntity(string entity, Vector2 position)
    {
        try
        {
            var entityPosition = new Vector2
            (
                Mathf.Floor(position.x) + 0.5f,
                Mathf.Floor(position.y) + 0.5f
            );

            GameObject entityObject = Instantiate(Resources.Load<GameObject>($"{entity}"), entityPosition, Quaternion.identity);

            entityObject.name = entity;

            if (entityObject.GetComponent<Item>())
            {
                if (entityObject.GetComponent<Item>().GetConsumable || entityObject.GetComponent<Ring>())
                {
                    if(entityObject.GetComponent<Item>().CurrName == "")
                    {
                        entityObject.GetComponent<Item>().CurrName = GameManager.init.makeRandName(entityObject.GetComponent<Item>().RealName);
                        if (entityObject.name.Contains("Potion")) { entityObject.GetComponent<Item>().CurrName += "Potion"; }
                        if (entityObject.name.Contains("Scroll")) { entityObject.GetComponent<Item>().CurrName += "Scroll"; }
                        if (entityObject.name.Contains("Ring")) { entityObject.GetComponent<Item>().CurrName += "Ring"; }
                    }
                }
                else
                {
                    entityObject.GetComponent<Item>().CurrName = entityObject.GetComponent<Item>().RealName;
                }
               
            }
            
            if(entityObject.GetComponent<Actor>() is not null)
            {
                entityObject.GetComponent<Actor>().addToGameManager();
            }
            else if(entityObject.GetComponent<Item>() is not null)
            {
                entityObject.GetComponent<Item>().addToGameManager();
            }

            return entityObject;
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong when creating entity!");
            Debug.Log($"{entity} or {position} may not exist. Make sure things are spelled right");
            return null;
        }
       
    }

    public void updateFogMap(List<Vector3Int> playerFov)
    {
        foreach(Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].IsExplored)
            { tiles[pos].IsExplored = true; }

            tiles[pos].IsVisible = false;
            fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1f, .5f));
        }

        visibleTiles.Clear();

        foreach(Vector3Int pos in playerFov)
        {
            tiles[pos].IsVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    public void setEntitiesVisibilities()
    {
        foreach(Entity entity in GameManager.init.getEntities)
        {
            if (entity.GetComponent<Player>())
                continue;

            bool isVisible = false;
            Vector3Int entityPos = Vector3Int.zero;

            if(entity.Size.x > 1 || entity.Size.y > 1)
            {
                foreach(Vector3 pos in entity.OccupiedTiles)
                {
                    entityPos = floorMap.WorldToCell(pos);
                    if (visibleTiles.Contains(entityPos))
                    {
                        isVisible = true;
                        break;
                    }
                }
            }
            else
            {
                entityPos = floorMap.WorldToCell(entity.transform.position);
                if (visibleTiles.Contains(entityPos))
                {
                    isVisible = true;
                }
            }

            entity.SR.enabled = isVisible;
        }
    }

    private void addTileMapToDictionary(Tilemap tilemap)
    {
        foreach(Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
                continue;

            TileData tile = new TileData(
                _name: tilemap.GetTile(pos).name,
                _isExplored: false,
                _isVisible: false
                );
            tiles.Add(pos, tile);
        }
    }

    private void setupFogMap()
    {
        foreach(Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);

            if (tiles[pos].IsExplored)
            {
                fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, .5f));
            }
            else
            {
                fogMap.SetColor(pos, Color.white);
            }
        }
    }

    public void UpdateTile(Entity entity)
    {
        Vector3Int gridPosition;

        if(entity.Size.x > 1 || entity.Size.y > 1)
        {
            foreach(Vector3 pos in entity.OccupiedTiles)
            {
                gridPosition = floorMap.WorldToCell(pos);
                if (tiles.ContainsKey(gridPosition))
                {
                    if(tiles[gridPosition].Name == closedDoor.name)
                    {
                        interactableMap.SetTile(gridPosition, openDoor);
                        tiles[gridPosition].Name = openDoor.name;
                    }
                }
            }
        }
        else
        {
            gridPosition = floorMap.WorldToCell(entity.transform.position);
            if (tiles.ContainsKey(gridPosition))
            {
                if (tiles[gridPosition].Name == closedDoor.name)
                {
                    interactableMap.SetTile(gridPosition, openDoor);
                    tiles[gridPosition].Name = openDoor.name;
                }
            }
        }
    }

    public bool isValidPos(Vector3 futurePos)
    {
        Vector3Int gridPos = floorMap.WorldToCell(futurePos);
        if (!inBounds(gridPos.x, gridPos.y) || getObstacleMap.HasTile(gridPos))
            return false;

        return true;
    }

    private void Reset()
    {
        rooms.Clear();
        tiles.Clear();
        visibleTiles.Clear();
        nodes.Clear();

        floorMap.ClearAllTiles();
        interactableMap.ClearAllTiles();
        obstacleMap.ClearAllTiles();
        fogMap.ClearAllTiles();
    }

    public MapState SaveState() => new MapState(tiles, rooms);

    public void LoadState(MapState mapState)
    {
        if(floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }

        rooms = mapState.StoredRooms;
        tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);
        if(visibleTiles.Count > 0)
        {
            visibleTiles.Clear();
        }

        foreach(Vector3Int pos in tiles.Keys)
        {
            if(tiles[pos].Name == floorTile.name)
            {
                floorMap.SetTile(pos, FloorTile);
            }
            else if (tiles[pos].Name == WallTile.name)
            {
                obstacleMap.SetTile(pos, WallTile);
            }
            else if(tiles[pos].Name == upStairsTile.name)
            {
                floorMap.SetTile(pos, upStairsTile);
            }
            else if(tiles[pos].Name == downStairsTile.name)
            {
                floorMap.SetTile(pos, downStairsTile);
            }
            else if(tiles[pos].Name == closedDoor.name)
            {
                interactableMap.SetTile(pos, closedDoor);
            }
            else if(tiles[pos].Name == openDoor.name)
            {
                interactableMap.SetTile(pos, openDoor);
            }
        }
        setupFogMap();
    }

}

[System.Serializable]
public class MapState
{
    [SerializeField] private Dictionary<Vector3, TileData> storedTiles;
    [SerializeField] private List<RectangularRoom> storedRooms;

    public Dictionary<Vector3, TileData> StoredTiles { get => storedTiles; set => storedTiles = value; }
    public List<RectangularRoom> StoredRooms { get => storedRooms; set => storedRooms = value; }

    public MapState(Dictionary<Vector3Int, TileData> _tiles, List<RectangularRoom> _rooms)
    {
        storedTiles = _tiles.ToDictionary(x => (Vector3)x.Key, x => x.Value);
        storedRooms = _rooms;
    }
}
