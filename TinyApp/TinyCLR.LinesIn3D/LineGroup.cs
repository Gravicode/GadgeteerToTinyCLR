using System;
using System.Collections;
using System.Drawing;

namespace TinyCLR.LinesIn3D
{
    public class Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class EventArgs
    {
        public object data { set; get; }
        public static EventArgs Empty { get
            {
                return new EventArgs();
            }
        }
    }
    public class IndexData
    {
        public IndexData(string Key,Line2D Data)
        {
            this.Key = Key;
            this.Data = Data;
        }
        public IndexData()
        {

        }
        public string Key { get; set; }
        public Line2D Data { get; set; }
    }

    public class IndexHelper
    {
        public static bool TryGetValue(IList data, string Key,out Line2D obj)
        {
            foreach(var item in data)
            {
                var x = item as IndexData;
                if (x.Key == Key) { obj = x.Data; return true; }
            }
            obj = null;
            return false;
        }
    }
    public class LineGroup
    {
        private readonly IList _lines = new ArrayList();//<Line2D>();
        Hashtable hash1 = new Hashtable();
      
        private readonly IList _index = new ArrayList();//<string, Line2D>
       
        private Matrix3D _currentMatrix = new Matrix3D();

        private bool _hasChange = true;

        public delegate void LineEventHandler(object Sender, LineChangedEventArgs e);
        public delegate void GenericEventHandler(object Sender, EventArgs e);


        public event LineEventHandler LineAdded;
        public event LineEventHandler LineRemoved;
        public event GenericEventHandler ReDrawn;


        public LineGroup(Point centerPoint)
        {
            this.CenterPoint = centerPoint;
            this.DefaultBrush = new SolidBrush(Color.FromArgb(0xFF, 0x0, 0x0, 0x0));
            this.DefaultScaleCoef = 0.15;
            this.DefaultZoom = 1;
        }

        public Point CenterPoint { get; private set; }

        public int RotationDegreesX { get; private set; }
        public int RotationDegreesY { get; private set; }
        public int RotationDegreesZ { get; private set; }

        public Brush DefaultBrush { get; set; }
        public double DefaultScaleCoef { get; set; }

        private double _defaultZoom;
        public double DefaultZoom
        {
            get { return this._defaultZoom; }
            set
            {
                if (value != this._defaultZoom && value > 0.8 && value < 3.0)
                {
                    this._hasChange = true;
                    this._defaultZoom = value;
                }
            }
        }


        public void AddLine(string id, int[] array1, int[] array2)
        {
            this.AddLine(id, array1, array2, this.DefaultBrush);
        }
        public void AddLine(string id, int[] array1, int[] array2, Brush brush)
        {
            if (array1 == null || array1.Length < 3) { throw new ArgumentException(SR.AddLineArrayLimit); }
            if (array2 == null || array2.Length < 3) { throw new ArgumentException(SR.AddLineArrayLimit); }
            this.AddLine(id, array1[0], array1[1], array1[2], array2[0], array2[1], array2[2], brush);
        }
        public void AddLine(string id, double[] array1, double[] array2)
        {
            this.AddLine(id, array1, array2, this.DefaultBrush);
        }
        public void AddLine(string id, double[] array1, double[] array2, Brush brush)
        {
            if (array1 == null || array1.Length < 3) { throw new ArgumentException(SR.AddLineArrayLimit); }
            if (array2 == null || array2.Length < 3) { throw new ArgumentException(SR.AddLineArrayLimit); }
            this.AddLine(id, array1[0], array1[1], array1[2], array2[0], array2[1], array2[2], brush);
        }
        public void AddLine(string id, double x1, double y1, double z1, double x2, double y2, double z2)
        {
            this.AddLine(id, x1, y1, z1, x2, y2, z2, this.DefaultBrush);
        }
        public void AddLine(string id, double x1, double y1, double z1, double x2, double y2, double z2, Brush brush)
        {
            this.AddLine(id, new Point3D(x1,y1,z1), new Point3D(x2, y2, z2), brush);
        }
        public void AddLine(string id, Point3D point1, Point3D point2)
        {
            this.AddLine(id, point1, point2, this.DefaultBrush);
        }



        public void AddLine(string id, Point3D point1, Point3D point2, Brush brush)
        {
            this.AddLine(new Line2D
                             {
                                 ID = id,
                                 Point1 = point1,
                                 Point2 = point2,
                                 Stroke = brush
                             });
        }
        public void AddLine(Line2D line)
        {
            Line2D same;
            if (IndexHelper.TryGetValue(this._index, line.ID, out same))
            {
                throw new Exception(SR.LineWithSameIdExists.Args(line.ID));
            }
            this._index.Add( new IndexData(line.ID, line));
            this._lines.Add(line);

            if (this.RotationDegreesX != 0 || this.RotationDegreesY != 0 || this.RotationDegreesZ != 0)
            {
                line.Point1.SetBy(
                        this._currentMatrix * line.Point1
                    );
                line.Point2.SetBy(
                        this._currentMatrix * line.Point2
                    );
            }
            this._hasChange = true;
            this.OnLineAdded(new LineChangedEventArgs(line, true));
        }
        public void RemoveLine(Line2D line)
        {
            this._index.Remove(line.ID);
            this._lines.Remove(line);
            this.OnLineRemoved(new LineChangedEventArgs(line, false));
        }


        public void RotateToDegrees(double xDegrees, double yDegrees, double zDegrees)
        {
            var xCandidate = Angle.As180((int)MathExt.Round(xDegrees, 0));
            var yCandidate = Angle.As180((int)MathExt.Round(yDegrees, 0));
            var zCandidate = Angle.As180((int)MathExt.Round(zDegrees, 0));

            if(
                xCandidate == this.RotationDegreesX &&
                yCandidate == this.RotationDegreesY &&
                zCandidate == this.RotationDegreesZ
            ){
                return;   
            }
            this._hasChange = true;
            this.RotationDegreesX = xCandidate;
            this.RotationDegreesY = yCandidate;
            this.RotationDegreesZ = zCandidate;

            this.ReplaceMatrix( 
                Matrix3D.NewRotateByDegrees(this.RotationDegreesX, this.RotationDegreesY, this.RotationDegreesZ) 
            );
        }

        public void RotateToDegreesAroundX(double degrees)
        {
            var candidate = Angle.As180((int)MathExt.Round(degrees, 0));
            if (candidate == this.RotationDegreesX) { return; }
            this._hasChange = true;
            this.RotationDegreesX = candidate;
            this.ReplaceMatrix(
                Matrix3D.NewRotateByDegrees(this.RotationDegreesX, this.RotationDegreesY, this.RotationDegreesZ)
            );
        }

        public void RotateToDegreesAroundY(double degrees)
        {
            var candidate = Angle.As180((int)MathExt.Round(degrees, 0));
            if (candidate == this.RotationDegreesY) { return; }
            this._hasChange = true;
            this.RotationDegreesY = candidate;
            this.ReplaceMatrix(
                Matrix3D.NewRotateByDegrees(this.RotationDegreesX, this.RotationDegreesY, this.RotationDegreesZ)
            );
            
        }

        public void RotateToDegreesAroundZ(double degrees)
        {
            var candidate = Angle.As180((int)MathExt.Round(degrees, 0));
            if (candidate == this.RotationDegreesZ) { return; }
            this._hasChange = true;
            this.RotationDegreesZ = candidate;
            this.ReplaceMatrix(
                Matrix3D.NewRotateByDegrees(this.RotationDegreesX, this.RotationDegreesY, this.RotationDegreesZ)
            );
        }

        public void ShiftDegrees(double xShiftDegrees, double yShiftDegrees, double zShiftDegrees)
        {
            var xCandidate = Angle.As180(this.RotationDegreesX + (int)MathExt.Round(xShiftDegrees, 0));
            var yCandidate = Angle.As180(this.RotationDegreesY + (int)MathExt.Round(yShiftDegrees, 0));
            var zCandidate = Angle.As180(this.RotationDegreesZ + (int)MathExt.Round(zShiftDegrees, 0));

            if (
                xCandidate == this.RotationDegreesX &&
                yCandidate == this.RotationDegreesY &&
                zCandidate == this.RotationDegreesZ
            )
            {
                return;
            }
            this._hasChange = true;
            this.RotationDegreesX = xCandidate;
            this.RotationDegreesY = yCandidate;
            this.RotationDegreesZ = zCandidate;

            this.MultiplyMatrix(
                Matrix3D.NewRotateByDegrees(xShiftDegrees, yShiftDegrees, zShiftDegrees)
            );
        }

        public void ShiftDegreesAroundX(double degreesShift)
        {
            var candidate = Angle.As180(this.RotationDegreesX + (int)MathExt.Round(degreesShift, 0));
            if (candidate == this.RotationDegreesX) { return; }
            this._hasChange = true;
            this.RotationDegreesX = candidate;
            this.MultiplyMatrix(
                Matrix3D.NewRotateFromDegreesAroundX(degreesShift)
            );
        }

        public void ShiftDegreesAroundY(double degreesShift)
        {
            var candidate = Angle.As180(this.RotationDegreesY + (int)MathExt.Round(degreesShift, 0));
            if (candidate == this.RotationDegreesY) { return; }
            this._hasChange = true;
            this.RotationDegreesY = candidate;
            this.MultiplyMatrix(
                Matrix3D.NewRotateFromDegreesAroundY(degreesShift)
            );
        }

        public void ShiftDegreesAroundZ(double degreesShift)
        {
            var candidate = Angle.As180(this.RotationDegreesZ + (int)MathExt.Round(degreesShift, 0));
            if (candidate == this.RotationDegreesZ) { return; }
            this._hasChange = true;
            this.RotationDegreesZ = candidate;
            this.MultiplyMatrix(
                Matrix3D.NewRotateFromDegreesAroundZ(degreesShift)
            );
        }

        public void MultiplyMatrix(Matrix3D matrix)
        {
            this._currentMatrix = this._currentMatrix * matrix;
            this._lines.MultiplyInitialPointsBy(this._currentMatrix);
        }

        public void ReplaceMatrix(Matrix3D matrix)
        {
            this._currentMatrix = matrix;
            this._lines.MultiplyInitialPointsBy(this._currentMatrix);
        }

        public void Move(double x, double y)
        {
            this.CenterPoint = new Point(x, y);
            this.ReDraw();
        }


        public void ReDraw()
        {
            this.ReDraw(this.DefaultScaleCoef);
        }

        public void ReDraw(double scaleCoef)
        {
            this.ReDraw(scaleCoef, this.DefaultZoom);
        }

        public void ReDraw(double scaleCoef, double zoom)
        {
            if (!this._hasChange)
            {
                return;
            }
            if (scaleCoef < 0 || scaleCoef > 1)
            {
                throw new ArgumentException(SR.ScaleCoefFrom0To1);
            }
            this._lines.MapToPoints(this.CenterPoint.X, this.CenterPoint.Y, scaleCoef, zoom);
            this._hasChange = false;
            this.OnReDrawn(EventArgs.Empty);
        }



        protected virtual void OnLineAdded(LineChangedEventArgs args)
        {
            if(this.LineAdded != null)
            {
                this.LineAdded(this, args);
            }
        }

        protected virtual void OnLineRemoved(LineChangedEventArgs args)
        {
            if (this.LineRemoved != null)
            {
                this.LineRemoved(this, args);
            }
        }

        protected virtual void OnReDrawn(EventArgs args)
        {
            if (this.ReDrawn != null)
            {
                this.ReDrawn(this, args);
            }
        }


        
    }
}