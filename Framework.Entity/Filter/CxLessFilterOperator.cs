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
  /// Less filter operator.
  /// </summary>
  public class CxLessFilterOperator : CxBinaryFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxLessFilterOperator(
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
      get { return "<"; }
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
          value = CxDate.GetDateWithLowestTime(CxDate.GetFirstMonthDay(value));
        }
        else
        {
          value = CxDate.GetDateWithLowestTime(value);
        }
      }
      return value;
    }
    //-------------------------------------------------------------------------
  }
}