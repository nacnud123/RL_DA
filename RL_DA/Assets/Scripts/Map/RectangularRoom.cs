using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectangularRoom
{

    [SerializeField] private int x, y, width, height;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    public RectangularRoom(int _x, int _y, int _width, int _height)
    {
        x = _x;
        y = _y;
        width = _width;
        height = _height;
    }

    // Return center of the room
    public Vector2Int center() => new Vector2Int(x + width / 2, y + height / 2);

    /// <summary>
    /// Return a random inner position inside the room
    /// </summary>
    public Vector2Int RandomPoint() => new Vector2Int(Random.Range(x + 1, x + width - 1), Random.Range(y + 1, y + height - 1));

    // Return the area of the room as a bound
    public Bounds getBounds() => new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));

    // same thing as getBounds but now an int
    public BoundsInt getBoundsInt() => new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));

    public bool Overlaps(List<RectangularRoom> otherRooms)
    {
        foreach(RectangularRoom otherRoom in otherRooms)
        {
            if (getBounds().Intersects(otherRoom.getBounds()))
                return true;
        }
        return false;
    }

    ///<summary>
    ///return a list of positions allong the walls of the room
    /// </summary>
    public List<Vector2Int> getWallPositions()
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();

        //Top and Bottom walls
        for(int i = x; i < x + width; i++)
        {
            wallPositions.Add(new Vector2Int(i, y));
            wallPositions.Add(new Vector2Int(i, y + height - 1));
        }

        //Left and right
        for(int j = y + 1; j < y+ height - 1; j++)
        {
            wallPositions.Add(new Vector2Int(x, j));
            wallPositions.Add(new Vector2Int(x + width - 1, j));
        }

        return wallPositions;
    }

}
