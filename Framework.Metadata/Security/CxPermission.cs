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
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Defines permission.
  /// </summary>
  public class CxPermission : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected CxPermissionGroup m_Group = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="group">parent permission group</param>
    public CxPermission(
      CxMetadataHolder holder, 
      XmlElement element,
      CxPermissionGroup group) : base(holder, element)
    {
      m_Group = group;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True, if permission is default.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule ID.
    /// </summary>
    public string RuleId
    { get {return this["rule_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule.
    /// </summary>
    public CxPermissionRule Rule
    {
      get
      {
        return Holder.Security.GetRule(RuleId);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if rule is applicable to the given entity metadata object.
    /// </summary>
    /// <param name="entity">entity metadata to check</param>
    public bool IsRuleApplicableTo(CxEntityMetadata entity)
    {
      CxPermissionRule rule = Rule;
      if (rule != null)
      {
        return rule.IsApplicableTo(entity);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if permission rule is an 'allow' rule.
    /// </summary>
    public bool IsAllowPermission
    { get {return Rule != null && Rule == Holder.Security.AllowRule;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the permission group to set automatically on set this permission.
    /// </summary>
    public string AutoSetPermissionGroupId
    { get { return this["auto_set_permission_group_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the permission to set automatically in the group specified above 
    /// on set this permission.
    /// </summary>
    public string AutoSetPermissionId
    { get { return this["auto_set_permission_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if auto-set permission is specified.
    /// </summary>
    public bool IsAutoSetPermissionSpecified
    { 
      get 
      { 
        return CxUtils.NotEmpty(AutoSetPermissionGroupId) &&
               CxUtils.NotEmpty(AutoSetPermissionId) &&
               Group != null &&
               !CxText.Equals(AutoSetPermissionGroupId, Group.Id);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent permission group.
    /// </summary>
    public CxPermissionGroup Group
    { get { return m_Group; } }
    //-------------------------------------------------------------------------
  }
}