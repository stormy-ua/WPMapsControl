using System.Linq;
using MapsControl.Infrastructure;
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
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Tile Tile { get; set; }
        public int LevelOfDetails { get; set; }
        public Point2D MapCoords { get; set; }
        public CancellationToken CancellationToken { get; private set; }
        public TileDownloadRequest()
        {
            CancellationToken = _cancellationTokenSource.Token;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }

    public class TileLoader : ITileLoader
    {
        #region Fields

        private readonly ITileSourceProvider _tileUriProvider;
        private readonly List<TileDownloadRequest> _downloadRequests =
            new List<TileDownloadRequest>();
        private readonly object _syncObject = new object();

        #endregion

        #region Constructors

        public TileLoader(ITileSourceProvider tileUriProvider)
        {
            _tileUriProvider = tileUriProvider;
        }

        #endregion

        #region ITileLoader

        public Task LoadAsync(Tile tile)
        {
            return Task.Factory.StartNew(() => Load(tile));
        }

        private void Load(Tile tile)
        {
            var downloadRequest = new TileDownloadRequest
            {
                Tile = tile,
                LevelOfDetails = (int)tile.LevelOfDetails,
                MapCoords = new Point2D(tile.MapX, tile.MapY)
            };
            lock (_syncObject)
            {
                _downloadRequests.Where(r => r.Tile == downloadRequest.Tile).ForEach(r => r.Cancel());
                _downloadRequests.Add(downloadRequest);
            }

            var tileSource = _tileUriProvider
                .GetTileSource(downloadRequest.LevelOfDetails, downloadRequest.MapCoords.X,
                    downloadRequest.MapCoords.Y);

            lock (_syncObject)
            {
                _downloadRequests.Remove(downloadRequest);
            }
            if (downloadRequest.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            downloadRequest.Tile.TileSource = tileSource;
        }

        #endregion
    }
}