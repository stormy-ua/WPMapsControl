#if WINDOWS_PHONE
using System.Collections.Generic;
using Microsoft.Phone.Reactive;
#endif
#if DESKTOP
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.IO;
using System.Reactive.Subjects;
#endif
using System.Threading.Tasks;
using System.Threading;

namespace MapsControl.Engine
{
    public class TileDownloadRequest
    {
        public Tile Tile { get; set; }
        public int LevelOfDetails { get; set; }
        public Point2D MapCoords { get; set; }
    }

    public class TileLoader : ITileLoader
    {
        #region Fields

        private readonly ITileSourceProvider _tileUriProvider;
        private readonly Queue<TileDownloadRequest> _downloadRequests =
            new Queue<TileDownloadRequest>(); 

        #endregion

        #region Constructors

        public TileLoader(ITileSourceProvider tileUriProvider)
        {
            _tileUriProvider = tileUriProvider;

            for (int j = 0; j < 1; ++j)
            {
                Task.Factory.StartNew(DownloadsProcessing);
            }
        }

        #endregion

        #region ITileLoader

        public async Task LoadAsync(Tile tile)
        {
            //tile.TileSource = TileSource.Empty;
            //tile.TileSource = await _tileUriProvider.GetTileSourceAsync((int)tile.LevelOfDetails, tile.MapX, tile.MapY);
            var downloadRequest = new TileDownloadRequest
                {
                    Tile = tile,
                    LevelOfDetails = (int) tile.LevelOfDetails,
                    MapCoords = new Point2D(tile.MapX, tile.MapY)
                };
            _downloadRequests.Enqueue(downloadRequest);
        }

        private void DownloadsProcessing()
        {
            for (;;Thread.Sleep(100))
            {
                if(_downloadRequests.Count == 0)
                { 
                    continue;
                }

                TileDownloadRequest downloadRequest = _downloadRequests.Dequeue();

                downloadRequest.Tile.TileSource = _tileUriProvider
                    .GetTileSource(downloadRequest.LevelOfDetails, downloadRequest.MapCoords.X, downloadRequest.MapCoords.Y);
            }
        }

        #endregion
    }
}