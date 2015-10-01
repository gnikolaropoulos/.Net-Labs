//-------------------------------------------------------------------------------------------------
// Code from Nikos Baxevanis http://nikobaxevanis.com/
//-------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;

namespace System.SoftBytes.Globalization
{
    public static class GlobalizationServices
    {
        public static void InvokeWithInvariantCulture(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "Action can not be null.");
            }

            CultureInfo capturedCulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            action.Invoke();

            Thread.CurrentThread.CurrentCulture = capturedCulture;
        }
    }
}
