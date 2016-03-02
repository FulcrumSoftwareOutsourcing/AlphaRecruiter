using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxExportToCsvInfo", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxExportToCsvInfo
  {
    [DataMember]
    public Guid StreamId;
    //----------------------------------------------------------------------------
    [DataMember]
    public CxExceptionDetails Error { get;  set; }
  }
}
