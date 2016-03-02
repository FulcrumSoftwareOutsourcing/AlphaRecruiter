/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;

namespace Framework.Web.Utils
{
	/// <summary>
	/// Wrapper class for Uri class, to get access to protected methods
	/// </summary>
	public class CxUriWrapper : Uri
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxUriWrapper(string s) : base(s)
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Escape string method wrapper.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static public string EscapeStringWrapper(string s)
    {
      return Uri.EscapeString(s);
    }
    //-------------------------------------------------------------------------
  }
}