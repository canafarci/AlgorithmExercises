namespace CaveGeneration
{
	public struct Triangle
	{
		private readonly int _vertexIndexA;
		private readonly int _vertexIndexB;
		private readonly int _vertexIndexC;
		
		public int[] vertices { get; private set; }

		public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
		{
			_vertexIndexA = vertexIndexA;
			_vertexIndexB = vertexIndexB;
			_vertexIndexC = vertexIndexC;
			
			vertices = new int[3] { _vertexIndexA, _vertexIndexB, _vertexIndexC };
		}

		public bool ContainsVertexIndex(int vertexIndex)
		{
			return vertexIndex == _vertexIndexA || vertexIndex == _vertexIndexB || vertexIndex == _vertexIndexC;
		}
	}
}