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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns assembly.
    /// </summary>
    /// <param name="id">Assembly Id.</param>
    /// <returns>CxAssemblyContainer with assembly data.</returns>
    public CxAssemblyContainer GetAssembly(string id)
    {
      try
      {
        CxAssemblyMetadata assemblyMeta = m_Holder.Assemblies[id];
        string pluginFolder = assemblyMeta["sl_plugin_path"];
        if (string.IsNullOrEmpty(pluginFolder))
        {
          throw new ExException(
            string.Format("'sl_plugin_path' attribute is not defined for assembly with id '{0}.'", id));
        }

        string pluginPath = HttpContext.Current.Server.MapPath(
          Path.Combine("SlClientAssemblies", pluginFolder));
        string pluginFullPath = Path.Combine(pluginPath, assemblyMeta.FileName);
        if (!File.Exists(pluginFullPath))
        {
          throw new ExException(
            string.Format("Plugin client assembly '{0}' does not exists.", pluginFullPath));
        }

        CxAssemblyContainer asmContainer = new CxAssemblyContainer();
        using (FileStream fs = new FileStream(pluginFullPath, FileMode.Open, FileAccess.Read))
        {
          asmContainer.Assembly = new byte[fs.Length];
          fs.Read(asmContainer.Assembly, 0, asmContainer.Assembly.Length);
        }
        return asmContainer;
      }
      catch (Exception ex)
      {
        CxAssemblyContainer asmContainer = new CxAssemblyContainer();
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        asmContainer.Error = exceptionDetails;
        return asmContainer;
      }
    }
  }
}
