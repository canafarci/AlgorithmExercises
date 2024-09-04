using System;
using UnityEngine;

namespace CaveGeneration
{
	public class MapGenerator : MonoBehaviour
	{
		[SerializeField] private int Width; // Map width
		[SerializeField] private int Height; // Map height
		[SerializeField] private string Seed; // Seed for random generation
		[SerializeField] private bool UseRandomSeed;
		[SerializeField] private int SmoothingSteps;
		[SerializeField] private float WallHeight;
		[Range(0, 100)] [SerializeField] private int RandomFillPercent; // Percentage chance to fill a cell 
		
		[SerializeField] MeshFilter MeshFilter2D;
		[SerializeField] MeshFilter MeshFilterWall;

		public MarchingSquaresMeshGenerator marchingSquaresMeshGenerator { get; private set; }
		public CellularAutomataMapGenerator cellularAutomataMapGenerator { get; private set; }
		public int width { get { return Width; } }
		public int height { get { return Height; } }

		private void Awake()
		{
			cellularAutomataMapGenerator = new CellularAutomataMapGenerator(Width, Height, UseRandomSeed, SmoothingSteps, RandomFillPercent, Seed);
			marchingSquaresMeshGenerator = new MarchingSquaresMeshGenerator(WallHeight);
		}

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
			cellularAutomataMapGenerator.GenerateMap();
			(Mesh mesh2d, Mesh wallMesh) mesh = marchingSquaresMeshGenerator.GenerateMesh(cellularAutomataMapGenerator.map, 1);

			MeshFilter2D.mesh = mesh.mesh2d;
			MeshFilterWall.mesh = mesh.wallMesh;
		}
	}
}