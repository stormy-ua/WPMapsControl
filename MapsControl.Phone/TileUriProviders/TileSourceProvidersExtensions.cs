using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public static class TileSourceProvidersExtensions
    {
        public static ITileSourceProvider WithCache(this ITileSourceProvider tileSourceProvider, string cacheTilePathTemplate)
        {
            return new IsoStorageCacheTileProvider(tileSourceProvider, cacheTilePathTemplate);            
        }
    }
}
