using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;
using MapsControl.Infrastructure;

namespace MapsControl.TileUriProviders
{
    public class TileUriProvider : ITileSourceProvider
    {
        #region Fields

        private readonly string _tileUriTemplate;

        #endregion

        #region Constructors

        public TileUriProvider(string tileUriTemplate)
        {
            _tileUriTemplate = tileUriTemplate;
        }

        #endregion

        #region ITileUriProvider

        public TileSource GetTileSource(int levelOfDetail, int x, int y)
        {
            string uriString = _tileUriTemplate.ReplaceTileTemplates(levelOfDetail, x, y);

            return new TileUriSource(uriString);
        }

        #endregion
    }
}
