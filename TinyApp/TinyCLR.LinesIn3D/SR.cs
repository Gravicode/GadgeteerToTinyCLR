using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TinyCLR.LinesIn3D
{
    public class SR
    {
        public const string AddLineArrayLimit = "Array passed to LineGroup.AddLine cannot be NULL or have less than 3 elements";
        public const string Line2D = "Line2D";
        public const string LineWithSameIdExists = "Line with the same id '{0}' already exists";
        public const string ScaleCoefFrom0To1 = "scale coefficient passed to LineGroup.ReDraw(scaleCoef) must be between 0 and 1";

        public const string CopyAndPasteLink = "Copy and paste the link";
        public const string PageUrlCannotBeNullOrEmpty = "Page URL passed to IList<VectorUI>.ToLink(string) cannot be null or empty";
        public const string PleaseEnterNumericValue = "Please enter numeric value!";
        public const string TheValueMustBeBetween = "The value must be between -{0} and {0}!";
        public const string VectorCannotHaveValueBiggerThan = "Vector {0} cannot have values bigger that {1} or lower than {2}";
        public const string VectorIsAlreadyDefined = "Vector {0} is already defined";
        public const string VectorToStringTemplate = "(X1={0},Y1={1},Z1={2} : X2={3},Y2={4},Z2={5})";
    }
}
