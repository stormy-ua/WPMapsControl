namespace MapsControl.Engine
{
    public class TileByteArraySource : TileSource
    {
        public byte[] TileBytes { get; private set; }

        public TileByteArraySource(byte[] stream)
        {
            TileBytes = stream;
        }
    }
}