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

namespace Framework.Metadata
{
	/// <summary>
	/// Interfaceto implement by entity usage chooser classes.
	/// </summary>
	public interface IxEntityUsageChooser
	{
    //--------------------------------------------------------------------------
    /// <summary>
    /// Selects entity usage metadata by the given values.
    /// </summary>
    /// <param name="values">list of values to select by</param>
    /// <param name="context">calling context (database connection ID, etc.)</param>
    /// <returns>entity usage metadata selected by the given values</returns>
    CxEntityUsageMetadata Choose(object context, params object[] values);
    //--------------------------------------------------------------------------
  }
}
