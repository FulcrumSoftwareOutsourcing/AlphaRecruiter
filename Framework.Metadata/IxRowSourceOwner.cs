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
using System.Data;

namespace Framework.Metadata
{
	/// <summary>
	/// Interface to implement by classes who shoudl provide data for lookups.
	/// </summary>
	public interface IxRowSourceOwner
	{
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns current data table for the given entity usage.
    /// </summary>
    /// <param name="entityUsage">entity usage table should be returned for</param>
    /// <returns>current data table for the given entity usage</returns>
    DataTable GetDataTable(CxEntityUsageMetadata entityUsage);
    //--------------------------------------------------------------------------
	}
}
