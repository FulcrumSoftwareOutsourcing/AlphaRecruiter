/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
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
using System.Runtime.Serialization;

namespace Framework.Remote
{

  [DataContract]
  public sealed class CxClientSectionMetadata
  {

    [DataMember]
    public readonly string Id;

    [DataMember]
    public readonly string Text;

    [DataMember]
    public readonly string ImageId;

    [DataMember]
    public readonly bool IsDefault;

    [DataMember]
    public readonly string UiProviderClassId;

    [DataMember]
    public readonly bool Visible;

    [DataMember]
    public readonly int DisplayOrder;

    [DataMember]
    public readonly CxClientTreeItemMetadata[] TreeItems;

    [DataMember]
    public string SectionFont;

    [DataMember]
    public int SectionFontSize;

    [DataMember]
    public string TreeItemsFont;

    [DataMember]
    public int TreeItemsFontSize;

    //----------------------------------------------------------------------------
    internal CxClientSectionMetadata(Metadata.CxSlSectionMetadata sectionMetadata)
    {
      if (sectionMetadata == null)
        throw new ArgumentNullException();
      if (sectionMetadata.Items == null)
        throw new ArgumentNullException();

      Id = sectionMetadata.Id;
      Text = sectionMetadata.Text;
      ImageId = sectionMetadata.ImageId;
      DisplayOrder = sectionMetadata.DisplayOrder;
      UiProviderClassId = sectionMetadata["ui_provider_class_id"];
      IsDefault = sectionMetadata.IsDefault;
      Visible = sectionMetadata.Visible;

      SectionFont = sectionMetadata["section_font"];
      int.TryParse(sectionMetadata["section_font_size"], out SectionFontSize);

      TreeItemsFont = sectionMetadata["tree_items_font"];
      int.TryParse(sectionMetadata["tree_items_font_size"], out TreeItemsFontSize);

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
      TreeItems = treeItems.ToArray();

    }

  }

}