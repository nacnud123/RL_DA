using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager : MonoBehaviour
{
    public static MapManager init;

    [Header("Map settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;
    [SerializeField] private int maxItemsPerRoom = 2;


    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;

    [Header("TileMaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    
    private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    // Get methods
    public int getWidth { get => width; }
    public int getHeight { get => height; }

    public TileBase FloorTile { get => floorTile; }
    public TileBase WallTile { get => wallTile; }

    public Tilemap getFloorMap { get => floorMap; }
    public Tilemap getObstacleMap { get => obstacleMap; }
    public Tilemap getFogMap { get => fogMap; }

    public List<RectangularRoom> getRooms { get => rooms; }
    public Dictionary<Vector2Int, Node> getNodes { get => nodes; set => nodes = value; }

    private void Awake()
    {
        if (init == null) init = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ProcGen procGen = new ProcGen();
        procGen.generateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

        addTileMapToDictionary(floorMap);
        addTileMapToDictionary(obstacleMap);

        setupFogMap();

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;
    }

    public bool inBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public void createEntity(string entity, Vector2 position)
    {
        switch (entity)
        {
            case "Player":
                Instantiate(Resources.Load<GameObject>("Player"), new Vector3(position.x + .5f, position.y + .5f, 0), Quaternion.identity).name = "Player";
                break;
            case "Orc":
                Instantiate(Resources.Load<GameObject>("Orc"), new Vector3(position.x + .5f, position.y + .5f, 0), Quaternion.identity).name = "Orc";
                break;
            case "Troll":
                Instantiate(Resources.Load<GameObject>("Troll"), new Vector3(position.x + .5f, position.y + .5f, 0), Quaternion.identity).name = "Troll";
                break;
            case "Potion of Health":
                Instantiate(Resources.Load<GameObject>("Potion of Health"), new Vector3(position.x + .5f, position.y + .5f, 0), Quaternion.identity).name = "Potion of Health";
                break;
            default:
                Debug.LogError("No eneity of name: {entity}");
                break;
        }

        
    }

    public void updateFogMap(List<Vector3Int> playerFov)
    {
        foreach(Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].IsExplored)
                tiles[pos].IsExplored = true;

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

            Vector3Int entityPos = floorMap.WorldToCell(entity.transform.position);

            if (visibleTiles.Contains(entityPos))
                entity.GetComponent<SpriteRenderer>().enabled = true;
            else
                entity.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void addTileMapToDictionary(Tilemap tilemap)
    {
        foreach(Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
                continue;

            TileData tile = new TileData();
            tiles.Add(pos, tile);
        }
    }

    private void setupFogMap()
    {
        foreach(Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);
        }
    }
}
