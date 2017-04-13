using System;
using System.Drawing;
using TinyCLR.LinesIn3D;
//using Brush = System.Windows.Media.Brush;

namespace VectorVisualizerApp
{
    public class VectorUI : Line2D
    {
        private double _beginningX;
        private double _beginningY;
        private double _beginningZ;
        private double _endX;
        private double _endY;
        private double _endZ;
        

        public VectorUI(double factor)
        {
            this.Factor = factor;

            this.HeadUI = new Ellipse { Width = 5, Height = 5 };
            this.InitialHeadUiWidth = 5;
        }

        public double Factor { get; private set; }

        public bool IsZero { 
            get 
            { 
                return  this.BeginningX == this.EndX &&
                        this.BeginningY == this.EndY &&
                        this.BeginningZ == this.EndZ;
            }
        }

        public double BeginningX 
        {
            get { return this._beginningX; }
            set
            {
                this._beginningX = value;
                this.Point1.X = value * this.Factor;
            }
        }
        public double BeginningY
        {
            get { return this._beginningY; }
            set
            {
                this._beginningY = value;
                this.Point1.Y = value * this.Factor * -1;
            }
        }
        public double BeginningZ
        {
            get { return this._beginningZ; }
            set
            {
                this._beginningZ = value;
                this.Point1.Z = value * this.Factor;
            }
        }
        public double EndX
        {
            get { return this._endX; }
            set
            {
                this._endX = value;
                this.Point2.X = value * this.Factor;
            }
        }
        public double EndY
        {
            get { return this._endY; }
            set
            {
                this._endY = value;
                this.Point2.Y = value * this.Factor * -1;
            }
        }
        public double EndZ
        {
            get { return this._endZ; }
            set
            {
                this._endZ = value;
                this.Point2.Z = value * this.Factor;
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(
                            Math.Pow(this.EndX - this.BeginningX, 2) +
                            Math.Pow(this.EndY - this.BeginningY, 2) +
                            Math.Pow(this.EndZ - this.BeginningZ, 2)
                        );            
            }
        }

        public double AbsoluteLength
        {
            get
            {
                return Math.Sqrt(
                            Math.Pow(this.Point2.X - this.Point1.X, 2) +
                            Math.Pow(this.Point2.Y - this.Point1.Y, 2) +
                            Math.Pow(this.Point2.Z - this.Point1.Z, 2)
                        );
            }
        }

        public Brush Brush
        {
            get { return this.Stroke; }
            set
            {
                this.Stroke = value;
            }
        }


        public bool Equals(VectorUI vector)
        {
            return  this.BeginningX == vector.BeginningX && 
                    this.BeginningY == vector.BeginningY &&
                    this.BeginningZ == vector.BeginningZ &&
                    this.EndX == vector.EndX &&
                    this.EndY == vector.EndY &&
                    this.EndZ == vector.EndZ;
        }

        public override string ToString()
        {
            return SR.VectorToStringTemplate.Args(
                        this.BeginningX, this.BeginningY, this.BeginningZ, 
                        this.EndX, this.EndY, this.EndZ
                    );
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (VectorUI)) return false;
            return Equals((VectorUI)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _beginningX.GetHashCode();
                result = (result*397) ^ _beginningY.GetHashCode();
                result = (result*397) ^ _beginningZ.GetHashCode();
                result = (result*397) ^ _endX.GetHashCode();
                result = (result*397) ^ _endY.GetHashCode();
                result = (result*397) ^ _endZ.GetHashCode();
                result = (result*397) ^ Factor.GetHashCode();
                return result;
            }
        }

        public bool HasValuesExceeding(double limit)
        {
            return !IsWithin(this.BeginningX, limit) ||
                   !IsWithin(this.BeginningY, limit) ||
                   !IsWithin(this.BeginningZ, limit) ||
                   !IsWithin(this.EndX, limit) ||
                   !IsWithin(this.EndY, limit) ||
                   !IsWithin(this.EndZ, limit);
        }
        private static bool IsWithin(double value, double limit)
        {
            return value <= limit && value >= (limit*-1);
        }
    }
}
