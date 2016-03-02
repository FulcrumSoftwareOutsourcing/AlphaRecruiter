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
  /// Base class for Today, Yesterday, etc. filter operations
  /// </summary>
  abstract public class CxDateRangeFilterOperator : CxUnaryFilterOperator
  {
    //-------------------------------------------------------------------------
    private CxFilterOperator m_FilterOperator;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxDateRangeFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
      if (MinDate > DateTime.MinValue && MaxDate < DateTime.MaxValue)
      {
        m_FilterOperator = Create(entityUsage, 
          new CxFilterElement(filterElement, NxFilterOperation.Between, MinDate, MaxDate));
      }
      else if (MinDate > DateTime.MinValue)
      {
        m_FilterOperator = Create(entityUsage,
          new CxFilterElement(filterElement, NxFilterOperation.GreaterEqual, MinDate));
      }
      else if (MaxDate < DateTime.MaxValue)
      {
        m_FilterOperator = Create(entityUsage,
          new CxFilterElement(filterElement, NxFilterOperation.LessEqual, MaxDate));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minumum date for filtering.
    /// </summary>
    virtual public DateTime MinDate
    {
      get { return DateTime.MinValue; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns maximum date for filtering.
    /// </summary>
    virtual public DateTime MaxDate
    {
      get { return DateTime.MaxValue; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter WHERE condition text.
    /// </summary>
    /// <returns>filter WHERE condition text</returns>
    protected internal override string GetConditionInternal()
    {
      if (m_FilterOperator != null)
      {
        return m_FilterOperator.GetConditionInternal();
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
      if (m_FilterOperator != null)
      {
        m_FilterOperator.InitializeValueProviderInternal(valueProvider);
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
      return String.Format("{0} = {1}", FieldText, OperationText);
    }
    //-------------------------------------------------------------------------
  }
}