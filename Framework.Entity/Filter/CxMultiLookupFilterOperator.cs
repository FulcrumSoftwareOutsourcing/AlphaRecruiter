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
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity.Filter
{
  public class CxMultiLookupFilterOperator : CxFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxMultiLookupFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter WHERE condition text.
    /// </summary>
    /// <returns>filter WHERE condition text</returns>
    protected internal override string GetConditionInternal()
    {
      if (NotEmpty)
      {
        IList<string> valuesList = CxText.DecomposeWithSeparator(CxUtils.ToString(FilterElement.Values[0]), ",");
        if (valuesList.Count > 1)
        {
          string sqlValuesList = "";
          foreach (string s in valuesList)
          {
            sqlValuesList += (sqlValuesList != "" ? "," : "") + CxText.GetQuotedString(s, '\'', "''");
          }
          string setOperator = "";
          switch (Operation)
          {
            case NxFilterOperation.Equal: setOperator = "IN"; break;
            case NxFilterOperation.NotEqual: setOperator = "NOT IN"; break;
            case NxFilterOperation.NotExists: setOperator = "IN"; break;
          }
          return FieldName + " " + setOperator + " (" + sqlValuesList + ")";
        }
        else
        {
          CxFilterElement filterElement;
          CxFilterOperator filterOperator = null;
          switch (Operation)
          {
            case NxFilterOperation.Equal:
              filterElement = new CxFilterElement(FilterElement, Operation, FilterElement.Values[0]);
              filterOperator = new CxEqualFilterOperator(EntityUsage, filterElement);
              break;
            case NxFilterOperation.NotEqual:
              filterElement = new CxFilterElement(FilterElement, Operation, FilterElement.Values[0]);
              filterOperator = new CxNotEqualFilterOperator(EntityUsage, filterElement);
              break;
            case NxFilterOperation.NotExists:
              filterElement = new CxFilterElement(FilterElement, NxFilterOperation.Equal, FilterElement.Values[0]);
              filterOperator = new CxEqualFilterOperator(EntityUsage, filterElement);
              break;
          }
          if (filterOperator != null)
          {
            return filterOperator.GetConditionInternal();
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes value provider.
    /// </summary>
    /// <param name="valueProvider">value provider to initialize</param>
    protected internal override void InitializeValueProviderInternal(IxValueProvider valueProvider)
    {
      // Do nothing for multiple expression, all parameters are listed in the condition as constants
      // Specify parameter only if single expression is used (one value in list)
      if (NotEmpty)
      {
        IList<string> valuesList = CxText.DecomposeWithSeparator(CxUtils.ToString(FilterElement.Values[0]), ",");
        if (valuesList.Count == 1)
        {
          valueProvider[GetParameterName(0)] = GetValue(0);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for the filter condition.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <returns>display text</returns>
    public override string GetDisplayText(CxDbConnection connection)
    {
      if (NotEmpty)
      {
        if (Operation != NxFilterOperation.NotExists)
        {
          return String.Format("{0} {1} {2}",
                               FieldText, OperationText, GetValueText(connection, FilterElement.Values[0]));
        }
        else
        {
          return String.Format("{0}: {1} {2}",
                               FieldText, OperationText, GetValueText(connection, FilterElement.Values[0]));
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if filter element is not empty.
    /// </summary>
    protected bool NotEmpty
    {
      get
      {
        return FilterElement.Values.Count > 0 && CxUtils.NotEmpty(FilterElement.Values[0]);
      }
    }
    //-------------------------------------------------------------------------
  }
}