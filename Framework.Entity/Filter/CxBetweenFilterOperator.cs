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
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity.Filter
{
  /// <summary>
  /// Between filter operator.
  /// </summary>
  public class CxBetweenFilterOperator : CxFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxBetweenFilterOperator(
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
        return String.Format("{0} BETWEEN :{1} AND :{2}", 
                             FieldName, GetParameterName(0), GetParameterName(1));
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
      if (NotEmpty)
      {
        valueProvider[GetParameterName(0)] = GetValue(0);
        valueProvider[GetParameterName(1)] = GetValue(1);
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
        return String.Format("{0} {1} {2} {3} {4}",
                             FieldText, 
                             OperationText, 
                             GetValueText(connection, FilterElement.Values[0]), 
                             EntityUsage.Holder.GetTxt("And"),
                             GetValueText(connection, FilterElement.Values[1]));
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter parameter value.
    /// </summary>
    /// <param name="valueIndex">value index</param>
    /// <returns>parameter value</returns>
    protected override object GetValueInternal(int valueIndex)
    {
      object value = base.GetValueInternal(valueIndex);
      if (Attribute.Type == CxAttributeMetadata.TYPE_DATE ||
          Attribute.Type == CxAttributeMetadata.TYPE_DATETIME)
      {
        if (Attribute.WebControl == CxAttributeMetadata.WEB_CONTROL_MONTH ||
            Attribute.WebControl == CxAttributeMetadata.WEB_CONTROL_FUTURE_MONTH)
        {
          if (valueIndex == 0)
          {
            value = CxDate.GetDateWithLowestTime(CxDate.GetFirstMonthDay(value));
          }
          else if (valueIndex == 1)
          {
            value = CxDate.GetDateWithHighestTime(CxDate.GetLastMonthDay(value), NxMaxMilliseconds.SqlServer);
          }
        }
        else
        {
          if (valueIndex == 0)
          {
            value = CxDate.GetDateWithLowestTime(value);
          }
          else if (valueIndex == 1)
          {
            value = CxDate.GetDateWithHighestTime(value, NxMaxMilliseconds.SqlServer);
          }
        }
      }
      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if filter element is not empty.
    /// </summary>
    public bool NotEmpty
    {
      get
      {
        return FilterElement.Values.Count > 1 && 
               CxUtils.NotEmpty(FilterElement.Values[0]) &&
               CxUtils.NotEmpty(FilterElement.Values[1]);
      }
    }
    //-------------------------------------------------------------------------
  }
}