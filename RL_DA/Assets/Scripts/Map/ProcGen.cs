using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class ProcGen : MonoBehaviour
{
    public void generateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRoom, int maxMonstersPerRoom, int maxItemsPerRoom, List<RectangularRoom> rooms)
    {
        for(int roomNum = 0; roomNum < maxRoom; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            if (newRoom.Overlaps(rooms)) // If it overlaps then don't make it
                continue;

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
            else { }

            placeEntities(newRoom, maxMonstersPerRoom, maxItemsPerRoom);


            rooms.Add(newRoom);
        }
        MapManager.init.createEntity("Player", rooms[0].center());
    }

    private void tunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom) // Create a tunnel between two rooms
    {
        Vector2Int oldRoomCenter = oldRoom.center();
        Vector2Int newRoomCenter = newRoom.center();
        Vector2Int tunnelCorner;

        if (Random.value < .5f) // Should room start vertically or horizontally
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y); // Move horizontally, then vertically
        else
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y); // Move vertically, then horizontally

        //Gen coords for the tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);
        
        //Set Tiles for tunnel
        for(int i = 0; i < tunnelCoords.Count; i++)
        {
            setFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            MapManager.init.getFloorMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), MapManager.init.FloorTile);

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
            return true;
        else
        {
            MapManager.init.getObstacleMap.SetTile(pos, MapManager.init.WallTile);
            return false;
        }
    }

    private void setFloorTile(Vector3Int pos)
    {
        if (MapManager.init.getObstacleMap.GetTile(pos))
            MapManager.init.getObstacleMap.SetTile(pos, null);
        MapManager.init.getFloorMap.SetTile(pos, MapManager.init.FloorTile);
    }

    private void placeEntities(RectangularRoom newRoom, int maxMon, int maxItems)
    {
        int numOfMon = Random.Range(0, maxMon + 1);
        int numOfItems = Random.Range(0, maxItems + 1);

        for (int mon = 0; mon < numOfMon;)
        {
            int x = Random.Range(newRoom.X, newRoom.X + newRoom.Width);
            int y = Random.Range(newRoom.Y, newRoom.Y + newRoom.Height);

            if (x == newRoom.X || x == newRoom.X + newRoom.Width - 1 || y == newRoom.Y || y == newRoom.Y + newRoom.Height - 1)
                continue;

            for(int entity = 0; entity < GameManager.init.getEntities.Count; entity++)
            {
                Vector3Int pos = MapManager.init.getFloorMap.WorldToCell(GameManager.init.getEntities[entity].transform.position);

                if (pos.x == x && pos.y == y)
                    return;
            }

            if (Random.value < .8f)
                MapManager.init.createEntity("Orc", new Vector2(x, y));
            else
                MapManager.init.createEntity("Troll", new Vector2(x, y));

            mon++;
        }

        for (int item = 0; item < numOfItems;)
        {
            int x = Random.Range(newRoom.X, newRoom.X + newRoom.Width);
            int y = Random.Range(newRoom.Y, newRoom.Y + newRoom.Height);

            if (x == newRoom.X || x == newRoom.X + newRoom.Width - 1 || y == newRoom.Y || y == newRoom.Y + newRoom.Height - 1)
                continue;

            for(int entity = 0; entity < GameManager.init.getEntities.Count; entity++)
            {
                Vector3Int pos = MapManager.init.getFloorMap.WorldToCell(GameManager.init.getEntities[entity].transform.position);

                if (pos.x == x && pos.y == y)
                    return;
            }

            float randValue = Random.value;
            if(randValue < .7f)
            {
                MapManager.init.createEntity("Potion of Health", new Vector2(x, y));
            }
            else if (randValue < .8f)
            {
                MapManager.init.createEntity("Fireball Scroll", new Vector2(x, y));
            }
            else if (randValue < .9f)
            {
                MapManager.init.createEntity("Confusion Scroll", new Vector2(x, y));
            }
            else
            {
                MapManager.init.createEntity("Lightning Scroll", new Vector2(x, y));
            }
            item++;
        }
    }

}
