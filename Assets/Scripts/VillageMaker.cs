using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class VillageMaker : MonoBehaviour
{
    public Tilemap villageMap;
    public int villageWidth;
    public int villageHeight;
    public List<Tile> houseTiles;
    public List<Tile> fourSetRoads;
    public List<Tile> eightSetRoads;
    public bool drawAlgorithm;
    public Complexity complexity;

    private Dictionary<Tile, TileInfo> _infos;
    private List<Tile> roadTiles;
    private HashSet<Tile>[,] _villageState;
    private List<TileState> _states = new List<TileState>();

    private bool _villageInContradiction = true;
    private int _unassignedTiles;
    
    public enum Complexity
    {
        FourTiles,
        EightTiles
    }

    void Start()
    {
        Debug.Log("Starting");
        List<Tile> allTiles;

        switch (complexity)
        {
            case Complexity.FourTiles:
                allTiles = new List<Tile>(fourSetRoads) {houseTiles[0]};
                _infos = FourTileInfoGenerator.GetCompatibilityInfo(allTiles);
                roadTiles = fourSetRoads;
                break;
            case Complexity.EightTiles:
            default:
                allTiles = new List<Tile>(eightSetRoads) {houseTiles[0]};
                _infos = EightTileInfoGenerator.GetCompatibilityInfo(allTiles);
                roadTiles = eightSetRoads;
                break;
        }
    
        Debug.Log("Generated compatibility info");
        
        int count = 0;
        while (_villageInContradiction && count < 1000)
        {
            count++;
            Debug.Log("Starting village calc #" + count);
            _villageState = InitVillageState(allTiles);
            if (drawAlgorithm) _states = new List<TileState>();
            _villageInContradiction = false;
            CalculateVillageState();
        }
        
        Debug.Log("Village in contradiction = " + _villageInContradiction);

        if (!_villageInContradiction)
        {
            StartCoroutine(DrawStates());
        }
    }

    private IEnumerator DrawStates()
    {
        foreach (TileState state in _states)
        {
            villageMap.SetTile(state.Position, state.Tile);
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private void CalculateVillageState()
    {
        _unassignedTiles = villageHeight * villageWidth;
        SetFirstTile();
        while (!_villageInContradiction && _unassignedTiles > 0)
        {
            Vector2 coords = SelectTileWithFewestPossibilities();
            SetTile(coords);
        }
    }

    private void SetFirstTile()
    {
        int x = Random.Range(1, villageWidth - 2);
        int y = Random.Range(1, villageHeight - 2);

        // Init first tile to either a house or road tile with 50/50 chance
        Tile firstTile = Random.Range(0f, 1f) > 0.5
            ? houseTiles[0]
            : roadTiles[Random.Range(0, roadTiles.Count)];
        
        _villageState[y, x] = new HashSet<Tile> {firstTile};
        
        SetTile(x, y);
    }

    private Vector2 SelectTileWithFewestPossibilities()
    {
        int lowestCount = roadTiles.Count + 2;
        List<Vector2> coords = new List<Vector2>(); // current coords with the lowest count

        for (int row = 0; row < villageHeight; row++)
        {
            for (int column = 0; column < villageWidth; column++)
            {
                var tilePoss = _villageState[row, column];
                if (tilePoss.Count > 1 && tilePoss.Count < lowestCount) // tiles with 1 poss are already set
                {
                    lowestCount = tilePoss.Count;
                    coords = new List<Vector2> {new Vector2(column, row)};
                }
                else if (tilePoss.Count == lowestCount)
                {
                    coords.Add(new Vector2(column, row));
                }
            }
        }

        return coords[Random.Range(0, coords.Count)];
    }

    private void SetTile(Vector2 coords)
    {
        SetTile((int) coords.x, (int) coords.y);
    }

    private void SetTile(int x, int y)
    {
        List<Tile> possibilities = new List<Tile>(_villageState[y, x]);
        Tile chosen = possibilities[0];

        if (possibilities.Count > 1)
        {
            float totalPoss = 0f;
            foreach (Tile tile in possibilities)
            {
                totalPoss += _infos[tile].Weight;
            }

            float chosenValue = Random.Range(0f, totalPoss);
            foreach (Tile tile in possibilities)
            {
                chosenValue -= _infos[tile].Weight;
                if (chosenValue < 0)
                {
                    chosen = tile;
                    break;
                }
            }
        }

        // chosen = possibilities.ElementAt(Random.Range(0, possibilities.Count));

        _villageState[y, x] = new HashSet<Tile> {chosen};
        Vector3Int position = new Vector3Int(x - villageWidth / 2, y - villageHeight / 2, 0); // offset to keep centred on camera
        Tile toDraw = chosen.Equals(houseTiles[0]) ? houseTiles[Random.Range(0, houseTiles.Count)] : chosen; // choose a random house if its a house

        if (drawAlgorithm)
        {
            _states.Add(new TileState(toDraw, position));
        }
        else
        {
            villageMap.SetTile(position, toDraw); 

        }
        
        _unassignedTiles--;
        // Debug.Log("Unassigned Tiles = " + unassignedTiles);

        PropagateWaveformAfterSetting(x, y);
    }

    private void PropagateWaveformAfterSetting(Vector2 coords)
    {
        PropagateWaveformAfterSetting((int) coords.x, (int) coords.y);
    }

    private void PropagateWaveformAfterSetting(int x, int y)
    {
        var newTileInfo = _infos[_villageState[y, x].First()];

        // Check the tile list above
        if (y < villageHeight - 1)
        {
            var currentPossAbove = _villageState[y + 1, x];
            if (currentPossAbove.Count > 1) // if it hasn't been set yet
            {
                currentPossAbove.IntersectWith(newTileInfo.AllowedUp); // cut out not allowed options
                if (currentPossAbove.Count == 0)
                {
                    _villageInContradiction = true;
                    Debug.Log("Tried to set tile " + newTileInfo.Name + " below " + GetTileString(_villageState[y + 1, x]));
                }
                else
                {
                    _villageState[y + 1, x] = currentPossAbove;
                    if (currentPossAbove.Count == 1)
                    {
                        SetTile(x, y + 1); 
                    }
                }
            }
        }
        
        // Check the tile list below
        if (y > 0)
        {
            var currentPossBelow = _villageState[y - 1, x];
            if (currentPossBelow.Count > 1) // if it hasn't been set yet
            {
                currentPossBelow.IntersectWith(newTileInfo.AllowedDown); // cut out not allowed options
                if (currentPossBelow.Count == 0)
                {
                    _villageInContradiction = true;
                    Debug.Log("Tried to set tile " + newTileInfo.Name + " above " + GetTileString(_villageState[y - 1, x]));
                }
                else
                {
                    _villageState[y - 1, x] = currentPossBelow;
                    if (currentPossBelow.Count == 1) {
                        SetTile(x, y - 1);
                    }
                }
            }
        }
        
        // Check the tile list to the left
        if (x > 0)
        {
            var currentPossLeft = _villageState[y, x - 1];
            if (currentPossLeft.Count > 1) // if it hasn't been set yet
            {
                currentPossLeft.IntersectWith(newTileInfo.AllowedLeft); // cut out not allowed options
                if (currentPossLeft.Count == 0)
                {
                    _villageInContradiction = true;
                    Debug.Log("Tried to set tile " + newTileInfo.Name + " to the right of " + GetTileString(_villageState[y, x - 1]));
                }
                else
                {
                    _villageState[y, x - 1] = currentPossLeft;
                    if (currentPossLeft.Count == 1)
                    {
                        SetTile(x - 1, y);
                    }
                }
            }
        }
        
        // Check the tile list to the right
        if (x < villageWidth - 1)
        {
            var currentPossRight = _villageState[y, x + 1];
            if (currentPossRight.Count > 1) // if it hasn't been set yet
            {
                currentPossRight.IntersectWith(newTileInfo.AllowedRight); // cut out not allowed options
                if (currentPossRight.Count == 0)
                {
                    _villageInContradiction = true;
                    Debug.Log("Tried to set tile " + newTileInfo.Name + " to the left of " + GetTileString(_villageState[y, x + 1]));
                }
                else
                {
                    _villageState[y, x + 1] = currentPossRight;
                    if (currentPossRight.Count == 1)
                    {
                        SetTile(x + 1, y);
                    }
                }
            }
        }
    }

    private HashSet<Tile>[,] InitVillageState(List<Tile> allTiles)
    {
        var startingTilePossibilities = new HashSet<Tile>[villageHeight, villageWidth];
        for (int row = 0; row < villageHeight; row++)
        {
            for (int column = 0; column < villageWidth; column++)
            {
                startingTilePossibilities[row, column] = new HashSet<Tile>(allTiles);
            }
        }
        return startingTilePossibilities;
    }

    public static string GetTileString(IEnumerable<Tile> tiles)
    {
        string toPrint = "{ ";
        foreach (Tile tile in tiles)
        {
            toPrint += tile.name + ", ";
        }

        toPrint = toPrint.Substring(0, toPrint.Length - 2) + " }";
        return toPrint;
    }
}

internal class TileState
{
    public Tile Tile;
    public Vector3Int Position;

    public TileState(Tile tile, Vector3Int position)
    {
        Tile = tile;
        Position = position;
    }
}
