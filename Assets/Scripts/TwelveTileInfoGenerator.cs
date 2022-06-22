using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TwelveTileInfoGenerator : MonoBehaviour
{
    public static Dictionary<Tile, TileInfo> GetCompatibilityInfo(List<Tile> tiles, Tile houseTile)
    {
        // Create compatibility infos for everyone (relies on me knowing the list order)
        TileInfo cnrDownLeft = new TileInfo(tiles[0], "cnrDownLeft", 1f);
        TileInfo cnrDownRight = new TileInfo(tiles[1], "cnrDownRight", 1f);
        TileInfo cnrUpLeft = new TileInfo(tiles[2], "cnrUpLeft", 1f);
        TileInfo cnrUpRight = new TileInfo(tiles[3], "cnrUpRight", 1f);
        TileInfo strHor = new TileInfo(tiles[4], "strHor", 1f);
        TileInfo strVer = new TileInfo(tiles[5], "strVer", 1f);
        TileInfo tintDownLeftRight = new TileInfo(tiles[6], "tintDownLeftRight", 1f);
        TileInfo tintUpDownLeft = new TileInfo(tiles[7], "tintUpDownLeft", 1f);
        TileInfo tintUpDownRight = new TileInfo(tiles[8], "tintUpDownRight", 1f);
        TileInfo tintUpLeftRight = new TileInfo(tiles[9], "tintUpLeftRight", 1f);
        TileInfo xint = new TileInfo(tiles[10], "xint", 1f);
        TileInfo house = new TileInfo(houseTile, "house", 1f);

        // Create hashsets of which tiles have roads exiting in each direction
        
        HashSet<TileInfo> roadsUp = new HashSet<TileInfo>()
        {
            cnrUpLeft, cnrUpRight,
            strVer,
            tintUpDownLeft, tintUpDownRight, tintUpLeftRight,
            xint
        };
        
        HashSet<TileInfo> roadsDown = new HashSet<TileInfo>()
        {
            cnrDownLeft, cnrDownRight,
            strVer,
            tintDownLeftRight, tintUpDownLeft, tintUpDownRight,
            xint
        };
        
        HashSet<TileInfo> roadsLeft = new HashSet<TileInfo>()
        {
            cnrDownLeft, cnrUpLeft,
            strHor,
            tintDownLeftRight, tintUpDownLeft, tintUpLeftRight,
            xint
        };
        
        HashSet<TileInfo> roadsRight = new HashSet<TileInfo>()
        {
            cnrDownRight, cnrUpRight,
            strHor, 
            tintDownLeftRight, tintUpDownRight, tintUpLeftRight,
            xint
        };

        // Add compatibility between sets of tiles with roads facing each other
        
        strVer.AllowedDown.UnionWith(roadsUp.Select(info => info.Tile));
        foreach (TileInfo info in roadsUp)
        {
            // info.AllowedUp.UnionWith(roadsDown.Select(tileInfo => tileInfo.Target));
            info.AllowedUp.Add(strVer.Tile);
        }
        
        strVer.AllowedUp.UnionWith(roadsDown.Select(info => info.Tile));
        foreach (TileInfo info in roadsDown)
        {
            // info.AllowedDown.UnionWith(roadsUp.Select(tileInfo => tileInfo.Target));
            info.AllowedDown.Add(strVer.Tile);
        }

        strVer.AllowedLeft.Add(houseTile);
        strVer.AllowedRight.Add(houseTile);
        
        strHor.AllowedRight.UnionWith(roadsLeft.Select(info => info.Tile));
        foreach (TileInfo info in roadsLeft)
        {
            // info.AllowedLeft.UnionWith(roadsRight.Select(tileInfo => tileInfo.Target));
            info.AllowedLeft.Add(strHor.Tile);
        }

        strHor.AllowedLeft.UnionWith(roadsRight.Select(info => info.Tile));
        foreach (TileInfo info in roadsRight)
        {
            // info.AllowedRight.UnionWith(roadsLeft.Select(tileInfo => tileInfo.Target));
            info.AllowedRight.Add(strHor.Tile);
        }

        strHor.AllowedUp.Add(houseTile);
        strHor.AllowedDown.Add(houseTile);

        // Create final dictionary
        Dictionary<Tile, TileInfo> infos = new Dictionary<Tile, TileInfo>();
        infos[tiles[0]] = cnrDownLeft;
        infos[tiles[1]] = cnrDownRight;
        infos[tiles[2]] = cnrUpLeft;
        infos[tiles[3]] = cnrUpRight;
        infos[tiles[4]] = strHor;
        infos[tiles[5]] = strVer;
        infos[tiles[6]] = tintDownLeftRight;
        infos[tiles[7]] = tintUpDownLeft;
        infos[tiles[8]] = tintUpDownRight;
        infos[tiles[9]] = tintUpLeftRight;
        infos[tiles[10]] = xint;
        infos[houseTile] = house;

        // Add house tiles as compatible with edges
        foreach (Tile tile in tiles)
        {
            TileInfo info = infos[tile];
            if (!roadsUp.Contains(info)) info.AllowedUp.Add(houseTile);
            if (!roadsDown.Contains(info)) info.AllowedDown.Add(houseTile);
            if (!roadsLeft.Contains(info)) info.AllowedLeft.Add(houseTile);
            if (!roadsRight.Contains(info)) info.AllowedRight.Add(houseTile);
        }

        var noRoadsUp = new HashSet<Tile>(tiles);
        noRoadsUp.ExceptWith(roadsUp.Select(tileInfo => tileInfo.Tile));
        foreach (var tile in noRoadsUp) { infos[tile].AllowedUp.Add(houseTile); }
        house.AllowedDown = noRoadsUp;

        var noRoadsDown = new HashSet<Tile>(tiles);
        noRoadsDown.ExceptWith(roadsDown.Select(tileInfo => tileInfo.Tile));
        foreach (var tile in noRoadsDown) { infos[tile].AllowedDown.Add(houseTile); }
        house.AllowedUp = noRoadsDown;
        
        var noRoadsLeft = new HashSet<Tile>(tiles);
        noRoadsLeft.ExceptWith(roadsLeft.Select(tileInfo => tileInfo.Tile));
        foreach (var tile in noRoadsLeft) { infos[tile].AllowedLeft.Add(houseTile); }
        house.AllowedRight = noRoadsLeft;
        
        var noRoadsRight = new HashSet<Tile>(tiles);
        noRoadsRight.ExceptWith(roadsRight.Select(tileInfo => tileInfo.Tile));
        foreach (var tile in noRoadsRight) { infos[tile].AllowedRight.Add(houseTile); }
        house.AllowedLeft = noRoadsRight;

        return infos;
    }
}
