using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote.Mobile
{
  public partial class CxClientTreeItemMetadata
  {
    public CxClientTreeItemMetadata(CxSlTreeItemMetadata treeItemMetadata)
    {
      if (treeItemMetadata == null)
        throw new ArgumentNullException();
      if (treeItemMetadata.Items == null)
        throw new ArgumentNullException();

      Id = treeItemMetadata.Id;
      Text =  treeItemMetadata.Text ;
      ImageId = treeItemMetadata.ImageId;
      Visible = treeItemMetadata.Visible;
      Expanded = treeItemMetadata.Expanded;
      UiProviderClassId = treeItemMetadata["ui_provider_class_id"];
      EntityMetadataId = treeItemMetadata.EntityUsageId;
      FrameClassId = treeItemMetadata.FrameClassId;
      IsDefault = treeItemMetadata.DefaultSelected;
      ToolTip = treeItemMetadata["tooltip"];
      DashboardId = treeItemMetadata["dashboard_id"];
      
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
