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
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;

namespace Framework.Db
{
	/// <summary>
	/// Serializable class holding description of DB command parameter.
	/// </summary>
	[Serializable]
	public class CxDbParameterDescription
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxDbParameterDescription()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxDbParameterDescription(IDataParameter parameter)
    {
      Name = parameter.ParameterName;
      DataType = parameter.DbType;
      Direction = parameter.Direction;
      Value = parameter.Value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parameter">a parameter to create a description by</param>
    public CxDbParameterDescription(CxDbParameter parameter)
    {
      Name = parameter.Name;
      Value = parameter.Value;
      DataType = parameter.DataType;
      Direction = parameter.Direction;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">a name of the parameter</param>
    /// <param name="value">a value of the parameter</param>
    public CxDbParameterDescription(string name, object value)
    {
      Name = name;
      Value = value;
    }
	  //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxDbParameterDescription(CxDbParameterDescription parameter)
    {
      Name = parameter.Name;
      DataType = parameter.DataType;
      Direction = parameter.Direction;
      Value = parameter.Value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public string Name;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Data type of the parameter.
    /// </summary>
    [DefaultValue(DbType.String)]
    public DbType DataType = DbType.String;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parameter direction.
    /// </summary>
    [DefaultValue(ParameterDirection.Input)]
    public ParameterDirection Direction = ParameterDirection.Input;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Value of the parameter.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public object Value;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Value of the parameter to serialize / deserialize.
    /// </summary>
    public object SerializableValue
    {
      get
      {
        return CxDbUtils.ParamValueToSerializableValue(Value);
      }
      set
      {
        Value = CxDbUtils.ParamValueFromSerializableValue(value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates array of parameter descriptions from the DB command.
    /// </summary>
    static public CxDbParameterDescription[] GetParameterDescriptions(IDbCommand command)
    {
      CxDbParameterDescription[] result = new CxDbParameterDescription[command.Parameters.Count];
      for (int i = 0; i < command.Parameters.Count; i++)
      {
        CxDbParameterDescription description = 
          new CxDbParameterDescription((IDataParameter)command.Parameters[i]);
        result[i] = description;
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates array of parameter descriptions from the DB command.
    /// </summary>
    static public CxDbParameterDescription[] GetParameterDescriptions(CxDbCommand command)
    {
      CxDbParameterDescription[] result = new CxDbParameterDescription[command.Parameters.Length];
      for (int i = 0; i < command.Parameters.Length; i++)
      {
        CxDbParameterDescription description =
          new CxDbParameterDescription(command.Parameters[i]);
        result[i] = description;
      }
      return result;
    }
    //-------------------------------------------------------------------------
  }
}