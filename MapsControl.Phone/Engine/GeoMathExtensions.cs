﻿using System;
using System.Device.Location;
using System.Windows;

namespace MapsControl.Engine
{
    public static class GeoMathExtensions
    {
        private static readonly MapMath _mapMath = new MapMath();

        public static Point2D ToPointInAbsolutePixels(this GeoCoordinate geoCoordinate, double levelOfDetail)
        {
            int pixelY;
            int pixelX;

            _mapMath.LatLongToPixelXY(geoCoordinate.Latitude, geoCoordinate.Longitude, (int)levelOfDetail, out pixelX, out pixelY);
            return new Point2D(pixelX, pixelY);
        }

        public static Point2D ToPointInTileRelativePixels(this GeoCoordinate geoCoordinate, double levelOfDetail, int tileSize)
        {
            Point2D pointInAbsolutePixels = geoCoordinate.ToPointInAbsolutePixels(levelOfDetail);
            return new Point2D(pointInAbsolutePixels.X % tileSize, pointInAbsolutePixels.Y % tileSize);
        }

        public static Point2D ToPointInTileIndexes(this GeoCoordinate geoCoordinate, double levelOfDetail, int tileSize)
        {
            Point2D pointInAbsolutePixels = geoCoordinate.ToPointInAbsolutePixels(levelOfDetail);

            int tileX = (int)Math.Floor(pointInAbsolutePixels.X / (double)tileSize);
            int tileY = (int)Math.Floor(pointInAbsolutePixels.Y / (double)tileSize);
            return new Point2D(tileX, tileY);
        }

        public static GeoCoordinate AbsolutePixelsToGeoCoordinate(this Point2D pointInAbsolutePixels, double levelOfDetail)
        {
            double latitute;
            double longitude;
            _mapMath.PixelXYToLatLong(pointInAbsolutePixels.X, pointInAbsolutePixels.Y, (int)levelOfDetail, out latitute, out longitude);

            return new GeoCoordinate(latitute, longitude);
        }

        public static Point2D DistanceInPixelsTo(this GeoCoordinate first, GeoCoordinate second, double levelOfDetail)
        {
            Point2D firstPoint = first.ToPointInAbsolutePixels(levelOfDetail);
            Point2D secondPoint = second.ToPointInAbsolutePixels(levelOfDetail);

            return new Point2D(secondPoint.X - firstPoint.X, secondPoint.Y - firstPoint.Y);
        }

        public static GeoCoordinate OffsetByPixels(this GeoCoordinate geoCoordinate, Point2D offset, double levelOfDetail)
        {
            Point2D pointInAbsolutePixels = geoCoordinate.ToPointInAbsolutePixels(levelOfDetail);

            pointInAbsolutePixels.X -= offset.X;
            pointInAbsolutePixels.Y -= offset.Y;

            return pointInAbsolutePixels.AbsolutePixelsToGeoCoordinate(levelOfDetail);
        }

        public static Point2D ToWindowPoint(
            this GeoCoordinate geoCoordinate, GeoCoordinate centerGeoCoordinate, double levelOfDetail, Size windowSize)
        {
            Point2D centerPointInTileAbsolutePixels = centerGeoCoordinate.ToPointInAbsolutePixels(levelOfDetail);
            Point2D pinAbsoluteInPixels = geoCoordinate.ToPointInAbsolutePixels(levelOfDetail);
            Point2D distanceToCenterInPixels = centerPointInTileAbsolutePixels.Distance(pinAbsoluteInPixels);
            return new Point2D(
                distanceToCenterInPixels.X + (int)(windowSize.Width / 2),
                distanceToCenterInPixels.Y + (int)(windowSize.Height / 2));
        }
    }
}
