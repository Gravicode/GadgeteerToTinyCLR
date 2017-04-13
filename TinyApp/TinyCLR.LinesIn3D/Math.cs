using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TinyCLR.LinesIn3D
{
    
    public class MathExt
    {

        public static double Round(double Input, int Digits = 2)
        {
            return (int)Input;
            /*
            if (Input == 0.0) return 0;
            int Multiplier = 1;
            for (int i = 0; i < Digits; ++i) Multiplier *= 10;
            string Rounded = ((int)(Input * Multiplier)).ToString();

            var roundedStr = (Rounded.Substring(0, Rounded.Length - 2) + "." + Rounded.Substring(Rounded.Length - 2)).TrimEnd(new char[] { '0', '.' });
            return double.Parse(roundedStr);
            */
        }

    }
}
