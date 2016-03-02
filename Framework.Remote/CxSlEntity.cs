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
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Base entity class for Silverlight applications.
  /// </summary>
  public class CxSlEntity : CxBaseEntity
  {
    /// <summary>
    /// Initializes new instance entity class for the given entity usage metadata.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxSlEntity(CxEntityUsageMetadata metadata)
      : base(metadata)
    {
    }

    //----------------------------------------------------------------------------
    public const string BLOB_PRESENT_IN_DB = "BLOB_PRESENT_IN_DB";
    public const string REMOVE_BLOB_FROM_DB = "REMOVE_BLOB_FROM_DB";
    
   
    //----------------------------------------------------------------------------
    /// <summary>
    /// Inserts entity (and all "owned" child) entities to the database.
    /// </summary>
    /// <param name="connection">connection an INSERT should work in context of</param>
    public override void Insert(CxDbConnection connection)
    {
      AssignBlobsFromCache();
      base.Insert(connection);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Updates entity (and all "owned" child entities) in the database.
    /// </summary>
    /// <param name="connection">connection an UPDATE should work in context of</param>
    public override void Update(CxDbConnection connection)
    {
      AssignBlobsFromCache();
      base.Update(connection);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets binary data from cache nt fields with type equals 'file'  
    /// </summary>
    protected virtual void AssignBlobsFromCache()
    {
      foreach (Metadata.CxAttributeMetadata atrMetadata in Metadata.Attributes)
      {
        if (IsBlobToRestoreFromCache(atrMetadata))
        {
          string cacheId = this[atrMetadata.Id].ToString();
          Cache cache = HttpContext.Current.Cache;
          if (cache[cacheId] == null)
            throw new ExException("Cached binary data is expired.");

          this[atrMetadata.Id] = ((CxUploadHandler)cache[cacheId]).GetData();
          cache.Remove(cacheId);
          this[GetBlobStateAttributeId(atrMetadata.Id)] = BLOB_PRESENT_IN_DB;
        }
        if (IsBlobToRemoveAttribute(atrMetadata))
        {
          this[atrMetadata.Id] = null;
          this[GetBlobStateAttributeId(atrMetadata.Id)] = REMOVE_BLOB_FROM_DB;
        }
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of atrtribytes with type 'file'.
    /// </summary>
    /// <param name="attributes">List of all attribytes.</param>
    /// <returns></returns>
    protected virtual IList<Metadata.CxAttributeMetadata> FilterBlobAttributes(IEnumerable<Metadata.CxAttributeMetadata> attributes)
    {
      List<Metadata.CxAttributeMetadata> updatedAttrs = new List<Metadata.CxAttributeMetadata>();

      foreach (Metadata.CxAttributeMetadata atrMetadata in attributes)
      {
        if (!IsBlobAttribute(atrMetadata))
        {
          updatedAttrs.Add(atrMetadata);
          continue;
        }
        if (IsMustBeStoredBlob(atrMetadata))
        {
          updatedAttrs.Add(atrMetadata);
          continue;          
        }
        if(IsBlobToRemoveAttribute(atrMetadata))
        {
          updatedAttrs.Add(atrMetadata);
          continue;                    
        }
      }
      return updatedAttrs;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL INSERT statement to for entity.
    /// </summary>
    /// <param name="dbObjectName">database table or view name</param>
    /// <param name="attributes">list of available attributes</param>
    /// <returns>insert SQL statement</returns>
    protected override string ComposeInsert(string dbObjectName, IList<Metadata.CxAttributeMetadata> attributes)
    {
      return base.ComposeInsert(dbObjectName, FilterBlobAttributes(attributes));
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if 'Type' property of given attribute equals 'file'.
    /// </summary>
    /// <param name="attrMetadata"></param>
    /// <returns></returns>
    private bool IsBlobAttribute(Metadata.CxAttributeMetadata attrMetadata)
    {
      return (string.Compare(attrMetadata.Type, "file") == 0) ||
        (string.Compare(attrMetadata.Type, "image") == 0);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if value of given CxAttributeMetadata means 'this blob must be stored to DB'.
    /// </summary>
    private bool IsMustBeStoredBlob(Metadata.CxAttributeMetadata attrMetadata)
    {
      if (IsBlobAttribute(attrMetadata) &&
        this[attrMetadata.Id] is byte[] &&
        ((byte[])this[attrMetadata.Id]).Length > 0)
      {
        return true;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if value of given CxAttributeMetadata means 'this blob must be removed'.
    /// </summary>
    protected bool IsBlobToRemoveAttribute(Metadata.CxAttributeMetadata attrMetadata)
    {
      if (IsBlobAttribute(attrMetadata) &&
            this[GetBlobStateAttributeId(attrMetadata.Id)] != null &&
            this[GetBlobStateAttributeId(attrMetadata.Id)].Equals(REMOVE_BLOB_FROM_DB))
      {
        return true;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if value of given CxAttributeMetadata means 'need to get binary data from Cache'.
    /// </summary>
    protected bool IsBlobToRestoreFromCache(Metadata.CxAttributeMetadata attrMetadata)
    {
      if (IsBlobAttribute(attrMetadata) &&
        this[attrMetadata.Id] is Guid)
      {
        return true;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if value of given CxAttributeMetadata
    /// means 'exclude this field from Insert and Update statements'.
    /// </summary>
    private bool IsNothingToDoBlob(Metadata.CxAttributeMetadata attrMetadata)
    {
      if (IsBlobAttribute(attrMetadata) &&
        ((this[GetBlobStateAttributeId(attrMetadata.Id)] == null) ||
        this[GetBlobStateAttributeId(attrMetadata.Id)].Equals(BLOB_PRESENT_IN_DB)))
      {
        return true;
      }
      return false;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns blob state attribute Id.
    /// </summary>
    /// <param name="blobAttributeId">Id of attribute that contains blob.</param>
    /// <returns></returns>
    public static string GetBlobStateAttributeId(string blobAttributeId)
    {
      return string.Concat(blobAttributeId, "STATE");
    }
 


  }
}
