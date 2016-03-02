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
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity.Filter
{
  /// <summary>
  /// Equal filter operator.
  /// </summary>
  public class CxEqualFilterOperator : CxBinaryFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxEqualFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL operator corresponding to the filter operation.
    /// </summary>
    public override string SqlOperator
    {
      get { return "="; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns mask for date condition.
    /// </summary>
    virtual protected string DateConditionMask
    {
      get { return "{0} BETWEEN :{1} AND :{2}"; }
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
        if (Attribute.Type == CxAttributeMetadata.TYPE_DATE ||
            Attribute.Type == CxAttributeMetadata.TYPE_DATETIME)
        {
          return String.Format(DateConditionMask, FieldName, GetParameterName(0), GetParameterName(1));
        }
      }
      return base.GetConditionInternal();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes value provider.
    /// </summary>
    /// <param name="valueProvider">value provider to initialize</param>
    protected internal override void InitializeValueProviderInternal(IxValueProvider valueProvider)
    {
      base.InitializeValueProviderInternal(valueProvider);

      if (NotEmpty)
      {
        if (Attribute.Type == CxAttributeMetadata.TYPE_DATE ||
            Attribute.Type == CxAttributeMetadata.TYPE_DATETIME)
        {
          if (Attribute.WebControl == CxAttributeMetadata.WEB_CONTROL_MONTH ||
              Attribute.WebControl == CxAttributeMetadata.WEB_CONTROL_FUTURE_MONTH)
          {
            valueProvider[GetParameterName(0)] = CxDate.GetDateWithLowestTime(CxDate.GetFirstMonthDay(GetValue(0)));
            valueProvider[GetParameterName(1)] = CxDate.GetDateWithHighestTime(CxDate.GetLastMonthDay(GetValue(0)), NxMaxMilliseconds.SqlServer);
          }
          else
          {
            valueProvider[GetParameterName(0)] = CxDate.GetDateWithLowestTime(GetValue(0));
            valueProvider[GetParameterName(1)] = CxDate.GetDateWithHighestTime(GetValue(0), NxMaxMilliseconds.SqlServer);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}
