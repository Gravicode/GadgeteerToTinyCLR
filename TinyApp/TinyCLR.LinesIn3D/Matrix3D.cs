using System;

namespace TinyCLR.LinesIn3D
{
    public class Matrix3D
    {

        private readonly double[][] _matrix = new double[3][];

        public Matrix3D()
        {
            for(int i = 0; i < 3; i++)
            {
                _matrix[i] = new double[3];
            }
            this.MakeIdentity();
        }

        public Matrix3D(double xRadians, double yRadians, double zRadians)
        {
            this.MakeIdentity();
            this.SetBy(NewRotate(xRadians, yRadians, zRadians));
        }

        public Matrix3D MakeIdentity()
        {
            this._matrix[0][0] = this._matrix[1][ 1] = this._matrix[2][2] = 1;
            this._matrix[0][1] = this._matrix[0][2] =
                                 this._matrix[1][0] = this._matrix[1][2] =
                                                      this._matrix[2][0] = this._matrix[2][1] = 0;
            return this;
        }

        public void SetBy(Matrix3D matrix)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    this._matrix[i][j] = matrix._matrix[i][j];
                }
            }
        }

        public static Matrix3D NewRotateAroundX(double radians)
        {
            var matrix = new Matrix3D();
            matrix._matrix[1][1] = Math.Cos(radians);
            matrix._matrix[1][2] = Math.Sin(radians);
            matrix._matrix[2][1] = -(Math.Sin(radians));
            matrix._matrix[2][2] = Math.Cos(radians);
            return matrix;
        }
        public static Matrix3D NewRotateAroundY(double radians)
        {
            var matrix = new Matrix3D();
            matrix._matrix[0][0] = Math.Cos(radians);
            matrix._matrix[0][2] = -(Math.Sin(radians));
            matrix._matrix[2][0] = Math.Sin(radians);
            matrix._matrix[2][2] = Math.Cos(radians);
            return matrix;
        }
        public static Matrix3D NewRotateAroundZ(double radians)
        {
            var matrix = new Matrix3D();
            matrix._matrix[0][0] = Math.Cos(radians);
            matrix._matrix[0][1] = Math.Sin(radians);
            matrix._matrix[1][0] = -(Math.Sin(radians));
            matrix._matrix[1][1] = Math.Cos(radians);
            return matrix;
        }

        public static Matrix3D NewRotate(double radiansX, double radiansY, double radiansZ)
        {
            var matrix = NewRotateAroundX(radiansX);
            matrix = matrix * NewRotateAroundY(radiansY);
            matrix = matrix * NewRotateAroundZ(radiansZ);
            return matrix;
        }

        public static Matrix3D NewRotateByDegrees(double degreesX, double degreesY, double degreesZ)
        {
            return NewRotate(
                        Angle.DegreesToRadians(degreesX), 
                        Angle.DegreesToRadians(degreesY), 
                        Angle.DegreesToRadians(degreesZ)
                   );
        }

        public static Matrix3D NewRotateFromDegreesAroundX(double degrees)
        {
            return NewRotateAroundX(Angle.DegreesToRadians(degrees));
        }
        public static Matrix3D NewRotateFromDegreesAroundY(double degrees)
        {
            return NewRotateAroundY(Angle.DegreesToRadians(degrees));
        }
        public static Matrix3D NewRotateFromDegreesAroundZ(double degrees)
        {
            return NewRotateAroundZ(Angle.DegreesToRadians(degrees));
        }

        public static Matrix3D operator *(Matrix3D matrix1, Matrix3D matrix2)
        {
            var matrix = new Matrix3D();
            for(var i = 0; i < 3; i++)
            {
                for(var j = 0; j < 3; j++)
                {
                    matrix._matrix[i][j] = 
                        (matrix2._matrix[i][0] * matrix1._matrix[0][j]) +
                        (matrix2._matrix[i][1] * matrix1._matrix[1][j]) +
                        (matrix2._matrix[i][2] * matrix1._matrix[2][j]);
                }
            }
            return matrix;
        }

        public static Point3D operator *(Matrix3D matrix1, Point3D point3D)
        {
            var x = point3D.InitialX * matrix1._matrix[0][ 0] +
                    point3D.InitialY * matrix1._matrix[0][ 1] +
                    point3D.InitialZ * matrix1._matrix[0][ 2];
            var y = point3D.InitialX * matrix1._matrix[1][ 0] +
                    point3D.InitialY * matrix1._matrix[1][ 1] +
                    point3D.InitialZ * matrix1._matrix[1][ 2];
            var z = point3D.InitialX * matrix1._matrix[2][ 0] +
                    point3D.InitialY * matrix1._matrix[2][ 1] +
                    point3D.InitialZ * matrix1._matrix[2][2];
            point3D.X = x;
            point3D.Y = y;
            point3D.Z = z;
            return point3D;
        }

    }
}