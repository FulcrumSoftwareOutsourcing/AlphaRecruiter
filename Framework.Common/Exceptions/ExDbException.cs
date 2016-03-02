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
	/// Summary description for ExDbException.
	/// </summary>
	public class ExDbException : ExIncapsulatedException
	{
    //----------------------------------------------------------------------------
    protected string m_Sql = ""; // SQL statement that caused an exception
    protected object[] m_Parameters = null; // Array with parameter values
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="sql">SQL that caused an exception</param>
    public ExDbException(string message, string sql) : this(message, sql, null)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="sql">SQL that caused an exception</param>
    /// <param name="parameters">array with parameter values</param>
    public ExDbException(string message, string sql, object[] parameters) : base(message, null)
    {
      m_Sql = sql;
      if (parameters != null)
      {
        m_Parameters = new object[parameters.Length];
        parameters.CopyTo(m_Parameters, 0);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sql">SQL that caused an exception</param>
    /// <param name="innerException">exception that is the cause of the current exception</param>
    public ExDbException(string sql, Exception innerException) : 
      this(sql, null, innerException)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sql">SQL that caused an exception</param>
    /// <param name="parameters">array with parameter values</param>
    /// <param name="innerException">exception that is the cause of the current exception</param>
    public ExDbException(string sql, object[] parameters, Exception innerException) : 
      base(innerException.Message, innerException)
    {
      m_Sql = sql;
      if (parameters != null)
      {
        m_Parameters = new object[parameters.Length];
        parameters.CopyTo(m_Parameters, 0);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// SQL statement taht caused an exception.
    /// </summary>
    public string Sql 
    {
      get { return m_Sql; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Array with parameter values.
    /// </summary>
    public object[] Parameters
    {
      get { return m_Parameters; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// SQL statement with paremeters that caused this exception.
    /// </summary>
    public string Context
    {
      get { return CxUtils.ComposeSqlDescription(m_Sql, m_Parameters); }
    }
    //----------------------------------------------------------------------------
  }
}
