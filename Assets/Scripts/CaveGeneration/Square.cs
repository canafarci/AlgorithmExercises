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
		
		public int configurationIndex {get; private set;} //configuration index in marching squares algorithm

		public Square(ControlNode topLeftNode, ControlNode topRightNode, ControlNode bottomRightNode, ControlNode bottomLeftNode)
		{
			this.topLeftNode = topLeftNode;
			this.topRightNode = topRightNode;
			this.bottomLeftNode = bottomLeftNode;
			this.bottomRightNode = bottomRightNode;

			centerTopNode = topLeftNode.RightNode;
			centerRightNode = bottomRightNode.AboveNode;
			centerBottomNode = bottomLeftNode.RightNode;
			centerLeftNode = bottomLeftNode.AboveNode;

			DetermineConfigurationIndex();
		}

		private void DetermineConfigurationIndex()
		{
			//determine marching squares configuration index relative to neighboring nodes.
			//reference: https://jamie-wong.com/images/14-08-11/marching-squares-mapping.png
			
			configurationIndex = 0;
			
			if (topLeftNode.isActive)
				configurationIndex += 8;
			if (topRightNode.isActive)
				configurationIndex += 4;
			if (bottomRightNode.isActive)
				configurationIndex += 2;
			if (bottomLeftNode.isActive)
				configurationIndex += 1;
		}
	}
}