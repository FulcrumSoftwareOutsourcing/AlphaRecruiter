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

namespace Framework.Db
{
  /// <summary>
  /// Incapsulates script generation methods for 
  /// the specific PL/SQL syntax.
  /// </summary>
  internal class CxOracleScriptGenerator: CxDbScriptGenerator
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="connection">connection context of the generator</param>
    public CxOracleScriptGenerator(CxOracleConnection connection)
      : base(connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns lock clause for SELECT statement.
    /// </summary>
    override public string GetLockClauseForSelect()
    {
      return "FOR UPDATE";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the field name in a format that eliminates incorrect field-name
    /// comprehension. 
    /// That means, all the field-names should be percepted by SQL server as 
    /// field-names, doesn't matter if they're equal to SQL keyword.
    /// </summary>
    /// <param name="fieldName">a field name to be corrected</param>
    /// <returns>processed field name</returns>
    public override string GetExplicitFieldName(string fieldName)
    {
      return "\"" + fieldName + "\"";
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
      return string.Format("select t.* from ({0}) where rowcount <= {1}", sql, rowCount);
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
      return (name.StartsWith(":") ? name : ":" + name);
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
      return name; //(name.StartsWith(":") ? name : ":" + name);
    }
    //----------------------------------------------------------------------------
  }
}
