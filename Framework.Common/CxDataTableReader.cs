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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
	/// <summary>
	/// A reader interface to DataSet or DataTable data.
	/// </summary>
  public class CxDataTableReader : IDataReader, IEnumerable
  {
    //-------------------------------------------------------------------------
    protected DataSet m_DataSet = null;
    protected int m_CurrentTableIndex = 0;
    protected DataTable m_DataTable = null;
    protected int m_RecordsAffected = -1; // -1 for select statement
    protected int m_CurrentRecordIndex = -1;
    protected int m_CurrentRecordIndex_Cache = -1;
    protected DataRow m_CurrentRecord_Cache = null;
    protected DataTable m_SchemaTable = null;
    protected ICollection m_FilterColumns = null;
    protected bool m_IsClosed = false;
    protected Dictionary<string, int> m_ColumnMap = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataTable">Data table to be read</param>
    /// <param name="filterColumns">
    ///  List of the columns to be read (Vertical filter.)
    ///  Null - no vertical filter
    /// </param>
    public CxDataTableReader(DataTable dataTable, ICollection filterColumns)
    {
      DataTable     = dataTable;
      m_FilterColumns = filterColumns;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataTable">DataTable to be read</param>
    public CxDataTableReader(DataTable dataTable) : this(dataTable, null)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSet">DataSet to be read</param>
    public CxDataTableReader(DataSet dataSet)
    {
      m_DataSet = dataSet;
      if (m_DataSet != null && m_DataSet.Tables.Count > 0)
      {
        DataTable = m_DataSet.Tables[0];
      }
    }
    //-------------------------------------------------------------------------
    protected bool IsColumnAvailable(string name) 
    {
      bool res = true;
      if (FilterColumns != null) 
      {
        res = false;
        foreach (string fieldName in FilterColumns) 
        {
          if (CxText.Equals(fieldName, name)) 
          {
            res = true;
            break;
          }
        }
      }
      return res;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns number of fields
    /// </summary>
    /// <returns></returns>
    public int GetColumnsCount() 
    {
      int res;
      if (FilterColumns == null) 
        res = DataTable.Columns.Count;
      else 
        res = (FilterColumns.Count > DataTable.Columns.Count ? DataTable.Columns.Count : FilterColumns.Count);

      return res;
    }
    //-------------------------------------------------------------------------
    protected DataColumn GetColumn(string name) 
    {
      if (DataTable != null && IsColumnAvailable(name)) 
      {
        return DataTable.Columns[name];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    protected string GetColumnName(int index) 
    {
      string res = null;

      if (FilterColumns != null) 
      {
        int i = 0;
        foreach(string fieldName in FilterColumns) 
        {
          if (i == index) 
          {
            res = fieldName;
            break;
          }
          i++;
        }
      } 
      else 
      {
        res = DataTable.Columns[index].ColumnName;
      }

      return res;
    }
    //-------------------------------------------------------------------------
    protected int GetColumnIndex(int index) 
    {
      if (m_ColumnMap != null)
        return m_ColumnMap[GetColumnName(index)];
      else
        return -1;
    }
    //-------------------------------------------------------------------------
    protected DataColumn GetColumn(int index) 
    {
      return DataTable.Columns[GetColumnName(index)];
    }
    //-------------------------------------------------------------------------
    protected IList<DataColumn> GetColumns() 
    {
      IList<DataColumn> res = new List<DataColumn>();
      for(int i = 0; i < ColumnsCount; i++) 
      {
        res.Add(GetColumn(i));
      }
      return res;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets data table row count
    /// </summary>
    public int RowCount 
    {
      get {return DataTable.Rows.Count; }
    }
    //-------------------------------------------------------------------------
    public int Depth 
    { 
      get { return 0; } 
    }
    //-------------------------------------------------------------------------
    public bool IsClosed 
    { 
      get { return m_IsClosed; } 
    }
    //-------------------------------------------------------------------------
    public int RecordsAffected 
    { 
      get { return m_RecordsAffected;} 
    }
    //-------------------------------------------------------------------------
    public void Close()
    {
      m_IsClosed = true;
    }
    //-------------------------------------------------------------------------
    virtual public DataTable GetSchemaTable()
    {
      if (m_SchemaTable != null)
      {
        return m_SchemaTable;
      }
      m_SchemaTable = new DataTable();
      // create columns
      m_SchemaTable.Columns.Add("ColumnName", typeof(string));
      m_SchemaTable.Columns.Add("DataType", typeof(Type));
      m_SchemaTable.Columns.Add("ColumnSize", typeof(int));
      m_SchemaTable.Columns.Add("NumericPrecision", typeof(int));
      m_SchemaTable.Columns.Add("NumericScale", typeof(int));
      m_SchemaTable.Columns.Add("AllowDBNull", typeof(bool));
      // load table info
      if (Columns != null) 
      {
        foreach( DataColumn column in Columns /*DataTable.Columns*/ )
        {
          DataRow row = m_SchemaTable.NewRow();
          row[0] = column.ColumnName;
          row[1] = column.DataType; //type
          row[2] = column.MaxLength; // size
          row[3] = -1; // precision
          row[4] = -1; // scale
          row[5] = column.AllowDBNull; // is null

          m_SchemaTable.Rows.Add(row);
        }
      }
      return m_SchemaTable;
    }
    //-------------------------------------------------------------------------
    public bool NextResult()
    {
      m_CurrentTableIndex++;
      if (m_DataSet != null && m_CurrentTableIndex < m_DataSet.Tables.Count)
      {
        DataTable = m_DataSet.Tables[m_CurrentTableIndex];
        m_CurrentRecordIndex = -1;
        m_SchemaTable = null;
        return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    public bool Read() 
    {
      m_CurrentRecordIndex++;
      return m_CurrentRecordIndex < DataTable.Rows.Count; 
    }
    //-------------------------------------------------------------------------
    public object this[string columnName] 
    { 
      get 
      {
        DataRow currentRow = CurrentRow;
        if (currentRow != null) 
        {
          return currentRow[m_ColumnMap[columnName]];
        } 
        else 
        {
          return null;
        }
      } 
    }
    //-------------------------------------------------------------------------
    public object this[int index] 
    {
      get 
      {
        return this[GetColumnName(index)];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets number of the columns
    /// </summary>
    public int FieldCount
    {
      get { return GetColumnsCount(); }
    }
    //-------------------------------------------------------------------------
    public bool GetBoolean( int i )
    {
      return Convert.ToBoolean(this[i]);
    }
    //-------------------------------------------------------------------------
    public byte GetByte( int i )
    {
      return Convert.ToByte(this[i]);
    }
    //-------------------------------------------------------------------------
    public long GetBytes( int i,
                          long fieldOffset,
                          byte[] buffer,
                          int bufferOffset,
                          int length )
    {
      byte[] source = (byte[]) this[i];
      long actualLength = fieldOffset + length <= source.Length ? length : source.Length - fieldOffset;
      Array.Copy(source, fieldOffset, buffer, bufferOffset, actualLength);
      return actualLength;
    }
    //-------------------------------------------------------------------------
    public char GetChar( int i )
    {
      return Convert.ToChar(this[i]);
    }
    //-------------------------------------------------------------------------
    public long GetChars( int i,
                          long fieldOffset,
                          char[] buffer,
                          int bufferOffset,
                          int length )
    {
      char[] source = ((string) this[i]).ToCharArray();
      long actualLength = fieldOffset + length <= source.Length ? length : source.Length - fieldOffset;
      Array.Copy(source, fieldOffset, buffer, bufferOffset, actualLength);
      return actualLength;
    }
    //-------------------------------------------------------------------------
    public IDataReader GetData( int i )
    {
      return null;
    }
    //-------------------------------------------------------------------------
    public string GetDataTypeName( int i )
    {
      string res = null;

      if (GetColumn(i) != null) 
      {
        res = GetColumn(i).DataType.FullName;
      }

      return res;
    }
    //-------------------------------------------------------------------------
    public DateTime GetDateTime( int i )
    {
      return Convert.ToDateTime(this[i]);
    }
    //-------------------------------------------------------------------------
    public decimal GetDecimal( int i )
    {
      return Convert.ToDecimal(this[i]);
    }
    //-------------------------------------------------------------------------
    public double GetDouble( int i )
    {
      return Convert.ToDouble(this[i]);
    }
    //-------------------------------------------------------------------------
    public Type GetFieldType( int i )
    {
      return DataTable.Columns[GetColumnName(i)].DataType;
    }
    //-------------------------------------------------------------------------
    public float GetFloat( int i )
    {
      return Convert.ToSingle(this[i]);
    }
    //-------------------------------------------------------------------------
    public Guid GetGuid( int i )
    {
      return (Guid) this[i];
    }
    //-------------------------------------------------------------------------
    public short GetInt16( int i )
    {
      return Convert.ToInt16(this[i]);
    }
    //-------------------------------------------------------------------------
    public int GetInt32( int i )
    {
      return Convert.ToInt32(this[i]);
    }
    //-------------------------------------------------------------------------
    public long GetInt64( int i )
    {
      return Convert.ToInt64(this[i]);
    }
    //-------------------------------------------------------------------------
    public string GetName( int i )
    {
      DataColumn column = GetColumn(i);
      return column != null ? column.ColumnName : null;
    }
    //-------------------------------------------------------------------------
    public int GetOrdinal(string name)
    {
      DataColumn column = GetColumn(name);
      return column != null ? column.Ordinal : 0;
    }
    //-------------------------------------------------------------------------
    public string GetString( int i )
    {
      return CxUtils.ToString(this[i]);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Get value from the current record at specified position
    /// </summary>
    public object GetValue( int i )
    {
      return this[i];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Represents current record as array of values.
    /// </summary>
    /// <param name="values">Placeholder for the values</param>
    /// <returns>Number of values</returns>
    public int GetValues( object[] values )
    {
      int count = ColumnsCount;
      if (values != null) 
      {
        for (int i = 0; i < count && i < values.Length; i++) 
        {
          values[i] = this[i];
        }
      }
      return count;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks if value of the current record at the specified position
    /// contains null
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public bool IsDBNull( int i )
    {
      DataRow currentRow = CurrentRow;
      return currentRow != null && currentRow.IsNull(GetColumnIndex(i));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
    }
    //------------------------------------------------------------------------
    /// <summary>
    /// Gets columns list (vertcal filter)
    /// </summary>
    protected ICollection FilterColumns
    {
      get { return m_FilterColumns; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table to be read
    /// </summary>
    protected DataTable DataTable 
    {
      get 
      { 
        return m_DataTable ?? new DataTable(); 
      }
      set 
      { 
        m_DataTable = value;
        BuildColumnMap();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Builds columns map that maps column names to column indices.
    /// </summary>
    protected void BuildColumnMap()
    {
      m_ColumnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      for (int i = 0; i < m_DataTable.Columns.Count; i++)
      {
        m_ColumnMap[m_DataTable.Columns[i].ColumnName] = i;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets current row
    /// </summary>
    protected DataRow CurrentRow 
    {
      get 
      {
        if (m_CurrentRecordIndex < DataTable.Rows.Count)
        {
          if (m_CurrentRecordIndex_Cache != m_CurrentRecordIndex)
          {
            m_CurrentRecord_Cache = DataTable.Rows[m_CurrentRecordIndex];
            m_CurrentRecordIndex_Cache = m_CurrentRecordIndex;
          }
          return m_CurrentRecord_Cache;
        }
        else
        {
          return null;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets number of the columns
    /// </summary>
    public int ColumnsCount
    {
      get { return GetColumnsCount(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets list of the columns
    /// </summary>
    public IList<DataColumn> Columns 
    {
      get { return GetColumns(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns enumerator (IEnumerable implementation).
    /// </summary>
    public IEnumerator GetEnumerator()
    {
      return new DbEnumerator(this);
    }
    //-------------------------------------------------------------------------
  }
}