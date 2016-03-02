using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Remote
{
  [DataContract]
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
