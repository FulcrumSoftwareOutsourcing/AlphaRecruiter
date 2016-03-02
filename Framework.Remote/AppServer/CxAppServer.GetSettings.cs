using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Framework.Db;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Gets user settings.
    /// </summary>
    /// <param name="settingsRequestXml">CxSettingsContainer with requst xml</param>
    /// <returns>CxSettingsContainer with user settings xml</returns>
    public Mobile.CxSettingsContainer GetSettings(Mobile.CxSettingsContainer settingsRequestXml)
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

          bool wasWritten = false;
          ReadSetting(
            layoutStorage,
            request.Elements(),
            request.Attribute("n").Value,
            ref wasWritten);          

          if(wasWritten)
            layoutStorage.Save(conn);
        }
        return new Mobile.CxSettingsContainer() { SettingsXml = request.ToString() };
      }
      catch (Exception ex)
      {
        Mobile.CxExceptionDetails exceptionDetails = new Mobile.CxExceptionDetails(ex);
        return new Mobile.CxSettingsContainer() { Error = exceptionDetails };
      }
    }

    //----------------------------------------------------------------------------
    private void ReadSetting(
      CxSettingsStorage layoutStorage, 
      IEnumerable<XElement> settinsgElements,
      string section,
      ref bool wasWritten)
    {
      List<string> sectionNames = new List<string>();
      sectionNames.AddRange(layoutStorage.GetNames(section) ?? new List<string>());
      sectionNames.AddRange(layoutStorage.GetFolderNames(section) ?? new List<string>());

      List<string> loweredNames = new List<string>();
      foreach (string sectionName in sectionNames)
      {
        loweredNames.Add(sectionName.ToLower());
      }

      foreach (XElement element in settinsgElements)
      {
        string key = element.Attribute("k").Value;
        XAttribute valueAttr = element.Attribute("v");
        if (!loweredNames.Contains(key.ToLower()))
        {
          layoutStorage.Write(section, key, valueAttr.Value);
          wasWritten = true;
          valueAttr.Remove();
          ReadSetting(
            layoutStorage,
            element.Elements(),
            string.Concat(section, @"\", key),
            ref wasWritten);
          continue;
        }
        string fromStorage = layoutStorage.Read(section, key, valueAttr.Value);
        if (valueAttr.Value == fromStorage)
        {
          valueAttr.Remove();
        }
        else
        {
          valueAttr.Value = fromStorage;  
        }
        ReadSetting(
          layoutStorage,
          element.Elements(),
          string.Concat(section, @"\", key),
          ref wasWritten);
      }
    }
  }
}
