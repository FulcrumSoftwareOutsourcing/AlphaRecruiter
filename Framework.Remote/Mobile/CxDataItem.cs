using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxDataItem", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxDataItem
  {
    [DataMember]
    public object Value { get; set; }

    [DataMember]
    public bool Readonly { get; set; }

    [DataMember]
    public bool Visible { get; set; }

    [DataMember]
    public Dictionary<string, string> DisabledCommandIds { get; set; }
  }
}
