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
		public SquareGrid squareGrid { get; private set; }

		public void GenerateMesh(int[,] map, float squareSize)
		{
			squareGrid = new SquareGrid(map, squareSize);
		}
	}
}