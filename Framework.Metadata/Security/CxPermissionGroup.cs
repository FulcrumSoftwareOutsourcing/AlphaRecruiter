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
  /// Defines permission group.
  /// </summary>
  public class CxPermissionGroup : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected List<CxPermission> m_PermissionList = new List<CxPermission>();
    protected Hashtable m_PermissionMap = new Hashtable();
    protected CxPermission m_DefaultPermission = null;
    protected CxPermission m_AllowPermission = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxPermissionGroup(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
      foreach (XmlElement permissionElement in element.SelectNodes("permission"))
      {
        CxPermission permission = new CxPermission(Holder, permissionElement, this);
        m_PermissionList.Add(permission);
        m_PermissionMap.Add(permission.Id, permission);
        if (m_DefaultPermission == null && permission.IsDefault)
        {
          m_DefaultPermission = permission;
        }
      }
      if (m_DefaultPermission == null && m_PermissionList.Count > 0)
      {
        m_DefaultPermission = m_PermissionList[0];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True, if permission group is default.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default permission option.
    /// </summary>
    public CxPermission DefaultPermission
    { get {return m_DefaultPermission;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns 'allow' permission option.
    /// </summary>
    public CxPermission AllowPermission
    { 
      get 
      {
        if (m_AllowPermission == null)
        {
          foreach (CxPermission permission in m_PermissionList)
          {
            if (permission.IsAllowPermission)
            {
              m_AllowPermission = permission;
              break;
            }
          }
        }
        return m_AllowPermission;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission object by ID.
    /// </summary>
    /// <param name="id">ID of permission option</param>
    public CxPermission GetPermission(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        return (CxPermission) m_PermissionMap[id.ToUpper()];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all available permission options.
    /// </summary>
    public IList<CxPermission> Permissions
    { get {return m_PermissionList;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom permission (with rule.allow=undefined).
    /// </summary>
    public CxPermission CustomRulePermission
    { 
      get 
      {
        foreach (CxPermission permission in m_PermissionList)
        {
          CxPermissionRule rule = permission.Rule;
          if (rule != null && rule.IsCustomRule)
          {
            return permission;
          }
        }
        return null;
      } 
    }
    //-------------------------------------------------------------------------
  }
}