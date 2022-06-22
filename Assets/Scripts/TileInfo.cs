using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TileInfo
{
    public Tile Tile;
    public string Name;
    public float Weight;
    public HashSet<Tile> AllowedUp;
    public HashSet<Tile> AllowedDown;
    public HashSet<Tile> AllowedLeft;
    public HashSet<Tile> AllowedRight;

    public TileInfo(Tile tile, string name, float weight)
    {
        Tile = tile;
        Name = name;
        Weight = weight;
        AllowedUp = new HashSet<Tile>();
        AllowedDown = new HashSet<Tile>();
        AllowedLeft = new HashSet<Tile>();
        AllowedRight = new HashSet<Tile>();
    }
}