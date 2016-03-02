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

using System.Xml;
using System.Collections.Generic;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Interface for custom metadata provider.
	/// </summary>
	public interface IxCustomMetadataProvider
	{
    //-------------------------------------------------------------------------
    IDictionary<string, IDictionary<string, XmlDocument>> GetCustomMetadata(IxValueProvider valueProvider);
    //-------------------------------------------------------------------------
  }
}