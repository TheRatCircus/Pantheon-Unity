using System;

namespace Pantheon.World
{
	[Serializable]
	public sealed class Chunk
	{
		public const ushort Width = 64;
		public const ushort Height = 64;

		public byte X { get; private set; }
		public byte Y { get; private set; }

		public byte[] Terrain { get; private set; } = new byte[Width * Height];
		//public List<Entity> Entities { get; private set; }

		public Chunk(byte x, byte y)
		{
			X = x;
			Y = y;
		}

		public int CellIndex(int x, int y)
		{
			if (x > Width || y > Height || x < 0 || y < 0)
				throw new ArgumentException();

			return Width * x + y;
		}
	}
}
