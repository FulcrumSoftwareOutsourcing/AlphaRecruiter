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

using System.Collections;
using System.Collections.Generic;

namespace Framework.Utils
{
	/// <summary>
	/// Interface to implement classes that provides values by name.
	/// </summary>
	public interface IxValueProvider
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indexed property to get or set value by name.
    /// </summary>
    object this [string name] { get; set; }
    //-------------------------------------------------------------------------

    IDictionary<string, string> ValueTypes { get; }

       

    }
}
