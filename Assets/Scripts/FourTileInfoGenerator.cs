using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FourTileInfoGenerator : MonoBehaviour
{
    public static Dictionary<Tile, TileInfo> GetCompatibilityInfo(List<Tile> tiles)
    {
        // Create compatibility infos for everyone (relies on me knowing the list order)
        TileInfo strHor = new TileInfo(tiles[0], "Horizontal Road", 1f);
        TileInfo strVer = new TileInfo(tiles[1], "Vertical Road", 1f);
        TileInfo xInt = new TileInfo(tiles[2], "X Intersection", 0.1f);
        TileInfo house = new TileInfo(tiles[3], "House", 2f);

        // Define which tiles are allowed next to each other
        strHor.AllowedUp.Add(house.Tile);
        strHor.AllowedDown.Add(house.Tile);
        strHor.AllowedLeft.UnionWith(new HashSet<Tile>{strHor.Tile, xInt.Tile});
        strHor.AllowedRight.UnionWith(new HashSet<Tile>{strHor.Tile, xInt.Tile});
        
        strVer.AllowedUp.UnionWith(new HashSet<Tile>{strVer.Tile, xInt.Tile});
        strVer.AllowedDown.UnionWith(new HashSet<Tile>{strVer.Tile, xInt.Tile});
        strVer.AllowedLeft.Add(house.Tile);
        strVer.AllowedRight.Add(house.Tile);
        
        xInt.AllowedUp.Add(strVer.Tile);
        xInt.AllowedDown.Add(strVer.Tile);
        xInt.AllowedLeft.Add(strHor.Tile);
        xInt.AllowedRight.Add(strHor.Tile);
        
        house.AllowedUp.UnionWith(new HashSet<Tile>{strHor.Tile, house.Tile});
        house.AllowedDown.UnionWith(new HashSet<Tile>{strHor.Tile, house.Tile});
        house.AllowedLeft.UnionWith(new HashSet<Tile>{strVer.Tile, house.Tile});
        house.AllowedRight.UnionWith(new HashSet<Tile>{strVer.Tile, house.Tile});

        Dictionary<Tile, TileInfo> infos = new Dictionary<Tile, TileInfo>();
        infos[strHor.Tile] = strHor;
        infos[strVer.Tile] = strVer;
        infos[xInt.Tile] = xInt;
        infos[house.Tile] = house;

        return infos;
    }
}


