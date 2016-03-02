/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace Framework.Remote
{
  /// <summary>
  /// Represents Exception that will be send to Client.
  /// </summary>
  [DataContract]
  public class CxExceptionDetails
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

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the inner Exception details. 
    /// </summary>
    [DataMember]
    public CxExceptionDetails InnerException { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the Exception message.
    /// </summary>
    [DataMember]
    public string Message { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the Exception Stack Trace.
    /// </summary>
    [DataMember]
    public string StackTrace { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the Exception class type name.
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Represents the exception casted as a string.
    /// </summary>
    [DataMember]
    public string AsString { get; set; }
  }
}
