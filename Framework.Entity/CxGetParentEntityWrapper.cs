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
using Framework.Db;
using Framework.Metadata;

namespace Framework.Entity
{
	/// <summary>
	/// Wrapper class for getting parent entities.
	/// </summary>
	public class CxGetParentEntityWrapper
	{
    //-------------------------------------------------------------------------
    protected CxDbConnection m_Connection = null;
    protected CxBaseEntity m_Entity = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="entity"></param>
		public CxGetParentEntityWrapper(CxDbConnection connection, CxBaseEntity entity)
		{
      m_Connection = connection;
      m_Entity = entity;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent entity by the given entity usage.
    /// </summary>
    /// <param name="entityUsage"></param>
    /// <returns></returns>
    public CxBaseEntity GetParentEntity(CxEntityUsageMetadata entityUsage)
    {
      return m_Entity != null ? m_Entity.GetParentEntity(m_Connection, entityUsage) : null;
    }
    //-------------------------------------------------------------------------
  }
}