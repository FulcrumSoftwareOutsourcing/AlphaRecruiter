using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Framework.Db;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Saves user settings.
    /// </summary>
    /// <param name="settingsRequestXml">CxSettingsContainer with requst xml</param>
    /// <returns>CxSettingsContainer with error information, if occured.</returns>
    public CxSettingsContainer SaveSettings(CxSettingsContainer settingsRequestXml)
    {
      try
      {
        XElement request = XElement.Parse(settingsRequestXml.SettingsXml);
        CxAppServerContext serverContext = new CxAppServerContext();
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          CxDbSettingsStorage layoutStorage = CxDbSettingsStorage.CreateCurrentUser(
            conn,
            m_Holder.ApplicationCode,
            CxInt.Parse(serverContext.UserId, 0),
            CxDbSettingsStorage.DEFAULT_SL_OPTION_TYPE);
          
          SaveSetting(
            layoutStorage,
            request.Attribute("n").Value,
            request.Elements());

            layoutStorage.Save(conn);
        }
        return new CxSettingsContainer();
      }
      catch (Exception ex)
      {
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        return new CxSettingsContainer() { Error = exceptionDetails };
      }
    }

   
    //----------------------------------------------------------------------------
    private void SaveSetting(
      CxSettingsStorage layoutStorage,
      string section,
      IEnumerable<XElement> settinsgElements)
    {
      foreach (XElement element in settinsgElements)
      {
        string key = element.Attribute("k").Value;
        XAttribute valueAttr = element.Attribute("v");
        layoutStorage.Write(section, key, valueAttr.Value);

        SaveSetting(
          layoutStorage,
          string.Concat(section, @"\", key),
          element.Elements());
      }
    }


  }
}
