﻿using System;
using System.Windows;

namespace MapsControl.Engine
{
    public static class Point2DExtensions
    {
        public static bool IsZero(this Point2D point)
        {
            return point.X == 0 && point.Y == 0;
        }

        public static Point2D ToPoint2D(this Point point)
        {
            return new Point2D((int)point.X, (int)point.Y);
        }

#if DESKTOP
        public static Point2D ToPoint2D(this Vector vector)
        {
            return new Point2D((int)vector.X, (int)vector.Y);
        }
#endif

        public static Point2D Offset(this Point2D point, Point2D offset)
        {
            return new Point2D(point.X + offset.X, point.Y + offset.Y);
        }

        public static Point2D Distance(this Point2D first, Point2D second)
        {
            return new Point2D(second.X - first.X, second.Y - first.Y);
        }

        public static Point Move(this Point point, double offsetX, double offsetY)
        {
            return new Point(point.X + offsetX, point.Y + offsetY); 
        }

        public static Point Move(this Point point, Point2D offset)
        {
            return new Point(point.X + offset.X, point.Y + offset.Y);
        }

        public static Point ToPoint(this Point2D point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
