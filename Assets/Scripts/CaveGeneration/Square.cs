namespace CaveGeneration
{
	public class Square
	{
		public ControlNode topLeftNode { get; private set; }
		public ControlNode topRightNode { get; private set; }
		public ControlNode bottomLeftNode { get; private set; }
		public ControlNode bottomRightNode { get; private set; }

		public Node centerTopNode { get; private set;}
		public Node centerRightNode { get; private set;}
		public Node centerBottomNode { get; private set;}
		public Node centerLeftNode { get; private set;}

		public Square(ControlNode topLeftNode, ControlNode topRightNode, ControlNode bottomLeftNode, ControlNode bottomRightNode)
		{
			this.topLeftNode = topLeftNode;
			this.topRightNode = topRightNode;
			this.bottomLeftNode = bottomLeftNode;
			this.bottomRightNode = bottomRightNode;

			centerTopNode = topLeftNode.RightNode;
			centerRightNode = bottomRightNode.AboveNode;
			centerBottomNode = bottomLeftNode.RightNode;
			centerLeftNode = bottomLeftNode.AboveNode;
		}
	}
}