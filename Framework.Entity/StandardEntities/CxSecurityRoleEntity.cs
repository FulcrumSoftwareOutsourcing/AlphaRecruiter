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
using Framework.Metadata;
using Framework.Utils;
using Framework.Db;

namespace Framework.Entity
{
	/// <summary>
	/// Entity for security role object.
	/// </summary>
	public class CxSecurityRoleEntity : CxBaseEntity
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxSecurityRoleEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity from the DB.
    /// </summary>
    /// <param name="connection">Db connection</param>
    override public void Delete(CxDbConnection connection)
    {
      if (CxUtils.ToString(this["SpecialCategory"]) == CxUserPermissionProvider.SC_ADMINISTRATOR)
      {
        throw new ExValidationException(
          Metadata.Holder.GetErr("Administrator role could not be deleted."));
      }
      base.Delete(connection);
    }
    //-------------------------------------------------------------------------
  }
}