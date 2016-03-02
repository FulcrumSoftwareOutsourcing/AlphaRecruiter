using System;
using System.Configuration;

namespace Framework.Remote.Mobile
{
    public partial class CxExceptionDetails
    {
        //----------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the CxExceptionDetails class.
        /// </summary>
        /// <param name="exception">Exception, whose details need to send.</param>
        public CxExceptionDetails(Exception exception)
        {
            StackTrace = string.Empty;
            Message = exception.Message;
            Type = exception.GetType().Name;
            AsString = exception.ToString();

            bool showExceptionDetails =
                Convert.ToBoolean(ConfigurationManager.AppSettings["ShowExceptionDetails"].ToLower());
            if (showExceptionDetails)
            {
                StackTrace = exception.StackTrace;
            }

            if (exception.InnerException != null)
            {
                InnerException = new CxExceptionDetails(exception.InnerException);
            }
        }
    }
}
