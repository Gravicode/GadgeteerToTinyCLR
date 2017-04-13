using System;

namespace TinyCLR.LinesIn3D
{
    public static class Angle
    {
        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static double RadiansToDegrees(double radians)
        {
            return (180 / Math.PI) * radians;
        }

        public static int As180(int angle)
        {
            angle = angle % 360;

            angle = (angle < -180) ?
                        180 - Math.Abs(angle % 180)
                        : (angle > 180) ?
                            Math.Abs(angle % 180) - 180
                            : angle;
            return angle;
        }
    }
}