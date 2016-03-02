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
using System.ServiceModel;
using System.Xml.Linq;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  [ServiceContract]
  public interface IxAppServer
  {
     [OperationContract]
    CxUniformContainer DoWork(CxUniformContainer input);

    /// <summary>
    /// Returns CxClientPortalMetadata by workspace Id. 
    /// </summary>
    /// <param name="workspaceId">Current workspace Id. </param>
    /// <returns>CxClientPortalMetadata</returns>
    //[OperationContract]
    CxClientPortalMetadata GetPortalMetadata(object workspaceId);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns CxClientEntityMetadata by entity usage Id.
    /// </summary>
    /// <param name="entityUsageId">Entity Usage Id.</param>
    /// <returns>CxClientEntityMetadata</returns>
    //[OperationContract]
    CxClientEntityMetadata GetEntityMetadata(string entityUsageId);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes command.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="commandParams">Command parameters.</param>
    /// <returns>CxModel</returns>
   // [OperationContract]
    CxModel ExecuteCommand(Guid marker, CxCommandParameters commandParams);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entities.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>
  //  [OperationContract]
    CxModel GetEntityList(Guid marker, CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of child entities.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>
 //   [OperationContract]
    CxModel GetChildEntityList(Guid marker, CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity by primary keys.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>    
 //   [OperationContract]
    CxModel GetEntityFromPk(Guid marker, CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns roesources for filter form.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>    
 //   [OperationContract]
    CxModel GetFilterFormRowSources(Guid marker, CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity by primary keys.
    /// </summary>
    /// <param name="attributeId">Id of attribute that need rowsource.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>    
 //   [OperationContract]
    CxClientRowSource GetRowSource(string attributeId, CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Logouts current user.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <returns>Initialized CxModel</returns>
  //  [OperationContract]
    CxModel Logout(Guid marker);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns assembly.
    /// </summary>
    /// <param name="id">Assembly Id.</param>
    /// <returns>CxAssemblyContainer with assembly data.</returns>
  //  [OperationContract]
    CxAssemblyContainer GetAssembly(string id);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates expression.
    /// </summary>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxExpressionResult</returns>
//    [OperationContract]
    CxExpressionResult CalculateExpressions(CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Uploads data on server.
    /// </summary>
    /// <param name="uploadData">Upload data.</param>
    /// <param name="uploadParams">Upload parameters.</param>
    /// <returns>Initialized CxUploadResponse.</returns>
 //   [OperationContract]
    CxUploadResponse Upload(CxUploadData uploadData, CxUploadParams uploadParams);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Method for test ping on server. 
    /// </summary>
    /// <returns>Some number.</returns>
 //   [OperationContract]
    int Ping();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Exporets data to CSV format.
    /// </summary>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxExportToCsvInfo</returns>
//    [OperationContract]
    CxExportToCsvInfo ExportToCsv(CxQueryParams prms);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds entity to bookmarks.
    /// </summary>
    /// <param name="entityUsageId">Entity usage Id.</param>
    /// <param name="pkValues">Entity primary keys values.</param>
    /// <param name="openMode">Entity open mode.</param>
    /// <returns>Initialized CxModel</returns>
 //   [OperationContract]
    CxModel AddToBookmarks(string entityUsageId, Dictionary<string, object> pkValues, string openMode);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes all bookmarks.
    /// </summary>
    /// <returns>Initialized CxModel</returns>
//    [OperationContract]
    CxModel RemoveAllBookmarks();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes bookmark.
    /// </summary>
    ///<param name="uniqueId">Id of removed bookmark.</param>
    /// <returns>Initialized CxModel</returns>
 //   [OperationContract]
    CxModel RemoveBookmark(string uniqueId);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Clears all history.
    /// </summary>
    /// <returns>Initialized CxModel</returns>
 //   [OperationContract]
    CxModel ClearHistory();
    //----------------------------------------------------------------------------
    /// <summary>
    /// For developer mode.
    /// </summary>
 //   [OperationContract]
    string F1(string p1);

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets user settings.
    /// </summary>
    /// <param name="settingsRequestXml">CxSettingsContainer with requst xml</param>
    /// <returns>CxSettingsContainer with user settings xml</returns>
 //   [OperationContract]
    Mobile.CxSettingsContainer GetSettings(Mobile.CxSettingsContainer settingsRequestXml);

    //----------------------------------------------------------------------------
    /// <summary>
    /// Saves user settings.
    /// </summary>
    /// <param name="settingsRequestXml">CxSettingsContainer with requst xml</param>
    /// <returns>CxSettingsContainer with error information, if occured.</returns>
 //   [OperationContract]
    CxSettingsContainer SaveSettings(CxSettingsContainer settingsRequestXml);

    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes Import/Export to CSV multilanguage.
    /// </summary>
    /// <param name="commandParams">Parameters for operation(Export not translated, import translated, etc.)</param>
    /// <param name="importData">data to import translated items </param>
    /// <returns>Initialized CxExportToCsvInfo</returns>
 //   [OperationContract]
    CxExportToCsvInfo ExecuteMultilanguageCsvOperations(CxCommandParameters commandParams, string importData);

    //---------------------------------------------------------------------------
    /// <summary>
    /// Returns skin by skin Id.
    /// </summary>
    /// <param name="skinId">Id of skin.</param>
    /// <returns>Found CxSkin.</returns>
 //   [OperationContract]
    CxSkin GetSkin(string skinId);

    //---------------------------------------------------------------------------
    /// <summary>
    /// Clears all setting for logged user.
    /// </summary>
 //   [OperationContract]
    void ClearSettings();
    //---------------------------------------------------------------------------
    /// <summary>
    /// Returns dashboard items
    /// </summary>
    CxClientDashboardData GetDashboardData(string dasboardId);




  }
}
