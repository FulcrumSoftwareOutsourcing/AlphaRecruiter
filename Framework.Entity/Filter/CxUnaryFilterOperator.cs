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
  /// Unary filter operator base abstract class
  /// </summary>
  abstract public class CxUnaryFilterOperator : CxFilterOperator
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    public CxUnaryFilterOperator(
      CxEntityUsageMetadata entityUsage, 
      IxFilterElement filterElement) : base(entityUsage, filterElement)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes value provider.
    /// </summary>
    /// <param name="valueProvider">value provider to initialize</param>
    protected internal override void InitializeValueProviderInternal(IxValueProvider valueProvider)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for the filter condition.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <returns>display text</returns>
    public override string GetDisplayText(CxDbConnection connection)
    {
      return String.Format("{0} {1}", FieldText, OperationText);
    }
    //-------------------------------------------------------------------------
  }
}