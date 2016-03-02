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
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Defines permission rule.
  /// </summary>
  public class CxPermissionRule : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    private List<CxEntityUsageCondition> m_EntityUsageConditions =
      new List<CxEntityUsageCondition>();
    private List<string> m_AttributeConditionIds = new List<string>();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxPermissionRule(CxMetadataHolder holder, XmlElement element)
      : base(holder, element)
    {
      AddNodeToProperties(element, "where_clause");
      AddNodeToProperties(element, "attribute_condition");
      foreach (XmlElement entityUsageElement in element.SelectNodes("entity_usage_condition"))
      {
        CxEntityUsageCondition condition = new CxEntityUsageCondition(holder, entityUsageElement);
        if (condition.IsNotEmpty)
        {
          m_EntityUsageConditions.Add(condition);
        }
      }
      m_AttributeConditionIds.AddRange(CxList.ToList<string>(CxText.DecomposeWithWhiteSpaceAndComma(AttributeCondition)));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns WHERE clause to add to entity SQL SELECT statement.
    /// </summary>
    protected string WhereClause
    { get { return this["where_clause"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns comma separated list of entity attribute names 
    /// that are required for WhereClause.
    /// </summary>
    public string AttributeCondition
    { get { return this["attribute_condition"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns rule result.
    /// </summary>
    public NxBoolEx Allow
    {
      get
      {
        var isAllow = CxBool.ParseEx(this["allow"], null);
        if (isAllow != null)
        {
          if (isAllow.Value)
            return NxBoolEx.True;
          else
            return NxBoolEx.False;
        }
        else if (CxUtils.NotEmpty(WhereClause))
        {
          return NxBoolEx.True;
        }
        else
        {
          return NxBoolEx.Undefined;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True, if rule is default.
    /// </summary>
    public bool IsDefault
    { get { return this["is_default"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given entity metadata contains all attributes listed
    /// in the attribute condition.
    /// </summary>
    /// <param name="entity">entity metadata</param>
    /// <returns></returns>
    protected bool HasAllConditionAttributes(CxEntityMetadata entity)
    {
      if (entity == null)
      {
        return false;
      }
      foreach (string attributeId in m_AttributeConditionIds)
      {
        if (entity.GetAttribute(attributeId) == null)
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage condition applicable to the given entity.
    /// </summary>
    /// <param name="entity">entity metadata</param>
    /// <returns>entity usage condition applicable to the given entity</returns>
    protected CxEntityUsageCondition GetCondition(CxEntityMetadata entity)
    {
      foreach (CxEntityUsageCondition condition in m_EntityUsageConditions)
      {
        if (condition.IsApplicableTo(entity as CxEntityUsageMetadata))
        {
          return condition;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns security SQL WHERE clause to apply to the given entity usage.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <returns>SQL WHERE clause or empty string</returns>
    public string GetWhereClause(CxEntityUsageMetadata entityUsage)
    {
      if (m_EntityUsageConditions.Count > 0)
      {
        CxEntityUsageCondition condition = GetCondition(entityUsage);
        if (condition != null)
        {
          return condition.WhereClause;
        }
      }
      if (!String.IsNullOrEmpty(WhereClause) && m_AttributeConditionIds.Count > 0)
      {
        if (HasAllConditionAttributes(entityUsage))
        {
          return WhereClause;
        }
      }
      return String.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if rule is applicable to the given entity metadata object.
    /// </summary>
    /// <param name="entity">entity to check</param>
    public bool IsApplicableTo(CxEntityMetadata entity)
    {
      if (m_EntityUsageConditions.Count == 0 && m_AttributeConditionIds.Count == 0)
      {
        return true;
      }
      if (m_EntityUsageConditions.Count > 0 && GetCondition(entity) != null)
      {
        return true;
      }
      if (m_AttributeConditionIds.Count > 0 && HasAllConditionAttributes(entity))
      {
        return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if rule is a custom (undefined) rule.
    /// </summary>
    public bool IsCustomRule
    { get { return Allow == NxBoolEx.Undefined; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if rule is an 'allow' rule.
    /// </summary>
    public bool IsAllowRule
    { get { return Allow == NxBoolEx.True && CxUtils.IsEmpty(WhereClause); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image representing rule in a grid view.
    /// </summary>
    public string ImageId
    { get { return this["image_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the image representing rule in a grid view.
    /// </summary>
    public CxImageMetadata Image
    {
      get
      {
        if (CxUtils.NotEmpty(ImageId))
        {
          return Holder.Images[ImageId];
        }
        return Holder.Security.DefaultRuleImage;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns URL to the image representing rule in a grid view.
    /// </summary>
    public string ImageUrl
    {
      get
      {
        if (Image != null)
        {
          return Image.FullPath;
        }
        return "";
      }
    }
    //-------------------------------------------------------------------------

    #region Entity Usage Condition class
    //-------------------------------------------------------------------------
    /// <summary>
    /// Permission rule entity usage condition.
    /// </summary>
    protected class CxEntityUsageCondition
    {
      //-----------------------------------------------------------------------
      private CxMetadataHolder m_Holder;
      private UniqueList<CxEntityUsageMetadata> m_EntityUsages = new UniqueList<CxEntityUsageMetadata>();
      private string m_WhereClause;
      //-----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="holder">metadata holder</param>
      /// <param name="element">XML element with the entity usage condition</param>
      public CxEntityUsageCondition(CxMetadataHolder holder, XmlElement element)
      {
        m_Holder = holder;
        XmlElement entityUsagesElement = element.SelectSingleNode("entity_usages") as XmlElement;
        if (entityUsagesElement != null)
        {
          foreach (string entityUsageId in CxText.DecomposeWithWhiteSpaceAndComma(CxText.TrimSpace(entityUsagesElement.InnerText)))
          {
            m_EntityUsages.Add(m_Holder.EntityUsages.Find(entityUsageId));
          }
        }
        XmlElement whereClauseElement = element.SelectSingleNode("where_clause") as XmlElement;
        if (whereClauseElement != null)
        {
          m_WhereClause = CxText.TrimSpace(whereClauseElement.InnerText);
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns true if condition is applicable to the given entity usage.
      /// </summary>
      /// <param name="entityUsage">entity usage</param>
      /// <returns>true if condition is applicable to the given entity usage</returns>
      public bool IsApplicableTo(CxEntityUsageMetadata entityUsage)
      {
        return entityUsage != null && m_EntityUsages.Contains(entityUsage);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Entity usages list.
      /// </summary>
      public IList<CxEntityUsageMetadata> EntityUsages
      {
        get { return m_EntityUsages; }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Where clause.
      /// </summary>
      public string WhereClause
      {
        get { return m_WhereClause; }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// True if condition is not empty.
      /// </summary>
      public bool IsNotEmpty
      {
        get { return m_EntityUsages.Count > 0 && !String.IsNullOrEmpty(m_WhereClause); }
      }
      //-----------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}