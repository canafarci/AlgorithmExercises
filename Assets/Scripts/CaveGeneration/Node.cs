using UnityEngine;

namespace CaveGeneration
{
	public class Node
	{
		public int VertexIndex = -1;

		public Vector3 position { get; private set; }

		public Node(Vector3 position)
		{
			this.position = position;
		}
	}

	public class ControlNode : Node
	{
		public Node AboveNode;
		public Node RightNode;
		
		public bool isWall { get; private set; }
		
		public ControlNode(Vector3 position, bool isWall, float squareSize) : base(position)
		{
			this.isWall = isWall;
			AboveNode = new Node(position + Vector3.forward * squareSize / 2f);
			RightNode = new Node(position + Vector3.right * squareSize / 2f);
		}
	}
}