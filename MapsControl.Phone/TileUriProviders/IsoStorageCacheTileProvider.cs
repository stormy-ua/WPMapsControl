using MapsControl.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Infrastructure;

namespace MapsControl.TileUriProviders
{
    public class IsoStorageCacheTileProvider : ITileSourceProvider
    {
        #region Fields

        private readonly IIsoStorage _isoStorage = new IsoStorage();
        private readonly ITileSourceProvider _tileSourceProvider;
        private readonly string _tilePathTemplate;

        #endregion

        #region Constructors

        public IsoStorageCacheTileProvider(ITileSourceProvider tileSourceProvider, string tilePathTemplate)
        {
            _tileSourceProvider = tileSourceProvider;
            _tilePathTemplate = tilePathTemplate;
        }

        #endregion

        #region ITileUriProvider

        public TileSource GetTileSource(int levelOfDetail, int x, int y)
        {
            string relativePath = _tilePathTemplate.ReplaceTileTemplates(levelOfDetail, x, y);
            string absolutePath = _isoStorage.GetIsoStorageAbsolutePath(relativePath);

            if (!_isoStorage.FileExists(relativePath))
            {
                return _tileSourceProvider.GetTileSource(levelOfDetail, x, y);
            }

            return new TileUriSource(absolutePath);
        }

        public async Task<TileSource> GetTileSourceAsync(int levelOfDetail, int x, int y)
        {
            return GetTileSource(levelOfDetail, x, y);
        }

        #endregion
    }
}
