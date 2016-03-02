using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientDashboardData", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxClientDashboardData
  {
    [DataMember]
    public CxClientDashboardItem[] DashboardItems;
    
    [DataMember]
    public string Text;

    [DataMember]
    public CxExceptionDetails Error { get; set; }
  }
}
