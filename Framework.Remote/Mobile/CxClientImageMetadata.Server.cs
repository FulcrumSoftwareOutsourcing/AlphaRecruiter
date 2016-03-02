using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
    public partial class CxClientImageMetadata
    {
        internal CxClientImageMetadata(CxImageMetadata imageMetadata)
        {
            Id = imageMetadata.Id;
            ProviderClassId = imageMetadata.ImageProviderClassId;
            ImageIndex = imageMetadata.ImageIndex;
            Folder = imageMetadata.Folder;
            FileName = imageMetadata.FileName;
        }
    }
}
