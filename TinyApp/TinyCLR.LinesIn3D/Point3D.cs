namespace TinyCLR.LinesIn3D
{
    public class Point3D
    {
        private double _x;
        private double _y;
        private double _z;
        private bool _xIsDefined;
        private bool _yIsDefined;
        private bool _zIsDefined;

        public Point3D()
        {
        }
        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double InitialX { get; private set; }
        public double InitialY { get; private set; }
        public double InitialZ { get; private set; }

        public double X 
        {
            get { return this._x; }
            set
            {
                if (!this._xIsDefined)
                {
                    this._xIsDefined = true;
                    this.InitialX = value;
                }
                this._x = value;
            }
        }
        public double Y
        {
            get { return this._y; }
            set
            {
                if (!this._yIsDefined)
                {
                    this._yIsDefined = true;
                    this.InitialY = value;
                }
                this._y = value;
            }
        }
        public double Z
        {
            get { return this._z; }
            set
            {
                if (!this._zIsDefined)
                {
                    this._zIsDefined = true;
                    this.InitialZ = value;
                }
                this._z = value;
            }
        }
        public Point3D SetBy(Point3D point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            return this;
        }

        public void ResetToInitial()
        {
            this.X = this.InitialX;
            this.Y = this.InitialY;
            this.Z = this.InitialZ;
        }
    }
}