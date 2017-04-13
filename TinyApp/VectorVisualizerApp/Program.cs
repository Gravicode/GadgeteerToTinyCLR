using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using TinyCLR.LinesIn3D;
using VectorVisualizerApp.Properties;

namespace VectorVisualizerApp
{
    public class Program
    {
        public static void Main()
        {
            var Lcd = new DisplayNHVN(FEZRaptor.I2cBus.I2c1, FEZRaptor.Socket16.Pin9, FEZRaptor.Socket13.Pin3, DisplayNHVN.DisplayTypes.Display7inch);
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            var mybrush = new SolidBrush(Color.Black);
            var pen = new Pen(mybrush);
            Lcd.Screen.Clear(Color.Black);
            Lcd.Screen.DrawString("Lets show some magic...", font, new SolidBrush(Color.White), 20, 400);
            Lcd.Screen.Flush();

            var page = new Page(Lcd.Screen);
            page.AddVector(1, 1, 1, 100, 1, 1);
            page.AddVector(1, 1, 1, 1, 100, 1);
            page.AddVector(100, 1, 1, 100, 100, 1);
            page.AddVector(100, 100, 1, 100, 1, 1);

            Thread.Sleep(2000);
            double Degree = 1;
            while (true)
            {
                page.SliderX = Degree;
                Degree += 0.5;
                if (Degree > 10) Degree = 1;
                Thread.Sleep(200);

            }
        }
    }

    public class Page : IVectorVisualizerUI
    {
        /// <summary>
        /// Used to slow down the movement
        /// </summary>
        private const double COEF_MOVE = 0.3;
        private const string VECTORS_CHANGE_JS_FUNC = "VV_ChangeVectors";

        private static readonly Brush _AxisDefaultBrush = new SolidBrush(Color.FromArgb(0xFF, 0x77, 0x77, 0x77));
        private static readonly Brush _SubAxisDefaultBrush = new SolidBrush(Color.FromArgb(0xFF, 0xE1, 0xE1, 0xE1));
        private const double _AxisDefaultLength = 216.0;
        private static readonly Point _AxisDefaultCenter = new Point(290, 280);
        private const double _AxisArrowSize = 10.0;
        private const double _LetterShift = 5.0;
        private const double _LetterWidth = 10.0;
        private const double _LetterHeight = 20.0;
        private const double _VectorFactor = 20.0;
        private const double _VectorLimit = 10.0;

        public double _SliderX;
        public double SliderX
        {
            set
            {
                _SliderX = value;
                this.Visualizer.Lines.ShiftDegreesAroundX(((_SliderX / 10) * 360 - 180) - this.Visualizer.Lines.RotationDegreesX);
                this.Visualizer.Lines.ReDraw();
                this.Visualizer.UpdateCurrentPage();
            }
            get
            {
                return _SliderX;
            }
        }
        public double _SliderY;
        public double SliderY
        {
            set
            {
                _SliderY = value;
                this.Visualizer.Lines.ShiftDegreesAroundY(((_SliderY / 10) * 360 - 180) - this.Visualizer.Lines.RotationDegreesY);
                this.Visualizer.Lines.ReDraw();
                this.Visualizer.UpdateCurrentPage();
            }
            get
            {
                return _SliderY;
            }
        }
        public double _SliderZ;
        public double SliderZ
        {
            set
            {
                _SliderZ = value;
                this.Visualizer.Lines.ShiftDegreesAroundZ(((_SliderZ / 10) * 360 - 180) - this.Visualizer.Lines.RotationDegreesZ);
                this.Visualizer.Lines.ReDraw();
                this.Visualizer.UpdateCurrentPage();
            }
            get
            {
                return _SliderZ;
            }
        }


        //private bool _isDragging;
        private bool _isOverSliderX;
        private bool _isOverSliderY;
        private bool _isOverSliderZ;
        //private Point _mouseLastPoint;
        //private string _currentPageUrl;

        private static readonly Color[] Colors =
            new[]
            {
                Color.FromArgb(0xFF,0x00,0x00,0x99),
                Color.FromArgb(0xFF,0x99,0x00,0x00),
                Color.FromArgb(0xFF,0x00,0x66,0x00),
                Color.FromArgb(0xFF,0x66,0x33,0x00),
                Color.FromArgb(0xFF,0x00,0x33,0x66),
                Color.FromArgb(0xFF,0x66,0x66,0x00),
                Color.FromArgb(0xFF,0x00,0x66,0x66),
                Color.FromArgb(0xFF,0x33,0x66,0x00),
                Color.FromArgb(0xFF,0x00,0x66,0x33),
                Color.FromArgb(0xFF,0x66,0x00,0x66),
                Color.FromArgb(0xFF,0x33,0x66,0x33),
                Color.FromArgb(0xFF,0x33,0x00,0x33),
                Color.FromArgb(0xFF,0x66,0x33,0x66),
                Color.FromArgb(0xFF,0x00,0x00,0x00)
            };

        private int _currentColorIndex = 0;

        public Brush AxisBrush { get { return _AxisDefaultBrush; } }
        public Brush SubAxisBrush { get { return _SubAxisDefaultBrush; } }
        public double AxisLength { get { return _AxisDefaultLength; } }
        public Point AxisCenter { get { return _AxisDefaultCenter; } }
        public double AxisArrowSize { get { return _AxisArrowSize; } }
        public double LetterShift { get { return _LetterShift; } }
        public double LetterWidth { get { return _LetterWidth; } }
        public double LetterHeight { get { return _LetterHeight; } }
        public double VectorFactor { get { return _VectorFactor; } }

      

        public VectorVisualizer Visualizer { get; private set; }

        public static Page Current;

        public Graphics Screen { set; get; }

        public Page(Graphics screen)
        {
            Current = this;

            Screen = screen;

            this.Visualizer = new VectorVisualizer(this);

            this.Visualizer.Lines.ReDrawn += this.Lines_ReDrawn;

            this.Visualizer.Initialize();

            //this.MouseLeftButtonDown += this.Page_MouseLeftButtonDown;
            //this.MouseLeftButtonUp += this.Page_MouseLeftButtonUp;
            //this.MouseLeave += this.Page_MouseLeave;
            //this.MouseMove += this.Page_MouseMove;
            //this.MouseWheel += this.Page_MouseWheel;
        }

        private void SetDefaultZoom(double  Delta)
        {
            var sign = Delta > 0 ? 1 : -1;
           
            this.Visualizer.Lines.DefaultZoom += 0.2 * sign;
            this.Visualizer.Lines.ReDraw();
        }

        private void Lines_ReDrawn(object sender, EventArgs e)
        {
            if (!this._isOverSliderX) this.SliderX = ((this.Visualizer.Lines.RotationDegreesX + 180) / 360D) * 10;
            if (!this._isOverSliderY) this.SliderY = ((this.Visualizer.Lines.RotationDegreesY + 180) / 360D) * 10;
            if (!this._isOverSliderZ) this.SliderZ = ((this.Visualizer.Lines.RotationDegreesZ + 180) / 360D) * 10;

            Debug.WriteLine($"DegreesX = {this.Visualizer.Lines.RotationDegreesX.ToString()}");
            Debug.WriteLine($"DegreesY = {this.Visualizer.Lines.RotationDegreesY.ToString()}");
            Debug.WriteLine($"DegreesZ = {this.Visualizer.Lines.RotationDegreesZ.ToString()}");
        }

        public void AddLine(Line2D line)
        {
            Debug.WriteLine($"line added : {line.ToString()}");
            //this.GraphContainer.Children.Add(line.LineUI);
        }
        public void RemoveLine(Line2D line)
        {
            Debug.WriteLine($"line remove : {line.ToString()}");
            //do nothingg
            /*
            if (line.HeadUI != null)
            {
                this.GraphContainer.Children.Remove(line.HeadUI);
            }
            this.GraphContainer.Children.Remove(line.LineUI);
        */
        }

        public void ResetColorIndex()
        {
            this._currentColorIndex = 0;
            //this.ColorRectangle.Fill = new SolidColorBrush(Colors[this._currentColorIndex]);
        }
        /*
        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this._isDragging = true;
            this._mouseLastPoint = e.GetPosition(this.GraphContainer);
        }

        private void Page_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.UndoMouseDown();
        }
        private void Page_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UndoMouseDown();
            this._isOverSliderX = this._isOverSliderY = this._isOverSliderZ = false;
        }

        private void UndoMouseDown()
        {
            this._isDragging = false;
        }


        private void Page_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this._isDragging)
            {
                return;
            }
            var mousePoint = e.GetPosition(this.GraphContainer);
            if (e.GetPosition(this.LayoutRoot).X > 600)
            {
                this._isDragging = false;
                return;
            }
            else
            {
                this._isOverSliderX = this._isOverSliderY = this._isOverSliderZ = false;
            }

            this.Visualizer.MouseMove(
                (mousePoint.X - this._mouseLastPoint.X) * COEF_MOVE,
                (mousePoint.Y - this._mouseLastPoint.Y) * COEF_MOVE
            );
            this._mouseLastPoint = mousePoint;
        }
        */
        private void Reset()
        {
            this.Visualizer.SetInitialPosition();
        }
        /*
        private void DegreesX_LostFocus(object sender, RoutedEventArgs e)
        {
            this.VerifyAndSetAxisValue(
                    sender as TextBox,
                    (degrees, visualizer) =>
                    {
                        visualizer.Lines.RotateToDegreesAroundX(degrees);
                        this.Visualizer.Lines.ReDraw();
                        return degrees.ToString();
                    });
        }

        private void DegreesY_LostFocus(object sender, RoutedEventArgs e)
        {
            this.VerifyAndSetAxisValue(
                    sender as TextBox,
                    (degrees, visualizer) =>
                    {
                        visualizer.Lines.RotateToDegreesAroundY(degrees);
                        this.Visualizer.Lines.ReDraw();
                        return degrees.ToString();
                    });
        }

        private void DegreesZ_LostFocus(object sender, RoutedEventArgs e)
        {
            this.VerifyAndSetAxisValue(
                    sender as TextBox,
                    (degrees, visualizer) =>
                    {
                        visualizer.Lines.RotateToDegreesAroundZ(degrees);
                        this.Visualizer.Lines.ReDraw();
                        return degrees.ToString();
                    });
        }

        private void DegreesX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DegreesX_LostFocus(sender, e);
            }
        }

        private void DegreesY_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DegreesY_LostFocus(sender, e);
            }
        }

        private void DegreesZ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DegreesZ_LostFocus(sender, e);
            }
        }
        


        private void VerifyAndSetAxisValue(TextBox textBox, Func<double, VectorVisualizer, string> func)
        {
            var text = textBox.Text.Trim().Replace(',', '.');
            if (text == String.Empty)
            {
                textBox.Text = func(0, this.Visualizer);
                textBox.Select(textBox.Text.Length, 0);
                return;
            }
            double degrees;
            if (Double.TryParse(text, out degrees))
            {
                degrees = degrees % 360;
                textBox.Text = func(degrees, this.Visualizer);
                textBox.Select(textBox.Text.Length, 0);
                return;
            }
            HtmlPage.Window.Alert(SR.PleaseEnterNumericValue);
            textBox.Text = func(0, this.Visualizer);
            textBox.Select(textBox.Text.Length, 0);
        }

        private void VerifyVectorValue(TextBox textBox)
        {
            var text = textBox.Text.Trim().Replace(',', '.');
            if (text == "-")
            {
                return;
            }
            if (text == String.Empty)
            {
                textBox.Text = 0.ToString();
                return;
            }
            double value;
            if (Double.TryParse(text, out value))
            {
                if (value < _VectorLimit * -1 || value > _VectorLimit)
                {
                    HtmlPage.Window.Alert(SR.TheValueMustBeBetween.Args(_VectorLimit));
                    textBox.Text = (value > 0 ? _VectorLimit : _VectorLimit * -1).ToString();
                }
                return;
            }
            HtmlPage.Window.Alert(SR.PleaseEnterNumericValue);
            textBox.Text = 0.ToString();
        }

        private void SliderX_MouseEnter(object sender, EventArgs e)
        {
            this._isOverSliderX = true;
        }

        private void SliderY_MouseEnter(object sender, EventArgs e)
        {
            this._isOverSliderY = true;
        }

        private void SliderZ_MouseEnter(object sender, EventArgs e)
        {
            this._isOverSliderZ = true;
        }

    
        
     

        private void Vector_TextChanged(object sender, EventArgs e)
        {
            this.VerifyVectorValue(sender as TextBox);
        }

        private void Vector_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Vector_TextChanged(sender, e);
            }
        }
        */
        public Brush NextColor
        {
            get
            {
                var brush = new SolidBrush(Colors[this._currentColorIndex]);
                this._currentColorIndex = (this._currentColorIndex == (Colors.Length - 1)) ? 0 : this._currentColorIndex + 1;
                //this.ColorRectangle.Fill = new SolidColorBrush(Colors[this._currentColorIndex]);
                return brush;
            }
        }
        

        public void AddVector(double VectorX1,double VectorY1, double VectorZ1, double VectorX2, double VectorY2, double VectorZ2)
        {
            this.ValidateAndAddVector(
                ParseVectorDouble(VectorX1),
                ParseVectorDouble(VectorY1),
                ParseVectorDouble(VectorZ1),
                ParseVectorDouble(VectorX2),
                ParseVectorDouble(VectorY2),
                ParseVectorDouble(VectorZ2)
            );
        }
        /*
        private void AddNullVectorBtn_Click(object sender, RoutedEventArgs e)
        {
            var x = ParseVectorDouble(this.NullVectorX.Text);
            var y = ParseVectorDouble(this.NullVectorY.Text);
            var z = ParseVectorDouble(this.NullVectorZ.Text);

            this.ValidateAndAddVector(x, y, z, x, y, z);
        }
        */
        private void ValidateAndAddVector(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            var vector = new VectorUI(_VectorFactor)
            {
                BeginningX = x1,
                BeginningY = y1,
                BeginningZ = z1,
                EndX = x2,
                EndY = y2,
                EndZ = z2,
            };
            if (vector.HasValuesExceeding(_VectorLimit))
            {
                Debug.WriteLine(SR.VectorCannotHaveValueBiggerThan.Args(vector, _VectorLimit, _VectorLimit * -1));
                return;
            }
            foreach(var item in this.Visualizer.Vectors)
            {
                var vUI = item as VectorUI;
                if(vUI.Equals(vector)){
                    Debug.WriteLine(SR.VectorIsAlreadyDefined.Args(vector));
                    return;
                }
            }
            vector.Brush = this.NextColor;
            this.AddVector(vector);
        }
        
        private void ClearVectors()
        {
            this.Visualizer.ClearVectors();
        }

        private void AddVector(VectorUI vector)
        {
            this.Visualizer.AddVector(vector);
        }

        private static double ParseVectorDouble(double d)
        {
            return (d < _VectorLimit * -1) ? _VectorLimit * -1 : (d > _VectorLimit) ? _VectorLimit : d;
            //return 0;
        }
        
    }
}
