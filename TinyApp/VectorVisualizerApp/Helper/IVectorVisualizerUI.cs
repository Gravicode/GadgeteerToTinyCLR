
using System.Drawing;
using TinyCLR.LinesIn3D;

namespace VectorVisualizerApp
{
    public interface IVectorVisualizerUI
    {
        Brush AxisBrush { get; }
        Brush SubAxisBrush { get; }
        double AxisLength { get; }
        Point AxisCenter { get; }
        double AxisArrowSize { get; }
        double LetterShift { get; }
        double LetterWidth { get; }
        double LetterHeight { get; }
        double VectorFactor { get; }
        Brush NextColor { get; }

        void AddLine(Line2D line);
        void RemoveLine(Line2D line);
        void ResetColorIndex();

        Graphics Screen { set; get; }
    }
}
