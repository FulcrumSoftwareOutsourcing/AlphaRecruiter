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
using System.Text;

namespace Framework.Utils
{
	/// <summary>
	/// Exception to raise when validation failed.
	/// </summary>
  public class ExValidationException : ExPlannedException
  {
    //-------------------------------------------------------------------------
    protected string m_PropertyName = ""; // Name of the property that caused an exception
    protected string[] m_PropertyNames;
    protected string m_Stack = ""; // Error stack to show (before original one)
    //-------------------------------------------------------------------------
    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="propertyName">name of the property that caused an exception</param>
    public ExValidationException(string message, string propertyName)
      : base(message)
    {
      m_PropertyName = propertyName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="propertyName">name of the property that caused an exception</param>
    public ExValidationException(string message, string[] propertyNames)
      : base(message)
    {
      m_PropertyNames = propertyNames;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExValidationException(string message, string propertyName, string debugMessage) : 
      base(message, debugMessage)
    {
      m_PropertyName = propertyName;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExValidationException(string message) : this(message, "")
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="stack">error stack to show (before original one)</param>
    public ExValidationException(string message, StringBuilder stack) : this(message)
    {
      m_Stack = stack.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="innerException">exception that is the cause of the current exception</param>
    public ExValidationException(string message, Exception innerException) : 
      base(message, innerException)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Name of the property that caused an exception.
    /// </summary>
    public string PropertyName 
    {
      get { return m_PropertyName; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Names of the properties that caused the exception.
    /// </summary>
    public string[] PropertyNames
    {
      get { return m_PropertyNames; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns title for the error handler dialog.
    /// </summary>
    /// <returns>title for the error handler dialog</returns>
    override protected string GetTitle()
    {
      return "Validation Error";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns error stack to show (before original one).
    /// </summary>
    /// <returns>error stack to show (before original one)</returns>
    public string GetStack()
    {
      return m_Stack;
    }
    //----------------------------------------------------------------------------
  }
}
