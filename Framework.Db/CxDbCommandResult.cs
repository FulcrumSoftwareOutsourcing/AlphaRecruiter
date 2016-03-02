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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;
using Framework.Utils;

namespace Framework.Db
{
	/// <summary>
	/// Class holding database command execution result.
	/// </summary>
	[Serializable]
	public class CxDbCommandResult
	{
    //-------------------------------------------------------------------------
    protected int m_RowsAffected = -1;
    protected object m_ScalarValue = null;
    protected DataSet m_DataSet = null;
    protected DataTable m_DataTable = null;
    protected ArrayList m_OutputParameters = new ArrayList();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxDbCommandResult()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets count of rows affected by the command.
    /// </summary>
    [DefaultValue(-1)]
    public int RowsAffected
    { get { return m_RowsAffected; } set { m_RowsAffected = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets scalar value returned by the command.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public object ScalarValue
    { get { return m_ScalarValue; } set { m_ScalarValue = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets serializable scalar value returned by the command.
    /// </summary>
    public object SerializableScalarValue
    { 
      get 
      { 
        return CxDbUtils.ParamValueToSerializableValue(m_ScalarValue); 
      } 
      set 
      {
        m_ScalarValue = CxDbUtils.ParamValueFromSerializableValue(value);
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets data set returned by the command.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public DataSet DataSet
    { get { return m_DataSet; } set { m_DataSet = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets data table returned by the command.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public DataTable DataTable
    { get { return m_DataTable; } set { m_DataTable = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets serialized data set.
    /// </summary>
    public byte[] SerializedDataSet
    { 
      get 
      { 
        if (m_DataSet != null)
        {
          return CxDbUtils.SerializeDataSet(m_DataSet);
        }
        return null; 
      } 
      set 
      { 
        if (value != null && value.Length > 0)
        {
          m_DataSet = CxDbUtils.DeserializeDataSet(value);
        }
        else
        {
          m_DataSet = null;
        }
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets serialized data table.
    /// </summary>
    public byte[] SerializedDataTable
    { 
      get 
      { 
        if (m_DataTable != null)
        {
          DataSet ds = new DataSet();
          ds.Tables.Add(m_DataTable);
          byte[] result = CxDbUtils.SerializeDataSet(ds);
          ds.Tables.Remove(m_DataTable);
          return result;
        }
        return null; 
      } 
      set 
      { 
        if (value != null && value.Length > 0)
        {
          
          DataSet ds = CxDbUtils.DeserializeDataSet(value);
          if (ds != null && ds.Tables.Count > 0)
          {
            DataTable dt = ds.Tables[0];
            ds.Tables.Remove(dt);
            m_DataTable = dt;
          }
        }
        else
        {
          m_DataTable = null;
        }
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets array of the output parameters.
    /// </summary>
    public CxDbParameterDescription[] OutputParameters
    { 
      get 
      { 
        CxDbParameterDescription[] array = new CxDbParameterDescription[m_OutputParameters.Count];
        m_OutputParameters.CopyTo(array);
        return array; 
      } 
      set 
      { 
        m_OutputParameters.Clear();
        if (value != null && value.Length > 0)
        {
          m_OutputParameters.AddRange(value);
        }
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns output parameter value by parameter name.
    /// </summary>
    /// <param name="name">name of the output parameter</param>
    /// <returns>value of the output parameter</returns>
    public object GetOutputParamValue(string name)
    {
      foreach (CxDbParameterDescription p in m_OutputParameters)
      {
        if (CxText.Equals(p.Name, name))
        {
          return p.Value;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets output parameter value by its name.
    /// </summary>
    /// <param name="name">name of the output parameter</param>
    /// <param name="value">value of the output parameter</param>
    public void SetOutputParamValue(string name, object value)
    {
      if (CxUtils.NotEmpty(name))
      {
        foreach (CxDbParameterDescription p in m_OutputParameters)
        {
          if (CxText.Equals(p.Name, name))
          {
            p.Value = value;
            return;
          }
        }
        CxDbParameterDescription param = new CxDbParameterDescription();
        param.Name = name;
        param.Value = value;
        m_OutputParameters.Add(param);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Assigns output parameters from the given command.
    /// </summary>
    public void AssignOutputParamsFromCommand(CxDbCommand command)
    {
      m_OutputParameters.Clear();
      if (command.Parameters != null)
      {
        foreach (CxDbParameter param in command.Parameters)
        {
          if (param.Direction != ParameterDirection.Input)
          {
            SetOutputParamValue(param.Name, param.Value);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies output parameter values to the given command.
    /// </summary>
    public void CopyOutputParamValuesToCommand(CxDbCommand command)
    {
      if (command.Parameters != null)
      {
        foreach (CxDbParameter param in command.Parameters)
        {
          if (param.Direction != ParameterDirection.Input)
          {
            param.Value = GetOutputParamValue(param.Name);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies output parameter values to the given command.
    /// </summary>
    public void CopyOutputParamValuesToCommand(IDbCommand command)
    {
      if (command.Parameters != null)
      {
        foreach (IDataParameter param in command.Parameters)
        {
          if (param.Direction != ParameterDirection.Input)
          {
            param.Value = GetOutputParamValue(param.ParameterName);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}