namespace MapsControl.Engine
{
    public class TileControllerBuilder
    {
        #region Fields

        private int _tileImageSize = 256;
        private int _tileImagesResolution = 5;

        #endregion

        #region Methods

        public TileControllerBuilder WithTileImageSize(int tileImageSize)
        {
            _tileImageSize = tileImageSize;
            return this;
        }

        public TileControllerBuilder WithTileImageResolution(int tileImageResolution)
        {
            _tileImagesResolution = tileImageResolution;
            return this;
        }

        public TileController Build()
        {
            return new TileController(_tileImagesResolution, _tileImageSize);
        }

        #endregion
    }
}