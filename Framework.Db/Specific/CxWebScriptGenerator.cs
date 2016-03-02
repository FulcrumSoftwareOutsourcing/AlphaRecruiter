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

using System.Collections.Generic;

namespace Framework.Db
{
  /// <summary>
  /// Incapsulates script generation methods for 
  /// the specific SQL syntax of the underlying connection.
  /// </summary>
  internal class CxWebScriptGenerator: CxDbScriptGenerator
  {
    //--------------------------------------------------------------------------
    private CxDbScriptGenerator m_TargetScriptGenerator_Cache;
    //--------------------------------------------------------------------------
    protected override CxDbScriptGenerator TargetScriptGenerator
    {
      get
      {
        if (m_TargetScriptGenerator_Cache == null)
        {
          using (var connection = WebConnection.CreateTargetDbConnection())
          {
            m_TargetScriptGenerator_Cache = connection.ScriptGenerator;
          }
        }
        return m_TargetScriptGenerator_Cache;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Web-connection context.
    /// </summary>
    protected CxWebConnection WebConnection
    {
      get { return Connection as CxWebConnection; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="connection"></param>
    public CxWebScriptGenerator(CxWebConnection connection)
      : base(connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns lock clause for SELECT statement.
    /// </summary>
    public override string GetLockClauseForSelect()
    {
      return TargetScriptGenerator.GetLockClauseForSelect();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL text for getting top N records from the original SQL statement result.
    /// </summary>
    /// <param name="sqlText">original SQL text</param>
    /// <param name="topCount"></param>
    /// <returns>modified SQL text</returns>
    public override string GetTopRecordsSqlText(string sqlText, int topCount)
    {
      return TargetScriptGenerator.GetTopRecordsSqlText(sqlText, topCount);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a statement that gets count of records in the result set.
    /// </summary>
    /// <param name="select"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public override CxDbCommandDescription GetCountQueryForSelect(string select, CxCriteriaOperator criteria)
    {
      return TargetScriptGenerator.GetCountQueryForSelect(select, criteria);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a query that provides the paging feature for the given select.
    /// </summary>
    /// <param name="selectCommand">a select query to be wrapped</param>
    /// <param name="primaryKeys">an array of primary key names (should be one as for now)</param>
    /// <param name="keyValues">values of keys</param>
    /// <param name="sortings">a list of sort descriptors</param>
    /// <param name="startIndex">a record start index to obtain data from</param>
    /// <param name="count">an amount of records to get</param>
    /// <param name="simplifiedMode">indicates if values of keys could be 
    /// used directly in the script (to raise up performance when possible)</param>
    /// <returns>a database command to be called to obtain a page of data</returns>
    public override CxQueryDescriptor GetPagedQueryForSelect(
      CxQueryDescriptor selectCommand, 
      string[] primaryKeys, 
      IList<string> keyValues, 
      CxSortDescriptorList sortings, 
      int startIndex, 
      int count,
      bool simplifiedMode)
    {
      return TargetScriptGenerator.GetPagedQueryForSelect(
          selectCommand, primaryKeys, keyValues, sortings, startIndex, count, simplifiedMode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a database command that returns grouped query results 
    /// with amount of items in each group.
    /// </summary>
    /// <param name="selectCommand">a select query to be wrapped</param>
    /// <param name="groupByFields">an array of field-names to group by</param>
    /// <param name="groupByValues">values of the groups to obtain data for</param>
    /// <param name="filterCriteria">filter criteria</param>
    /// <param name="sortings">sort descriptors list</param>
    /// <param name="aggregateItems">aggregate query items</param>
    /// <returns>a database command</returns>
    public override CxAggregateQueryDescriptor GetQueryForSubGroups(
      CxQueryDescriptor selectCommand, 
      string[] groupByFields, 
      object[] groupByValues, 
      CxCriteriaOperator filterCriteria, 
      CxSortDescriptorList sortings,
      Dictionary<object, CxAggregateDescriptor> aggregateItems)
    {
      return TargetScriptGenerator.GetQueryForSubGroups(
          selectCommand, groupByFields,
          groupByValues, filterCriteria, sortings, aggregateItems);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a database script that returns a list of primary key values.
    /// </summary>
    /// <param name="selectCommand">a select statement to be wrapped</param>
    /// <param name="primaryKeys">an array of primary key names</param>
    /// <param name="criteria">filter criteria</param>
    /// <param name="sortings">a list of sort descriptors</param>
    /// <returns>processed query string</returns>
    public override CxQueryDescriptor GetQueryForPrimaryKeys(
      CxQueryDescriptor selectCommand, 
      string[] primaryKeys, 
      CxCriteriaOperator criteria, 
      CxSortDescriptorList sortings)
    {
      return TargetScriptGenerator.GetQueryForPrimaryKeys(
          selectCommand, primaryKeys, criteria, sortings);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes the TOP clause from the given select query.
    /// </summary>
    /// <param name="select">a select query to be wrapped</param>
    /// <returns>a select query without a TOP clause</returns>
    public override string RemoveTopClauseFromSelect(string select)
    {
      return TargetScriptGenerator.RemoveTopClauseFromSelect(select);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Generates a query that returns unique values of the given field
    /// (taking into account the given filter criteria).
    /// </summary>
    /// <param name="selectCommand">a select command to get unique values for</param>
    /// <param name="criteria">a filter criteria</param>
    /// <param name="fieldName">a field name to get unique values for</param>
    /// <returns>a database command that returns a list of unique values</returns>
    public override CxQueryDescriptor GetQueryForUniqueFieldValues(
      CxQueryDescriptor selectCommand, CxCriteriaOperator criteria, string fieldName)
    {
      return TargetScriptGenerator.GetQueryForUniqueFieldValues(
          selectCommand, criteria, fieldName);
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
      return TargetScriptGenerator.AddRowCountLimitation(sql, rowCount);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a query that gets some aggregate values.
    /// </summary>
    /// <param name="selectQuery">a command to get aggregate values for</param>
    /// <param name="filterCriteria">filter criteria for the aggregate query</param>
    /// <param name="aggregateItems">aggregate description items</param>
    /// <returns>a command that gets the described aggregate values</returns>
    public override CxAggregateQueryDescriptor GetQueryForAggregates(
      CxQueryDescriptor selectQuery, 
      CxCriteriaOperator filterCriteria,
      Dictionary<object, CxAggregateDescriptor> aggregateItems,
      CxSortDescriptorList sortings)
    {
      return TargetScriptGenerator.GetQueryForAggregates(
          selectQuery, filterCriteria, aggregateItems, sortings);
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
      return TargetScriptGenerator.PrepareParameterSubstitute(name, index);
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
      return TargetScriptGenerator.PrepareParameterName(name, index);
    }
    //----------------------------------------------------------------------------
    public override string GetCleanFieldName(string fieldName)
    {
        return TargetScriptGenerator.GetCleanFieldName(fieldName);
    }
    //----------------------------------------------------------------------------
  }
}
