using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxSettingsContainer", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  [KnownType(typeof(Remote.Mobile.CxExceptionDetails))]
  public partial class CxSettingsContainer 
  {
    [DataMember]
    public string SettingsXml { get; set; }

    //----------------------------------------------------------------------------
    [DataMember]
    public CxExceptionDetails Error { get; set; }

   
  }
}
