using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public static class TileUriProvidersExtensions
    {
        public static ITileUriProvider WithCache(this ITileUriProvider tileUriProvider, string cacheTilePathTemplate)
        {
            return new IsoStorageCacheTileProvider(KnownTileUriProviders.OpenCycleMap, cacheTilePathTemplate);            
        }
    }
}
