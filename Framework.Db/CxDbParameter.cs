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
using System.Data.OracleClient;
using System.Data.SqlClient;

using Framework.Utils;

namespace Framework.Db
{
	/// <summary>
  /// Class to encapsulate database parameter.
  /// </summary>
	public class CxDbParameter
	{
    //----------------------------------------------------------------------------
    protected IDataParameter m_Parameter = null; // Database parameter this class encapsulates
    protected CxDbCommand m_Command = null; // Command this parameter used in
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parameter">ADO.NET parameter this class encapsulates</param>
    protected internal CxDbParameter(IDataParameter parameter)
    {
      m_Parameter = parameter;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns database parameter this class encapsulates.
    /// </summary>
    public IDataParameter Parameter
    {
      get { return m_Parameter; }
      set { m_Parameter = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Command this parameter used in.
    /// </summary>
    public CxDbCommand Command 
    {
      get { return m_Command; }
      set { m_Command = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if parameter value is null.
    /// </summary>
    public bool IsNull
    {
      get 
      { 
        return CxUtils.IsNull(Value) || 
               (Value is System.Data.OracleClient.OracleLob) && ((System.Data.OracleClient.OracleLob) Value).IsNull;  
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value.
    /// </summary>
    public object Value
    {
      get { return (m_Parameter.Value == DBNull.Value ? null : m_Parameter.Value); }
      set 
      { 
        m_Parameter.Value = (value == null || (value is string && (string) value == "") ? 
                             DBNull.Value : value);
        if (DataType == DbType.String && this.Direction == ParameterDirection.Output && m_Parameter is SqlParameter)
        {
          ((SqlParameter) m_Parameter).Size = 4000;
        }
        else if (value is string && CxUtils.NotEmpty(value) && m_Parameter is SqlParameter)
        {
          ((SqlParameter) m_Parameter).Size = ((string) value).Length;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as string.
    /// </summary>
    public string ValueAsString
    {
      get 
      { 
        return 
         (IsNull             ? "" : 
          Value is OracleLob ? (string) ((OracleLob) Value).Value : 
                               Value.ToString());
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as integer.
    /// </summary>
    public int ValueAsInt
    {
      get { return (IsNull ? 0 : Convert.ToInt32(Value)); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as decimal.
    /// </summary>
    public decimal ValueAsDecimal
    {
      get { return (IsNull ? 0 : Convert.ToDecimal(Value)); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as double.
    /// </summary>
    public double ValueAsDouble
    {
      get { return (IsNull ? 0 : Convert.ToDouble(Value)); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as datetime.
    /// </summary>
    public DateTime ValueAsDateTime
    {
      get { return (IsNull ? DateTime.MinValue : (DateTime) Value); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as boolean.
    /// </summary>
    public bool ValueAsBool
    {
      get { return (IsNull ? false : (bool) Value); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter value as boolean.
    /// </summary>
    public Byte[] ValueAsBytes
    {
      get 
      { 
        return 
         (IsNull                                       ? new byte[0] : 
          Value is System.Data.OracleClient.OracleLob ? (byte[]) ((OracleLob) Value).Value : 
                                                        (byte[]) Value);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter name.
    /// </summary>
    public string Name
    {
      get { return m_Parameter.ParameterName; }
      set { m_Parameter.ParameterName = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter direction.
    /// </summary>
    public ParameterDirection Direction
    {
      get { return m_Parameter.Direction; }
      set { m_Parameter.Direction = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parameter data type.
    /// </summary>
    public DbType DataType
    {
      get { return m_Parameter.DbType; }
      set { m_Parameter.DbType = value; }
    }
    //----------------------------------------------------------------------------
    public override string ToString()
    {
      return Name ?? base.ToString();
    }
    //----------------------------------------------------------------------------
  }
}
