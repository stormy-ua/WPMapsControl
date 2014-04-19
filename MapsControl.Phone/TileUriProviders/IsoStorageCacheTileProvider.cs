using MapsControl.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Infrastructure;

namespace MapsControl.TileUriProviders
{
    public class IsoStorageCacheTileProvider : ITileUriProvider
    {
        #region Fields

        private readonly IIsoStorage _isoStorage = new IsoStorage();
        private readonly ITileUriProvider _tileUriProvider;
        private readonly string _tilePathTemplate;

        #endregion

        #region Constructors

        public IsoStorageCacheTileProvider(ITileUriProvider tileUriProvider, string tilePathTemplate)
        {
            _tileUriProvider = tileUriProvider;
            _tilePathTemplate = tilePathTemplate;
        }

        #endregion

        #region ITileUriProvider

        public Uri GetTileUri(int levelOfDetail, int x, int y)
        {
            string relativePath = _tilePathTemplate.ReplaceTileTemplates(levelOfDetail, x, y);
            string absolutePath = _isoStorage.GetIsoStorageAbsolutePath(relativePath);

            if (!_isoStorage.FileExists(relativePath))
            {
                return _tileUriProvider.GetTileUri(levelOfDetail, x, y);
            }

            return new Uri(absolutePath);
        }

        #endregion
    }
}
