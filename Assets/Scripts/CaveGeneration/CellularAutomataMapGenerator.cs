using System;
using UnityEngine;

public class CellularAutomataMapGenerator
{
    private readonly int _width;
    private readonly int _height;
    private string _seed;
    private readonly bool _useRandomSeed;
    private readonly int _smoothingSteps;
    private readonly int _randomFillPercent;

    public CellularAutomataMapGenerator(int width, int height, bool useRandomSeed, int smoothingSteps, int randomFillPercent, string seed = null)
    {
        _width = width;
        _height = height;
        _seed = seed;
        _useRandomSeed = useRandomSeed;
        _smoothingSteps = smoothingSteps;
        _randomFillPercent = randomFillPercent;
    }
    
    public int[,] map { get; private set; } // Array representing the map
    
    public void GenerateMap()
    {
        map = new int[_width, _height]; // Initialize the map array
        RandomFillMap(); // Fill the map randomly

        // Apply the smoothing process multiple times
        for (int i = 0; i < _smoothingSteps; i++)
        {
            SmoothMap();
        }
    }

    // Smooths the map by applying cellular automata rules
    private void SmoothMap()
    {
        int[,] newMap = new int[_width, _height]; // Temporary array to store the new state of the map

        // Iterate through each cell in the map
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int neighborWallTiles = GetSurroundingWallCount(x, y); // Count surrounding walls
                // Apply the smoothing rule: if more than 4 neighbors are walls, the cell becomes a wall; if fewer than 4, it becomes empty
                newMap[x, y] = (neighborWallTiles > 4) ? 1 : (neighborWallTiles < 4) ? 0 : map[x, y];
            }
        }

        // Update the map to the new state
        map = newMap;
    }

    // Counts the number of wall tiles around a given cell
    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        // Iterate through the neighboring cells
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                // Check if the neighbor is within the bounds of the map
                if (IsInBounds(neighbourX, neighbourY))
                {
                    // Exclude the center cell
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY]; // Count the wall
                    }
                }
                else
                {
                    wallCount++; // Consider out-of-bounds neighbors as walls
                }
            }
        }

        return wallCount; // Return the count of surrounding wall tiles
    }

    // Checks if a given position is within the bounds of the map
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    // Randomly fills the map based on the provided seed and random fill percentage
    private void RandomFillMap()
    {
        if (_useRandomSeed || _seed == null)
        {
            _seed = Time.time.ToString(); // Generate a new seed based on the current time
        }

        System.Random pseudoRandomNumberGenerator = new System.Random(_seed.GetHashCode()); // Create a random number generator using the seed

        // Iterate through each cell in the map
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Ensure the edges of the map are always walls
                bool isEdge = x == 0 || x == _width - 1 || y == 0 || y == _height - 1;
                // Fill the cell based on the random fill percentage
                map[x, y] = isEdge || pseudoRandomNumberGenerator.Next(0, 100) < _randomFillPercent ? 1 : 0;
            }
        }
    }


}
