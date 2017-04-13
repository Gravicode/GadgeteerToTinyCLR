
using System;
using System.Drawing;

namespace TinyCLR.LinesIn3D
{
    public class Shape
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double ZIndex { get; set; }
    }
    public class Ellipse:Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush Fill { get; set; }
        public double FillOpacity { set; get; }
    }
    public class Line:Shape
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public Brush Stroke { get; set; }
        public double LineStroke { get; set; }
    }
    public class Line2D
    {
        private readonly Point3D _point1;
        private readonly Point3D _point2;

        public Line2D()
        {
            
            this.LineUI = new Line();

            this._point1 = new Point3D();
            this._point2 = new Point3D();
            
            this.ID = Guid.NewGuid().ToString();//SR.Line2D + this.GetHashCode();
        }

        public string ID { get; set; }

        public Line LineUI { get; private set; }
        public Ellipse HeadUI { get; protected set; }

        public Point3D Point1
        {
            get { return this._point1; }
            set { this.Point1.SetBy(value); }
        }
        public Point3D Point2 {
            get { return this._point2; }
            set { this.Point2.SetBy(value); }
        }

        public double X1
        {
            get { return this.LineUI.X1; }
            set { this.LineUI.X1 = value;}
        }
        public double X2
        {
            get { return this.LineUI.X2; }
            set { this.LineUI.X2 = value; }
        }
        public double Y1
        {
            get { return this.LineUI.Y1; }
            set { this.LineUI.Y1 = value; }
        }
        public double Y2
        {
            get { return this.LineUI.Y2; }
            set { this.LineUI.Y2 = value; }
        }
        public Brush Stroke
        {
            get { return this.LineUI.Stroke; }
            set
            {
                this.LineUI.Stroke = value;
                if(this.HeadUI != null)
                {
                    this.HeadUI.Fill = value;
                }
            }
        }

        public double LengthX
        {
            get { return this.Point2.X - this.Point1.X; }
        }
        public double LengthY
        {
            get { return this.Point2.Y - this.Point1.Y; }
        }
        public double LengthZ
        {
            get { return this.Point2.Z - this.Point1.Z; }
        }

        public double InitialHeadUiWidth { get; set; }

        private double _zoom;
        public double Zoom
        {
            get { return this._zoom; }
            set
            {
                if (value > 0.00001)
                {
                    this._zoom = value;
                }
            }
        }

        public void MapToPoints(double xOffset, double yOffset, double scaleCoef)
        {
            var scale = 1 - scaleCoef;
            var coef = (200 + scale*800) * -1;
            this.X1 = this.Point1.X * coef / ((this.Point1.Z + coef) / this.Zoom) + xOffset;
            this.Y1 = this.Point1.Y * coef / ((this.Point1.Z + coef) / this.Zoom) + yOffset;
            this.X2 = this.Point2.X * coef / ((this.Point2.Z + coef) / this.Zoom) + xOffset;
            this.Y2 = this.Point2.Y * coef / ((this.Point2.Z + coef) / this.Zoom) + yOffset;

            if (this.HeadUI == null) return;

            if (this.InitialHeadUiWidth != 0)
                this.HeadUI.Width = this.HeadUI.Height = this.InitialHeadUiWidth * this.Zoom;

            var zIndex = (int)this.Point2.Z + 350;
            this.LineUI.LineStroke = this.HeadUI.FillOpacity = zIndex > 350 ? 0.8 : zIndex > 270 ? 0.5 : zIndex > 200 ? 0.4 : zIndex > 100 ? 0.35 : 0.3;
            this.LineUI.ZIndex = zIndex;//SetValue(Canvas.ZIndexProperty, zIndex);
            this.HeadUI.ZIndex = zIndex;//SetValue(Canvas.ZIndexProperty, zIndex);
            this.HeadUI.Left = this.X2 - this.HeadUI.Width / 2; //SetValue(Canvas.LeftProperty, this.X2 - this.HeadUI.Width / 2);
            this.HeadUI.Top = this.Y2 - this.HeadUI.Height / 2;//SetValue(Canvas.TopProperty, this.Y2 - this.HeadUI.Height / 2);
        }

    }
}