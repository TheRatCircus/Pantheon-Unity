namespace Pantheon.World
{
	[System.Serializable]
    public sealed class GameWorld
    {
        public const ushort Depth = 1;

        public Layer[] Layers { get; set; } = new Layer[Depth];

        public GameWorld()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new Layer();
            }
        }
    }
}
