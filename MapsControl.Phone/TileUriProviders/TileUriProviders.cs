using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;

namespace MapsControl.TileUriProviders
{
    public static class TileUriProviders
    {
        public static ITileUriProvider OpenCycleMap { get; private set; }
        public static ITileUriProvider OpenStreetMap { get; private set; }
        public static ITileUriProvider GoogleHybridMap { get; private set; }
        public static ITileUriProvider YandexMap { get; private set; }

        static TileUriProviders()
        {
            OpenCycleMap = new TileUriProvider("http://a.tile.opencyclemap.org/cycle/{zoom}/{x}/{y}.png");
            OpenStreetMap = new TileUriProvider("http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png");
            GoogleHybridMap = new TileUriProvider("http://mt0.google.com/vt/lyrs=y&z={zoom}&x={x}&y={y}");
            YandexMap = new TileUriProvider("http://01.pvec.maps.yandex.net/tiles?x={x}&y={y}&z={zoom}&l=pmap");
        }
    }
}
