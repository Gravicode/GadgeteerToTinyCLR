using System;
using System.Collections;
using System.Drawing;
using TinyCLR.LinesIn3D;

namespace VectorVisualizerApp
{
    public class VectorVisualizer
    {
        private readonly LineGroup _lines;

        public IVectorVisualizerUI Page { get; private set; }
        public LineGroup Lines { get { return this._lines; } }
        public IList AllLines { set; get; } 
        private readonly IList _vectors = new ArrayList();//<VectorUI>();


        public VectorVisualizer(IVectorVisualizerUI page)
        {
            AllLines = new ArrayList();   
            this.Page = page;
            this._lines = new LineGroup(AxisCenter);
            this.Lines.LineAdded += this.Lines_LineAdded;
            this.Lines.LineRemoved += this.Lines_LineRemoved;
           
        }

        public void Initialize()
        {
            this.DrawAxes();

            this.DrawSubAxes();

            this.SetInitialPosition();

            //this.SetVectorsPassedToQueryString();
        }

        public Brush AxisBrush { get { return this.Page.AxisBrush; } }
        public Brush SubAxisBrush { get { return this.Page.SubAxisBrush; } }
        public double AxisLength { get { return this.Page.AxisLength; } }
        public Point AxisCenter { get { return this.Page.AxisCenter; } }
        public double AxisArrowSize { get { return this.Page.AxisArrowSize; } }
        public double LetterShift { get { return this.Page.LetterShift; } }
        public double LetterWidth { get { return this.Page.LetterWidth; } }
        public double LetterHeight { get { return this.Page.LetterHeight; } }

        public IList Vectors { get { return this._vectors; } }//VectorUI
        public IEnumerable VectorLines {
            get
            {
                return Vectors as IEnumerable;
            }
        }
        public void SetInitialPosition()
        {

            this.Lines.RotateToDegrees(20, 40, -13);

            this.Lines.ReDraw();

            this.UpdateCurrentPage();
        }

        /*
        private void SetVectorsPassedToQueryString()
        {
            var pageUrl = HtmlPage.Document.DocumentUri.AbsoluteUri;
            var qMarkIndex = pageUrl.IndexOf('?');
            var vVarIndex = pageUrl.IndexOf("vectors=");
            if (qMarkIndex <= 0 || vVarIndex <= 0)
            {
                return;
            }
            var vVarEndIndex = pageUrl.IndexOf("&", vVarIndex + 1);
            if (vVarEndIndex < 0) vVarEndIndex = pageUrl.Length;
            var vectors = pageUrl.Substring(vVarIndex, vVarEndIndex - vVarIndex);
            var vectorsArray = vectors.Replace("vectors=", "").Split('v');
            for(var i = 0; i < vectorsArray.Length; i++)
            {
                var partsArray = vectorsArray[i].Split('/');

                if (partsArray.Length == 3)
                {
                    partsArray = new [] { "0", "0", "0", partsArray[0], partsArray[1], partsArray[2] };
                }
                else if(partsArray.Length < 6)
                {
                    continue;
                }
                double x1;
                double y1=0;
                double z1=0;
                double x2=0;
                double y2=0;
                double z2=0;
                if(
                    !Double.TryParse(partsArray[0], out x1) ||
                    !Double.TryParse(partsArray[1], out y1) ||
                    !Double.TryParse(partsArray[2], out z1) ||
                    !Double.TryParse(partsArray[3], out x2) ||
                    !Double.TryParse(partsArray[4], out y2) ||
                    !Double.TryParse(partsArray[5], out z2)
                  )
                {
                    continue;
                }
                x1 = EnsureRange(x1, this.Page.AxisArrowSize);
                y1 = EnsureRange(y1, this.Page.AxisArrowSize);
                z1 = EnsureRange(z1, this.Page.AxisArrowSize);
                x2 = EnsureRange(x2, this.Page.AxisArrowSize);
                y2 = EnsureRange(y2, this.Page.AxisArrowSize);
                z2 = EnsureRange(z2, this.Page.AxisArrowSize);
                var vector = new VectorUI(this.Page.VectorFactor)
                {
                    BeginningX = x1,
                    BeginningY = y1,
                    BeginningZ = z1,
                    EndX = x2,
                    EndY = y2,
                    EndZ = z2,
                    Brush = this.Page.NextColor
                };
                try
                {
                    this.AddVector(vector);
                }catch(Exception)
                {
                    continue;
                }
            }

        }*/

        private static double EnsureRange(double value, double limit)
        {
            if (value > limit) return limit;
            if (value < (-1*limit)) return -1*limit;
            return value;
        }

        public void AddVector(VectorUI vector)
        {
            foreach(var v in this.Vectors)
            {
                var item = v as VectorUI;
                if (item.Equals(vector))
                {
                    throw new Exception(SR.VectorIsAlreadyDefined.Args(vector));
                }
            }
            vector.ID = "Vector" + this.Vectors.Count;
            
            this.Vectors.Add(vector);

            this.Lines.AddLine(vector);

            this.UpdateCurrentPage();

            this.Lines.ReDraw();
        }

        public void UpdateCurrentPage()
        {
            var pen = new Pen(new SolidBrush(Color.White));
            Page.Screen.Clear(Color.Black);
            foreach(var item in AllLines)
            {
                var line = item as Line2D;
                Page.Screen.DrawLine(pen, (int)line.X1, (int)line.Y1, (int)line.X2, (int)line.Y2);
            }
            Page.Screen.Flush();
        }

        private void Lines_LineAdded(object sender, LineChangedEventArgs e)
        {
            this.Page.AddLine(e.Line);
            AllLines.Add(e.Line);
        }

        private void Lines_LineRemoved(object sender, LineChangedEventArgs e)
        {
            this.Page.RemoveLine(e.Line);
            AllLines.Remove(e.Line);
        }
        
        private void DrawAxes()
        {

            this.Lines.DefaultBrush = AxisBrush;

            // draw X axis
            this.Lines.AddLine("X0", 
                new []{0D, 0, 0}, 
                new []{AxisLength, 0, 0});
            // draw X axis arrow
            this.Lines.AddLine("X0_Arrow1",
                new[] { AxisLength, 0, 0 },
                new[] { AxisLength - AxisArrowSize, AxisArrowSize * -1, 0 });
            this.Lines.AddLine("X0_Arrow2", 
                new[] { AxisLength, 0, 0 },
                new[] { AxisLength - AxisArrowSize, AxisArrowSize, 0 });
            // draw X letter
            this.Lines.AddLine("X0_Letter1",
                new[] { AxisLength + LetterShift, AxisArrowSize * -1, 0 },
                new[] { AxisLength + LetterShift + LetterWidth, LetterHeight / 2D, 0 });
            this.Lines.AddLine("X0_Letter2",
                new[] { AxisLength + LetterShift, AxisArrowSize, 0 },
                new[] { AxisLength + LetterShift + LetterWidth, (LetterHeight / 2D) * -1, 0 });

            // draw Y axis
            this.Lines.AddLine("Y0", 
                new[] { 0D, 0, 0 }, 
                new[] { 0, AxisLength * -1, 0 });
            // draw Y axis arrow
            this.Lines.AddLine("Y0_Arrow1", 
                new[] { 0, AxisLength * -1, 0 },
                new[] { AxisArrowSize * -1, AxisLength * -1 + AxisArrowSize, 0 });
            this.Lines.AddLine("Y0_Arrow2", 
                new[] { 0, AxisLength * -1, 0 },
                new[] { AxisArrowSize, AxisLength * -1 + AxisArrowSize, 0 });
            // draw Y letter
            this.Lines.AddLine("Y0_Letter1",
                new[] { 0 + LetterWidth / 2, AxisLength * -1 - (LetterShift + LetterHeight), 0 },
                new[] { 0, AxisLength * -1 - (LetterShift + LetterHeight / 2), 0 });
            this.Lines.AddLine("Y0_Letter2",
                new[] { 0 - LetterWidth / 2, AxisLength * -1 - (LetterShift + LetterHeight), 0 },
                new[] { 0, AxisLength * -1 - (LetterShift + LetterHeight/2), 0 });
            this.Lines.AddLine("Y0_Letter3",
                new[] { 0, AxisLength * -1 - (LetterShift + LetterHeight / 2), 0 },
                new[] { 0, AxisLength * -1 - LetterShift, 0 });

            // draw Z axis
            this.Lines.AddLine("Z0", 
                new[] { 0D, 0, 0 }, 
                new[] { 0, 0, AxisLength });
            // draw Z axis arrow
            this.Lines.AddLine("Z0_Arrow1", 
                new[] { 0, 0, AxisLength },
                new[] { 0, AxisArrowSize * -1, AxisLength - AxisArrowSize });
            this.Lines.AddLine("Z0_Arrow2", 
                new[] { 0, 0, AxisLength },
                new[] { 0, AxisArrowSize, AxisLength - AxisArrowSize });
            // draw Z letter
            this.Lines.AddLine("Z0_Letter1",
                new[] { 0, 0 - LetterHeight / 2, AxisLength + (LetterShift + LetterWidth) },
                new[] { 0, 0 - LetterHeight / 2, AxisLength + LetterShift });
            this.Lines.AddLine("Z0_Letter2",
                new[] { 0, 0 - LetterHeight / 2, AxisLength + LetterShift },
                new[] { 0, 0 + LetterHeight / 2, AxisLength + (LetterShift + LetterWidth) });
            this.Lines.AddLine("Z0_Letter3",
                new[] { 0, 0 + LetterHeight / 2, AxisLength + (LetterShift + LetterWidth) },
                new[] { 0, 0 + LetterHeight / 2, AxisLength + LetterShift });

        }

        private void DrawSubAxes()
        {
            this.Lines.DefaultBrush = SubAxisBrush;

            this.Lines.AddLine("X-0",
                new[] { AxisLength * -1, 0, 0 },
                new[] { 0D, 0, 0 });

            this.Lines.AddLine("Y-0",
                new[] { 0D, AxisLength, 0 },
                new[] { 0D, 0, 0 });

            this.Lines.AddLine("Z-0",
                new[] { 0D, 0, AxisLength * -1 },
                new[] { 0D, 0, 0 });

            var step = 20;
            var axisLength = (AxisLength - AxisArrowSize - LetterShift);

            for (var i = 0; (i + step) < axisLength; i += step)
            {
                // sub axes starting from X axes
                this.Lines.AddLine("XY" + ((i / step) + 1),
                    new[] { i + step, 0, 0D },
                    new[] { i + step, axisLength * -1, 0D });

                this.Lines.AddLine("XZ" + ((i / step) + 1),
                    new[] { i + step, 0, 0D },
                    new[] { i + step, 0, axisLength });

                // sub axes starting from Y axes
                this.Lines.AddLine("YX" + ((i / step) + 1),
                    new[] { 0, i * -1 - step, 0D },
                    new[] { axisLength, i * -1 - step, 0D });

                this.Lines.AddLine("YZ" + ((i / step) + 1),
                    new[] { 0D, i * -1 - step, 0D },
                    new[] { 0D, i * -1 - step, axisLength });

                // sub axes starting from Z axes
                this.Lines.AddLine("ZX" + ((i / step) + 1),
                    new[] { 0D, 0, i + step },
                    new[] { 0D, axisLength * -1, i + step });

                this.Lines.AddLine("ZY" + ((i / step) + 1),
                    new[] { 0D, 0D, i + step },
                    new[] { axisLength, 0D, i + step });
            }
        }




        public void MouseMove(double xChange, double yChange)
        {
            // here X is Y and Y is X because moving along X rotates around Y and moving along Y rotates around X
            // Y rotation is multiplied by -1 because 0,0 point on the screen is at the top and not at the bottom
            this.Lines.ShiftDegrees(yChange, xChange * -1, 0);
            this.Lines.ReDraw();
        }


        public void ClearVectors()
        {
            foreach(var x in this.VectorLines)
            {
                var line = x as Line2D;
                this.Lines.RemoveLine(line);
            }
            this.Vectors.Clear();
            this.Page.ResetColorIndex();
            this.UpdateCurrentPage();
        }
    }
}
