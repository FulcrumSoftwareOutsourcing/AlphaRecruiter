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
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
	/// Class that describes common-purpose error condition metadata object,
	/// containing at least 2 properties: expression and error_text.
	/// </summary>
	public class CxErrorConditionMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected CxMetadataObject m_ParentObject = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxErrorConditionMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxMetadataObject parentObject) : base (holder, element, "")
		{
      m_ParentObject = parentObject;

      if (element.SelectSingleNode("expression") != null)
      {
        AddNodeToProperties(element, "expression");
      }
      if (element.SelectSingleNode("error_text_expression") != null)
      {
        AddNodeToProperties(element, "error_text_expression");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxErrorConditionMetadata(
      CxErrorConditionMetadata sourceObject,
      CxMetadataObject parentObject) : base (sourceObject.Holder)
    {
      m_ParentObject = parentObject;
      CopyPropertiesFrom(sourceObject);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns error text. 
    /// If expression returns non-empty text, this text will be displayed.
    /// </summary>
    public string ErrorTextExpression
    { get {return this["error_text_expression"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns boolean expression. 
    /// If expression calculates to true, error text should be displayed.
    /// </summary>
    public string Expression
    { get {return this["expression"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns error text. 
    /// If expression calculates to true, error text should be displayed.
    /// </summary>
    public string ErrorText
    { get {return this["error_text"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage ID for the entity that expression should be calculated for.
    /// </summary>
    public string EntityUsageId
    { get {return this["entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata for the entity that expression should be calculated for.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    { get {return CxUtils.NotEmpty(EntityUsageId) ? Holder.EntityUsages[EntityUsageId] : null;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity ID for the entity that expression should be calculated for.
    /// </summary>
    public string EntityId
    { get {return this["entity_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity metadata for the entity that expression should be calculated for.
    /// </summary>
    public CxEntityMetadata Entity
    { get {return CxUtils.NotEmpty(EntityId) ? Holder.Entities[EntityId] : null;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if condition is active and should be checked.
    /// </summary>
    public bool IsActive
    { 
      get 
      {
        return (CxUtils.NotEmpty(ErrorTextExpression)) ||
               (CxUtils.NotEmpty(Expression) && CxUtils.NotEmpty(ErrorText));
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns key to compare expressions during expression list combine.
    /// </summary>
    public string CompareKey
    {
      get
      {
        return CxText.ToUpper(CxUtils.Nvl(ErrorTextExpression, Expression));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads list of conditions located under the given XML node.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="node">XML node</param>
    /// <param name="targetList">list to add items to</param>
    /// <param name="parentObject">metadata object condition belongs to</param>
    static public void LoadListFromNode(
      CxMetadataHolder holder,
      XmlNode node,
      IList<CxErrorConditionMetadata> targetList,
      CxMetadataObject parentObject)
    {
      if (node != null)
      {
        foreach (XmlElement element in node.SelectNodes("condition"))
        {
          CxErrorConditionMetadata metadata = new CxErrorConditionMetadata(holder, element, parentObject);
          targetList.Add(metadata);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds all elements from source list to target list, 
    /// if target list does not contain element with the same expression.
    /// </summary>
    static public void CombineLists(
      IList<CxErrorConditionMetadata> targetList,
      IList<CxErrorConditionMetadata> sourceList, 
      CxMetadataObject parentObject)
    {
      Hashtable keyMap = new Hashtable();
      foreach (CxErrorConditionMetadata metadata in targetList)
      {
        string key = metadata.CompareKey;
        keyMap[key] = true;
      }
      foreach (CxErrorConditionMetadata metadata in sourceList)
      {
        string key = metadata.CompareKey;
        if (!keyMap.ContainsKey(key))
        {
          targetList.Add(new CxErrorConditionMetadata(metadata, parentObject));
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and returns error condition metadata in the list using error metadata compare key.
    /// </summary>
    static public CxErrorConditionMetadata FindInList(
      CxErrorConditionMetadata condition,
      IList<CxErrorConditionMetadata> list)
    {
      if (list != null && condition != null)
      {
        foreach (CxErrorConditionMetadata listCondition in list)
        {
          if (listCondition.CompareKey == condition.CompareKey)
          {
            return listCondition;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.ErrorCondition";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        string name = "." + Id;
        if (m_ParentObject != null)
        {
          if (m_ParentObject is CxCommandMetadata)
          {
            name = "Command." + m_ParentObject.LocalizationObjectName + name;
          }
          else if (m_ParentObject is CxCommandGroupMetadata)
          {
            if (((CxCommandGroupMetadata)m_ParentObject).EntityMetadata != null)
            {
              name = "." + ((CxCommandGroupMetadata)m_ParentObject).EntityMetadata.LocalizationObjectName + name;
            }
            name = "CommandGroup" + name;
          }
          else if (m_ParentObject is CxWebPartMetadata)
          {
            name = "WebPart." + m_ParentObject.LocalizationObjectName + name;
          }
        }
        return name;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of metadata objects properties was inherited from.
    /// </summary>
    override public IList<CxMetadataObject> InheritanceList
    {
      get
      {
        if (m_ParentObject != null)
        {
          IList<CxMetadataObject> inheritanceList = m_ParentObject.InheritanceList;
          if (inheritanceList != null)
          {
            List<CxMetadataObject> list = new List<CxMetadataObject>();
            if (m_ParentObject is CxCommandMetadata)
            {
              foreach (CxCommandMetadata parentCommand in inheritanceList)
              {
                CxErrorConditionMetadata condition = FindInList(this, parentCommand.DisableConditions);
                if (condition != null)
                {
                  list.Add(condition);
                }
              }
            }
            else if (m_ParentObject is CxWebPartMetadata)
            {
              foreach (CxWebPartMetadata parentWebPart in inheritanceList)
              {
                CxErrorConditionMetadata condition = FindInList(this, parentWebPart.DisableConditions);
                if (condition != null)
                {
                  list.Add(condition);
                }
              }
            }
            return list;
          }
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
  }
}