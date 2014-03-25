using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public class TileUriProvider : ITileUriProvider
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

        public Uri GetTileUri(int levelOfDetail, int x, int y)
        {
            string uriString = _tileUriTemplate
                .Replace("{zoom}", levelOfDetail.ToString())
                .Replace("{x}", x.ToString())
                .Replace("{y}", y.ToString());

            return new Uri(uriString);
        }

        #endregion
    }
}
