using System;

namespace MapsControl.Engine
{
    public interface ITileUriProvider
    {
        Uri GetTileUri(int levelOfDetail, int x, int y);
    }
}
