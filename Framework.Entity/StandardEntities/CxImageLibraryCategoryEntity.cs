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
	/// Entity representing DB image library category.
	/// </summary>
	public class CxImageLibraryCategoryEntity : CxBaseEntity
	{
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxImageLibraryCategoryEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Updates entity (and all "owned" child entities) in the database.
    /// </summary>
    /// <param name="connection">connection an UPDATE should work in context of</param>
    override public void Update(CxDbConnection connection)
    {
      if (IsPredefined)
      {
        throw new ExValidationException(
          Metadata.Holder.GetErr("{0} is pre-defined and could not be modified.",
                                 new object[]{Metadata.SingleCaption}));
      }
      base.Update(connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity (and all "owned" child entities) from the database.
    /// </summary>
    /// <param name="connection">connection an DELETE should work in context of</param>
    override public void Delete(CxDbConnection connection)
    {
      if (IsPredefined)
      {
        throw new ExValidationException(
          Metadata.Holder.GetErr("{0} is pre-defined and could not be deleted.",
                                 new object[]{Metadata.SingleCaption}));
      }
      base.Delete(connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if category is a predefined category that cannot be 
    /// modified by user.
    /// </summary>
    public bool IsPredefined
    {
      get
      {
        CxAttributeMetadata codeAttr = 
          Metadata.GetAttribute(Metadata.FileLibraryCategoryCodeAttributeId);
        if (codeAttr != null)
        {
          return CxUtils.NotEmpty(this[codeAttr.Id]);
        }
        return false;
      }
    }
    //-------------------------------------------------------------------------
  }
}