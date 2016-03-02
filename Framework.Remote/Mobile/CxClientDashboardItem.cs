using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientDashboardItem", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxClientDashboardItem
  {
    //----------------------------------------------------------------------------
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string EntityUsageId { get; set; }
    
    //----------------------------------------------------------------------------
    [DataMember]
    public string ImageId { get; set; }

    //----------------------------------------------------------------------------
    [DataMember]
    public string Text { get; set; }

    //----------------------------------------------------------------------------
    [DataMember]
    public string Content { get; set; }

    //----------------------------------------------------------------------------
    [DataMember]
    public string TreeItemId { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string SectionId { get; set; }
  }
}
