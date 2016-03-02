using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxExpressionResult", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxExpressionResult
  {
    [DataMember]
    public List<CxClientAttributeMetadata> Attributes { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public Dictionary<string, object> Values { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public Dictionary<string, CxClientRowSource> UnfilteredRowSources = new Dictionary<string, CxClientRowSource>();

    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientRowSource> FilteredRowSources = new List<CxClientRowSource>();

    //----------------------------------------------------------------------------
    [DataMember]
    public CxExceptionDetails Error { get;  set; }

   
  }
}
