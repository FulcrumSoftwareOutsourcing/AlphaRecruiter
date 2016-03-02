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
using System.Collections;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Cache for store previously calculated entity rules defined by WHERE clause.
	/// </summary>
	public class CxEntityRuleCache
	{
    //-------------------------------------------------------------------------
    protected Hashtable m_Cache = new Hashtable();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxEntityRuleCache()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity primary key value.
    /// </summary>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <param name="entityValueProvider">entity value provider</param>
    /// <returns>primary key value</returns>
    protected string GetEntityPkValue(
      CxEntityUsageMetadata entityUsage,
      IxValueProvider entityValueProvider)
    {
      if (entityUsage != null && entityValueProvider != null)
      {
        return entityUsage.EncodePrimaryKeyValuesAsString(entityValueProvider);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached rule calculation value.
    /// </summary>
    /// <param name="rule">rule to get value for</param>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <param name="entityValueProvider">value provider to get entity values</param>
    /// <returns>True, False or Undefined (if value was not cached)</returns>
    public NxBoolEx GetCachedValue(
      CxPermissionRule rule,
      CxEntityUsageMetadata entityUsage,
      IxValueProvider entityValueProvider)
    {
      if (rule != null && 
          entityUsage != null && 
          entityValueProvider != null)
      {
        string entityPkValue = GetEntityPkValue(entityUsage, entityValueProvider);
        if (CxUtils.NotEmpty(entityPkValue))
        {
          Hashtable entityUsageCache = (Hashtable) m_Cache[rule];
          if (entityUsageCache != null)
          {
            Hashtable entityCache = (Hashtable) entityUsageCache[entityUsage];
            if (entityCache != null)
            {
              object result = entityCache[entityPkValue];
              if (result is NxBoolEx)
              {
                return (NxBoolEx) result;
              }
            }
          }
        }
      }
      return NxBoolEx.Undefined;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves calculated value in the cache.
    /// </summary>
    /// <param name="rule">rule to set value for</param>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <param name="entityValueProvider">value provider to get entity values</param>
    /// <param name="isAllowed">calculated rule result</param>
    public void AddValueToCache(
      CxPermissionRule rule,
      CxEntityUsageMetadata entityUsage,
      IxValueProvider entityValueProvider,
      NxBoolEx isAllowed)
    {
      if (rule != null && 
          entityUsage != null && 
          entityValueProvider != null &&
          isAllowed != NxBoolEx.Undefined)
      {
        string entityPkValue = GetEntityPkValue(entityUsage, entityValueProvider);
        if (CxUtils.NotEmpty(entityPkValue))
        {
          Hashtable entityUsageCache = (Hashtable) m_Cache[rule];
          if (entityUsageCache == null)
          {
            entityUsageCache = new Hashtable();
            m_Cache[rule] = entityUsageCache;
          }
          Hashtable entityCache = (Hashtable) entityUsageCache[entityUsage];
          if (entityCache == null)
          {
            entityCache = new Hashtable();
            entityUsageCache[entityUsage] = entityCache;
          }
          entityCache[entityPkValue] = isAllowed;
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}