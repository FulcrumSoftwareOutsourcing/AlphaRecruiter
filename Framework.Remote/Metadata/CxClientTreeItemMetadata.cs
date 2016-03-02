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
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  [DataContract]
  public sealed class CxClientTreeItemMetadata
  {

    [DataMember]
    public readonly string Id;

    [DataMember]
    public readonly string Text;

    [DataMember]
    public readonly bool Visible;

    [DataMember]
    public readonly string ImageId;

    [DataMember]
    public readonly string EntityMetadataId;

    [DataMember]
    public readonly string FrameClassId;

    [DataMember]
    public readonly string UiProviderClassId;

    [DataMember]
    public readonly bool Expanded;

    [DataMember]
    public readonly CxClientTreeItemMetadata[] TreeItems;

    [DataMember]
    public readonly bool IsDefault;

    [DataMember]
    public string TreeItemFont;

    [DataMember]
    public int TreeItemFontSize;

    [DataMember]
    public string ToolTip;
    //----------------------------------------------------------------------------
    internal CxClientTreeItemMetadata(CxSlTreeItemMetadata treeItemMetadata)
    {
      if (treeItemMetadata == null)
        throw new ArgumentNullException();
      if (treeItemMetadata.Items == null)
        throw new ArgumentNullException();

      Id = treeItemMetadata.Id;
      Text = treeItemMetadata.Text;
      ImageId = treeItemMetadata.ImageId;
      Visible = treeItemMetadata.Visible;
      Expanded = treeItemMetadata.Expanded;
      UiProviderClassId = treeItemMetadata["ui_provider_class_id"];
      EntityMetadataId = treeItemMetadata.EntityUsageId;
      FrameClassId = treeItemMetadata.FrameClassId;
      IsDefault = CxBool.Parse(treeItemMetadata["sl_is_default"], false);
      ToolTip = treeItemMetadata["tooltip"];

      List<CxClientTreeItemMetadata> treeItems = new List<CxClientTreeItemMetadata>();
      foreach (CxSlTreeItemMetadata treeItem in treeItemMetadata.Items.Items)
      {

        if (!treeItem.ItemProviderReplacement && treeItem.Visible)
          treeItems.Add(new CxClientTreeItemMetadata(treeItem));
      }
      TreeItems = treeItems.ToArray();


    }

  }
}