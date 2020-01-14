using System;

namespace Pantheon.World
{
	[Serializable]
	public sealed class Layer
	{
		public const ushort Height = 8;
		public const ushort Width = 8;

		public Chunk[] Chunks { get; private set; } = new Chunk[Width * Height];

		public Layer()
		{
			for (byte x = 0; x < Width; x++)
				for (byte y = 0; y < Height; y++)
				{
					Chunks[ChunkIndex(x, y)] = new Chunk(x, y);
				}
		}

		public int ChunkIndex(int x, int y)
		{
			if (x > Width || y > Height || x < 0 || y < 0)
				throw new ArgumentException();

			return Width * x + y;
		}
	}
}
