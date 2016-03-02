/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Interface can be implemented by the class specified as entity_class_id
  /// to receive different events by the actual entity class.
  /// </summary>
  public interface IxEntity: IxDefinition
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata the entity based on.
    /// </summary>
    CxEntityUsageMetadata Metadata { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invoked immediately after a data source has been initiated.
    /// </summary>
    /// <param name="dataSource">a data source containing entity or entity list</param>
    /// <param name="customHandler">a custom pre-processing handler to be applied to the data-source</param>
    void PreProcessDataSource(IxGenericDataSource dataSource, DxCustomPreProcessDataSource customHandler);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invoked immediately after a data row has been initiated.
    /// </summary>
    /// <param name="dataRow">a data row containing entity</param>
    void PreProcessDataRow(CxGenericDataRow dataRow);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom WHERE clause.
    /// </summary>
    string GetCustomWhereClause();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom workspace filter WHERE clause 
    /// or null if custom workspace clause is not needed.
    /// </summary>
    /// <param name="defaultWorkspaceClause">default workspace WHERE clause</param>
    /// <returns>custom clause or null if no custom clause should be applied</returns>
    string GetCustomWorkspaceWhereClause(string defaultWorkspaceClause);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <param name="startRecordIndex">start index of record set to get</param>
    /// <param name="recordsAmount">amount of records to get</param>
    /// <param name="sortings">a list of sort descriptors</param>
    /// <returns>an array of entities</returns>
    IxEntity[] ReadEntities(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider,
      int startRecordIndex = -1,
      int recordsAmount = -1,
      CxSortDescriptorList sortings = null);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads an entity of the given entity usage and with the given entity PK.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values, including PK</param>
    /// <returns>the entity read</returns>
    IxEntity ReadEntity(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the command should be visible from the current entity usage's point of view.
    /// </summary>
    bool GetIsCommandVisible(CxCommandMetadata commandMetadata, IxEntity parentEntity);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the list of primary key attributes for the given DB object.
    /// </summary>
    CxAttributeMetadata[] GetPrimaryKeyAttributes(int dbObjectIndex);
    //-------------------------------------------------------------------------
  }
}
