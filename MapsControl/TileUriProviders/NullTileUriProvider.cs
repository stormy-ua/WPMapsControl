using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public class NullTileUriProvider : ITileUriProvider
    {
        public Uri GetTileUri(int levelOfDetail, int x, int y)
        {
            return null;
        }
    }
}
