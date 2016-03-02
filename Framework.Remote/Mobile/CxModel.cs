using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxModel", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxModel
  {
    [DataMember]
    public Guid Marker;

    [DataMember]
    public string EntityUsageId;

    [DataMember]
    public List<CxDataItem> Data;

    [DataMember]
    public int TotalDataRecordAmount;

    [DataMember]
    public CxSortDescription[] SortDescriptions = new CxSortDescription[] { };

    [DataMember]
    public Dictionary<string, CxClientRowSource> UnfilteredRowSources = new Dictionary<string, CxClientRowSource>();

    [DataMember]
    public List<CxClientRowSource> FilteredRowSources = new List<CxClientRowSource>();

    [DataMember]
    public CxExceptionDetails Error { get;  set; }

    [DataMember]
    public bool IsNewEntity { get;  set; }

    [DataMember]
    public Dictionary<string, object> ApplicationValues = new Dictionary<string, object>();

    [DataMember]
    public CxClientEntityMarks EntityMarks { get; set; }

  
  
    //-------------------------------------------------------------------------
  }
}
