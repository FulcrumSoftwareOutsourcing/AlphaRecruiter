using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxUploadData", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxUploadData
  {
    /// <summary>
    /// Uploading Id.
    /// </summary>
    [DataMember]
    public Guid UploadId { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Part of uploading data.
    /// </summary>
    [DataMember]
    public byte[] Data { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Number of uploading part.
    /// </summary>
    [DataMember]
    public int ChunkNumber { get; set; }
  }
}
