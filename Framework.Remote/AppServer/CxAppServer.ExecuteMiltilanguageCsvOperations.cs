using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Executes Import/Export to CSV multilanguage.
    /// </summary>
    /// <param name="commandParams">Parameters for operation(Export not translated, import translated, etc.)</param>
    /// <returns>Initialized CxExportToCsvInfo</returns>
    public CxExportToCsvInfo ExecuteMultilanguageCsvOperations(CxCommandParameters commandParams,
      string importData)
    {
      CxCommandData commandData = new CxCommandData();
      IxValueProvider entityValueProvider = CxQueryParams.CreateValueProvider(commandParams.CurrentEntity);

      CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[commandParams.EntityUsageId];
      CxCommandMetadata commandMetadata = entityUsage.GetCommand(commandParams.CommandId);
      if (commandMetadata != null && !string.IsNullOrEmpty(commandMetadata.EntityUsageId))
      {
        entityUsage = m_Holder.EntityUsages[commandMetadata.EntityUsageId];
      }

      if (!GetIsCommandEnabled(commandParams.CommandId, entityUsage, commandMetadata, entityValueProvider))
      {
        throw new ExException("The command you're trying to execute is not applicable in the current context");
      }

      commandData.CommandId = commandParams.CommandId;
      commandData.EntityUsage = entityUsage;
      commandData.Command = commandMetadata;
      commandData.QueryParams = commandParams.QueryParams;
      commandData.IsNewEntity = commandParams.IsNewEntity;

      commandData.CurrentEntity = CxBaseEntity.CreateByValueProvider(
          entityUsage,
          entityValueProvider);
      commandData.QueryParams = commandParams.QueryParams;


      try
      {
        Guid csvId = Guid.Empty;
        switch (commandData.CommandId)
        {
          case CxCommandIDs.LOCALIZATION_EXPORT_NON_TRANSLATED:
            csvId = ExportNonTranslated(commandData);
            break;
          case CxCommandIDs.LOCALIZATION_EXPORT_TRANSLATED:
            //ExportTranslated(commandController, commandData);
            break;
          case CxCommandIDs.LOCALIZATION_IMPORT_TRANSLATED:
            ImportTranslated(commandData, importData);
            break;
        }



       

        return new CxExportToCsvInfo() { StreamId = csvId };
      }
      catch (Exception ex)
      {
        CxExportToCsvInfo emptyInfo = new CxExportToCsvInfo { Error = new CxExceptionDetails(ex) };
        return emptyInfo;
      }
    }

     //-------------------------------------------------------------------------
    /// <summary>
    /// Exports non-translated localization items.
    /// </summary>
    /// <param name="commandData">prepared command data</param>
    /// <returns> Id of cashed csv data.</returns>
    protected Guid ExportNonTranslated(
      CxCommandData commandData)
    {
        DataTable dt = new DataTable();
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          m_Holder.Multilanguage.ExportNonTranslatedItems(
            connection,
            dt,
            GetLanguageCode(commandData),
            m_Holder.ApplicationCode);
        }
        string csvContent = CxCSV.DataTableToCsv(dt, null, true, null, null);

        Guid csvId = Guid.NewGuid();
        TimeSpan timeoutSpan = new TimeSpan(0, 0, 0, 0, 60000);
        Cache cache = HttpContext.Current.Cache;
        cache.Insert(
          csvId.ToString(),
          csvContent,
          null,
          Cache.NoAbsoluteExpiration,
          timeoutSpan,
          CacheItemPriority.NotRemovable,
          null);
        return csvId;
      }
  
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns language code for the given command data.
    /// </summary>
    /// <param name="commandData">command data to get language code from</param>
    /// <returns>language code</returns>
    protected string GetLanguageCode(CxCommandData commandData)
    {
      string languageCode = m_Holder.LanguageCode;
      if (commandData != null &&
          commandData.CurrentEntity != null &&
          CxUtils.NotEmpty(commandData.CurrentEntity["LanguageCd"]))
      {
        languageCode = CxUtils.ToString(commandData.CurrentEntity["LanguageCd"]);
      }
      return languageCode;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Imports translated localization items.
    /// </summary>
    /// <param name="commandData">prepared command data</param>
    /// <returns>command execution result</returns>
    protected void ImportTranslated(
      CxCommandData commandData, 
      string csvContent)
    {
        XmlDocument xmlDesc = CxXml.LoadXmlFromResource(
          GetType().Assembly, "LocalizationImportFileDescription.xml", null);
        CxTextFileLoader loader = new CxTextFileLoader( xmlDesc);
        loader.Separator = CxCSV.ListSeparator;
        loader.TextQualifier = CxCSV.TextQualifier;
        DataTable dt = loader.LoadData(csvContent);
        
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          string importLog;
          m_Holder.Multilanguage.ImportTranslatedItems(
            connection,
            dt,
            GetLanguageCode(commandData),
            m_Holder.ApplicationCode,
            true,
            out importLog);
        }
        
      }
      
    }

 }



  

