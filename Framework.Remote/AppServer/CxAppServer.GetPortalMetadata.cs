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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Framework.Db;
using Framework.Metadata;
using System.Linq;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns CxClientPortalMetadata by workspace Id. 
    /// </summary>
    /// <param name="workspaceId">Current workspace Id. </param>
    /// <returns>CxClientPortalMetadata</returns>
    public CxClientPortalMetadata GetPortalMetadata(object workspaceId)
    {
      try
      {
        if (workspaceId is int && (int)workspaceId != int.MinValue)
        {
          (new CxAppServerContext()).CurrentWorkspaceId = ((int)workspaceId);
        }

        List<CxClientRowSource> staticRowSourses = new List<CxClientRowSource>();

        IEnumerable<CxRowSourceMetadata> staticRsMetadataList =
          from CxRowSourceMetadata r in m_Holder.RowSources.RowSources.Values
          where string.IsNullOrEmpty(r.EntityUsageId)
          select r;

        foreach (CxRowSourceMetadata rsMetadata in staticRsMetadataList)
        {
          List<CxClientRowSourceItem> rsItems =
            (from i in rsMetadata.GetList(null, null, null, null, false)
             select new CxClientRowSourceItem(i.Description, i.Value, i.ImageReference)).ToList();

          CxClientRowSource clientRowSource = new CxClientRowSource
          {
            RowSourceId = rsMetadata.Id.ToUpper(),
            RowSourceData = rsItems
          };
          staticRowSourses.Add(clientRowSource);
        }
        
        CxClientPortalMetadata portalMetadata = new CxClientPortalMetadata(
          m_Holder.SlSections,
          staticRowSourses,
          m_Holder.Assemblies,
          m_Holder.Classes,
          m_Holder.SlFrames,
          m_Holder.Images,
          m_Holder.Constraints, 
          m_Holder);

        if (!m_Holder.IsDevelopmentMode)
        {
          CxClientSectionMetadata developerSection =
            portalMetadata.Sections.FirstOrDefault(devSect =>
              string.Compare(devSect.Id, "Development", true) == 0);

          if (developerSection != null)
            portalMetadata.Sections.Remove(developerSection);
        }

        portalMetadata.ApplicationValues = new Dictionary<string, object>();
        InitApplicationValues(portalMetadata.ApplicationValues);
        portalMetadata.MultilanguageItems = GetClientMultilanguage();

        portalMetadata.Languages = GetLanguages();
        try
        {
          portalMetadata.Skins = GetSkins();
        }
        catch (Exception ex)
        {
          CxLogger.SafeWrite(ex.ToString());
        }

        return portalMetadata;
      }
      catch (Exception ex)
      {
        CxClientPortalMetadata portalMetadata = new CxClientPortalMetadata();
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        portalMetadata.Error = exceptionDetails;
        return portalMetadata;
      }
    }

    //---------------------------------------------------------------------------
    private List<CxClientMultilanguageItem> GetClientMultilanguage()
    {
      using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
      {
        string userLang = GetUserLanguage(conn);
        DataTable mlItemsTbl = new DataTable();
        conn.GetQueryResult(mlItemsTbl,
            @"select * from Framework_LocalizationItems 
                where IsNotUsed = 0 and
                  ApplicationCd = '" + m_Holder.ApplicationCode + @"' and
                  ObjectTypeCd like 'Sl_%'");

        List<CxClientMultilanguageItem> items = new List<CxClientMultilanguageItem>();
        foreach (DataRow row in mlItemsTbl.Rows)
        {
          string objectTypeCd = row["ObjectTypeCd"].ToString();
          string propertyCd = row["PropertyCd"].ToString();
          string objectName = row["ObjectName"].ToString();
          string defaultValue = row["DefaultValue"].ToString();

          string slLocalizedValue = m_Holder.Multilanguage.GetLocalizedValue(
              userLang,
              objectTypeCd,
              propertyCd,
              objectName,
              defaultValue);

          string objectNamespace = "";
          string objectNamePart = "";
          string objectParent = "";

          if (string.Compare(objectTypeCd, "SL_Text", true) != 0)
          {
            string[] splittedName = objectName.Split('|');
            objectNamespace = splittedName[0];
            string[] parentAndName = splittedName[1].Split('.');
            objectParent = parentAndName[0];
            objectNamePart = parentAndName[1];
          }
          else
          {
            objectNamePart = objectName;
          }

          CxClientMultilanguageItem item = new CxClientMultilanguageItem(
              slLocalizedValue,
              defaultValue,
              objectTypeCd.TrimStart(new[] { 'S', 'L', '_' }),
              objectNamespace,
              objectNamePart,
              propertyCd,
              objectParent);
          items.Add(item);
        }

        return items;
      }

    }
    //---------------------------------------------------------------------------
    private string GetUserLanguage(CxDbConnection conn)
    {
        CxAppServerContext context = new CxAppServerContext();
        object userLangObj = conn.ExecuteScalar(@"select [Value] from Framework_UserSettings
                                   where UserId= " + context.UserId + @" and 
                                    ApplicationCd = '" + m_Holder.ApplicationCode + @"' and 
                                 OptionKey = 'UserLang' ");

        if (userLangObj != null && !(userLangObj is DBNull) && userLangObj is string)
           return userLangObj.ToString();

        string fromSettings = ConfigurationManager.AppSettings["localizationLanguageCode"];
        if (!string.IsNullOrEmpty(fromSettings))
          return fromSettings;
          
        return "EN";
    }
    //---------------------------------------------------------------------------
    private string GetSelectedSkin(CxDbConnection conn)
    {
      CxAppServerContext context = new CxAppServerContext();
      object selSkinObj = conn.ExecuteScalar(@"select [Value] from Framework_UserSettings
                                   where UserId= " + context.UserId + @" and 
                                    ApplicationCd = '" + m_Holder.ApplicationCode + @"' and 
                                 OptionKey = 'SkinId' ");

      if (selSkinObj != null && !(selSkinObj is DBNull) && selSkinObj is string)
        return selSkinObj.ToString();

      var defaultSkin = m_Holder.SlSkins.AllItems.FirstOrDefault(s => s.IsDefault);
      if (defaultSkin != null)
        return defaultSkin.Id;
      
      return null;
    }
    //---------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all languages.
    /// </summary>
    /// <returns>List of all languages</returns>
    public List<CxLanguage> GetLanguages()
    {
      List<CxLanguage> langs = new List<CxLanguage>();
      using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
      {
        string userLang = GetUserLanguage(conn);

        DataTable langsTbl = new DataTable();
        IDataReader reader = conn.ExecuteReader(@"select * from Framework_Languages");
        while (reader.Read())
        {
          CxLanguage lang = new CxLanguage(
              reader["LanguageCd"].ToString(),
              reader["Name"].ToString());
          if (lang.LanguageCd == userLang)
            lang.IsSelected = true;
          langs.Add(lang);
        }

      }
      return langs;
    }
    //---------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all skins.
    /// </summary>
    /// <returns>List of all skins</returns>
    public List<CxSkin> GetSkins()
    {
      string selectedSkinId;
      using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
      {
        selectedSkinId = GetSelectedSkin(conn);
      }
      List<CxSkin> skins = new List<CxSkin>();
      foreach (CxSlSkinMetadata skinMetadata in m_Holder.SlSkins.Items)
      {
        CxSkin skin = new CxSkin(
          skinMetadata.Id,
          skinMetadata.Text,
          null,
          string.Compare(skinMetadata.Id, selectedSkinId, true) == 0);

        if (skin.IsSelected)
        {
          CxSkin skinWithData = GetSkin(skinMetadata.Id);
          if (skinWithData.Error != null)
            throw new ExException(skinWithData.Error.Message);
          skin.SkinData = skinWithData.SkinData;
        }
        skins.Add(skin);
      }
      return skins;
    }
  }
}
