using UnityEngine;

namespace CaveGeneration
{
	public class MapGizmoVisualizer : MonoBehaviour
	{
		[SerializeField] private MapGenerator MapGenerator;
		[SerializeField] private bool DisplayCellularAutomataMap;
		[SerializeField] private bool DisplayMarchingSquareNodes;
		
		// Visualizes the map using Gizmos in the Unity editor
		private void OnDrawGizmos()
		{
			if (DisplayCellularAutomataMap && MapGenerator.cellularAutomataMapGenerator != null)
				DrawCellularAutomataMap();

			if (DisplayMarchingSquareNodes && MapGenerator.marchingSquaresMeshGenerator != null)
				DrawMarchingSquaresNodes();

		}

		private void DrawMarchingSquaresNodes()
		{
			SquareGrid squareGrid = MapGenerator.marchingSquaresMeshGenerator.squareGrid;
			
			if (squareGrid == null) return;
			
			// Iterate through each square node
			for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
			{
				for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
				{
					Gizmos.color = (squareGrid.squares[x,y].topLeftNode.isActive)?Color.black:Color.cyan;
					Gizmos.DrawCube(squareGrid.squares[x,y].topLeftNode.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares[x,y].topRightNode.isActive)?Color.black:Color.cyan;
					Gizmos.DrawCube(squareGrid.squares[x,y].topRightNode.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares[x,y].bottomRightNode.isActive)?Color.black:Color.cyan;
					Gizmos.DrawCube(squareGrid.squares[x,y].bottomRightNode.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares[x,y].bottomLeftNode.isActive)?Color.black:Color.cyan;
					Gizmos.DrawCube(squareGrid.squares[x,y].bottomLeftNode.position, Vector3.one * .4f);
					
					Gizmos.color = Color.grey;
					Gizmos.DrawCube(squareGrid.squares[x,y].centerTopNode.position, Vector3.one * .15f);
					Gizmos.DrawCube(squareGrid.squares[x,y].centerRightNode.position, Vector3.one * .15f);
					Gizmos.DrawCube(squareGrid.squares[x,y].centerBottomNode.position, Vector3.one * .15f);
					Gizmos.DrawCube(squareGrid.squares[x,y].centerLeftNode.position, Vector3.one * .15f);

				}
			}
		}

		private void DrawCellularAutomataMap()
		{
			int[,] map = MapGenerator.cellularAutomataMapGenerator.map;
			
			if (map == null) return; // Exit if the map is not generated

			// Iterate through each cell in the map
			for (int x = 0; x < MapGenerator.width; x++)
			{
				for (int y = 0; y < MapGenerator.height; y++)
				{
					// Set the color based on whether the cell is a wall or empty
					Gizmos.color = (map[x, y] == 1) ? Color.black : Color.cyan;
					// Draw a cube at the cell's position
					Vector3 pos = new Vector3(-MapGenerator.width / 2 + x + 0.5f,
												0f,
												-MapGenerator.height / 2 + y + 0.5f);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}
		}
	}
}