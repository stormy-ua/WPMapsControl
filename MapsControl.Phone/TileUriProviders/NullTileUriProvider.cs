using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public class NullTileUriProvider : ITileSourceProvider
    {
        public TileSource GetTileSource(int levelOfDetail, int x, int y)
        {
            return null;
        }

        public async Task<TileSource> GetTileSourceAsync(int levelOfDetail, int x, int y)
        {
            return null;
        }
    }
}
