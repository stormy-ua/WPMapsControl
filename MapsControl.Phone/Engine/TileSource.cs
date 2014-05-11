namespace MapsControl.Engine
{
    public class TileSource
    {
        public static TileSource Empty { get; private set; }

        static TileSource()
        {
            Empty = new TileSource();
        }
    }
}