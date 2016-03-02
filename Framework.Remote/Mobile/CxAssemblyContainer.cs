using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxAssemblyContainer", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxAssemblyContainer
  {
    /// <summary>
    /// Assembly file as binary array.
    /// </summary>
    [DataMember]
    public byte[] Assembly { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the CxExceptionDetails that contains information about occured Exception.
    /// </summary>
    [DataMember]
    public Mobile.CxExceptionDetails Error { get;  set; }
  }
}
