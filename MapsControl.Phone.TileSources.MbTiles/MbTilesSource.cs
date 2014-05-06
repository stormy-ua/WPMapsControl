using System;
using System.IO;
using System.Threading.Tasks;
using MapsControl.Engine;
using SQLite;
using Sqlite;
using Windows.Storage;

namespace MapsControl.Phone.TileSources.MbTiles
{
    public class MbTilesSource : ITileSourceProvider
    {
        #region Fields

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public MbTilesSource(string databasePath)
        {
            _connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, databasePath);
        }

        #endregion

        #region ITileSourceProvider

        public TileSource GetTileSource(int levelOfDetail, int x, int y)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                y = (int)Math.Pow(2, levelOfDetail) - y - 1;

                var command = new SQLiteCommand(connection);
                command.CommandText = string.Format("select tile_data from tiles where zoom_level={0} and tile_column={1} and tile_row={2}", levelOfDetail, x, y);
                var blob = command.ExecuteScalar<byte[]>();

                if (blob != null)
                {
                    return new TileByteArraySource(blob);
                }
            }

            return null;
        }

        public Task<TileSource> GetTileSourceAsync(int levelOfDetail, int x, int y)
        {
            return Task.Factory.StartNew(() => GetTileSource(levelOfDetail, x, y));
        }

        #endregion
    }
}
