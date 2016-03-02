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
  public class CxSlTreeItemMetadata: CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected CxSlSectionMetadata m_Section = null;
    protected CxSlTreeItemsMetadata m_Items = null;
    protected CxSlTreeItemMetadata m_ProviderItem = null;
    protected object m_Tag = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="section">parent section</param>
    /// <param name="id">tree item id</param>
    public CxSlTreeItemMetadata(
      CxMetadataHolder holder,
      CxSlSectionMetadata section,
      string id)
      : base(holder)
    {
      Id = id;
      m_Section = section;
      m_Section.RegisterTreeItem(this);
      m_Items = new CxSlTreeItemsMetadata(Holder, Section);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="section">parent section</param>
    /// <param name="element">XML element to load data from</param>
    public CxSlTreeItemMetadata(
      CxMetadataHolder holder,
      CxSlSectionMetadata section,
      XmlElement element)
      : base(holder, element)
    {
      m_Section = section;
      m_Section.RegisterTreeItem(this);
      m_Items = new CxSlTreeItemsMetadata(Holder, Section, element);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the entity usage linked to the tree node.
    /// </summary>
    public string EntityUsageId
    {
      get
      {
        return this["entity_usage_id"];
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Corresponding entity usage metadata object.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(EntityUsageId) ? Holder.EntityUsages[EntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the frame class to display for the content pane.
    /// </summary>
    public string FrameClassId
    {
      get
      {
        return this["frame_class_id"];
      }
    }

    /// <summary>
    /// Returns true if the Tree Item is selected by default.
    /// </summary>
    public bool DefaultSelected
    {
      get
      {
        bool defSelected;
        if (bool.TryParse(this["default_selected"], out defSelected))
          return defSelected;
        return false;
      }
    }

    /// <summary>
    /// Returns true if the Tree Item will show in dashboard.
    /// </summary>
    public bool DashboardItem
    {
      get
      {
        bool dashboardItem;
        if (bool.TryParse(this["dashboard_item"], out dashboardItem))
          return dashboardItem;
        return false;
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the frame class to display for the content pane.
    /// </summary>
    public CxClassMetadata FrameClass
    {
      get
      {
        if (CxUtils.NotEmpty(FrameClassId))
        {
          return Holder.Classes[FrameClassId];
        }
        else if (EntityUsage != null)
        {
          return EntityUsage.FrameClass;
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The value of the display order for the section.
    /// The less the value the earlier it will be placed in the list.
    /// </summary>
    public int DisplayOrder
    { get { return CxInt.Parse(this["display_order"], int.MaxValue); } }
    //-------------------------------------------------------------------------
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
        else if (EntityUsage != null)
        {
          return EntityUsage.Image;
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the tree item provider class to get child or neighbour items.
    /// </summary>
    public string ItemProviderClassId
    {
      get
      {
        return this["item_provider_class_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tree item provider class to get child or neighbour items.
    /// </summary>
    public CxClassMetadata ItemProviderClass
    {
      get
      {
        return CxUtils.NotEmpty(ItemProviderClassId) ? Holder.Classes[ItemProviderClassId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Item provider constant parameters.
    /// </summary>
    public string ItemProviderParameters
    {
      get
      {
        return this["item_provider_parameters"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, items returned by the item provider should replace this item.
    /// Otherwise, items returned by item provider are added as child items.
    /// </summary>
    public bool ItemProviderReplacement
    {
      get
      {
        return this["item_provider_replacement"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to make item initially expanded.
    /// </summary>
    public bool Expanded
    {
      get
      {
        return this["expanded"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to draw item with italic font.
    /// </summary>
    public bool Italic
    {
      get
      {
        return this["italic"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For dynamic tree items created by item provider indicates that
    /// these dynamic items should be refreshed when some listed entity
    /// usage is changed. List is comma-separated.
    /// </summary>
    public string DependsOnEntityUsageIds
    {
      get { return this["depends_on_entity_usage_ids"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of items provided by the item provider class.
    /// </summary>
    public IList<CxSlTreeItemMetadata> GetItemProviderItems()
    {
      if (ItemProviderClass != null)
      {
        Type providerType = ItemProviderClass.Class;
        IxSlTreeItemsProvider provider = (IxSlTreeItemsProvider) CxType.CreateInstance(providerType);
        IList<CxSlTreeItemMetadata> items = provider.GetItems(this);
        if (items != null)
        {
          foreach (CxSlTreeItemMetadata item in items)
          {
            item.m_ProviderItem = this;
          }
        }
        return items;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Child tree items.
    /// </summary>
    public CxSlTreeItemsMetadata Items
    { get { return m_Items; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent portal.
    /// </summary>
    public CxSlSectionMetadata Section
    { get { return m_Section; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns item that is an item provider for this item.
    /// </summary>
    public CxSlTreeItemMetadata ProviderItem
    { get { return m_ProviderItem; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets custom tag of the tree item metadata.
    /// </summary>
    public object Tag
    {
      get { return m_Tag; }
      set { m_Tag = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks tree item access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this, EntityUsage);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if metadata object is visible
    /// </summary>
    protected override bool GetIsVisible()
    {
      bool isVisible = true;
      if (EntityUsage != null)
        isVisible = EntityUsage.Visible && EntityUsage.GetIsAccessGranted();

      // Just hide the item if it has some child items but no one of them is visible.
      if (isVisible && Items != null && Items.Items != null && Items.Items.Count > 0)
      {
        bool subItemsVisible = false;
        foreach (CxSlTreeItemMetadata item in Items.Items)
        {
          if (item.Visible)
          {
            subItemsVisible = true;
            break;
          }
        }
        isVisible = subItemsVisible;
      }
      return isVisible && base.GetIsVisible();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.SlTreeItem";
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
        return Section != null ? Section.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
  }
}
