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
using System.ServiceModel.Activation;
using System.Web;
using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Application service class.
  /// </summary>
  ///   

  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public abstract partial class CxAppServer : IxAppServer
  {
    private static CxSlMetadataHolder m_Holder;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the CxAppServer class.
    /// </summary>
    protected CxAppServer()
    {
      m_Holder = (CxSlMetadataHolder) HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY];

      using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
      {
        string langCode = GetUserLanguage(conn);

        if (langCode == null || string.IsNullOrEmpty(langCode))
          langCode = ConfigurationManager.AppSettings["localizationLanguageCode"];

        if (!string.IsNullOrEmpty(langCode))
          m_Holder.InitMultilanguage(conn, langCode);
        else
          m_Holder.InitMultilanguage(conn);
      }
    }


    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns WHERE condition composed from the given filter elements. 
    /// </summary>
    /// <param name="meta">entity usage</param>
    /// <param name="filterElements">list of IxFilterElement objects</param>
    /// <returns>composed WHERE clause</returns>
    virtual public string GetFilterCondition(CxEntityUsageMetadata meta, IList<IxFilterElement> filterElements)
    {
      string where = "";

      foreach (CxFilterItem element in filterElements)
      {
        element.Operation = (NxFilterOperation) Enum.Parse(typeof(NxFilterOperation), element.OperationAsString);
        CxFilterOperator filterOperator = CreateFilterOperator(meta, element);
        if (filterOperator != null)
        {
          string wherePart = filterOperator.GetCondition();
          if (CxUtils.NotEmpty(wherePart))
          {
            where += (where != "" ? " AND " : "") + wherePart;
          }
        }
      }

      return where;
    }
    //----------------------------------------------------------------------------

    private string GetWhereCondition(string filterCondition, string whereClause, string parentWhereClause)
    {
      string where = "";
      if (!string.IsNullOrEmpty(filterCondition))
      {
        where = where + " " + filterCondition + " ";
      }
      if (!string.IsNullOrEmpty(whereClause))
      {
        if (!string.IsNullOrEmpty(where))
          where += " AND ";
        where = where + " " + whereClause + " ";
      }
      if (!string.IsNullOrEmpty(parentWhereClause))
      {
        if (!string.IsNullOrEmpty(where))
          where += " AND ";
        where = where + " " + parentWhereClause + " ";
      }
      return where;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns filter operator for the given filter element.
    /// </summary>
    /// <param name="meta">entity usage</param>
    /// <param name="element">filter element</param>
    /// <returns>created filter operator or null</returns>
    virtual protected CxFilterOperator CreateFilterOperator(CxEntityUsageMetadata meta, IxFilterElement element)
    {
      return CxFilterOperator.Create(meta, element);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Updates recent items lists.
    /// </summary>
    private void UpdateRecentItems(CxDbConnection connection, CxModel model)
    {
      CxAppServerContext context = new CxAppServerContext();
      if (context.EntityMarks.OpenItems.Count > 0)
      {
        CxEntityMark openItem = context.EntityMarks.OpenItems[0];
        CxBaseEntity openEntity = openItem.CreateAndReadFromDb(connection);
        context.EntityMarks.DeleteMark(openItem);

        context.EntityMarks.AddMark(openEntity, NxEntityMarkType.Recent, true, openItem.OpenMode,
          context["APPLICATION$APPLICATIONCODE"].ToString());
        if (context.EntityMarks.RecentItems.Count > 30)
        {
          int lastItem = context.EntityMarks.RecentItems.Count - 1;
          context.EntityMarks.DeleteMark(context.EntityMarks.RecentItems[lastItem]);
        }
        context.EntityMarks.SaveAndReload(connection, m_Holder);
        CxEntityMark addedMark =
          context.EntityMarks.RecentItems[context.EntityMarks.RecentItems.Count - 1];
        model.EntityMarks = new CxClientEntityMarks();
        //model.EntityMarks.AddedRecentItems.Add(new CxClientEntityMark(addedMark));
        foreach (CxEntityMark recentItem in context.EntityMarks.RecentItems)
        {
          model.EntityMarks.AllRecentItems.Add(new CxClientEntityMark(recentItem));
        }
      }


    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds 'APPLICATION$...' values into given dictionary.
    /// </summary>
    /// <param name="toInit">Dictionary to initialize.</param>
    private void InitApplicationValues(IDictionary<string, object> toInit)
    {
      CxAppServerContext serverContext = new CxAppServerContext();
      toInit.Add("APPLICATION$USERID", serverContext["APPLICATION$USERID"]);
      toInit.Add("APPLICATION$USERNAME", serverContext["APPLICATION$USERNAME"]);
      toInit.Add("APPLICATION$USEREMAIL", serverContext["APPLICATION$USEREMAIL"]);
      toInit.Add("APPLICATION$APPLICATIONCODE", serverContext["APPLICATION$APPLICATIONCODE"]);
      toInit.Add("APPLICATION$FRAMEWORKUSERID", serverContext["APPLICATION$FRAMEWORKUSERID"]);
      toInit.Add("APPLICATION$CURRENTWORKSPACEID", serverContext["APPLICATION$CURRENTWORKSPACEID"]);
      toInit.Add("APPLICATION$LANGUAGECODE", serverContext["APPLICATION$LANGUAGECODE"]);
      toInit.Add("APPLICATION$LOCALIZATIONAPPLICATIONCODE", serverContext["APPLICATION$LOCALIZATIONAPPLICATIONCODE"]);
      toInit.Add("APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE_XML", serverContext["APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE_XML"]);
     
            
    }


    public CxUniformContainer DoWork(CxUniformContainer input)
    {
      string methodName = input[CxMobileIds.MethodName] as string;
      object[] args = (object[]) input[CxMobileIds.MethodArguments];

      CxUniformContainer response = new CxUniformContainer();
      switch (methodName)
      {
        case "AddToBookmarks":
          response[CxMobileIds.MethodResponse] = AddToBookmarks((string) args[0], (Dictionary<string, object>) args[1], (string) args[2]);
          break;
        case "CalculateExpressions":
          response[CxMobileIds.MethodResponse] = CalculateExpressions((CxQueryParams) args[0]);
          break;
        case "ClearHistory":
          response[CxMobileIds.MethodResponse] = ClearHistory();
          break;
        case "ExecuteCommand":
          response[CxMobileIds.MethodResponse] = ExecuteCommand((Guid) args[0], (CxCommandParameters) args[1]);
          break;
        case "ExecuteMultilanguageCsvOperations":
          response[CxMobileIds.MethodResponse] = ExecuteMultilanguageCsvOperations((CxCommandParameters) args[0], (string) args[1]);
          break;
        case "ExportToCsv":
          response[CxMobileIds.MethodResponse] = ExportToCsv((CxQueryParams) args[0]);
          break;
        case "F1":
          response[CxMobileIds.MethodResponse] = F1((string) args[0]);
          break;
        case "GetAssembly":
          response[CxMobileIds.MethodResponse] = GetAssembly((string) args[0]);
          break;
        case "GetChildEntityList":
          response[CxMobileIds.MethodResponse] = GetChildEntityList((Guid) args[0], (CxQueryParams) args[1]);
          break;
        case "GetEntityFromPk":
          response[CxMobileIds.MethodResponse] = GetEntityFromPk((Guid) args[0], (CxQueryParams) args[1]);
          break;
        case "GetEntityList":
          response[CxMobileIds.MethodResponse] = GetEntityList((Guid) args[0], (CxQueryParams) args[1]);
          break;
        case "GetEntityMetadata":
          response[CxMobileIds.MethodResponse] = GetEntityMetadata((string) args[0]);
          break;
        case "GetFilterFormRowSources":
          response[CxMobileIds.MethodResponse] = GetFilterFormRowSources((Guid) args[0], (CxQueryParams) args[1]);
          break;
        case "GetPortalMetadata":
          response[CxMobileIds.MethodResponse] = GetPortalMetadata(args[0]);
          break;
        case "GetRowSource":
          response[CxMobileIds.MethodResponse] = GetRowSource((string) args[0], (CxQueryParams) args[1]);
          break;
        case "GetSettings":
          response[CxMobileIds.MethodResponse] = GetSettings((CxSettingsContainer) args[0]);
          break;
        case "GetSkin":
          response[CxMobileIds.MethodResponse] = GetSkin((string) args[0]);
          break;
        case "Logout":
          response[CxMobileIds.MethodResponse] = Logout((Guid) args[0]);
          break;
        case "Ping":
          response[CxMobileIds.MethodResponse] = Ping();
          break;
        case "RemoveAllBookmarks":
          response[CxMobileIds.MethodResponse] = RemoveAllBookmarks();
          break;
        case "RemoveBookmark":
          response[CxMobileIds.MethodResponse] = RemoveBookmark((string) args[0]);
          break;
        case "SaveSettings":
          response[CxMobileIds.MethodResponse] = SaveSettings((CxSettingsContainer) args[0]);
          break;
        case "Upload":
          response[CxMobileIds.MethodResponse] = Upload((CxUploadData) args[0], (CxUploadParams) args[1]);
          break;
        case "ClearSettings":
          ClearSettingsSafe();
          break;
        case "GetDashboardItems":
          response[CxMobileIds.MethodResponse] = GetDashboardData((string)args[0]);
          break;
        default:
          throw new Exception(string.Format("Unsupported method {0}", methodName));

      }
      return response;
    }
  }
}
