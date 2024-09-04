using System.Collections.Generic;
using System.Linq;
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
		private readonly float _wallHeight;
		
		private List<Vector3> _vertices; 
		private List<int> _triangles;
		private Dictionary<int, List<Triangle>> _triangleLookup;
		private List<List<int>> _outlines;
		private HashSet<int> _checkedVertices;
		public SquareGrid squareGrid { get; private set; }

		public MarchingSquaresMeshGenerator(float wallHeight)
		{
			_wallHeight = wallHeight;
		}
		
		public (Mesh, Mesh) GenerateMesh(int[,] map, float squareSize)
		{
			_vertices = new List<Vector3>();
			_triangles = new List<int>();
			_triangleLookup = new Dictionary<int, List<Triangle>>();
			_outlines = new List<List<int>>();
			_checkedVertices = new HashSet<int>();
			
			squareGrid = new SquareGrid(map, squareSize);
			
			return (Create2DMesh(), CreateWallMesh());
		}

		private Mesh Create2DMesh()
		{
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

		private Mesh CreateWallMesh()
		{
			CalculateMeshOutlines();
			
			List<Vector3> wallVertices = new List<Vector3>();
			List<int> wallTriangles = new List<int>();
			Mesh wallMesh = new Mesh();

			foreach (List<int> outline in _outlines)
			{
				for (int i = 0; i < outline.Count - 1; i++)
				{
					int startIndex = wallVertices.Count;
					wallVertices.Add(_vertices[outline[i]]); //left vertex
					wallVertices.Add(_vertices[outline[i + 1]]); //right vertex
					wallVertices.Add(_vertices[outline[i]] - Vector3.up * _wallHeight); //bottom left vertex
					wallVertices.Add(_vertices[outline[i + 1]] - Vector3.up * _wallHeight); //bottom right vertex
					
					//tri 1
					wallTriangles.Add(startIndex + 0);  //top left
					wallTriangles.Add(startIndex + 2);  //bottom left
					wallTriangles.Add(startIndex + 3);  //bottom right
					//tri 2
					wallTriangles.Add(startIndex + 3); //bottom right
					wallTriangles.Add(startIndex + 1); //top right
					wallTriangles.Add(startIndex + 0); //top left
				}
			}
			
			wallMesh.vertices = wallVertices.ToArray();
			wallMesh.triangles = wallTriangles.ToArray();
			wallMesh.RecalculateNormals();

			return wallMesh;
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
					MeshFromPoints(square.centerLeftNode, square.centerBottomNode, square.bottomLeftNode);
					break;
				case 2:
					MeshFromPoints(square.bottomRightNode, square.centerBottomNode, square.centerRightNode);
					break;
				case 4:
					MeshFromPoints(square.topRightNode, square.centerRightNode, square.centerTopNode);
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
					//none of these vertices can be an outline
					_checkedVertices.Add(square.topLeftNode.VertexIndex);
					_checkedVertices.Add(square.topRightNode.VertexIndex);
					_checkedVertices.Add(square.bottomRightNode.VertexIndex);
					_checkedVertices.Add(square.bottomLeftNode.VertexIndex);
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
			
			Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
			
			AddTriangleToDictionary(triangle); ;
		}

		private void AddTriangleToDictionary(Triangle triangle)
		{
			foreach (int vertexIndexKey in triangle.vertices)
			{
				if (_triangleLookup.ContainsKey(vertexIndexKey))
				{
					_triangleLookup[vertexIndexKey].Add(triangle);
				}
				else
				{
					_triangleLookup.Add(vertexIndexKey, new List<Triangle> { triangle });
				}
			}
		}

		private int GetConnectedOutlineVertex(int vertexIndex)
		{
			List<Triangle> trianglesContainingVertex = _triangleLookup[vertexIndex];

			foreach (Triangle triangle in trianglesContainingVertex)
			{
				foreach (int vertexIndexB in triangle.vertices)
				{
					if (vertexIndexB != vertexIndex &&  !_checkedVertices.Contains(vertexIndexB) && IsOutlineEdge(vertexIndex, vertexIndexB))
					{
						return vertexIndexB;
					}
				}
			}

			return -1;
		}

		private bool IsOutlineEdge(int vertexA, int vertexB)
		{
			//if vertex A and vertex B only share one common triangle, it means it is an outline edge

			List<Triangle> trianglesContainingVertexA = _triangleLookup[vertexA];
			List<Triangle> trianglesContainingVertexB = _triangleLookup[vertexB];

			return trianglesContainingVertexA.Count(x => x.ContainsVertexIndex(vertexB)) == 1;
		}

		private void CalculateMeshOutlines()
		{
			for (var vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
			{
				if (!_checkedVertices.Contains(vertexIndex))
				{
					int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
					if (newOutlineVertex != -1)
					{
						_checkedVertices.Add(vertexIndex);
						
						List<int> newOutline = new List<int>();
						newOutline.Add(vertexIndex);
						_outlines.Add(newOutline);
						FollowOutline(newOutlineVertex, _outlines.Count - 1);
						_outlines[_outlines.Count - 1].Add(vertexIndex);
					}
				}
			}
		}

		private void FollowOutline(int vertexIndex, int outlineIndex)
		{
			_outlines[outlineIndex].Add(vertexIndex);
			_checkedVertices.Add(vertexIndex);

			int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
			
			if (nextVertexIndex != -1)
			{
				FollowOutline(nextVertexIndex, outlineIndex);
			}
		}
	}
}