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
using System.Collections.Generic;

using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Class to hold entity instance data in a portable format.
  /// </summary>
  [Serializable]
  public class CxEntityData
  {
    //-------------------------------------------------------------------------
    public const string FORMAT_NAME = "Framework.Entity.Data";
    //-------------------------------------------------------------------------
    protected string m_EntityUsageId = null;
    protected string[] m_Names = null;
    protected object[] m_Values = null;
    protected bool m_IsCutRequired = false;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityData()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityData(CxBaseEntity entity)
    {
      m_EntityUsageId = entity.Metadata.Id;

      IList<CxAttributeMetadata> attributes = entity.Metadata.Attributes;
      m_Names = new string[attributes.Count];
      m_Values = new object[attributes.Count];
      for (int i = 0; i < attributes.Count; i++)
      {
        m_Names[i] = attributes[i].Id;
        m_Values[i] = entity[m_Names[i]];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage ID.
    /// </summary>
    public string EntityUsageId
    { get { return m_EntityUsageId; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns names of entity properties.
    /// </summary>
    public string[] Names
    { get { return m_Names; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns values of entity properties.
    /// </summary>
    public object[] Values
    { get { return m_Values; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity cut is required.
    /// </summary>
    public bool IsCutRequired
    { get { return m_IsCutRequired; } set { m_IsCutRequired = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider to get entity values.
    /// </summary>
    public IxValueProvider ValueProvider
    {
      get
      {
        CxHashtable provider = new CxHashtable();
        if (m_Names != null && m_Values != null)
        {
          for (int i = 0; i < m_Names.Length && i < m_Values.Length; i++)
          {
            provider[m_Names[i]] = m_Values[i];
          }
        }
        return provider;
      }
    }
    //-------------------------------------------------------------------------
  }
}