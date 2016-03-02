using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientParentEntity", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxClientParentEntity
  {
    [DataMember]
    public string EntityId { get; set; }
    [DataMember]
    public string EntityUsageId { get; set; }
    [DataMember]
    public string[] WhereParamNames { get; set; }
  }
}
