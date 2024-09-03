using System.Collections.Generic;
using UnityEngine;

// The Marching Squares algorithm is a computer graphics algorithm used to generate contour lines or isosurfaces from a scalar field,
// typically in a 2D grid. It is commonly used in terrain generation, medical imaging, and fluid dynamics to visualize data.
//
//	----Key Points:----
// Grid and Scalar Field:
//			The algorithm works on a 2D grid where each cell in the grid has a scalar value (e.g., height, density, etc.). These values represent some continuous field.
// 	Binary Classification:
//			Each corner of a grid cell is classified as either inside or outside a contour based on whether its value is above or below a given threshold.
// 	Cell Configuration:
//			Depending on the classification of its corners, each cell is assigned a unique binary number (ranging from 0 to 15). This number corresponds to one of 16 possible patterns of how the contour line might pass through the cell.
// 	Interpolation:
//			The algorithm interpolates the position of the contour line along the edges of the cell based on the scalar values, ensuring a smooth transition between cells.
// 	Contour Line Creation:
//			By processing all the cells in the grid, the algorithm creates continuous contour lines or outlines of the regions where the scalar field meets the threshold.

namespace CaveGeneration
{
	public class MarchingSquaresMeshGenerator
	{
		private List<Vector3> _vertices; 
		private List<int> _triangles;
		public SquareGrid squareGrid { get; private set; }

		public Mesh GenerateMesh(int[,] map, float squareSize)
		{
			_vertices = new List<Vector3>();
			_triangles = new List<int>();
			
			squareGrid = new SquareGrid(map, squareSize);
			
			// Iterate through each square
			for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
			{
				for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
				{
					TriangulateSquare(squareGrid.squares[x,y]);
				}
			}
			
			Mesh mesh = new Mesh();
			mesh.vertices = _vertices.ToArray();
			mesh.triangles = _triangles.ToArray();
			mesh.RecalculateNormals();
			
			return mesh;
		}

		private void TriangulateSquare(Square square)
		{
			//create mesh from marching squares configuration index.
			//reference: https://jamie-wong.com/images/14-08-11/marching-squares-mapping.png
			
			
			switch (square.configurationIndex) {
				case 0:
					break;

				// 1 points:
				case 1:
					MeshFromPoints(square.centerBottomNode, square.bottomLeftNode, square.centerLeftNode);
					break;
				case 2:
					MeshFromPoints(square.centerRightNode, square.bottomRightNode, square.centerBottomNode);
					break;
				case 4:
					MeshFromPoints(square.centerTopNode, square.topRightNode, square.centerRightNode);
					break;
				case 8:
					MeshFromPoints(square.topLeftNode, square.centerTopNode, square.centerLeftNode);
					break;

				// 2 points:
				case 3:
					MeshFromPoints(square.centerRightNode, square.bottomRightNode, square.bottomLeftNode, square.centerLeftNode);
					break;
				case 6:
					MeshFromPoints(square.centerTopNode, square.topRightNode, square.bottomRightNode, square.centerBottomNode);
					break;
				case 9:
					MeshFromPoints(square.topLeftNode, square.centerTopNode, square.centerBottomNode, square.bottomLeftNode);
					break;
				case 12:
					MeshFromPoints(square.topLeftNode, square.topRightNode, square.centerRightNode, square.centerLeftNode);
					break;
				case 5:
					MeshFromPoints(square.centerTopNode, square.topRightNode, square.centerRightNode, square.centerBottomNode, square.bottomLeftNode, square.centerLeftNode);
					break;
				case 10:
					MeshFromPoints(square.topLeftNode, square.centerTopNode, square.centerRightNode, square.bottomRightNode, square.centerBottomNode, square.centerLeftNode);
					break;

				// 3 point:
				case 7:
					MeshFromPoints(square.centerTopNode, square.topRightNode, square.bottomRightNode, square.bottomLeftNode, square.centerLeftNode);
					break;
				case 11:
					MeshFromPoints(square.topLeftNode, square.centerTopNode, square.centerRightNode, square.bottomRightNode, square.bottomLeftNode);
					break;
				case 13:
					MeshFromPoints(square.topLeftNode, square.topRightNode, square.centerRightNode, square.centerBottomNode, square.bottomLeftNode);
					break;
				case 14:
					MeshFromPoints(square.topLeftNode, square.topRightNode, square.bottomRightNode, square.centerBottomNode, square.centerLeftNode);
					break;

				// 4 point:
				case 15:
					MeshFromPoints(square.topLeftNode, square.topRightNode, square.bottomRightNode, square.bottomLeftNode);
					break;
			}
		}

		private void MeshFromPoints(params Node[] points)
		{
			AssignVertices(points);
			
			if (points.Length >= 3)
				CreateTriangle(points[0], points[1], points[2]);
			if (points.Length >= 4)
				CreateTriangle(points[0], points[2], points[3]);
			if (points.Length >= 5) 
				CreateTriangle(points[0], points[3], points[4]);
			if (points.Length >= 6)
				CreateTriangle(points[0], points[4], points[5]);
		}

		private void AssignVertices(Node[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].VertexIndex == -1) //this has not been assigned yet
				{
					points[i].VertexIndex = _vertices.Count;
					_vertices.Add(points[i].position);
				}
			}
		}

		private void CreateTriangle(Node a, Node b, Node c)
		{
			_triangles.Add(a.VertexIndex);
			_triangles.Add(b.VertexIndex);
			_triangles.Add(c.VertexIndex);
		}
	}
}