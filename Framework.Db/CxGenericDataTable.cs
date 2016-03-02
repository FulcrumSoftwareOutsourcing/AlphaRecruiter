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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// A DataTable that implements the IxGenericDataSource interface.
  /// </summary>
  public class CxGenericDataTable: DataTable, IxGenericDataSource, ITypedList
  {
    #region IxGenericDataSource implementation
    //----------------------------------------------------------------------------
    private bool m_IsInAlwaysEmptyMode = false;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Return a row by its index in the data-source.
    /// </summary>
    /// <param name="index">row index</param>
    /// <returns>a data-row</returns>
    public CxGenericDataRow this[int index]
    {
      get { return (CxGenericDataRow) Rows[index]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given row belongs to the data-source.
    /// </summary>
    /// <param name="dataRow"></param>
    /// <returns></returns>
    public bool DoesRowBelongToDataSource(CxGenericDataRow dataRow)
    {
      return dataRow.Table == this;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Enables the update mode.
    /// </summary>
    public void BeginUpdate()
    {
      BeginLoadData();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Indicates the update mode should be disabled.
    /// </summary>
    public void EndUpdate()
    {
      EndLoadData();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds a row to the data-source.
    /// </summary>
    /// <param name="dataRow"></param>
    public void Add(CxGenericDataRow dataRow)
    {
      Rows.Add(dataRow);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes a row from the data-source.
    /// </summary>
    /// <param name="dataRow"></param>
    public void Remove(CxGenericDataRow dataRow)
    {
      Rows.Remove(dataRow);
    }
    //----------------------------------------------------------------------------
    new public CxGenericDataRow NewRow()
    {
      return (CxGenericDataRow) base.NewRow();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an amount of rows.
    /// </summary>
    public int Count
    {
      get
      {
        if (IsInAlwaysEmptyMode)
          return 0;
        else
          return Rows.Count;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns an enumerator for the data-rows.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return Rows.GetEnumerator();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Return index of a row by the given primary key values.
    /// </summary>
    /// <param name="keyInfo">values of the key fields</param>
    /// <returns>an index of the data-row</returns>
    public int FindByKey(IDictionary<string, object> keyInfo)
    {
      return CxDbUtils.GetDataRowIndexByValues(this, keyInfo);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a generic table.
    /// </summary>
    /// <returns>generic table</returns>
    public CxGenericDataTable ToTable()
    {
      return this;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates the given columns to the data-source.
    /// </summary>
    /// <param name="columns">columns to be populated</param>
    void IxGenericDataSource.PopulateColumns(DataColumn[] columns)
    {
      Columns.AddRange(columns);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Resets the data-source's schema.
    /// </summary>
    public void ClearDataAndSchema()
    {
      Clear();
      Columns.Clear();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the "always empty mode" state of the data source.
    /// If the data-source is this mode (property is set to "true") - 
    /// it does not contain any data ever.
    /// </summary>
    public bool IsInAlwaysEmptyMode
    {
      get { return m_IsInAlwaysEmptyMode; }
      set
      {
        if (value != m_IsInAlwaysEmptyMode)
        {
          m_IsInAlwaysEmptyMode = value;
          //OnTableCleared(new DataTableClearEventArgs(this));
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks existing field with the same name and recreates it on basis of the given type.
    /// </summary>
    /// <param name="columnName">the System.Data.DataColumn.ColumnName to use when you create the column</param>
    /// <param name="type">the System.Data.DataColumn.DataType of the new column</param>
    public void ReCreateColumn(string columnName, Type type)
    {
      if (Columns.Contains(columnName))
      {
        Columns.Remove(columnName);
        Columns.Add(columnName, type);
      }
      else
      {
        Columns.Add(columnName, type);
      }
    }
    //----------------------------------------------------------------------------
    #endregion

    #region ITypedList implementation (finished)
    //----------------------------------------------------------------------------
    string ITypedList.GetListName(PropertyDescriptor[] descriptors)
    {
      return "DataRow list";
    }
    //----------------------------------------------------------------------------
    PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] descriptors)
    {
      PropertyDescriptorCollection collection = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
      foreach (DataColumn column in Columns)
      {
        PropertyDescriptor descriptor = new CxColumnPropertyDescriptor(column);
        collection.Add(descriptor);
      }
      return collection;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns all the data-rows that were loaded from the data storage.
    /// </summary>
    /// <returns>an array of data rows</returns>
    public CxGenericDataRow[] GetLoadedDataRows()
    {
      return (new List<DataRow>(Select())).ConvertAll<CxGenericDataRow>(delegate(DataRow input) { return (CxGenericDataRow) input; }).ToArray();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns all the indices of the actually available data-rows 
    /// loaded from the data storage.
    /// </summary>
    /// <returns>an array of indices</returns>
    public int[] GetLoadedDataRowIndices()
    {
      int[] indices = new int[Rows.Count];
      for (int i = 0; i < Rows.Count; i++)
      {
        indices[i] = i;
      }
      return indices;
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxGenericDataTable()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="view"></param>
    public CxGenericDataTable(DataView view)
      : this()
    {
      ((IxGenericDataSource)this).PopulateColumns(CxDbUtils.CopyDataColumns(CxDbUtils.ConvertColumnCollectionToColumnArray(view.Table.Columns)).ToArray());
      foreach (DataRowView rowView in view)
      {
        ImportRow(rowView.Row);
      }
    }
    //----------------------------------------------------------------------------
    public CxGenericDataTable(IList<DataColumn> columns, IList<DataRow> rows)
    {
      ((IxGenericDataSource) this).PopulateColumns(CxDbUtils.CopyDataColumns(columns).ToArray());
      foreach (DataRow row in rows)
      {
        ImportRow(row);
      }
    }
    //----------------------------------------------------------------------------
    #endregion

    //----------------------------------------------------------------------------
    protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
    {
      return new CxGenericDataRow(builder);
    }
    //----------------------------------------------------------------------------
  }
}
