using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapsControl.Engine
{
    public static class Point2DExtensions
    {
        public static Point2D ToPoint2D(this Point point)
        {
            return new Point2D((int)point.X, (int)point.Y);
        }

        public static Point2D Offset(this Point2D point, Point2D offset)
        {
            return new Point2D(point.X + offset.X, point.Y + offset.Y);
        }

        public static Point2D Distance(this Point2D first, Point2D second)
        {
            return new Point2D(second.X - first.X, second.Y - first.Y);
        }
    }
}
