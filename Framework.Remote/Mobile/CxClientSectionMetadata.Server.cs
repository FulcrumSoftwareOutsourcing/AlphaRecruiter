using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote.Mobile
{
    public partial class CxClientSectionMetadata
    {
       
        public CxClientSectionMetadata(Metadata.CxSlSectionMetadata sectionMetadata)
        {
            if (sectionMetadata == null)
                throw new ArgumentNullException();
            if (sectionMetadata.Items == null)
                throw new ArgumentNullException();

            Id = sectionMetadata.Id;
            Text =  sectionMetadata.Text ;
            ImageId = sectionMetadata.ImageId;
            DisplayOrder = sectionMetadata.DisplayOrder;
            UiProviderClassId = sectionMetadata["ui_provider_class_id"];
            IsDefault = sectionMetadata.IsDefault;
            Visible = sectionMetadata.Visible;

            AppLogoImageId = sectionMetadata.AppLogoImageId;
            AppLogoText = sectionMetadata.AppLogoText;

            SectionFont = sectionMetadata["section_font"];
            int sectionFontSize;
            int.TryParse(sectionMetadata["section_font_size"], out sectionFontSize);
            if (sectionFontSize > 0)
                SectionFontSize = sectionFontSize;

            TreeItemsFont = sectionMetadata["tree_items_font"];
            int treeItemsFontSize;
            int.TryParse(sectionMetadata["tree_items_font_size"], out treeItemsFontSize);
            if (treeItemsFontSize > 0)
                TreeItemsFontSize = treeItemsFontSize;

            List<CxClientTreeItemMetadata> treeItems = new List<CxClientTreeItemMetadata>();
            foreach (var treeItemMetadata in sectionMetadata.Items.Items)
            {

                if (!treeItemMetadata.ItemProviderReplacement && treeItemMetadata.Visible)
                {
                    CxClientTreeItemMetadata clientTreeItemMetadata = new CxClientTreeItemMetadata(treeItemMetadata);
                    clientTreeItemMetadata.TreeItemFont = TreeItemsFont;
                    clientTreeItemMetadata.TreeItemFontSize = TreeItemsFontSize;
                    treeItems.Add(clientTreeItemMetadata);
                }
            }
            TreeItems = treeItems;

        }
    }
}
