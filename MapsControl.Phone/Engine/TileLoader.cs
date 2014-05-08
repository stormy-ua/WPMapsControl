using Microsoft.Phone.Reactive;
using System.Threading.Tasks;

namespace MapsControl.Engine
{
    public class TileLoader : ITileLoader
    {
        #region Fields

        private readonly ITileSourceProvider _tileUriProvider;
        private readonly ISubject<Tile> _loadTileSubject = new Subject<Tile>(); 

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
            //_loadTileSubject.OnNext(tile);
        }

        #endregion
    }
}