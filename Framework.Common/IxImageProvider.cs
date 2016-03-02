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
using System.Drawing;

namespace Framework.Utils
{
	/// <summary>
	/// Interface to implement classes that provides images.
	/// </summary>
	public interface IxImageProvider
	{
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns image of the given type with the given index.
    /// </summary>
    /// <param name="type">type of the image</param>
    /// <param name="index">image index</param>
    /// <returns>image of the given type with the given index</returns>
    Image GetImage(string type, int index);
    //----------------------------------------------------------------------------
  }
}
