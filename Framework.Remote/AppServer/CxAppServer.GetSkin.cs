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
using System.IO;
using System.Threading;
using System.Web;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns skin by skin Id.
    /// </summary>
    /// <param name="skinId">Id of skin.</param>
    /// <returns>Found CxSkin.</returns>
    public CxSkin GetSkin(string skinId)
    {
      try
      {
       
       
        CxSlSkinMetadata skinMeta = m_Holder.SlSkins[skinId];
        string skinFolder = skinMeta["skin_folder"];
        if (string.IsNullOrEmpty(skinFolder))
        {
          skinFolder = skinMeta.Id;
        }

        string skinPath = HttpContext.Current.Server.MapPath(
          Path.Combine("SlSkins", skinFolder));
        if (!Directory.Exists(skinPath))
        {
          throw new ExException(
            string.Format("Skin folder '{0}' does not exists.", skinPath));
        }

        string skinFile = skinMeta["skin_file"];
        if(string.IsNullOrEmpty(skinFile))
        {
          skinFile = skinMeta.Id + ".dll";
        }
        string skinFullPath = Path.Combine(skinPath, skinFile);
        if (!File.Exists(skinFullPath))
        {
          throw new ExException(
            string.Format("Skin assembly '{0}' does not exists.", skinFullPath));
        }

        byte[] skinAsmBytes;
        using (FileStream fs = File.OpenRead(skinFullPath))
        {
          skinAsmBytes = new byte[fs.Length];
          fs.Read(skinAsmBytes, 0, skinAsmBytes.Length);
        }

        CxSkin skin = new CxSkin(skinMeta.Id, skinMeta.Text, skinAsmBytes, false);
        return skin;
      }
      catch (Exception ex)
      {
        CxSkin error = new CxSkin("", "", null, false);
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        error.Error = exceptionDetails;
        return error;
      }

    }
  }
}
