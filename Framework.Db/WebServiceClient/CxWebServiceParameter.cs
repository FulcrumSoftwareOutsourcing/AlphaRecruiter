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
using System.Data;
using System.Data.SqlClient;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Class describing web service client parameter
	/// </summary>
	public class CxWebServiceParameter : IDbDataParameter
	{
    //-------------------------------------------------------------------------
    protected DbType m_DbType = DbType.String;
    protected ParameterDirection m_Direction = ParameterDirection.Input;
    protected bool m_IsNullable = true;
    protected string m_ParameterName = null;
    protected string m_SourceColumn = null;
    protected DataRowVersion m_SourceVersion = DataRowVersion.Current;
    protected object m_Value = null;
    protected byte m_Precision = 0;
    protected byte m_Scale = 0;
    protected int m_Size = 0;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceParameter()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceParameter(string name, DbType dbType)
    {
      m_ParameterName = name;
      m_DbType = dbType;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceParameter(string name, object value)
    {
      m_ParameterName = name;
      Value = value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceParameter(
      string name, 
      DbType dbType, 
      string sourceColumn) : this(name, dbType)
    {
      m_SourceColumn = sourceColumn;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter database type.
    /// </summary>
    public DbType DbType
	  {
	    get { return m_DbType; }
	    set { m_DbType = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter direction.
    /// </summary>
    public ParameterDirection Direction
	  {
	    get { return m_Direction; }
	    set { m_Direction = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets nullable flag.
    /// </summary>
    public bool IsNullable
	  {
	    get { return m_IsNullable; }
      set { m_IsNullable = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter name.
    /// </summary>
    public string ParameterName
	  {
	    get { return m_ParameterName; }
	    set { m_ParameterName = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets source column.
    /// </summary>
    public string SourceColumn
	  {
	    get { return m_SourceColumn; }
	    set { m_SourceColumn = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets source row version.
    /// </summary>
    public DataRowVersion SourceVersion
	  {
	    get { return m_SourceVersion; }
	    set { m_SourceVersion = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public object Value
	  {
	    get 
      { 
        return m_Value; 
      }
	    set 
      { 
        if (m_Value != value && value != null && value != DBNull.Value)
        {
          SqlParameter param = new SqlParameter("NewParameter", value);
          m_DbType = param.DbType;
          m_Precision = param.Precision;
          m_Scale = param.Scale;
        }
        m_Value = value;
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets precision.
    /// </summary>
	  public byte Precision
	  {
	    get { return m_Precision; }
	    set { m_Precision = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets scale.
    /// </summary>
	  public byte Scale
	  {
	    get { return m_Scale; }
	    set { m_Scale = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets size.
    /// </summary>
    public int Size
	  {
	    get { return m_Size; }
	    set { m_Size = value; }
	  }
    //-------------------------------------------------------------------------
  }
}