using System.Threading.Tasks;

namespace MapsControl.Engine
{
    public class TileLoader : ITileLoader
    {
        #region Fields

        private readonly ITileSourceProvider _tileUriProvider;

        #endregion

        #region Constructors

        public TileLoader(ITileSourceProvider tileUriProvider)
        {
            _tileUriProvider = tileUriProvider;
        }

        #endregion

        #region ITileLoader

        public async Task LoadAsync(Tile tile)
        {
            tile.TileSource = await _tileUriProvider.GetTileSourceAsync((int)tile.LevelOfDetails, tile.MapX, tile.MapY);
        }

        #endregion
    }
}