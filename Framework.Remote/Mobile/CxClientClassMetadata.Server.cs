using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
    public partial class CxClientClassMetadata
    {
        //----------------------------------------------------------------------------
        internal CxClientClassMetadata(CxClassMetadata classMetadata)
        {
            Id = classMetadata.Id;
            AssemblyId = classMetadata.AssemblyId;
            Name = classMetadata.Name;
           
        }
    }
}
