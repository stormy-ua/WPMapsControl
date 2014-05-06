using System;

namespace MapsControl.Engine
{
    public class TileUriSource : TileSource
    {
        public Uri Uri { get; private set; }

        public TileUriSource(Uri uri)
        {
            Uri = uri;
        }

        public TileUriSource(string uri)
            : this(new Uri(uri))
        {
        }
    }
}