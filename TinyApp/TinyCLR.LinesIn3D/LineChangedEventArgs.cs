using System;

namespace TinyCLR.LinesIn3D
{
    public class LineChangedEventArgs //: EventArgs
    {
        public LineChangedEventArgs(Line2D line, bool isAdded)
        {
            this.Line = line;
            this.IsAdded = isAdded;
            this.IsRemoved = !isAdded;
        }

        public Line2D Line { get; private set; }
        public bool IsAdded { get; private set; }
        public bool IsRemoved { get; private set; }
    }
}