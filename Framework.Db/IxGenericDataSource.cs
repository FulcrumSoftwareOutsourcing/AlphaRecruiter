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

using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// A generalized data-source interface.
  /// </summary>
  public interface IxGenericDataSource : 
    IListSource, IEnumerable, IDisposable
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Raised when a data-row of the data-source is changed.
    /// </summary>
    event DataRowChangeEventHandler RowChanged;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a data-row by its index.
    /// </summary>
    /// <param name="index">index of a data-row</param>
    /// <returns>a data-row</returns>
    CxGenericDataRow this[int index] { get; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the "always empty mode" state of the data source.
    /// If the data-source is this mode (property is set to "true") - 
    /// it does not contain any data ever.
    /// </summary>
    bool IsInAlwaysEmptyMode { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// A collection of data columns.
    /// </summary>
    DataColumnCollection Columns { get; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates the given columns to the data-source.
    /// </summary>
    /// <param name="columns">columns to be populated</param>
    void PopulateColumns(DataColumn[] columns);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Starts the "update" mode.
    /// </summary>
    void BeginUpdate();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finishes the "update" mode.
    /// </summary>
    void EndUpdate();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new row and returns it.
    /// </summary>
    /// <returns></returns>
    CxGenericDataRow NewRow();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds a row into the data-source.
    /// </summary>
    /// <param name="row">a data-row to be added</param>
    void Add(CxGenericDataRow row);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an amount of items in the data-source.
    /// </summary>
    int Count { get; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes a row from the data-source.
    /// </summary>
    /// <param name="row"></param>
    void Remove(CxGenericDataRow row);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an index of the data-row that has got the given values
    /// in the given fields.
    /// </summary>
    /// <param name="keys">
    /// A dictionary with information about key values:
    /// dictionary keys contain field names,
    /// dictionary values contain actual values to search for</param>
    /// <returns></returns>
    int FindByKey(IDictionary<string, object> keys);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if the given row belongs to the data-source.
    /// </summary>
    /// <param name="dataRow"></param>
    /// <returns></returns>
    bool DoesRowBelongToDataSource(CxGenericDataRow dataRow);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Clears the data-source.
    /// </summary>
    void Clear();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Resets the data-source's data and schema.
    /// </summary>
    void ClearDataAndSchema();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns all the data-rows that were loaded from the data storage.
    /// </summary>
    /// <returns>an array of data rows</returns>
    CxGenericDataRow[] GetLoadedDataRows();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns all the indices of the actually available data-rows 
    /// loaded from the data storage.
    /// </summary>
    /// <returns>an array of indices</returns>
    int[] GetLoadedDataRowIndices();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a generic table based upon the data-source.
    /// </summary>
    /// <returns>generic table</returns>
    CxGenericDataTable ToTable();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks existing field with the same name and recreates it on basis of the given type.
    /// </summary>
    /// <param name="columnName">the System.Data.DataColumn.ColumnName to use when you create the column</param>
    /// <param name="type">the System.Data.DataColumn.DataType of the new column</param>
    void ReCreateColumn(string columnName, Type type);
    //----------------------------------------------------------------------------
  }
}
