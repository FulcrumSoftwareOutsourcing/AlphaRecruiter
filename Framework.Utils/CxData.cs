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
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Framework.Utils
{
  using System.Collections;

  //---------------------------------------------------------------------------
  /// <summary>
  /// Boolean binary operator.
  /// </summary>
  public enum NxBooleanBinaryOperator { And, Or }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Utility methods to work with any database and/or in-memory data structures.
  /// </summary>
  public class CxData
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns DBNull if given string is empty, and given string otherwise.
    /// </summary>
    /// <param name="s">string to return if not empty</param>
    /// <returns>DBNull if given string is empty, and given string otherwise</returns>
    static public object EmptyToDbNull(string s)
    {
      return CxUtils.IsEmpty(s) ? (object) DBNull.Value : s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns DBNull if given object is empty, and given object otherwise.
    /// </summary>
    /// <param name="o">object to return if not null</param>
    /// <returns>DBNull if given object is empty, and given object otherwise</returns>
    static public object EmptyToDbNull(object o)
    {
      return CxUtils.IsNull(o) ? DBNull.Value : o;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has integer type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has integer type</returns>
    static public bool IsInteger(DataColumn column)
    {
      return CxType.IsInteger(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has number type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has number type</returns>
    static public bool IsNumber(DataColumn column)
    {
      return CxType.IsNumber(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has datetime type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has datetime type</returns>
    static public bool IsDateTime(DataColumn column)
    {
      return CxType.IsDateTime(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has boolean type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has boolean type</returns>
    static public bool IsBoolean(DataColumn column)
    {
      return CxType.IsBoolean(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has GUID type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has GUID type</returns>
    static public bool IsGuid(DataColumn column)
    {
      return CxType.IsGuid(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has binary type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has binary type</returns>
    static public bool IsBinary(DataColumn column)
    {
      return CxType.IsBinary(column.DataType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has string type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has string type</returns>
    static public bool IsString(DataColumn column)
    {
      return CxType.IsString(column.DataType) && column.MaxLength < int.MaxValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column has long string (memo) type.
    /// </summary>
    /// <param name="column">column to check</param>
    /// <returns>true if the given column has long string type</returns>
    static public bool IsLongString(DataColumn column)
    {
      return CxType.IsString(column.DataType) && column.MaxLength == int.MaxValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Moves the given row at the given offset inside the table.
    /// </summary>
    /// <param name="row">the row to move</param>
    /// <param name="offset">the offset to move by</param>
    /// <returns>new index of the row</returns>
    static public int MoveRow(DataRow row, int offset)
    {
      if (row == null)
        throw new ArgumentNullException("row");
      if (row.Table == null)
        throw new NullReferenceException("row.Table is null");
      DataTable table = row.Table;
      int currentIndex = table.Rows.IndexOf(row);
      int newIndex = Math.Max(Math.Min(currentIndex + offset, table.Rows.Count - 1), 0);
      
      if (currentIndex != newIndex)
      {
        object[] itemArray = (object[]) row.ItemArray.Clone();
        
        row = table.NewRow();
        row.ItemArray = itemArray;
        
        table.Rows.RemoveAt(currentIndex);
        table.Rows.InsertAt(row, newIndex);
      }
      return newIndex;
    }
    //-------------------------------------------------------------------------
    public static int MoveRowObject(IList collection, object rowObject, int offset)
    {
      if (rowObject == null)
        throw new ArgumentNullException("rowObject");
      if (collection == null)
        throw new ArgumentNullException("collection");

      int currentIndex = collection.IndexOf(rowObject);
      int newIndex = Math.Max(Math.Min(currentIndex + offset, collection.Count - 1), 0);

      if (currentIndex != newIndex)
      {
        collection.RemoveAt(currentIndex);
        collection.Insert(newIndex, rowObject);
      }
      return newIndex;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes all rows from given DataTable.
    /// </summary>
    /// <param name="dt">data table to clear</param>
    static public void Clear(DataTable dt)
    {
      if (dt != null)
      {
        dt.BeginLoadData();
        try
        {
          dt.Rows.Clear();
        }
        finally
        {
          dt.EndLoadData();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field value by field name, if such field exists. 
    /// Otherwise, returns defValue.
    /// </summary>
    /// <param name="row">data row to get value from</param>
    /// <param name="fieldName">name of the field</param>
    /// <param name="defValue">default value to return if row is empty</param>
    /// <returns>field value by field name, if such field exists. 
    /// Otherwise, returns defValue</returns>
    static public object GetValue(DataRow row, string fieldName, object defValue)
    {
      object result = defValue;
      if (row != null && row.Table != null)
      {
        int index = row.Table.Columns.IndexOf(fieldName);
        if (index >= 0)
        {
          result = row[index];
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field value by field name, if such field exists. 
    /// Otherwise, returns defValue.
    /// </summary>
    /// <param name="row">data row to get value from</param>
    /// <param name="fieldName">name of the field</param>
    /// <param name="defValue">default value to return if row is empty</param>
    /// <returns>field value by field name, if such field exists. 
    /// Otherwise, returns defValue</returns>
    static public string GetValue(DataRow row, string fieldName, string defValue)
    {
      string result = defValue;
      if (row != null && row.Table != null)
      {
        int index = row.Table.Columns.IndexOf(fieldName);
        if (index >= 0)
        {
          result = row[fieldName].ToString();
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field value by field name, if such field exists. 
    /// Otherwise, returns empty string.
    /// </summary>
    /// <param name="row">data row to get value from</param>
    /// <param name="fieldName">name of the field</param>
    /// <returns>field value by field name, if such field exists. 
    /// Otherwise, returns empty string</returns>
    static public string GetValue(DataRow row, string fieldName)
    {
      return GetValue(row, fieldName, "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares string to be used as SQL statements.
    /// </summary>
    /// <param name="s">string to prepare for usage in SQL statement</param>
    /// <param name="charFunction">function to use for "imprintable"characters</param>
    /// <returns>string ready to be used in SQL statements</returns>
    static public string PrepareSqlString(string s, string charFunction)
    {
      return PrepareSqlString(s, charFunction, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares string to be used as SQL statements.
    /// </summary>
    /// <param name="s">string to prepare for usage in SQL statement</param>
    /// <param name="charFunction">function to use for "imprintable" characters</param>
    /// <param name="wrapIntoQuotes">should result value be wrapped</param>
    /// <returns>string ready to be used in SQL statements</returns>
    static public string PrepareSqlString(string s, string charFunction, bool wrapIntoQuotes)
    {
      int len = s.Length;
      StringBuilder sb = new StringBuilder(len * 2);
      if (wrapIntoQuotes)
        sb.Append("'");
      for (int i = 0; i < len; i++)
      {
        char c = s[i];
        if (c < 32)
          sb.AppendFormat(charFunction, c);
        else if (c == '\'')
          sb.Append("''");
        else
          sb.Append(c);
      }
      if (wrapIntoQuotes)
        sb.Append("\'");
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data row value in the form applicable to store into the property.
    /// </summary>
    /// <param name="row">data row to return value from</param>
    /// <param name="column">data column</param>
    /// <returns>value applicable to store into the property</returns>
    static public object GetColumnValue(DataRow row, DataColumn column)
    {
      if (row != null && column != null)
      {
        object dataValue = row[column];
        bool isNull = CxUtils.IsNull(dataValue);
        if (isNull && CxType.IsString(column.DataType))
        {
          return "";
        }
        else if (isNull)
        {
          return null;
        }
        else
        {
          return dataValue;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data row value in the form applicable to store into the property.
    /// </summary>
    /// <param name="row">data row to return value from</param>
    /// <param name="name">name of the value column</param>
    /// <returns>value applicable to store into the property</returns>
    static public object GetColumnValue(DataRow row, string name)
    {
      if (row != null && row.Table != null)
      {
        DataColumn column = GetColumn(row.Table, name);
        if (column != null)
        {
          return GetColumnValue(row, column);
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets data row value from the property value.
    /// </summary>
    /// <param name="row">data row to set value in</param>
    /// <param name="column">value column</param>
    /// <param name="value">property value to write to the data row</param>
    static public void SetValue(DataRow row, DataColumn column, object value)
    {
      if (row != null && column != null)
      {
        object dataValue = value;
        bool isNull = CxUtils.IsEmpty(value);
        if (isNull && CxType.IsString(column.DataType))
          dataValue = "";
        else if (isNull)
          dataValue = DBNull.Value;
        row[column] = dataValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets data row value from the property value.
    /// </summary>
    /// <param name="row">data row to set value in</param>
    /// <param name="name">name of the value column</param>
    /// <param name="value">property value to write to the data row</param>
    static public void SetValue(DataRow row, string name, object value)
    {
      if (row != null)
      {
        SetValue(row, GetColumn(row.Table, name), value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds datatable column by the name (ignoring character case).
    /// Returns actual column name in original character case or null if column does not exist.
    /// </summary>
    /// <param name="dt">data table to find column in</param>
    /// <param name="name">name of the column</param>
    /// <returns>name of the column in the datatable or null if not found</returns>
    static public string GetColumnName(DataTable dt, string name)
    {
      int index = dt != null ? dt.Columns.IndexOf(name) : -1;
      return index >= 0 ? dt.Columns[index].ColumnName : null;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Finds datatable column by the name (ignoring character case).
    /// Returns actual column or null if column does not exist.
    /// </summary>
    /// <param name="dt">data table to find column in</param>
    /// <param name="name">name of the column</param>
    /// <returns>column in the datatable or null if not found</returns>
    static public DataColumn GetColumn(DataTable dt, string name)
    {
      int index = dt != null ? dt.Columns.IndexOf(name) : -1;
      return index >= 0 ? dt.Columns[index] : null;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Finds datatable column by the name (ignoring character case).
    /// </summary>
    /// <param name="dt">data table to find column in</param>
    /// <param name="name">name of the column</param>
    /// <param name="defName">default name for the column if not found</param>
    /// <returns>name of the column in the datatable or default name if not found</returns>
    static public string GetColumnNameDef(DataTable dt, string name, string defName)
    {
      return CxUtils.Nvl(GetColumnName(dt, name), defName);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Compared column values from the given data row with the given ones.
    /// </summary>
    /// <param name="dataRow">data row to compare primary key values with</param>
    /// <param name="columnNames">array with column names</param>
    /// <param name="columnValues">array with column values</param>
    /// <returns>true if data row column values matches the given ones</returns>
    static public bool CompareDataRowByPK(DataRow dataRow,
                                          string[] columnNames,
                                          object[] columnValues)
    {
      for (int i = 0; i < columnNames.Length; i++)
      {
        object value = dataRow[columnNames[i]];
        if (!CxUtils.Compare(value, columnValues[i]))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Locates row in the data table with the given column values.
    /// </summary>
    /// <param name="table">data table to scan</param>
    /// <param name="columnNames">array with column names</param>
    /// <param name="columnValues">array with column values</param>
    /// <returns>data row with the given column values or null if not found</returns>
    static public DataRow FindDataRowByPK(DataTable table,
                                          string[] columnNames,
                                          object[] columnValues)
    {
      foreach (DataRow dataRow in table.Rows)
      {
        if (CompareDataRowByPK(dataRow, columnNames, columnValues))
        {
          return dataRow;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Locates row in the data table with the given column values.
    /// </summary>
    /// <param name="dataView">data view to scan</param>
    /// <param name="columnNames">array with column names</param>
    /// <param name="columnValues">array with column values</param>
    /// <returns>data row with the given column values or null if not found</returns>
    static public DataRow FindDataRowByPK(DataView dataView,
                                          string[] columnNames,
                                          object[] columnValues)
    {
      foreach (DataRowView dataRowView in dataView)
      {
        DataRow dataRow = dataRowView.Row;
        if (CompareDataRowByPK(dataRow, columnNames, columnValues))
        {
          return dataRow;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates sum of table column values.
    /// </summary>
    /// <param name="table">data table to scan rows in</param>
    /// <param name="columnName">name of the column to calculate sum of</param>
    /// <returns>sum of table column values</returns>
    static public decimal CalculateSum(DataTable table, string columnName)
    {
      return CalculateSum(table.DefaultView, columnName);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Calculates sum of data view column values.
    /// </summary>
    /// <param name="dataView">data view to scan rows in</param>
    /// <param name="columnName">name of the column to calculate sum of</param>
    /// <returns>sum of data view column values</returns>
    static public decimal CalculateSum(DataView dataView, string columnName)
    {
      decimal sum = 0;
      foreach (DataRowView rowView in dataView)
      {
        sum += Convert.ToDecimal(CxUtils.Nvl(rowView[columnName], 0));
      }
      return sum;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Calculates maximal of table column values.
    /// </summary>
    /// <param name="table">data table to scan rows in</param>
    /// <param name="columnName">name of the column to calculate maximal of</param>
    /// <returns>maximal of table column values</returns>
    static public decimal CalculateMax(DataTable table, string columnName)
    {
      return CalculateMax(table.DefaultView, columnName);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Calculates maximal of data view column values.
    /// </summary>
    /// <param name="dataView">data view to scan rows in</param>
    /// <param name="columnName">name of the column to calculate maximal of</param>
    /// <returns>maximal of table column values</returns>
    static public decimal CalculateMax(DataView dataView, string columnName)
    {
      decimal max = decimal.MinValue;
      foreach (DataRowView rowView in dataView)
      {
        max = Math.Max(max, Convert.ToDecimal(CxUtils.Nvl(rowView[columnName], decimal.MinValue)));
      }
      return max;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Deletes rows from the data table with the given column values.
    /// </summary>
    /// <param name="table">table to delerte ros from</param>
    /// <param name="columnNames">name of columns to compare</param>
    /// <param name="columnValues">values of columns to compare</param>
    /// <returns>number of deleted rows</returns>
    static public int DeleteTableRows(DataTable table, string[] columnNames, object[] columnValues)
    {
      int count = 0;
      for (int i = table.Rows.Count - 1; i >= 0; i--)
      {
        DataRow row = table.Rows[i];
        if (CompareDataRowByPK(row, columnNames, columnValues))
        {
          table.Rows.Remove(row);
          count++;
        }
      }
      return count;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Deletes rows from the data table with the given column value.
    /// </summary>
    /// <param name="table">table to delete ros from</param>
    /// <param name="columnName">name of column to compare</param>
    /// <param name="columnValue">values of column to compare</param>
    /// <returns>number of deleted rows</returns>
    static public int DeleteTableRows(DataTable table, string columnName, object columnValue)
    {
      return DeleteTableRows(table, new string[] { columnName }, new object[] { columnValue });
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates new data row with the given values in the new table with 
    /// the given column names.
    /// </summary>
    /// <param name="columnNames">list of table column names</param>
    /// <param name="columnValues">list of data row values</param>
    /// <returns>new data row with the given values in the new table with 
    /// the given column names</returns>
    static public DataRow CreateDataRow(string[] columnNames, object[] columnValues)
    {
      DataTable dt = new DataTable();
      foreach (string columnName in columnNames)
      {
        dt.Columns.Add(columnName, typeof(object));
      }
      DataRow row = dt.Rows.Add(columnValues);
      return row;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates value of the given column in the all rows from the data view
    /// with the given value.
    /// </summary>
    /// <param name="dataView">data view to update rows in</param>
    /// <param name="columnName">name of the columm to update</param>
    /// <param name="value">value to set</param>
    static public void UpdateDataViewColumn(DataView dataView, string columnName, object value)
    {
      dataView.Table.BeginLoadData();
      try
      {
        foreach (DataRowView rowView in dataView)
        {
          SetValue(rowView.Row, columnName, value);
        }
      }
      finally
      {
        dataView.Table.EndLoadData();
      }
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Copies values from one data row into another.
    /// </summary>
    /// <param name="source">data row to get values from</param>
    /// <param name="target">data row to put values to</param>
    static public void CopyDataRow(DataRow source, DataRow target)
    {
      if (source != null && target != null && source.Table != null)
      {
        foreach (DataColumn dc in source.Table.Columns)
        {
          if (target.Table.Columns.Contains(dc.ColumnName))
          {
            target[dc.ColumnName] = source[dc];
          }
        }
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Extracts values from the given array of rows.
    /// </summary>
    /// <param name="table">the table rows belong to</param>
    /// <param name="rows">rows to extract values from</param>
    /// <returns>values extracted</returns>
    static public object[][] ExtractRowsValues(DataTable table, DataRow[] rows)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      if (rows == null)
        throw new ArgumentNullException("rows");

      object[][] result = new object[rows.Length][];
      for (int i = 0; i < rows.Length; i++)
      {
        result[i] = (object[]) rows[i].ItemArray.Clone();
      }
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Copies extended properties of data table columns.
    /// </summary>
    /// <param name="source">source table</param>
    /// <param name="target">target table</param>
    static public void CopyExtendedProperties(DataTable source, DataTable target)
    {
      foreach (DataColumn targetColumn in target.Columns)
      {
        int sourceColumnIndex = source.Columns.IndexOf(targetColumn.ColumnName);
        if (sourceColumnIndex >= 0)
        {
          DataColumn sourceColumn = source.Columns[sourceColumnIndex];
          targetColumn.Caption = sourceColumn.Caption;
          foreach (object key in sourceColumn.ExtendedProperties.Keys)
          {
            targetColumn.ExtendedProperties[key] = sourceColumn.ExtendedProperties[key];
          }
        }
      }
    }
    //-------------------------------------------------------------------------  
    /// <summary>
    /// Copies all structure and data from source table to target table.
    /// </summary>
    /// <param name="source">source table</param>
    /// <param name="target">target table</param>
    static public void CopyDataTable(DataTable source, DataTable target)
    {
      target.BeginLoadData();
      try
      {
        target.Clear();
        target.Columns.Clear();
        target.Load(new DataTableReader(source));
        CopyExtendedProperties(source, target);
      }
      finally
      {
        target.EndLoadData();
      }
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Copies all structure and data from source data view to the target table.
    /// </summary>
    /// <param name="source">source view</param>
    /// <param name="target">target table</param>
    static public void CopyDataTable(DataView source, DataTable target)
    {
      DataTable sourceTable = source.ToTable();
      CopyExtendedProperties(source.Table, sourceTable);
      CopyDataTable(sourceTable, target);
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Creates and returns empty datatable with the same structure as in the source table.
    /// </summary>
    /// <param name="source">source table to clone</param>
    /// <returns>cloned data table</returns>
    static public DataTable CloneDataTable(DataTable source)
    {
      DataTable target = new DataTable();
      foreach (DataColumn column in source.Columns)
      {
        DataColumn dc = new DataColumn(column.ColumnName, column.DataType);
        dc.Caption = column.Caption;
        dc.MaxLength = column.MaxLength;
        foreach (object key in column.ExtendedProperties.Keys)
        {
          dc.ExtendedProperties[key] = column.ExtendedProperties[key];
        }
        target.Columns.Add(dc);
      }
      return target;
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Calculates count of pages for the given row count and page size.
    /// </summary>
    /// <param name="rowCount">total count of table rows</param>
    /// <param name="pageSize">required page size</param>
    /// <returns>count of pages</returns>
    static public int GetPageCount(int rowCount, int pageSize)
    {
      if (rowCount > 0 && pageSize > 0 && rowCount > pageSize)
      {
        return (rowCount - 1) / pageSize + 1;
      }
      return 0;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns part of the data row array corresponding to the given page.
    /// </summary>
    /// <param name="rows">the whole row collection</param>
    /// <param name="pageSize">required page size</param>
    /// <param name="pageIndex">index of the page to return rows for</param>
    /// <returns>collection of rows belonging to the requested page index</returns>
    static public DataRow[] GetPageData(DataRow[] rows, int pageSize, int pageIndex)
    {
      if (rows.Length > 0 && pageSize > 0 && rows.Length > pageSize)
      {
        if (pageIndex >= pageSize)
        {
          pageIndex = pageSize - 1;
        }
        if (pageIndex < 0)
        {
          pageIndex = 0;
        }
        int startIndex = pageIndex * pageSize;
        int rowCount = startIndex + pageSize <= rows.Length ? pageSize : rows.Length - startIndex;
        DataRow[] pagedRows = new DataRow[rowCount];
        for (int i = 0; i < rowCount; i++)
        {
          pagedRows[i] = rows[startIndex + i];
        }
        return pagedRows;
      }
      return rows;
    }
    //--------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause from multiple parts.
    /// </summary>
    /// <param name="composeOperator">boolean operator to compose with (and, or)</param>
    /// <param name="whereParts">WHERE clause parts</param>
    /// <returns>WHERE clause that consists of all non-empty parts</returns>
    static public string ComposeWhereClause(
      NxBooleanBinaryOperator composeOperator,
      params string[] whereParts)
    {
      if (whereParts != null && whereParts.Length > 0)
      {
        List<string> actualParts = new List<string>();
        foreach (string wherePart in whereParts)
        {
          if (CxUtils.NotEmpty(wherePart))
          {
            actualParts.Add(wherePart);
          }
        }
        if (actualParts.Count == 1)
        {
          return actualParts[0];
        }
        else if (actualParts.Count > 0)
        {
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < actualParts.Count; i++)
          {
            if (i > 0)
            {
              sb.Append(" ");
              sb.Append(composeOperator.ToString().ToLower());
              sb.Append(" ");
            }
            sb.Append("(");
            sb.Append(actualParts[i]);
            sb.Append(")");
          }
          return sb.ToString();
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause from multiple parts.
    /// </summary>
    /// <param name="whereParts">WHERE clause parts</param>
    /// <returns>WHERE clause that consists of all non-empty parts</returns>
    static public string ComposeWhereClause(params string[] whereParts)
    {
      return ComposeWhereClause(NxBooleanBinaryOperator.And, whereParts);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Swaps values of given data columns in the given data row.
    /// </summary>
    /// <param name="dr">data row to swap values in</param>
    /// <param name="column1Name">first column to swap</param>
    /// <param name="column2Name">second column to swap</param>
    static public void SwapDataColumnValues(
      DataRow dr,
      string column1Name,
      string column2Name)
    {
      if (dr != null &&
          dr.Table != null &&
          dr.Table.Columns.Contains(column1Name) &&
          dr.Table.Columns.Contains(column2Name))
      {
        object value = dr[column1Name];
        dr[column1Name] = dr[column2Name];
        dr[column2Name] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Swaps values of given data columns in the whole data table.
    /// </summary>
    /// <param name="dt">data table to swap values in</param>
    /// <param name="column1Names">list of columns to swap</param>
    /// <param name="column2Names">list of corresponding columns to swap with</param>
    static public void SwapDataColumnValues(
      DataTable dt,
      string[] column1Names,
      string[] column2Names)
    {
      if (dt != null && column1Names != null && column2Names != null)
      {
        List<string> column1NameList = new List<string>();
        List<string> column2NameList = new List<string>();
        for (int i = 0; i < column1Names.Length && i < column2Names.Length; i++)
        {
          if (dt.Columns.Contains(column1Names[i]) && dt.Columns.Contains(column2Names[i]))
          {
            column1NameList.Add(column1Names[i]);
            column2NameList.Add(column2Names[i]);
          }
        }
        foreach (DataRow dr in dt.Rows)
        {
          for (int i = 0; i < column1NameList.Count; i++)
          {
            object value = dr[column1NameList[i]];
            dr[column1NameList[i]] = dr[column2NameList[i]];
            dr[column2NameList[i]] = value;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Swaps values of given data columns in the whole data table.
    /// </summary>
    /// <param name="dt">data table to swap values in</param>
    /// <param name="column1Name">first column to swap</param>
    /// <param name="column2Name">second column to swap</param>
    static public void SwapDataColumnValues(
      DataTable dt,
      string column1Name,
      string column2Name)
    {
      SwapDataColumnValues(dt, new string[] { column1Name }, new string[] { column2Name });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if data table has at least one non-empty value of the specified column.
    /// </summary>
    /// <param name="dt">data table to search for non-empty value in</param>
    /// <param name="columnName">column name to search for non-empty value in</param>
    static public bool HasNonEmptyValue(DataTable dt, string columnName)
    {
      if (dt != null && CxUtils.NotEmpty(columnName) && dt.Columns.Contains(columnName))
      {
        foreach (DataRow dr in dt.Rows)
        {
          if (CxUtils.NotEmpty(dr[columnName]))
          {
            return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns text of the file size.
    /// </summary>
    /// <param name="dataSize">file size</param>
    /// <returns>display text</returns>
    static public string GetDataSizeText(long dataSize)
    {
      if (dataSize >= 0 && dataSize < 1024)
      {
        return dataSize + " B";
      }
      else if (dataSize >= 1024 && dataSize < 1048576)
      {
        return (((double) dataSize) / 1024).ToString("F2") + " KB";
      }
      else if (dataSize >= 1048576 && dataSize < 1073741824)
      {
        return (((double) dataSize) / 1048576).ToString("F2") + " MB";
      }
      else if (dataSize >= 1073741824)
      {
        return (((double) dataSize) / 1073741824).ToString("F2") + " GB";
      }
      return null;
    }

  }
}