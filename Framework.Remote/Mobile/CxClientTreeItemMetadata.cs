using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientTreeItemMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxClientTreeItemMetadata
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

    [DataMember]
    public string DashboardId;


  }
}
