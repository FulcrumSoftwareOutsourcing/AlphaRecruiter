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
	/// Common class for all application-specific exceptions.
	/// </summary>
	public class ExException : ApplicationException
	{
    //----------------------------------------------------------------------------
    public string DebugMessage = "";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
		public ExException()
		{
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExException(string message) : base(message)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExException(string message, string debugMessage) : base(message)
    {
      DebugMessage = debugMessage;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="innerException">exception that is the cause of the current exception</param>
    public ExException(string message, Exception innerException) : 
      base(message, innerException)
    {
    }
    //----------------------------------------------------------------------------
	}
}
