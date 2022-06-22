using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EightTileInfoGenerator : MonoBehaviour
{
    public static Dictionary<Tile, TileInfo> GetCompatibilityInfo(List<Tile> tiles)
    {
        // Create compatibility infos for everyone (relies on me knowing the list order)
        TileInfo strHor = new TileInfo(tiles[0], "Horizontal Road", 1f);
        TileInfo strVer = new TileInfo(tiles[1], "Vertical Road", 1f);
        TileInfo xInt = new TileInfo(tiles[2], "X Intersection", 0.1f);
        TileInfo cnrDownLeft = new TileInfo(tiles[3], "Down to Left Corner", 0.1f);
        TileInfo cnrDownRight = new TileInfo(tiles[4], "Down to Right Corner", 0.1f);
        TileInfo cnrUpLeft = new TileInfo(tiles[5], "Up to Left Corner", 0.1f);
        TileInfo cnrUpRight = new TileInfo(tiles[6], "Up to Right Corner", 0.1f);
        TileInfo house = new TileInfo(tiles[7], "House", 1.5f);

        // Define which tiles are allowed next to each other
        strHor.AllowedUp.Add(house.Tile);
        strHor.AllowedDown.Add(house.Tile);
        strHor.AllowedLeft.UnionWith(new HashSet<Tile>{strHor.Tile, cnrDownRight.Tile, cnrUpRight.Tile, xInt.Tile});
        strHor.AllowedRight.UnionWith(new HashSet<Tile>{strHor.Tile, cnrDownLeft.Tile, cnrUpLeft.Tile, xInt.Tile});
        
        strVer.AllowedUp.UnionWith(new HashSet<Tile>{strVer.Tile, cnrDownLeft.Tile, cnrDownRight.Tile, xInt.Tile});
        strVer.AllowedDown.UnionWith(new HashSet<Tile>{strVer.Tile, cnrUpLeft.Tile, cnrUpRight.Tile, xInt.Tile});
        strVer.AllowedLeft.Add(house.Tile);
        strVer.AllowedRight.Add(house.Tile);
        
        xInt.AllowedUp.Add(strVer.Tile);
        xInt.AllowedDown.Add(strVer.Tile);
        xInt.AllowedLeft.Add(strHor.Tile);
        xInt.AllowedRight.Add(strHor.Tile);
        
        cnrDownLeft.AllowedUp.Add(house.Tile);
        cnrDownLeft.AllowedDown.Add(strVer.Tile);
        cnrDownLeft.AllowedLeft.Add(strHor.Tile);
        cnrDownLeft.AllowedRight.Add(house.Tile);
        
        cnrDownRight.AllowedUp.Add(house.Tile);
        cnrDownRight.AllowedDown.Add(strVer.Tile);
        cnrDownRight.AllowedLeft.Add(house.Tile);
        cnrDownRight.AllowedRight.Add(strHor.Tile);
        
        cnrUpLeft.AllowedUp.Add(strVer.Tile);
        cnrUpLeft.AllowedDown.Add(house.Tile);
        cnrUpLeft.AllowedLeft.Add(strHor.Tile);
        cnrUpLeft.AllowedRight.Add(house.Tile);
        
        cnrUpRight.AllowedUp.Add(strVer.Tile);
        cnrUpRight.AllowedDown.Add(house.Tile);
        cnrUpRight.AllowedLeft.Add(house.Tile);
        cnrUpRight.AllowedRight.Add(strHor.Tile);
        
        house.AllowedUp.UnionWith(new HashSet<Tile>{strHor.Tile, cnrUpLeft.Tile, cnrUpRight.Tile, house.Tile});
        house.AllowedDown.UnionWith(new HashSet<Tile>{strHor.Tile, cnrDownLeft.Tile, cnrDownRight.Tile, house.Tile});
        house.AllowedLeft.UnionWith(new HashSet<Tile>{strVer.Tile, cnrDownLeft.Tile, cnrUpLeft.Tile, house.Tile});
        house.AllowedRight.UnionWith(new HashSet<Tile>{strVer.Tile, cnrDownRight.Tile, cnrUpRight.Tile, house.Tile});

        Dictionary<Tile, TileInfo> infos = new Dictionary<Tile, TileInfo>();
        infos[strHor.Tile] = strHor;
        infos[strVer.Tile] = strVer;
        infos[xInt.Tile] = xInt;
        infos[cnrDownLeft.Tile] = cnrDownLeft;
        infos[cnrDownRight.Tile] = cnrDownRight;
        infos[cnrUpLeft.Tile] = cnrUpLeft;
        infos[cnrUpRight.Tile] = cnrUpRight;
        infos[house.Tile] = house;

        return infos;
    }
}


