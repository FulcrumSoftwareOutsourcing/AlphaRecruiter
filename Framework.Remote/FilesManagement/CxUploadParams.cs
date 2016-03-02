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

using System.Runtime.Serialization;

namespace Framework.Remote
{
  /// <summary>
  /// Represents uploading parameters.
  /// </summary>
  [DataContract]
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
