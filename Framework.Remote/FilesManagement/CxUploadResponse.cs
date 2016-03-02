/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote
{
  /// <summary>
  /// Represents server response for uploading request.
  /// </summary>
  [DataContract]
  public class CxUploadResponse : IxErrorContainer
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
    public CxExceptionDetails Error { get; internal set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Information about error, related on uploading, if occured.
    /// </summary>
    [DataMember]
    public CxExceptionDetails UploadError { get; internal set; }

  }
}
