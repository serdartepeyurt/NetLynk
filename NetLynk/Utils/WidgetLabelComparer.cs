namespace NetLynk.Utils
{
    using Types;
    using System.Collections.Generic;

    public class WidgetLabelComparer : IEqualityComparer<Widget>
    {
        public bool Equals(Widget x, Widget y)
        {
            if (x.Label == y.Label)
            {
                return true;
            }
            else { return false; }
        }

        public int GetHashCode(Widget codeh)
        {
            return 0;
        }
    }
}