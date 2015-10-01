using System.Collections.Generic;
using System.Text;

namespace System.SoftBytes.Exceptions
{
    public static class ExceptionServices
    {
        public static AggregateException Flatten(this Exception e)
        {
            List<Exception> innerExceptions = new List<Exception>();
            Flatten(e, ref innerExceptions);

            return new AggregateException(innerExceptions);
        }

        public static String GetMessageAndStackTrace(Exception e)
        {
            return GetMessageAndStackTrace(e.Flatten());
        }

        public static String GetMessageAndStackTrace(AggregateException e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e", "AggregateException can not be null.");
            }

            var sb = new StringBuilder();

            foreach (Exception innerException in e.InnerExceptions)
            {
                sb.AppendLine(innerException.Message);
                sb.AppendLine("\t" + innerException.StackTrace);
            }

            return sb.ToString();
        }

        private static void Flatten(Exception e, ref List<Exception> list)
        {
            list.Add(e);

            if (e.InnerException != null)
            {
                Flatten(e.InnerException, ref list);
            }
        }
    }
}
