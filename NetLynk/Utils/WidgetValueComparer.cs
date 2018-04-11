namespace NetLynk.Utils
{
    using Types;
    using System.Collections.Generic;

    public class WidgetValueComparer : IEqualityComparer<Widget>
    {
        public bool Equals(Widget x, Widget y)
        {
            if (x.Value == y.Value)
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