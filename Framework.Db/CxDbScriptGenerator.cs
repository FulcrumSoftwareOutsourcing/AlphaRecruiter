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
using Framework.Utils;
using System.ComponentModel;
using System.Collections.Generic;

namespace Framework.Db
{
  /// <summary>
  /// A container of methods that generate scripts 
  /// for a particular DB server type.
  /// </summary>
  public abstract class CxDbScriptGenerator
  {

    //--------------------------------------------------------------------------
    /// <summary>
    /// A context used during where clause generation.
    /// </summary>
    protected class CxWhereClauseGenerationContext
    {
      public int ParameterIndex;
    }
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    private CxDbConnection m_Connection;
    //--------------------------------------------------------------------------

    #region Properties
    //--------------------------------------------------------------------------
    /// <summary>
    /// The connection context of the current script generator.
    /// </summary>
    public CxDbConnection Connection
    {
      get { return m_Connection; }
      set { m_Connection = value; }
    }
    //--------------------------------------------------------------------------
    protected virtual CxDbScriptGenerator TargetScriptGenerator
    {
      get { return this; }
    }
    //--------------------------------------------------------------------------
    #endregion

    #region Ctors
    //--------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    /// <param name="connection"></param>
    public CxDbScriptGenerator(CxDbConnection connection)
    {
      Connection = connection;
    }
    //--------------------------------------------------------------------------
    #endregion

    #region Methods
    //--------------------------------------------------------------------------
    #region Aggregate functions
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an expression transformed by the "count" aggregate function
    /// depending on the type of connection used.
    /// </summary>
    /// <param name="expression">an expression to be transformed</param>
    /// <returns>a "count" function expression</returns>
    virtual public string GetAggregateCountStatement(string expression)
    {
      return string.Format("count({0})", expression);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an expression transformed by the "minimum" aggregate function
    /// depending on the type of connection used.
    /// </summary>
    /// <param name="expression">an expression to be transformed</param>
    /// <returns>a "minimum" function expression</returns>
    virtual public string GetAggregateMinStatement(string expression)
    {
      return string.Format("min({0})", expression);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an expression transformed by the "maximum" aggregate function
    /// depending on the type of connection used.
    /// </summary>
    /// <param name="expression">an expression to be transformed</param>
    /// <returns>a "maximum" function expression</returns>
    virtual public string GetAggregateMaxStatement(string expression)
    {
      return string.Format("max({0})", expression);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an expression transformed by the "average" aggregate function
    /// depending on the type of connection used.
    /// </summary>
    /// <param name="expression">an expression to be transformed</param>
    /// <returns>an "average" function expression</returns>
    virtual public string GetAggregateAverageStatement(string expression)
    {
      return string.Format("avg({0})", expression);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an expression transformed by the "sum" aggregate function
    /// depending on the type of connection used.
    /// </summary>
    /// <param name="expression">an expression to be transformed</param>
    /// <returns>a "sum" function expression</returns>
    virtual public string GetAggregateSumStatement(string expression)
    {
      return string.Format("sum({0})", expression);
    }
    //----------------------------------------------------------------------------
    #endregion
    //----------------------------------------------------------------------------
    #region Paging related methods
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a script which purpose is to return an empty record set 
    /// with defined columns only.
    /// </summary>
    /// <param name="selectCommand">a select command to obtain columns for</param>
    /// <returns>modified script</returns>
    virtual public CxQueryDescriptor GetQueryToGetColumns(
      CxQueryDescriptor selectCommand)
    {
      return new CxQueryDescriptor(
        GetQueryToGetColumns(selectCommand.CommandText),
        selectCommand.Parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a statement that gets count of records in the result set.
    /// </summary>
    /// <param name="select">a select statement to be wrapped</param>
    /// <param name="criteria">filter criteria</param>
    /// <returns>a SQL statement</returns>
    virtual public CxDbCommandDescription GetCountQueryForSelect(
      string select,
      CxCriteriaOperator criteria)
    {
      CxDbCommandDescription command = new CxDbCommandDescription();
      CxWhereClause whereClause = GetWhereClause(criteria);
      if (!string.IsNullOrEmpty(whereClause.Text))
        whereClause.Text = string.Format(" where {0}", whereClause.Text);
      command.CommandText = string.Format("select count(*) from ({0}) as _T {1}", select, whereClause.Text);
      command.Parameters = whereClause.Parameters.ToArray();
      return command;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a statement that gets the count of records in the record set.
    /// </summary>
    /// <param name="select">a select statement to get COUNT for</param>
    /// <returns>a SQL statement</returns>
    virtual public string GetCountSqlScriptForSelect(
      string select)
    {
      return string.Format("select count(*) from ({0}) as _T", select);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a statement that gets pair key and count of records in the record set for 
    /// the given dictionary.
    /// </summary>
    /// <param name="listChildDataQuery">the given dictionary</param>
    /// <returns></returns>
    virtual public string GetQueryForAmountOfItems(Dictionary<string, string> listChildDataQuery)
    {
      List<string> listQuerySpecification = new List<string>();

      foreach (KeyValuePair<string, string> keyValuePair in listChildDataQuery)
      {
        if (!string.IsNullOrEmpty(keyValuePair.Value))
        {
          listQuerySpecification.Add(
            string.Format("select {0}, count(*) from ({1}) as _T ",
              CxText.GetSingleQuotedString(keyValuePair.Key),
              keyValuePair.Value));
        }
      }

      return string.Join(" union all ", listQuerySpecification.ToArray());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a database command that returns grouped query results 
    /// with amount of items in each group.
    /// </summary>
    /// <param name="selectCommand">a select query to be wrapped</param>
    /// <param name="parentGroupFields">an array of field-names to group by</param>
    /// <param name="parentGroupValues">values of the groups to obtain data for</param>
    /// <param name="filterCriteria">filter criteria</param>
    /// <param name="sortings">sort descriptors list</param>
    /// <param name="aggregateItems">aggregate query items</param>
    /// <returns>a database command</returns>
    virtual public CxAggregateQueryDescriptor GetQueryForSubGroups(
      CxQueryDescriptor selectCommand,
      string[] parentGroupFields,
      object[] parentGroupValues,
      CxCriteriaOperator filterCriteria,
      CxSortDescriptorList sortings,
      Dictionary<object, CxAggregateDescriptor> aggregateItems)
    {
      CxGroupOperator groupOperator = new CxGroupOperator(NxGroupOperatorType.And);
      for (int i = 0; i < parentGroupFields.Length; i++)
      {
        CxCriteriaOperator @operator;
        if (parentGroupValues[i] != DBNull.Value)
        {
          @operator = new CxBinaryOperator(
           new CxPropertyOperand(parentGroupFields[i]),
           NxBinaryOperatorType.Equal,
           new CxValueOperand(parentGroupValues[i]));
        }
        else
        {
          @operator = new CxUnaryOperator(NxUnaryOperatorType.IsNull, new CxPropertyOperand(parentGroupFields[i]));
        }
        groupOperator.Operators.Add(@operator);
      }
      filterCriteria = CxCriteriaOperator.Combine(
        new CxCriteriaOperator[] { filterCriteria, groupOperator },
        NxGroupOperatorType.And);

      CxAggregateQueryDescriptor query = GetQueryForAggregates(
        selectCommand,
        filterCriteria,
        aggregateItems,
        sortings);

      return query;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Corrects the sorting list according to the aggregates composition.
    /// </summary>
    /// <param name="sortings">sortings to be corrected</param>
    /// <param name="aggregates">aggregates to be taken into account</param>
    /// <returns>a corrected sorting list</returns>
    protected CxSortDescriptorList GetWorkingSortDescriptorList(
      CxSortDescriptorList sortings,
      CxAggregateDescriptorList aggregates)
    {
      CxSortDescriptorList list = new CxSortDescriptorList(sortings);
      CxAggregateDescriptorList nonAggregatedList = aggregates.GetByDescriptorType(NxAggregateDescriptorType.None);
      for (int i = list.Count - 1; i >= 0; i--)
      {
        if (nonAggregatedList.GetByFieldName(list[i].FieldName).Count == 0)
          list.RemoveAt(i);
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a query that gets some aggregate values.
    /// </summary>
    /// <param name="selectQuery">a command to get aggregate values for</param>
    /// <param name="filterCriteria">filter criteria for the aggregate query</param>
    /// <param name="aggregateItems">aggregate description items</param>
    /// <param name="sortings">sort description items</param>
    /// <returns>a command that gets the described aggregate values</returns>
    virtual public CxAggregateQueryDescriptor GetQueryForAggregates(
      CxQueryDescriptor selectQuery,
      CxCriteriaOperator filterCriteria,
      Dictionary<object, CxAggregateDescriptor> aggregateItems,
      CxSortDescriptorList sortings)
    {
      // Validation
      if (aggregateItems == null)
        throw new ExNullArgumentException("aggregateItems");
      if (aggregateItems.Count == 0)
        throw new ExArgumentException("aggregateItems", "Length=0");
      if (selectQuery == null)
        throw new ExNullArgumentException("selectQuery");

      CxAggregateQueryDescriptor query = new CxAggregateQueryDescriptor();
      query.Parameters.AddRange(selectQuery.Parameters);

      string[] selectStatements = new string[aggregateItems.Count];
      {
        int i = 0;
        foreach (KeyValuePair<object, CxAggregateDescriptor> pair in aggregateItems)
        {
          string selectAlias = "_ag" + (i + 1);
          query.AggregateAliases.Add(pair.Key, selectAlias);
          selectStatements[i] = GetAggregateStatement(pair.Value) + " " + selectAlias;
          i++;
        }
      }

      CxWhereClause whereClause = GetWhereClause(filterCriteria);
      if (!string.IsNullOrEmpty(whereClause.Text))
        whereClause.Text = string.Format("where {0}", whereClause.Text);
      query.Parameters.AddRange(whereClause.Parameters);

      CxAggregateDescriptorList aggregateItemList = new CxAggregateDescriptorList(
        CxDictionary.ExtractDictionaryValueList(aggregateItems));
      string groupByStatement = ConvertAggregateDescriptorsToSqlStatement(
        aggregateItemList.ToArray());
      if (!string.IsNullOrEmpty(groupByStatement))
        groupByStatement = "group by " + groupByStatement;

      // We should exclude fields from the "order by" statement if they're
      // already in the aggregate statement.
      sortings = GetWorkingSortDescriptorList(sortings, aggregateItemList);

      string orderByStatement = ConvertSortDescriptorsToSqlStatement(sortings);
      if (!string.IsNullOrEmpty(orderByStatement))
        orderByStatement = "order by " + orderByStatement;

      query.CommandText = string.Format("select {0} from ({1}) as _T {2} {3} {4}",
        string.Join(",", selectStatements),
        selectQuery.CommandText,
        whereClause.Text,
        groupByStatement,
        orderByStatement);

      return query;
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
    virtual public CxQueryDescriptor GetQueryForUniqueFieldValues(
      CxQueryDescriptor selectCommand,
      CxCriteriaOperator criteria,
      string fieldName)
    {
      CxWhereClause whereClause = GetWhereClause(criteria);
      if (!string.IsNullOrEmpty(whereClause.Text))
        whereClause.Text = string.Format(" where {0}", whereClause.Text);

      string script = string.Format(
        "select distinct _T.{2} from ({0}) as _T {1} order by _T.{2}",
        selectCommand.CommandText,
        whereClause.Text,
        GetExplicitFieldName(fieldName));

      CxQueryDescriptor query = new CxQueryDescriptor(script, selectCommand.Parameters);
      query.Parameters.AddRange(whereClause.Parameters);
      return query;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a script that returns just the given sub-range of records
    /// in the given sorting conditions.
    /// </summary>
    /// <param name="sqlText">sql text to get a sub-range for</param>
    /// <param name="startRowIndex">starting index of the records to get</param>
    /// <param name="rowsAmount">amount of records to get</param>
    /// <param name="orderByClause">the clause defining the sorting conditions</param>
    /// <returns>script generated</returns>
    virtual public string GetPagedScriptForSelect(
      string sqlText,
      int startRowIndex,
      int rowsAmount,
      string orderByClause)
    {
      if (startRowIndex == -1 || rowsAmount == -1)
      {
        return sqlText;
      }
      else
      {
        if (string.IsNullOrEmpty(orderByClause))
          throw new ExNullArgumentException("orderByClause");
        return string.Format(
          "select __TO.* from (" +
          Environment.NewLine +
          "select __T.*, ROW_NUMBER() OVER(ORDER BY {0}) as __RowNumber " +
          "from ({1}) as __T) as __TO " +
          Environment.NewLine +
          "where __RowNumber Between {2} and {3} ",
          orderByClause, sqlText, startRowIndex + 1, startRowIndex + rowsAmount);
      }
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
    virtual public CxQueryDescriptor GetPagedQueryForSelect(
      CxQueryDescriptor selectCommand,
      string[] primaryKeys,
      IList<string> keyValues,
      CxSortDescriptorList sortings,
      int startIndex,
      int count,
      bool simplifiedMode)
    {
      string template =
        "select _T.* from ({0}) as _T where {1} in ({2}) order by {3}";

      string adoptedSelect = RemoveTopClauseFromSelect(selectCommand.CommandText);

      // Forming a sort statement taking into account primary keys
      string sortStatement = ConvertSortDescriptorsToSqlStatement(sortings, primaryKeys);

      string script;

      if (simplifiedMode)
      {
        string commaSeparatedValues =
          GetCommaSeparatedValues(keyValues, startIndex, count, false);
        script = string.Format(
          template, adoptedSelect, GetExplicitFieldName(primaryKeys[0]), commaSeparatedValues, sortStatement);
      }
      else
      {
        string primaryKeyValueParameters = GetCommaSeparatedParameters(startIndex, count);
        script = string.Format(
          template, adoptedSelect, primaryKeys[0], primaryKeyValueParameters, sortStatement);
      }
      CxQueryDescriptor query = new CxQueryDescriptor(
        script, selectCommand.Parameters);

      if (!simplifiedMode)
        GenerateParameters(query, keyValues, startIndex, count);

      return query;
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
    virtual public CxQueryDescriptor GetQueryForPrimaryKeys(
      CxQueryDescriptor selectCommand,
      string[] primaryKeys,
      CxCriteriaOperator criteria,
      CxSortDescriptorList sortings)
    {
      // Validation
      if (selectCommand == null)
        throw new ExNullArgumentException("selectCommand");
      if (primaryKeys == null)
        throw new ExNullArgumentException("primaryKeys");
      if (sortings == null)
        throw new ExNullArgumentException("sortings");

      string primaryKeysString = GetCommaSeparatedFields(primaryKeys);

      CxWhereClause whereClause = GetWhereClause(criteria);
      if (!string.IsNullOrEmpty(whereClause.Text))
        whereClause.Text = string.Format("where {0} ", whereClause.Text);

      // Forming a sort statement taking into account primary keys
      string sortStatement =
        ConvertSortDescriptorsToSqlStatement(sortings, primaryKeys);

      if (string.IsNullOrEmpty(sortStatement))
      {
        string commaSeparatedKeys = GetCommaSeparatedFields(primaryKeys);
        sortStatement = commaSeparatedKeys;
      }

      string script = string.Format("select {0} from ({1}) as _T {2} order by {3}",
        primaryKeysString, selectCommand.CommandText, whereClause.Text, sortStatement);

      List<CxDbParameterDescription> parameters = new List<CxDbParameterDescription>();
      parameters.AddRange(selectCommand.Parameters);
      parameters.AddRange(whereClause.Parameters);

      return new CxQueryDescriptor(
        script,
        parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a statement by the given aggregate descriptor.
    /// </summary>
    /// <param name="item">an aggregate descriptor to create a statement by</param>
    /// <returns>the created statement</returns>
    public string GetAggregateStatement(
      CxAggregateDescriptor item)
    {
      string fieldName = item.FieldName;
      if (fieldName != "*")
        fieldName = GetExplicitFieldName(fieldName);
      switch (item.DescriptorType)
      {
        case NxAggregateDescriptorType.None:
          return fieldName;
        case NxAggregateDescriptorType.Average:
          return GetAggregateAverageStatement(fieldName);
        case NxAggregateDescriptorType.Count:
          return GetAggregateCountStatement(fieldName);
        case NxAggregateDescriptorType.Max:
          return GetAggregateMaxStatement(fieldName);
        case NxAggregateDescriptorType.Min:
          return GetAggregateMinStatement(fieldName);
        case NxAggregateDescriptorType.Sum:
          return GetAggregateSumStatement(fieldName);
        default:
          throw new NotSupportedException();
      }
    }
    #endregion
    //----------------------------------------------------------------------------
    #region Where clause generation based upon Criteria Operator
    public const string WHERE_CLAUSE_PARAMETER_PREFIX = "_wp";
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given property operand.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="property">a property operand to be represented</param>
    /// <returns>a string that represents the given property operand</returns>
    protected CxWhereClause HandleProperty(
      CxWhereClauseGenerationContext context, CxPropertyOperand property)
    {
      CxWhereClause result = new CxWhereClause();
      result.Text = GetExplicitFieldName(property.PropertyName);
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given simple value operand.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="value">a simple value to be transformed</param>
    /// <returns>a where clause that represents the given value operand</returns>
    protected CxWhereClause HandleSimpleValue(
      CxWhereClauseGenerationContext context, CxSimpleValueOperand value)
    {
      CxWhereClause result = new CxWhereClause();
      result.Text = value.Value;
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given operand.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="operand">an operand to be transformed</param>
    /// <returns>a where clause that represents the given value operand</returns>
    protected CxWhereClause HandleOperand(
      CxWhereClauseGenerationContext context, CxCriteriaOperator operand)
    {
      if (operand is CxSimpleValueOperand)
        return HandleSimpleValue(context, (CxSimpleValueOperand) operand);
      if (operand is CxPropertyOperand)
        return HandleProperty(context, (CxPropertyOperand) operand);
      if (operand is CxValueOperand)
        return HandleValue(context, (CxValueOperand) operand);
      throw new NotSupportedException(string.Format("<{0}> is not supported", operand.GetType()));
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given value operand.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="value">a value to be transformed</param>
    /// <returns>a where clause that represents the given value operand</returns>
    protected CxWhereClause HandleValue(
      CxWhereClauseGenerationContext context, CxValueOperand value)
    {
      CxWhereClause result = new CxWhereClause();
      CxDbParameterDescription parameter = new CxDbParameterDescription(WHERE_CLAUSE_PARAMETER_PREFIX + context.ParameterIndex, value.Value);
      parameter.DataType = CxDbUtils.ConvertToDbType(value.Value.GetType());
      result.Parameters.Add(parameter);
      result.Text = PrepareParameterName(parameter.Name, context.ParameterIndex);

      context.ParameterIndex++;
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given binary operator.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="binary">a binary operator to be transformed</param>
    /// <returns>a where clause that represents the given binary operator</returns>
    protected CxWhereClause HandleBinary(
      CxWhereClauseGenerationContext context, CxBinaryOperator binary)
    {
      CxWhereClause result = new CxWhereClause();
      string binarySymbol;
      switch (binary.OperatorType)
      {
        case NxBinaryOperatorType.Equal:
          binarySymbol = "=";
          break;
        case NxBinaryOperatorType.NotEqual:
          binarySymbol = "<>";
          break;
        case NxBinaryOperatorType.Like:
          binarySymbol = "like";
          break;
        case NxBinaryOperatorType.Less:
          binarySymbol = "<";
          break;
        case NxBinaryOperatorType.LessOrEqual:
          binarySymbol = "<=";
          break;
        case NxBinaryOperatorType.Greater:
          binarySymbol = ">";
          break;
        case NxBinaryOperatorType.GreaterOrEqual:
          binarySymbol = ">=";
          break;
        case NxBinaryOperatorType.BitwiseAnd:
          binarySymbol = "&";
          break;
        case NxBinaryOperatorType.BitwiseOr:
          binarySymbol = "|";
          break;
        case NxBinaryOperatorType.BitwiseXor:
          binarySymbol = "^";
          break;
        default:
          throw new ExException(
            string.Format("Unknown binary operator type: <{0}>", binary.OperatorType));
      }

      CxWhereClause leftOperand = HandleOperand(context, binary.LeftOperand);
      CxWhereClause rightOperand = HandleOperand(context, binary.RightOperand);
      result.Text =
        leftOperand.Text + " " +
        binarySymbol + " " +
        rightOperand.Text;
      result.Parameters.AddRange(leftOperand.Parameters);
      result.Parameters.AddRange(rightOperand.Parameters);
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given "in" criterion.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="inCriterion">an operator to be represented</param>
    /// <returns>a where clause that represents the given "in" criterion</returns>
    protected CxWhereClause HandleInOperation(
      CxWhereClauseGenerationContext context, CxInOperator inCriterion)
    {
      CxWhereClause result = new CxWhereClause();

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < inCriterion.ValueOperands.Count; i++)
      {
        if (i > 0)
          sb.Append(",");

        CxWhereClause valueClause = HandleValue(context, inCriterion.ValueOperands[i]);
        sb.Append(valueClause.Text);
        result.Parameters.AddRange(valueClause.Parameters);
      }

      CxWhereClause property = HandleProperty(context, inCriterion.PropertyOperand);

      result.Text = string.Format("{0} in ({1})", property.Text, sb);
      result.Parameters.AddRange(property.Parameters);
      return result;

    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given group operator.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="group">a group operator to be represented</param>
    /// <returns>a where clause that represents the given group operator</returns>
    protected CxWhereClause HandleGroup(
      CxWhereClauseGenerationContext context, CxGroupOperator group)
    {
      CxWhereClause[] groupStrings = new CxWhereClause[group.Operators.Count];
      for (int i = 0; i < group.Operators.Count; i++)
      {
        groupStrings[i] = HandleInGroup(context, group.Operators[i]);
      }

      string operatorTypeString;
      switch (group.OperatorType)
      {
        case NxGroupOperatorType.And:
          operatorTypeString = "and";
          break;
        case NxGroupOperatorType.Or:
          operatorTypeString = "or";
          break;
        default:
          throw new ExException("Unknown Group operator type encountered");
      }
      CxWhereClause result = CxWhereClause.Join(" " + operatorTypeString + " ", groupStrings);
      if (!string.IsNullOrEmpty(result.Text))
        result.Text = "(" + result.Text + ")";
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given unary operator.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="unary">an unary operator to be transformed</param>
    /// <returns>a where clause that represents the given unary operator</returns>
    protected CxWhereClause HandleUnary(
      CxWhereClauseGenerationContext context, CxUnaryOperator unary)
    {
      switch (unary.OperatorType)
      {
        case NxUnaryOperatorType.Not:
          CxWhereClause clauseNot = HandleInGroup(context, unary.Operand);
          clauseNot.Text = "not " + clauseNot.Text;
          return clauseNot;
        case NxUnaryOperatorType.IsNull:
          CxWhereClause clauseIsNull = HandleInGroup(context, unary.Operand);
          clauseIsNull.Text = clauseIsNull.Text + " is null";
          return clauseIsNull;
        case NxUnaryOperatorType.BitwiseNot:
          CxWhereClause clauseBitwiseNot = HandleInGroup(context, unary.Operand);
          clauseBitwiseNot.Text = "~ " + clauseBitwiseNot.Text;
          return clauseBitwiseNot;
        default:
          throw new ExException("Unary operator type has not been recognized");
      }
    }

    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns a where clause that represents the given operator.
    /// Notice, the operator should be the one that could be included to a
    /// group operator.
    /// </summary>
    /// <param name="context">where clause generation context</param>
    /// <param name="criteria">an operator to be transformed</param>
    /// <returns>a where clause that represents the given operator</returns>
    protected CxWhereClause HandleInGroup(
      CxWhereClauseGenerationContext context, CxCriteriaOperator criteria)
    {
      if (criteria is CxGroupOperator)
        return HandleGroup(context, (CxGroupOperator) criteria);
      if (criteria is CxBinaryOperator)
        return HandleBinary(context, (CxBinaryOperator) criteria);
      if (criteria is CxUnaryOperator)
        return HandleUnary(context, (CxUnaryOperator) criteria);
      if (criteria is CxInOperator)
        return HandleInOperation(context, (CxInOperator) criteria);
      if (criteria is CxPropertyOperand)
        return HandleProperty(context, (CxPropertyOperand) criteria);
      if (criteria is CxValueOperand)
        return HandleValue(context, (CxValueOperand) criteria);

      throw new ExException("Criteria operator has not been recognized");
    }
    #endregion
    //--------------------------------------------------------------------------
    /// <summary>
    /// Generates a where clause according to the provided criteria operator.
    /// </summary>
    /// <param name="criteria">a criteria to be processed</param>
    /// <returns>a generated where clause (just conditions code, without "where")</returns>
    public CxWhereClause GetWhereClause(CxCriteriaOperator criteria)
    {
      if (criteria == null)
        return new CxWhereClause();
      CxWhereClauseGenerationContext context = new CxWhereClauseGenerationContext();
      return HandleInGroup(context, criteria);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL text for getting top N records from the original SQL statement result.
    /// </summary>
    /// <param name="sqlText">original SQL text</param>
    /// <param name="topCount">amount of top records to get</param>
    /// <returns>modified SQL text</returns>
    virtual public string GetTopRecordsSqlText(string sqlText, int topCount)
    {
      throw new NotImplementedException(
        "The GetTopRecordsSqlText method isn't implemented yet");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns lock clause for SELECT statement.
    /// </summary>
    virtual public string GetLockClauseForSelect()
    {
      throw new NotImplementedException(
        "The GetLockClauseForSelect method isn't implemented yet");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a word and its index by the position 
    /// in the input string provided.
    /// </summary>
    /// <param name="inputString">a string to be considered</param>
    /// <param name="wordPosition">the position of a word in the given string</param>
    /// <param name="word">obtains a literal value of the 
    /// word encountered at the given position</param>
    /// <param name="charIndex">obtains an index of the first character of
    /// the obtained word</param>
    /// <returns>true if the method managed to find a word at the given position,
    /// otherwise false</returns>
    protected bool GetWordByPosition(string inputString, int wordPosition, out string word, out int charIndex)
    {
      inputString = inputString.Trim();
      int currentWordIndex = -1;
      char previousChar = '\x0';
      for (int i = 0; i < inputString.Length; i++)
      {
        char currentChar = inputString[i];
        if (!char.IsWhiteSpace(currentChar) && (char.IsWhiteSpace(previousChar) || previousChar == '\x0'))
          currentWordIndex++;
        if (wordPosition == currentWordIndex)
        {
          charIndex = i;
          int wordEndIndex = inputString.Length - 1;
          for (int j = i + 1; j < inputString.Length; j++)
          {
            if (char.IsWhiteSpace(inputString[j]))
            {
              wordEndIndex = j - 1;
              break;
            }
          }
          word = inputString.Substring(charIndex, wordEndIndex - charIndex + 1);
          return true;
        }
        previousChar = currentChar;
      }
      word = string.Empty;
      charIndex = -1;
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes the TOP clause from the given select query.
    /// </summary>
    /// <param name="select">a select query to be wrapped</param>
    /// <returns>a select query without a TOP clause</returns>
    virtual public string RemoveTopClauseFromSelect(string select)
    {
      string top;
      int topIndex;
      if (GetWordByPosition(select, 1, out top, out topIndex))
      {
        if (string.Equals(top, "top", StringComparison.OrdinalIgnoreCase))
        {
          string topOperand;
          int topOparandIndex;
          if (GetWordByPosition(select, 2, out topOperand, out topOparandIndex))
          {
            return select.Remove(topIndex, topOparandIndex + topOperand.Length - topIndex + 1);
          }
        }
      }
      return select;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Converts a sort descriptor to a SQL statement.
    /// </summary>
    /// <param name="sorting">a sort descriptor to be converted</param>
    /// <returns>a SQL statement</returns>
    protected string ConvertSortDescriptorToSqlStatement(
      CxSortDescriptor sorting)
    {
      // Validation
      if (sorting == null)
        throw new ExNullArgumentException("sorting");

      return
        GetExplicitFieldName(sorting.FieldName) + " " +
        (sorting.SortDirection == ListSortDirection.Ascending ? "asc" : "desc");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Converts the given aggregate descriptors to a statement.
    /// </summary>
    /// <param name="aggregateDescriptors">an array of aggregate descriptors</param>
    /// <returns>a generated statement</returns>
    protected string ConvertAggregateDescriptorsToSqlStatement(
      CxAggregateDescriptor[] aggregateDescriptors)
    {
      List<string> groupByFields = new List<string>();
      for (int i = 0; i < aggregateDescriptors.Length; i++)
      {
        if (aggregateDescriptors[i].DescriptorType == NxAggregateDescriptorType.None)
          groupByFields.Add(aggregateDescriptors[i].FieldName);
      }
      return string.Join(",", groupByFields.ToArray());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Converts sort descriptor list to a SQL statement.
    /// </summary>
    /// <param name="sortings">a list of sort descriptors to be converted</param>
    /// <returns>a SQL statement</returns>
    protected string ConvertSortDescriptorsToSqlStatement(
      CxSortDescriptorList sortings)
    {
      // Validation
      if (sortings == null)
        throw new ExNullArgumentException("sortings");

      string sortStatement = string.Empty;
      for (int i = 0; i < sortings.Count; i++)
      {
        if (i > 0)
          sortStatement += ", ";
        sortStatement += ConvertSortDescriptorToSqlStatement(sortings[i]);
      }
      return sortStatement;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Converts sort descriptor list to a SQL statement taking 
    /// into account the given privary keys (as an additional sorting criterion).
    /// </summary>
    /// <param name="sortings">a list of sort descriptors to be converted</param>
    /// <param name="primaryKeys">an array of primary keys to be applied
    /// as sort parameters by default</param>
    /// <returns>a SQL statement</returns>
    protected string ConvertSortDescriptorsToSqlStatement(
      CxSortDescriptorList sortings, string[] primaryKeys)
    {
      CxSortDescriptorList sortBy = new CxSortDescriptorList();
      sortBy.AddRange(sortings);
      for (int i = 0; i < primaryKeys.Length; i++)
      {
        if (sortings.GetByFieldName(primaryKeys[i]) == null)
          sortBy.Add(new CxSortDescriptor(primaryKeys[i], ListSortDirection.Ascending));
      }
      return ConvertSortDescriptorsToSqlStatement(sortBy);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a string of comma-separated fields.
    /// </summary>
    /// <param name="fields">an array of field-names</param>
    /// <returns>a string of comma-separated field names</returns>
    public string GetCommaSeparatedFields(string[] fields)
    {
      string result = string.Empty;
      for (int i = 0; i < fields.Length; i++)
      {
        if (i > 0)
          result += ", ";
        result += GetExplicitFieldName(fields[i]);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets a string of comma-separated values.
    /// </summary>
    /// <param name="values">a list of values</param>
    /// <param name="startIndex">an index of values to use</param>
    /// <param name="count">an amout of values to use</param>
    /// <param name="stringBased">if true the values will be surrounded with quotes</param>
    /// <returns>a string of comma-separated values</returns>
    public string GetCommaSeparatedValues(
      IList<string> values, int startIndex, int count, bool stringBased)
    {
      if (startIndex < 0)
        throw new ArgumentException("startIndex");
      if (count <= 0)
        return string.Empty;

      StringBuilder result = new StringBuilder(count * 10);
      for (int i = startIndex; i < Math.Min(startIndex + count, values.Count); i++)
      {
        if (i > startIndex)
          result.Append(",");
        if (stringBased)
          result.AppendFormat("'{0}'", values[i]);
        else
          result.Append(values[i]);
      }
      return result.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Generates a string of comma-separated parameter names.
    /// </summary>
    /// <param name="startIndex">a start index to be used in generated names</param>
    /// <param name="count">an amount of parameter names to generate</param>
    /// <returns>a string of comma-seperated parameter names</returns>
    virtual public string GetCommaSeparatedParameters(
      int startIndex, int count)
    {
      if (startIndex < 0)
        throw new ArgumentException("startIndex");
      if (count <= 0)
        return string.Empty;

      StringBuilder result = new StringBuilder(count * 2);
      for (int i = startIndex; i < startIndex + count; i++)
      {
        if (i > startIndex)
          result.Append(",");
        result.Append(PrepareParameterSubstitute("p" + i, i));
      }
      return result.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Generates parameters for the given database command.
    /// </summary>
    /// <param name="command">a command to generate parameters for</param>
    /// <param name="values">parameter values</param>
    /// <param name="startIndex">index to start getting values from</param>
    /// <param name="count">amount of values to be set as parameters</param>
    public void GenerateParameters(
      CxDbCommand command, IList<string> values, int startIndex, int count)
    {
      CxDbConnection connection = command.Connection;

      if (startIndex < 0)
        throw new ArgumentException("startIndex");
      for (int i = startIndex; i < Math.Min(startIndex + count, values.Count); i++)
      {
        command.AddParameter(connection.CreateParameter("p" + i, values[i]));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Generates parameters for the given database command.
    /// </summary>
    /// <param name="query">a query to generate parameters for</param>
    /// <param name="values">parameter values</param>
    /// <param name="startIndex">index to start getting values from</param>
    /// <param name="count">amount of values to be set as parameters</param>
    public void GenerateParameters(
      CxQueryDescriptor query, IList<string> values, int startIndex, int count)
    {
      if (startIndex < 0)
        throw new ArgumentException("startIndex");
      for (int i = startIndex; i < Math.Min(startIndex + count, values.Count); i++)
      {
        query.Parameters.Add(new CxDbParameterDescription("p" + i, values[i]));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the field name in a format that eliminates incorrect field-name
    /// comprehension. 
    /// That means, all the field-names should be percepted by SQL server as 
    /// field-names, doesn't matter if they're equal to SQL keyword.
    /// </summary>
    /// <param name="fieldName">a field name to be corrected</param>
    /// <returns>processed field name</returns>
    virtual public string GetExplicitFieldName(string fieldName)
    {
      return fieldName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the field name cleaned up from any formatting rules applied to it to fit
    /// the target SQL engine.
    /// </summary>
    /// <param name="fieldName">a field name to be corrected</param>
    /// <returns>the pure field name</returns>
    virtual public string GetCleanFieldName(string fieldName)
    {
      return fieldName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a script which purpose is to return an empty record set 
    /// with defined columns only.
    /// </summary>
    /// <param name="query">a query to obtain columns for</param>
    /// <returns>modified script</returns>
    virtual public string GetQueryToGetColumns(
      string query)
    {
      return string.Format("select _T.* from ({0}) as _T where 1 = 2", query);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns ORDER BY query clause for the given list of sortings.
    /// </summary>
    /// <param name="sortings">list of sortings</param>
    /// <returns>ORDER BY clause or empty string</returns>
    public string GetOrderByClause(CxSortDescriptorList sortings)
    {
      return ConvertSortDescriptorsToSqlStatement(sortings);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds row count limitation to the query depending on the database type.
    /// </summary>
    /// <param name="sql">SQL SELECT statement to add limitation to</param>
    /// <param name="rowCount">number of rows to return</param>
    /// <returns>SQl SELECT statment with row count limitation</returns>
    virtual public string AddRowCountLimitation(string sql, int rowCount)
    {
      return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns substitute for the parameter in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>substitute for the parameter in the provider-dependent format</returns>
    abstract public string PrepareParameterSubstitute(string name, int index);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parameter name in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>parameter name in the provider-dependent format</returns>
    abstract public string PrepareParameterName(string name, int index);
    //----------------------------------------------------------------------------
    #endregion
  }
}
