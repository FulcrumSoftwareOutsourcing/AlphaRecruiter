using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
    public partial class CxClientAssemblyMetadata
    {
        internal CxClientAssemblyMetadata(CxAssemblyMetadata asm)
        {
            Id = asm.Id;
            Namespace = asm.Namespace;
            AssemblyName = asm["sl_assembly_name"];
            SlPluginPath = asm["sl_plugin_path"];
            FileName = asm.FileName;
        }
    }
}
