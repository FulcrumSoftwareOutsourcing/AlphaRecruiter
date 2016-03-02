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
using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Exception raised when non-registed client attemps to access web service.
	/// </summary>
	public class ExWebServiceClientNotRegisteredException : ExWebServiceConnectException
	{
    //-------------------------------------------------------------------------
		public ExWebServiceClientNotRegisteredException() : 
      base("Web service client is not registered.")
		{
		}
    //-------------------------------------------------------------------------
  }
}