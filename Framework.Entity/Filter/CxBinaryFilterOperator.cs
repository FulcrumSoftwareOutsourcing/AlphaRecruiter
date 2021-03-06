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
  /// Abstract class for binary filter operator.
  /// </summary>
  abstract public class CxBinaryFilterOperator : CxFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxBinaryFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL operator corresponding to the filter operation.
    /// </summary>
    abstract public string SqlOperator { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter WHERE condition text.
    /// </summary>
    /// <returns>filter WHERE condition text</returns>
    protected internal override string GetConditionInternal()
    {
      if (NotEmpty)
      {
        return String.Format("{0} {1} :{2}", FieldName, SqlOperator, GetParameterName(0));
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
        return String.Format("{0} {1} {2}",
                             FieldText, OperationText, GetValueText(connection, FilterElement.Values[0]));
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if filter element is not empty.
    /// </summary>
    public bool NotEmpty
    {
      get
      {
        return FilterElement.Values.Count > 0 && CxUtils.NotEmpty(FilterElement.Values[0]);
      }
    }
    //-------------------------------------------------------------------------
  }
}