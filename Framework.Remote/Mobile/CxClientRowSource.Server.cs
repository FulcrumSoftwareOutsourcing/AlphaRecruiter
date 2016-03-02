using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote.Mobile
{
    public partial class CxClientRowSource
    {
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

            if (!string.IsNullOrEmpty(attributeMetadata.RowSourceFilter))
            {
                OwnerEntityPks = entity.PrimaryKeyInfo;
                OwnerAttributeId = attributeMetadata.Id;
                IsFilteredRowSource = true;
            }
        }

        public CxClientRowSource()
        {
        }
    }
}
