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

namespace Framework.Metadata
{
    public class CxSlSectionMetadata : CxMetadataObject
    {
        //----------------------------------------------------------------------------
        private CxSlTreeItemsMetadata m_Items = null;
        private Hashtable m_ItemMap = new Hashtable();
        //----------------------------------------------------------------------------

        //----------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">XML element that holds metadata</param>
        public CxSlSectionMetadata(
          CxMetadataHolder holder,
          XmlElement element) : base(holder, element)
        {
            m_Items = new CxSlTreeItemsMetadata(Holder, this, element);
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Registers tree item in the tree items map.
        /// </summary>
        /// <param name="item">item to register</param>
        public void RegisterTreeItem(CxSlTreeItemMetadata item)
        {
            m_ItemMap.Add(item.Id.ToUpper(), item);
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Registers tree item in the tree items map.
        /// </summary>
        /// <param name="item">item to register</param>
        public void UnregisterTreeItem(CxSlTreeItemMetadata item)
        {
            m_ItemMap.Remove(item.Id.ToUpper());
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Finds tree item metadata by ID.
        /// </summary>
        /// <param name="id">ID of item to find</param>
        /// <returns>found item or null</returns>
        public CxSlTreeItemMetadata Find(string id)
        {
            if (CxUtils.NotEmpty(id))
            {
                CxSlTreeItemMetadata item = (CxSlTreeItemMetadata)m_ItemMap[id.ToUpper()];
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
                foreach (XmlElement itemElement in element.SelectNodes("tree_item"))
                {
                    CxSlTreeItemMetadata treeItem = Find(CxXml.GetAttr(itemElement, "id"));
                    if (treeItem != null)
                    {
                        treeItem.LoadOverride(itemElement);
                    }
                    else
                    {
                        treeItem = new CxSlTreeItemMetadata(Holder, this, itemElement);
                        m_Items.Add(treeItem);
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
                foreach (CxSlTreeItemMetadata item in Items.Items)
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

        //----------------------------------------------------------------------------
        /// <summary>
        /// Section root tree items collection.
        /// </summary>
        public CxSlTreeItemsMetadata Items
        { get { return m_Items; } }
        //----------------------------------------------------------------------------
        /// <summary>
        /// True if the section is a default section.
        /// </summary>
        public bool IsDefault
        { get { return this["is_default"] == "true"; } }
        //----------------------------------------------------------------------------
        /// <summary>
        /// The value of the display order for the section.
        /// The less the value the earlier it will be placed in the list.
        /// </summary>
        public int DisplayOrder
        { get { return CxInt.Parse(this["display_order"], int.MaxValue); } }

        public string AppLogoImageId
        { get { return this["app_logo_image_id"]; } }

        public string AppLogoText
        { get { return this["app_logo_text"]; } }

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
                return "Metadata.SlSection";
            }
        }
        //----------------------------------------------------------------------------
    }
}
