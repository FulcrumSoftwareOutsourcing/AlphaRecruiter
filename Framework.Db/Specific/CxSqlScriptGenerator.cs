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
using System.Collections.Generic;
using System.Text;
using Framework.Utils;

namespace Framework.Db
{
  /// <summary>
  /// Incapsulates script generation methods for 
  /// the specific Transact SQL syntax.
  /// </summary>
  internal class CxSqlScriptGenerator : CxDbScriptGenerator
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="connection"></param>
    public CxSqlScriptGenerator(CxSqlConnection connection)
      : base(connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL text for getting top N records from the original SQL statement result.
    /// </summary>
    /// <param name="sqlText">original SQL text</param>
    /// <param name="topCount">amount of top records to obtain</param>
    /// <returns>modified SQL text</returns>
    override public string GetTopRecordsSqlText(string sqlText, int topCount)
    {
      return "SELECT TOP " + topCount + " data.* FROM (" + sqlText + ") data";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns lock clause for SELECT statement.
    /// </summary>
    override public string GetLockClauseForSelect()
    {
      return "WITH (HOLDLOCK)";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the field name in a format that eliminates incorrect field-name
    /// comprehension by the DBMS layer. 
    /// That means, all the field-names should be percepted by SQL server as 
    /// field-names, doesn't matter if they're equal to SQL keyword.
    /// </summary>
    /// <param name="fieldName">a field name to be casted explicitly</param>
    /// <returns>an explicit cast of the given field name</returns>
    public override string GetExplicitFieldName(string fieldName)
    {
      if (!string.IsNullOrEmpty(fieldName) && !fieldName.StartsWith("[") && !fieldName.EndsWith("]"))
        return "[" + fieldName + "]";
      return fieldName;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the field name cleaned up from any formatting rules applied to it to fit
    /// the target SQL engine.
    /// </summary>
    /// <param name="fieldName">a field name to be corrected</param>
    /// <returns>the pure field name</returns>
    public override string GetCleanFieldName(string fieldName)
    {
      if (!string.IsNullOrEmpty(fieldName) && fieldName.StartsWith("[") && fieldName.EndsWith("]"))
        return fieldName.TrimStart('[').TrimEnd(']');
      return fieldName;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds row count limitation to the query depending on the database type.
    /// </summary>
    /// <param name="sql">SQL SELECT statement to add limitation to</param>
    /// <param name="rowCount">number of rows to return</param>
    /// <returns>SQl SELECT statment with row count limitation</returns>
    public override string AddRowCountLimitation(string sql, int rowCount)
    {
      string s = CxText.TrimSpace(sql);
      if (s.ToLower().StartsWith("select"))
        return s.Insert("select".Length, " top " + rowCount + " ");
      else
        return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns substitute for the parameter in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>substitute for the parameter in the provider-dependent format</returns>
    override public string PrepareParameterSubstitute(string name, int index)
    {
      return (name.StartsWith("@") ? name : "@" + name);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parameter name in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>parameter name in the provider-dependent format</returns>
    override public string PrepareParameterName(string name, int index)
    {
      return (name.StartsWith("@") ? name : "@" + name);
    }
    //----------------------------------------------------------------------------
  }
}