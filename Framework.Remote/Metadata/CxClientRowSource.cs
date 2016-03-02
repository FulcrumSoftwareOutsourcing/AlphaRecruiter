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

using System.Collections.Generic;
using System.Runtime.Serialization;

using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// RowSource container.
  /// </summary>
  [DataContract]
  public class CxClientRowSource : IxErrorContainer
  {
    [DataMember]
    public string RowSourceId { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientRowSourceItem> RowSourceData { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public Dictionary<string, object> OwnerEntityPks { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string OwnerAttributeId { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public bool IsFilteredRowSource { get; private set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the CxExceptionDetails that contains information about occured Exception.
    /// </summary>
    [DataMember]
    public CxExceptionDetails Error { get; internal set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public Dictionary<string, object> ApplicationValues = new Dictionary<string, object>();
    //----------------------------------------------------------------------------
    public CxClientRowSource(
      IEnumerable<CxComboItem> items,
      CxBaseEntity entity,
      CxAttributeMetadata attributeMetadata)
    {
      RowSourceId = attributeMetadata.RowSourceId;

      RowSourceData = new List<CxClientRowSourceItem>();
      foreach (CxComboItem item in items)
      {
        CxClientRowSourceItem rowSourceItem = new CxClientRowSourceItem
                                                {
                                                  Text = item.Description,
                                                  Value = item.Value ?? int.MinValue,
                                                  ImageId = item.ImageReference
                                                };

        RowSourceData.Add(rowSourceItem);
      }

      if(!string.IsNullOrEmpty(attributeMetadata.RowSourceFilter))
      {
        OwnerEntityPks = entity.PrimaryKeyInfo;
        OwnerAttributeId = attributeMetadata.Id;
        IsFilteredRowSource = true;
      }
    }
    //----------------------------------------------------------------------------
    public CxClientRowSource()
    {
    }
    //----------------------------------------------------------------------------
  }
}
