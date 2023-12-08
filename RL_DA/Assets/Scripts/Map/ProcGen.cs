using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysRandom = System.Random;
using UnityRandom = UnityEngine.Random;

sealed class ProcGen
{
    private List<Tuple<int, int>> maxItemsByFloor = new List<Tuple<int, int>>
    {
        new Tuple<int, int>(1,1),
        new Tuple<int, int>(4,2),
        new Tuple<int, int>(7,3),
        new Tuple<int, int>(10,4)
    };

    private List<Tuple<int, int>> maxMonstersByFloor = new List<Tuple<int, int>>
    {
        new Tuple<int, int>(1,2),
        new Tuple<int, int>(4,3),
        new Tuple<int, int>(6,5),
        new Tuple<int, int>(8,7),
        new Tuple<int, int>(10,10)
    };

    private List<Tuple<int, string, int>> itemChance = new List<Tuple<int, string, int>>
    {
        new Tuple<int, string, int>(0, "Potion of Health", 35),
        new Tuple<int, string, int>(2, "Confusion Scroll", 10),
        new Tuple<int, string, int>(4, "Lightning Scroll", 25),
        new Tuple<int, string, int>(6, "Fireball Scroll", 25)
    };

    private List<Tuple<int, string, int>> monsterChances = new List<Tuple<int, string, int>>
    {
        new Tuple<int, string, int>(1, "Orc", 80),
        new Tuple<int, string, int>(3, "Troll", 15),
        new Tuple<int, string, int>(5, "Troll", 30),
        new Tuple<int, string, int>(7, "Troll", 60)
    };

    public int getMaxValueForFloor(List<Tuple<int,int>> values, int floor)
    {
        int curVal = 0;
        
        foreach(Tuple<int,int> val in values)
        {
            if (floor >= val.Item1)
            {
                curVal = val.Item2;
            }
        }
        return curVal;
    }

    public List<string> getEntitiesAtRandom(List<Tuple<int,string,int>> chances, int numOfEntities, int floor)
    {
        List<string> entities = new List<string>();
        List<int> weightedChances = new List<int>();

        foreach(Tuple<int,string,int> chan in chances)
        {
            if (floor >= chan.Item1)
            {
                entities.Add(chan.Item2);
                weightedChances.Add(chan.Item3);
            }
        }

        SysRandom rnd = new SysRandom();
        List<string> chosenEntities = rnd.Choices(entities, weightedChances, numOfEntities);

        return chosenEntities;
    }

    /// <summary>
    /// Gen a new dungeon
    /// </summary>

    public void generateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRoom, List<RectangularRoom> rooms,bool isNewGame)
    {
        for(int roomNum = 0; roomNum < maxRoom; roomNum++)
        {
            int roomWidth = UnityRandom.Range(roomMinSize, roomMaxSize);
            int roomHeight = UnityRandom.Range(roomMinSize, roomMaxSize);

            int roomX = UnityRandom.Range(0, mapWidth - roomWidth - 1);
            int roomY = UnityRandom.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            if (newRoom.Overlaps(rooms)) // If it overlaps then don't make it
            { continue; }

            for(int x = roomX; x < roomX + roomWidth; x++)
            {
                for(int y = roomY; y < roomY + roomHeight; y++)
                {
                    if(x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (setWallTileIfEmpty(new Vector3Int(x, y)))
                            continue;
                    }
                    else
                    {
                        setFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                tunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            placeEntities(newRoom, SaveManager.init.CurrentFloor);

            rooms.Add(newRoom);
        }
        // Add the stairs
        MapManager.init.getFloorMap.SetTile((Vector3Int)rooms[rooms.Count - 1].RandomPoint(), MapManager.init.DownStairsTile);

        //Add the player to the first room
        Vector3Int playerPos = (Vector3Int)rooms[0].RandomPoint();

        while (GameManager.init.GetActorAtLocation(playerPos) is not null)
        {
            playerPos = (Vector3Int)rooms[0].RandomPoint();
        }

        MapManager.init.getFloorMap.SetTile(playerPos, MapManager.init.UpStairsTile);

        if (!isNewGame)
        {
            GameManager.init.getActors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);
        }
        else
        {
            MapManager.init.createEntity("Player", (Vector2Int)playerPos);
        }
    }

    private void tunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom) // Create a tunnel between two rooms
    {
        Vector2Int oldRoomCenter = oldRoom.center();
        Vector2Int newRoomCenter = newRoom.center();
        Vector2Int tunnelCorner;

        if (UnityRandom.value < .5f) // Should room start vertically or horizontally
        { tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y); } // Move horizontally, then vertically
        else
        { tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y); } // Move vertically, then horizontally

        //Gen coords for the tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);
        
        //Set Tiles for tunnel
        for(int i = 0; i < tunnelCoords.Count; i++)
        {
            setFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            //MapManager.init.getFloorMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), MapManager.init.FloorTile);

            for(int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for(int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (setWallTileIfEmpty(new Vector3Int(x, y)))
                        continue;
                }
            }
        }
    }

    private bool setWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.init.getFloorMap.GetTile(pos))
        { return true; }
        else
        {
            MapManager.init.getObstacleMap.SetTile(pos, MapManager.init.WallTile);
            return false;
        }
    }

    private void setFloorTile(Vector3Int pos)
    {
        if (MapManager.init.getObstacleMap.GetTile(pos))
        { MapManager.init.getObstacleMap.SetTile(pos, null); }
        MapManager.init.getFloorMap.SetTile(pos, MapManager.init.FloorTile);
    }

    private void placeEntities(RectangularRoom newRoom, int floorNum)
    {
        int numOfMon = UnityRandom.Range(0, getMaxValueForFloor(maxMonstersByFloor, floorNum) + 1);
        int numOfItems = UnityRandom.Range(0, getMaxValueForFloor(maxItemsByFloor, floorNum) + 1);

        List<string> monName = getEntitiesAtRandom(monsterChances, numOfMon, floorNum);
        List<string> itemNames = getEntitiesAtRandom(itemChance, numOfItems, floorNum);

        List<string> entityNames = monName.Concat(itemNames).ToList();

        foreach(string entName in entityNames)
        {
            Vector3Int entPos = (Vector3Int)newRoom.RandomPoint();

            while(GameManager.init.GetActorAtLocation(entPos) is not null)
            {
                entPos = (Vector3Int)newRoom.RandomPoint();
            }

            MapManager.init.createEntity(entName, (Vector2Int)entPos);
        }
    }

}
