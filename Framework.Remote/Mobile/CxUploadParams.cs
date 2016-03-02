using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxUploadParams", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxUploadParams
  {
    /// <summary>
    /// File lenght.
    /// </summary>
    [DataMember]
    public long FileLenght { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage Id.
    /// </summary>
    [DataMember]
    public string EntityUsageId { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute Id.
    /// </summary>
    [DataMember]
    public string AttributeId { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// One data part size.
    /// </summary>
    [DataMember]
    public int ChunkSize { get; set; }
  }
}
