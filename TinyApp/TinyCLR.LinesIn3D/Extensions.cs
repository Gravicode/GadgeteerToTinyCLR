using System;
using System.Collections;

namespace TinyCLR.LinesIn3D
{
    public static class Extensions
    {
        #region String
        public static string Args(this string str, params object[] args)
        {
            return String.Format(str, args);
        }
        #endregion

        #region IEnumerable<Line2D>
        public static void MapToPoints(this IList lines, double xOffset, double yOffset, double scaleCoef, double zoom)
        {
            foreach (var x in lines)
            {
                var line = x as Line2D;
                line.Zoom = zoom;
                line.MapToPoints(xOffset, yOffset, scaleCoef);
            }
        }

        public static void MultiplyInitialPointsBy(this IList lines, Matrix3D matrix)
        {
            foreach (var x in lines)
            {
                var line = x as Line2D;
                line.Point1.SetBy(matrix * line.Point1);
                line.Point2.SetBy(matrix * line.Point2);
            }
        }
        #endregion
    }
}