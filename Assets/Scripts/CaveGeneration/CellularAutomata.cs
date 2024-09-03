using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int Width; // Map width
    [SerializeField] private int Height; // Map height
    [SerializeField] private string Seed; // Seed for random generation
    [SerializeField] private bool UseRandomSeed;
    [SerializeField] private int SmoothingSteps;
    [Range(0, 100)] [SerializeField] private int RandomFillPercent; // Percentage chance to fill a cell 


    private int[,] _map; // Array representing the map

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        // Regenerate the map when the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        _map = new int[Width, Height]; // Initialize the map array
        RandomFillMap(); // Fill the map randomly

        // Apply the smoothing process multiple times
        for (int i = 0; i < SmoothingSteps; i++)
        {
            SmoothMap();
        }
    }

    // Smooths the map by applying cellular automata rules
    private void SmoothMap()
    {
        int[,] newMap = new int[Width, Height]; // Temporary array to store the new state of the map

        // Iterate through each cell in the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int neighborWallTiles = GetSurroundingWallCount(x, y); // Count surrounding walls
                // Apply the smoothing rule: if more than 4 neighbors are walls, the cell becomes a wall; if fewer than 4, it becomes empty
                newMap[x, y] = (neighborWallTiles > 4) ? 1 : (neighborWallTiles < 4) ? 0 : _map[x, y];
            }
        }

        // Update the map to the new state
        _map = newMap;
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
                        wallCount += _map[neighbourX, neighbourY]; // Count the wall
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
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    // Randomly fills the map based on the provided seed and random fill percentage
    private void RandomFillMap()
    {
        if (UseRandomSeed)
        {
            Seed = Time.time.ToString(); // Generate a new seed based on the current time
        }

        System.Random pseudoRandomNumberGenerator = new System.Random(Seed.GetHashCode()); // Create a random number generator using the seed

        // Iterate through each cell in the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Ensure the edges of the map are always walls
                bool isEdge = x == 0 || x == Width - 1 || y == 0 || y == Height - 1;
                // Fill the cell based on the random fill percentage
                _map[x, y] = isEdge || pseudoRandomNumberGenerator.Next(0, 100) < RandomFillPercent ? 1 : 0;
            }
        }
    }

    // Visualizes the map using Gizmos in the Unity editor
    private void OnDrawGizmos()
    {
        if (_map == null) return; // Exit if the map is not generated

        // Iterate through each cell in the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Set the color based on whether the cell is a wall or empty
                Gizmos.color = (_map[x, y] == 1) ? Color.black : Color.cyan;
                // Draw a cube at the cell's position
                Vector3 pos = new Vector3(-Width / 2 + x + 0.5f, -Height / 2 + y + 0.5f, 0f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }
}
