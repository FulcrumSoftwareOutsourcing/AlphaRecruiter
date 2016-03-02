using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxUploadResponse", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public class CxUploadResponse
  {
    /// <summary>
    /// Uploading Id.
    /// </summary>
    [DataMember]
    public Guid UploadId { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Number of uploading part.
    /// </summary>
    [DataMember]
    public int ChunkNumber { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Check sum for uploaded part.
    /// </summary>
    [DataMember]
    public byte[] CheckSum { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Information about error, if occured.
    /// </summary>
    [DataMember]
    public CxExceptionDetails Error { get;  set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Information about error, related on uploading, if occured.
    /// </summary>
    [DataMember]
    public CxExceptionDetails UploadError { get;  set; }
  }
}
