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
  /// Class to hold information about main menu item.
  /// </summary>
	public class CxMainMenuItemMetadata : CxMetadataObject
	{
    //----------------------------------------------------------------------------
    protected IList<CxMainMenuItemMetadata> m_Items = new List<CxMainMenuItemMetadata>(); // List of menu subitems
    protected Type m_UIClass = null; // Menu item user interface class
    protected int m_MainMenuImageIndex = -1; // Menu item icon index in the main menu image list
    protected CxEntityUsageMetadata[] m_DetailEntityUsages = null; // Array of detail entity usages
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxMainMenuItemMetadata(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of menu subitems.
    /// </summary>
    public IList<CxMainMenuItemMetadata> Items
    {
      get { return m_Items; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Menu item caption.
    /// </summary>
    public string Caption
    {
      get { return this["caption"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the menu item image.
    /// </summary>
    public string ImageId
    {
      get { return this["image_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the menu item user interface class.
    /// </summary>
    public string UIClassId
    {
      get { return this["ui_class_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if entity this item edits should be added to the New submenu.
    /// </summary>
    public bool AddToNew
    {
      get { return (this["add_to_new"] == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the menu item which properties are used when creating shortcuts 
    /// for the entity edited with this menu item.
    /// </summary>
    public string ShortcutMenuItemId
    {
      get { return this["shortcut_menu_item_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage menu item shows.
    /// </summary>
    public string EntityUsageId
    {
      get { return this["entity_usage_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// IDs of the detail entity usage menu item shows.
    /// </summary>
    public string DetailEntityUsageIds
    {
      get { return this["detail_entity_usage_ids"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if this menu item should be shown only for admins.
    /// </summary>
    public bool ForAdminOnly
    {
      get { return (this["for_admin_only"] == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Menu item user interface class.
    /// </summary>
    public Type UIClass
    {
      get
      {
        if (m_UIClass == null)
        {
          m_UIClass = Holder.Classes[UIClassId].Class;
        }
        return m_UIClass;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Menu item user interface class.
    /// </summary>
    public CxMainMenuItemMetadata ShorcutMenuItem
    {
      get 
      { 
        return (CxUtils.NotEmpty(ShortcutMenuItemId) ? 
                Holder.MainMenu[ShortcutMenuItemId] :
                null);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage menu item shows.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get { return Holder.EntityUsages[EntityUsageId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Detail entity usages menu item shows.
    /// </summary>
    public CxEntityUsageMetadata[] DetailEntityUsages
    {
      get 
      { 
        if (m_DetailEntityUsages == null)
        {
          IList<string> detailEntityUsagesIDs = CxText.DecomposeWithSeparator(DetailEntityUsageIds, ",");
          int count = detailEntityUsagesIDs.Count;
          m_DetailEntityUsages = new CxEntityUsageMetadata[count];
          for (int i = 0; i < count; i++)
          {
            string id = (string) detailEntityUsagesIDs[i];
            m_DetailEntityUsages[i] = Holder.EntityUsages[id];
          }
        }
        return m_DetailEntityUsages;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Image for the menu item.
    /// </summary>
    public CxImageMetadata Image
    {
      get { return Holder.Images[ImageId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Menu item icon index in the main menu image list.
    /// </summary>
    public int MainMenuImageIndex
    {
      get { return m_MainMenuImageIndex; }
      set { m_MainMenuImageIndex = value; }
    }
    //----------------------------------------------------------------------------
  }
}
