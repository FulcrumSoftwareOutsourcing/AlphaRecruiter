/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;

namespace Framework.Utils
{
	/// <summary>
	/// Summary description for ExWebServiceException.
	/// </summary>
	public class ExWebServiceException : ExException
	{
    //-------------------------------------------------------------------------
    protected Exception m_WebServiceException = null;
    protected string m_WebServiceStackTrace = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public ExWebServiceException(
      string message, 
      Exception innerException) : base(message, innerException)
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public ExWebServiceException(
      string message, 
      Exception webServiceException,
      string webServiceStackTrace,
      Exception innerException) : this(message, innerException)
    {
      m_WebServiceException = webServiceException;
      m_WebServiceStackTrace = webServiceStackTrace;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web service exception.
    /// </summary>
    public Exception WebServiceException
    { get { return m_WebServiceException; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web service full stack trace.
    /// </summary>
    public string WebServiceStackTrace
    { get { return m_WebServiceStackTrace; } }
    //-------------------------------------------------------------------------
  }
}