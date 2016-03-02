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
using System.Runtime.Serialization;


namespace Framework.Remote
{
  /// <summary>
  /// Container for uploading part of data on server.
  /// </summary>
  [DataContract]
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
