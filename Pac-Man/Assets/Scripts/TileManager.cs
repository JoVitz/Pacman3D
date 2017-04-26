using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileManager : MonoBehaviour {

    //def of class tile
    public class Tile
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool occupied { get; set; }
        public int adjacentCount { get; set; }
        public bool isIntersection { get; set; }

        public Tile left, right, up, down;

        public Tile(int x_in, int y_in)
        {
            x = x_in; y = y_in;
            occupied = false;
            left = right = up = down = null;
        }
    };

    public List<Tile> tiles = new List<Tile>();

    // Use this for initialization
    void Start()
    {
        ReadTiles();

    }

    void Update()
    {
        DrawNeighbors();

    }
    //-----------------------------------------------------------------------
    // hardcoded tile data: 1 = free tile, 0 = wall
    void ReadTiles()
    {
        // 19*21 hardwired data instead of reading from file (not feasible on web player)
        string data = @"0000000000000000000
0111111110111111110
0100100010100010010
0111111111111111110
0100101000001010010
0111101110111011110
0000100010100010000
0000101111111010000
0000101001001010000
1110111011101110111
0000101000001010000
0000101111111010000
0000101000001010000
0111111110111111110
0100100010100010010
0110111111111110110
0010101000001010100
0111101110111011110
0100000010100000010
0111111111111111110
0000000000000000000";

        int X = 1, Y = 21;
        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {

                X = 1; // for every line
                for (int i = 0; i < line.Length; ++i)
                {
                    Tile newTile = new Tile(X, Y);

                    // if the tile we read is a valid tile (movable)
                    if (line[i] == '1')
                    {
                        // check for left-right neighbor
                        if (i != 0 && line[i - 1] == '1')
                        {
                            // assign each tile to the corresponding side of other tile
                            newTile.left = tiles[tiles.Count - 1];
                            tiles[tiles.Count - 1].right = newTile;

                            // adjust adjcent tile counts of each tile
                            newTile.adjacentCount++;
                            tiles[tiles.Count - 1].adjacentCount++;
                        }
                    }

                    // if the current tile is not movable
                    else newTile.occupied = true;

                    // check for up-down neighbor, starting from second row (Y<30)
                    int upNeighbor = tiles.Count - line.Length; // up neighbor index
                    if (Y < 20 && !newTile.occupied && !tiles[upNeighbor].occupied)
                    {
                        tiles[upNeighbor].down = newTile;
                        newTile.up = tiles[upNeighbor];

                        // adjust adjcent tile counts of each tile
                        newTile.adjacentCount++;
                        tiles[upNeighbor].adjacentCount++;
                    }

                    tiles.Add(newTile);
                    X++;
                }

                Y--;
            }
        }

        // after reading all tiles, determine the intersection tiles
        foreach (Tile tile in tiles)
        {
            if (tile.adjacentCount > 2)
                tile.isIntersection = true;
        }

    }

    void DrawNeighbors()
    {
        foreach (Tile tile in tiles)
        {
            Vector2 pos = new Vector2(tile.x, tile.y);
            Vector2 up = new Vector2(tile.x + 0.1f, tile.y + 1);
            Vector2 down = new Vector2(tile.x - 0.1f, tile.y - 1);
            Vector2 left = new Vector2(tile.x - 1, tile.y + 0.1f);
            Vector2 right = new Vector2(tile.x + 1, tile.y - 0.1f);

            if (tile.up != null) Debug.DrawLine(pos, up);
            if (tile.down != null) Debug.DrawLine(pos, down);
            if (tile.left != null) Debug.DrawLine(pos, left);
            if (tile.right != null) Debug.DrawLine(pos, right);
        }

    }
    //----------------------------------------------------------------------
    // returns the index in the tiles list of a given tile's coordinates
    public int Index(int X, int Y)
    {
        // if the requsted index is in bounds
        //Debug.Log ("Index called for X: " + X + ", Y: " + Y);
        if (X >= 1 && X <= 19 && Y <= 21 && Y >= 1)
            return (21 - Y) * 19 + X - 1;

        // else, if the requested index is out of bounds
        // return closest in-bounds tile's index 
        if (X < 1) X = 1;
        if (X > 28) X = 19;
        if (Y < 1) Y = 1;
        if (Y > 31) Y = 21;

        return (21 - Y) * 19 + X - 1;
    }

    public int Index(Tile tile)
    {
        return (21 - tile.y) * 19 + tile.x - 1;
    }

    //----------------------------------------------------------------------
    // returns the distance between two tiles
    public float distance(Tile tile1, Tile tile2)
    {
        return Mathf.Sqrt(Mathf.Pow(tile1.x - tile2.x, 2) + Mathf.Pow(tile1.y - tile2.y, 2));
    }
}
