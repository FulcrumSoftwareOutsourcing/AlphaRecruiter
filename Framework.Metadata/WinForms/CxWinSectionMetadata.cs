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
  /// Class to hold information about Windows application navigation tree section.
  /// </summary>
  public class CxWinSectionMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected CxWinTreeItemsMetadata m_Items;
    protected Hashtable m_ItemMap = new Hashtable();
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder the object belongs to</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxWinSectionMetadata(
      CxMetadataHolder holder,
      XmlElement element)
      : base(holder, element)
    {
      m_Items = new CxWinTreeItemsMetadata(Holder, this, element);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Registers tree item in the tree items map.
    /// </summary>
    /// <param name="item">item to register</param>
    public void RegisterTreeItem(CxWinTreeItemMetadata item)
    {
      m_ItemMap.Add(item.Id.ToUpper(), item);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Registers tree item in the tree items map.
    /// </summary>
    /// <param name="item">item to register</param>
    public void UnregisterTreeItem(CxWinTreeItemMetadata item)
    {
      m_ItemMap.Remove(item.Id.ToUpper());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds tree item metadata by ID.
    /// </summary>
    /// <param name="id">ID of item to find</param>
    /// <returns>found item or null</returns>
    public CxWinTreeItemMetadata Find(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        CxWinTreeItemMetadata item = (CxWinTreeItemMetadata) m_ItemMap[id.ToUpper()];
        if (item != null && item.GetIsAllowed())
        {
          return item;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      if (m_Items != null)
      {
        if (element == null)
          throw new ExNullArgumentException("element");
        XmlNodeList xmlNodes = element.SelectNodes("tree_item");
        if (xmlNodes != null)
        {
          foreach (XmlElement itemElement in xmlNodes)
          {
            CxWinTreeItemMetadata treeItem = Find(CxXml.GetAttr(itemElement, "id"));
            if (treeItem != null)
            {
              treeItem.LoadOverride(itemElement);
            }
            else
            {
              treeItem = new CxWinTreeItemMetadata(Holder, this, itemElement);
              m_Items.Add(treeItem);
            }
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if metadata object is visible
    /// </summary>
    protected override bool GetIsVisible()
    {
      bool visible = false;
      if (Items != null && Items.Items != null)
      {
        foreach (CxWinTreeItemMetadata item in Items.Items)
        {
          visible |= item.Visible;
          // Commented since we plan to get rid of the section-level security.
          // && section.GetIsAllowed()
          if (visible)
            break;
        }
      }
      return visible && base.GetIsVisible();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns section caption translated in the context of the entity usage.
    /// </summary>
    /// <returns>section caption translated in the context of the entity usage</returns>
    public string GetCaption()
    {
      return GetCaption(null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns section caption translated.
    /// </summary>
    /// <param name="languageCd">language code to be used, null if default</param>
    /// <returns>section caption translated in the context of the entity usage</returns>
    public string GetCaption(string languageCd)
    {
      string localizedCaption = null;
      if (Holder.Multilanguage != null)
      {
        string languageCode = languageCd ?? Holder.Multilanguage.LanguageCode;
        string originalCaption = GetNonLocalizedPropertyValue("text");
        localizedCaption = Holder.Multilanguage.GetLocalizedValue(
          languageCode,
          LocalizationObjectTypeCode,
          "text",
          Id,
          originalCaption);
      }
      if (localizedCaption == null)
        localizedCaption = Text;

      localizedCaption = Holder.PlaceholderManager.ReplacePlaceholders(localizedCaption, languageCd);

      return localizedCaption;
    }
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Section root tree items collection.
    /// </summary>
    public CxWinTreeItemsMetadata Items
    { get { return m_Items; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if the section is a default section.
    /// </summary>
    public bool IsDefault
    { get { return this["is_default"] == "true"; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image to display for the tree item.
    /// </summary>
    public string ImageId
    {
      get
      {
        return this["image_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the the image to display for the tree item.
    /// </summary>
    public CxImageMetadata Image
    {
      get
      {
        if (CxUtils.NotEmpty(ImageId))
        {
          return Holder.Images[ImageId];
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks section access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WinSection";
      }
    }
    //----------------------------------------------------------------------------
    public override string GetTagName()
    {
      return "win_section";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the section is customizable.
    /// </summary>
    public bool Customizable
    {
      get { return CxBool.Parse(this["customizable"], true); }
    }
    //----------------------------------------------------------------------------
    public IList<CxWinTreeItemMetadata> AllItems
    {
      get { return GetAllSubItems(); }
    }
    //----------------------------------------------------------------------------
    private IList<CxWinTreeItemMetadata> GetAllSubItems()
    {
      IList<CxWinTreeItemMetadata> result = new List<CxWinTreeItemMetadata>();
      foreach (CxWinTreeItemMetadata treeItem in Items.Items)
      {
        result.Add(treeItem);
        AddAllSubItems(result, treeItem);
      }
      return result;
    }
    //----------------------------------------------------------------------------
    private void AddAllSubItems(IList<CxWinTreeItemMetadata> treeItemsStorage, CxWinTreeItemMetadata treeItem)
    {
      foreach (CxWinTreeItemMetadata subItem in treeItem.Items.Items)
      {
        treeItemsStorage.Add(subItem);
        AddAllSubItems(treeItemsStorage, subItem);
      }
    }
    //----------------------------------------------------------------------------
  }
}