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

using Framework.Metadata;

namespace Framework.Remote
{
  [DataContract]
  public class CxClientAssemblyMetadata
  {
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Id;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Namespace;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string AssemblyName;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string SlPluginPath;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string FileName;
    //----------------------------------------------------------------------------
    public CxClientAssemblyMetadata()
    {
    }
    //----------------------------------------------------------------------------
    internal CxClientAssemblyMetadata(CxAssemblyMetadata asm)
    {
      Id = asm.Id;
      Namespace = asm.Namespace;
      AssemblyName = asm["sl_assembly_name"];
      SlPluginPath = asm["sl_plugin_path"];
      FileName = asm.FileName;
    }
    //----------------------------------------------------------------------------
  }
}
