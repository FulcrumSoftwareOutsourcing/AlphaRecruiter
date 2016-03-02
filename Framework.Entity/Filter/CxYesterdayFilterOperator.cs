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
  public class CxYesterdayFilterOperator : CxDateRangeFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxYesterdayFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minumum date for filtering.
    /// </summary>
    public override DateTime MinDate
    {
      get
      {
        return CxDate.GetDateWithLowestTime(DateTime.Today.AddDays(-1));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns maximum date for filtering.
    /// </summary>
    public override DateTime MaxDate
    {
      get
      {
        return CxDate.GetDateWithHighestTime(DateTime.Today.AddDays(-1), NxMaxMilliseconds.SqlServer);
      }
    }
    //-------------------------------------------------------------------------
  }
}