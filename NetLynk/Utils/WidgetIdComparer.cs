namespace NetLynk.Utils
{
    using Types;
    using System.Collections.Generic;

    public class WidgetIdComparer : IEqualityComparer<Widget>
    {
        public bool Equals(Widget x, Widget y)
        {
            if (x.Id == y.Id)
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