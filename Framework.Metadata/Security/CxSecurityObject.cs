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

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Defines security schema for metadata object
  /// </summary>
  public class CxSecurityObject : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected List<CxPermissionGroup> m_PermissionGroupList = new List<CxPermissionGroup>();
    protected Hashtable m_PermissionGroupMap = new Hashtable();
    protected List<CxEntityGroup> m_EntityGroupList = new List<CxEntityGroup>();
    protected CxPermissionGroup m_DefaultPermissionGroup = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxSecurityObject(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
      XmlElement groupsElement = 
        (XmlElement) element.SelectSingleNode("permission_groups");
      if (groupsElement != null)
      {
        foreach (XmlElement groupElement in groupsElement)
        {
          CxPermissionGroup group = new CxPermissionGroup(Holder, groupElement);
          m_PermissionGroupList.Add(group);
          m_PermissionGroupMap.Add(group.Id, group);
          if (group.IsDefault && m_DefaultPermissionGroup == null)
          {
            m_DefaultPermissionGroup = group;
          }
        }
      }

      if (m_DefaultPermissionGroup == null && m_PermissionGroupList.Count > 0)
      {
        m_DefaultPermissionGroup = m_PermissionGroupList[0];
      }

      groupsElement = 
        (XmlElement) element.SelectSingleNode("entity_groups");
      if (groupsElement != null)
      {
        foreach (XmlElement groupElement in groupsElement)
        {
          CxEntityGroup group = new CxEntityGroup(Holder, groupElement);
          m_EntityGroupList.Add(group);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission group by it's ID.
    /// </summary>
    /// <param name="id">permission group ID</param>
    public CxPermissionGroup GetPermissionGroup(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        return (CxPermissionGroup) m_PermissionGroupMap[id.ToUpper()];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity group for the given metadata object.
    /// </summary>
    /// <param name="metadataObject">metadata object to get entity group for</param>
    public CxEntityGroup GetEntityGroup(CxMetadataObject metadataObject)
    {
      foreach (CxEntityGroup group in m_EntityGroupList)
      {
        if (group.GetIsMatch(metadataObject))
        {
          return group;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default permission group.
    /// </summary>
    public CxPermissionGroup DefaultPermissionGroup
    { get {return m_DefaultPermissionGroup;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of permission groups.
    /// </summary>
    public IList<CxPermissionGroup> PermissionGroups
    { get {return m_PermissionGroupList;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity groups.
    /// </summary>
    public IList<CxEntityGroup> EntityGroups
    { get {return m_EntityGroupList;} }
    //-------------------------------------------------------------------------
  }
}