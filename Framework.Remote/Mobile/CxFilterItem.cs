using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxFilterItem", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxFilterItem
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string OperationAsString { get; set; }

    [DataMember]
    public IList Values { get; set; }
  }
}
