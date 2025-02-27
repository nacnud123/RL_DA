using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysRandom = System.Random;
using UnityRandom = UnityEngine.Random;

internal sealed class ProcGen
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

    private List<Tuple<int, string, int>> itemChance = new List<Tuple<int, string, int>> // Floor, ItemName, Chance
    {
        new Tuple<int, string, int>(0, "Potions/Potion of Health", 35),
        new Tuple<int, string, int>(2, "Potions/Potion of Confusion", 35),
        new Tuple<int, string, int>(4, "Potions/Potion of Increase Level", 35),
        new Tuple<int, string, int>(6, "Potions/Potion of Poison", 35),
        new Tuple<int, string, int>(10, "Potions/Potion of Increase Level", 35),

        new Tuple<int, string, int>(0, "Scrolls/Die Scroll", 5),
        new Tuple<int, string, int>(0, "Scrolls/Spawn Monster Scroll", 5),
        new Tuple<int, string, int>(2, "Scrolls/Confusion Scroll", 10),
        new Tuple<int, string, int>(2, "Scrolls/Identify Scroll", 30),
        new Tuple<int, string, int>(2, "Scrolls/Freeze Scroll", 30),
        new Tuple<int, string, int>(4, "Scrolls/Lightning Scroll", 25),
        new Tuple<int, string, int>(4, "Scrolls/Sleep Scroll", 10),
        new Tuple<int, string, int>(4, "Scrolls/Poison Scroll", 10),
        new Tuple<int, string, int>(6, "Scrolls/Fireball Scroll", 25),
        new Tuple<int, string, int>(6, "Scrolls/Reveal Scroll", 25),
        new Tuple<int, string, int>(6, "Scrolls/Teleport Scroll", 10),

        new Tuple<int, string, int>(2, "Weapons/War Hammer", 5),
        new Tuple<int, string, int>(4, "Weapons/Mace", 5),
        new Tuple<int, string, int>(6, "Weapons/Long Sword", 5),
        new Tuple<int, string, int>(8, "Weapons/Two Handed Sword", 5),

        new Tuple<int, string, int>(5, "Wands/Magic Missile", 25),
        new Tuple<int, string, int>(8, "Wands/Fireball", 25),
        new Tuple<int, string, int>(9, "Wands/Drain Life", 25),

        new Tuple<int,string,int>(1, "Armor/Rings/Ring", 15),
        new Tuple<int,string,int>(4, "Armor/Rings/Ring Of Regen", 15),
        new Tuple<int,string,int>(8, "Armor/Rings/Ring of Stength", 15),

        new Tuple<int,string,int>(2, "Armor/Ring", 15),
        new Tuple<int,string,int>(4, "Armor/Scale", 15),
        new Tuple<int,string,int>(6, "Armor/Chainmail", 15),
        new Tuple<int,string,int>(8, "Armor/Banded", 15),
        new Tuple<int,string,int>(10, "Armor/Plate", 15),
        new Tuple<int,string,int>(12, "Armor/Splint", 15),

        new Tuple<int, string, int>(3, "Ranged Weapons/Arrows", 10)

    };


    private List<Tuple<int, int, string, int>> monsterChances = new List<Tuple<int, int, string, int>> // firstLevel, lastLevel, Name, Chance
    {
        new Tuple<int,int,string,int>(1,8,"Monsters/Bat", 100),
        new Tuple<int,int,string,int>(7,16,"Monsters/Centaur", 60),
        new Tuple<int,int,string,int>(21,126,"Monsters/Dragon", 100),
        new Tuple<int,int,string,int>(1,7,"Monsters/Emu", 80),
        new Tuple<int,int,string,int>(12,126,"Monsters/Fly Man", 80),
        new Tuple<int,int,string,int>(20,126,"Monsters/Griffin", 85),
        new Tuple<int,int,string,int>(1,10,"Monsters/Hobgoblin", 67),
        new Tuple<int,int,string,int>(2,11,"Monsters/Ice Monster", 68),
        new Tuple<int,int,string,int>(21,126,"Monsters/Jabberwock", 100),
        new Tuple<int,int,string,int>(1,6,"Monsters/Kestrel", 60),
        new Tuple<int,int,string,int>(18,126,"Monsters/Medusa", 85),
        new Tuple<int,int,string,int>(4,13,"Monsters/Orc", 70),
        new Tuple<int,int,string,int>(15,24,"Monsters/Phantom", 80),
        new Tuple<int,int,string,int>(8,17,"Monsters/Quagga", 78),
        new Tuple<int,int,string,int>(3,12,"Monsters/Rattlesnake", 70),
        new Tuple<int,int,string,int>(1,9,"Monsters/Snake", 50),
        new Tuple<int,int,string,int>(13,22,"Monsters/Troll", 75),
        new Tuple<int,int,string,int>(17,26,"Monsters/Black Unicorn", 85),
        new Tuple<int,int,string,int>(19,126,"Monsters/Vampire", 85),
        new Tuple<int,int,string,int>(14,23,"Monsters/Wraith", 75),
        new Tuple<int,int,string,int>(16,25,"Monsters/Xeroc", 75),
        new Tuple<int,int,string,int>(11,20,"Monsters/Yeti", 80),
        new Tuple<int,int,string,int>(5,14,"Monsters/Zombie", 69)
    };

    private readonly HashSet<Vector3Int> tunnelCoords = new HashSet<Vector3Int>();

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

    public List<string> getEntitiesAtRandom(List<Tuple<int,int,string,int>> chances, int numOfEntities, int floor)
    {
        List<string> entities = new List<string>();
        List<int> weightedChances = new List<int>();

        foreach(Tuple<int,int,string,int> chan in chances)
        {
            if (floor >= chan.Item1 && floor <= chan.Item2)
            {
                entities.Add(chan.Item3);
                weightedChances.Add(chan.Item4);
            }
        }

        SysRandom rnd = new SysRandom();
        List<string> chosenEntities = rnd.Choices(entities, weightedChances, numOfEntities);

        return chosenEntities;
    }

    public List<string> getEntitiesAtRandom(List<Tuple<int, string, int>> chances, int numOfEntities, int floor)
    {
        List<string> entities = new List<string>();
        List<int> weightedChances = new List<int>();

        foreach (Tuple<int, string, int> chan in chances)
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

        foreach(RectangularRoom room in rooms)
        {
            PlaceDoors(room, rooms);
        }

        // Add the player to the first room
        Vector3Int playerPos = (Vector3Int)rooms[0].RandomPoint();
        int maxAttempts = 10, attempts = 0;

        while (GameManager.init.GetActorAtLocation(new Vector2(playerPos.x + .5f, playerPos.y + .5f)) is not null)
        {
            playerPos = (Vector3Int)rooms[0].RandomPoint();
            if (attempts >= maxAttempts)
            {
                Actor actor = GameManager.init.GetActorAtLocation(new Vector2(playerPos.x + .5f, playerPos.y + .5f));

                if (actor is not null)
                {
                    GameManager.init.removeActor(actor);
                    GameManager.init.removeEntity(actor);
                    GameManager.init.DestroyEntity(actor);
                }
                break;
            }
            attempts++;
        }


        // Add the stairs
        MapManager.init.getFloorMap.SetTile((Vector3Int)rooms[rooms.Count - 1].RandomPoint(), MapManager.init.DownStairsTile);

        MapManager.init.getFloorMap.SetTile(playerPos, MapManager.init.UpStairsTile);

        if (!isNewGame)
        {
            GameManager.init.getActors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);
        }
        else
        {
            GameObject player = MapManager.init.createEntity("Player", (Vector2Int)playerPos);
            Actor playerActor = player.GetComponent<Actor>();

            Item starterWeapon = MapManager.init.createEntity("Weapons/Dagger", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterWeapon0 = MapManager.init.createEntity("Weapons/Dagger", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterWeapon2 = MapManager.init.createEntity("Weapons/LongSword", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterWeapon3 = MapManager.init.createEntity("Weapons/TwoHandedSword", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterWeapon4 = MapManager.init.createEntity("Weapons/WarHammer", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterArmor = MapManager.init.createEntity("Armor/Leather", (Vector2Int)playerPos).GetComponent<Item>();
            //Item starterSpell = MapManager.init.createEntity("Scrolls/Fireball Scroll", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterRanged = MapManager.init.createEntity("Ranged Weapons/Bow", (Vector2Int)playerPos).GetComponent<Item>();
            
            playerActor.GetInventory.Add(starterWeapon);
            playerActor.GetInventory.Add(starterWeapon2);
            playerActor.GetInventory.Add(starterWeapon3);
            playerActor.GetInventory.Add(starterWeapon4);
            playerActor.GetInventory.Add(starterWeapon0);
            playerActor.GetInventory.Add(starterArmor);
            playerActor.GetInventory.Add(starterRanged);
            //((Wand)starterWand.GetConsumable).initName();

            /*Item testingItem = MapManager.init.createEntity("Scrolls/Spawn Monster Scroll", (Vector2Int)playerPos).GetComponent<Item>();
            Item testingItem2 = MapManager.init.createEntity("Scrolls/Poison Scroll", (Vector2Int)playerPos).GetComponent<Item>();
            playerActor.GetInventory.Add(testingItem);
            playerActor.GetInventory.Add(testingItem2);
            */

            playerActor.GetEquipment.equipToSlot("Weapon", starterWeapon, false);
            playerActor.GetEquipment.equipToSlot("Armor", starterArmor, false);

            playerActor.GetEquipment.equipToSlot("Ranged", starterRanged, false);

            Camera.main.transform.parent = player.gameObject.transform;


        }
    }

    private void PlaceDoors(RectangularRoom room, List<RectangularRoom> allRooms)
    {
        Debug.Log("Place Doors!");
        var wallPositions = room.getWallPositions();

        foreach (var pos in wallPositions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);

            if (isAdjacentToFloor(tilePos) && !isPartOfAnotherRoom(tilePos, allRooms, room) && !isAdjacentToDoor(tilePos))
            {
                // Remove The floor tile
                MapManager.init.getFloorMap.SetTile(tilePos, null);
                // Remove the wall tile
                MapManager.init.getObstacleMap.SetTile(tilePos, null);
                // Add the door tile
                Debug.Log("Place Door!");
                MapManager.init.getInteractableMap.SetTile(tilePos, MapManager.init.ClosedDoor);
            }
        }
    }

    //Door stuff
    private bool isAdjacentToFloor(Vector3Int pos)
    {
        var adjacentTiles = new Vector3Int[]
        {
            pos + Vector3Int.up,
            pos + Vector3Int.down,
            pos + Vector3Int.left,
            pos + Vector3Int.right,
        };

        bool isHorizontalCorridor = MapManager.init.getFloorMap.GetTile(adjacentTiles[2]) && MapManager.init.getFloorMap.GetTile(adjacentTiles[3]);
        bool isVerticalCorridor = MapManager.init.getFloorMap.GetTile(adjacentTiles[0]) && MapManager.init.getFloorMap.GetTile(adjacentTiles[1]);

        if (isHorizontalCorridor || isVerticalCorridor)
        {
            Vector3Int orthogonalDirection = isHorizontalCorridor ? adjacentTiles[0] : adjacentTiles[2];
            Vector3Int oppositeDirection = isHorizontalCorridor ? adjacentTiles[1] : adjacentTiles[3];

            if (!MapManager.init.getFloorMap.GetTile(orthogonalDirection) && !MapManager.init.getFloorMap.GetTile(oppositeDirection))
            {
                return true;
            }
        }

        return false;
    }

    private bool isPartOfAnotherRoom(Vector3Int pos, List<RectangularRoom> allRooms, RectangularRoom currentRoom)
    {
        foreach (RectangularRoom room in allRooms)
        {
            if (room != currentRoom)
            {
                var bounds = room.getBoundsInt();
                if (bounds.Contains(pos))
                {
                    // If the position is inside the bounds of another room, return true
                    return true;
                }
            }
        }
        return false;
    }

    private bool isAdjacentToDoor(Vector3Int pos)
    {
        var checkPositions = new Vector3Int[]
        {
            pos + Vector3Int.up,
            pos + Vector3Int.down,
            pos + Vector3Int.left,
            pos + Vector3Int.right,
        };

        foreach (Vector3Int tile in checkPositions)
        {
            if (MapManager.init.getInteractableMap.GetTile(tile) == MapManager.init.ClosedDoor)
            {
                // Found a door adjacent to this position
                return true;
            }
        }

        return false;
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
        //bool isItem = false;
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
