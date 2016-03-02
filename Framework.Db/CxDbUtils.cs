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
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Framework.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using SimmoTech.Utils.Data;

namespace Framework.Db
{
  /// <summary>
  /// Class with utility methods for work with databases, SQL queries, etc.
  /// </summary>
  public class CxDbUtils
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds additional condition to the query where clause (if present).
    /// </summary>
    /// <param name="queryText">query text (with or without where clause)</param>
    /// <param name="where">additional condiiton to add</param>
    /// <returns>altered query text with additional condition in the where clause</returns>
    static public string AddToWhere(string queryText, string where)
    {
      if (CxUtils.IsEmpty(where)) return queryText;

      int pos = FindToken(queryText, "WHERE");
      if (pos == -1)
      {
        return queryText + "\r\n WHERE (" + where + ")";
      }
      else
      {
        return queryText.Substring(0, pos + 5) + " ( " +
          queryText.Substring(pos + 5) + " )\r\n" +
          " AND (" + where + ")";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause from multiple parts.
    /// </summary>
    /// <param name="whereParts">WHERE clause parts</param>
    /// <returns>WHERE clause that consists of all non-empty parts</returns>
    static public string ComposeWhereClause(params string[] whereParts)
    {
      return CxData.ComposeWhereClause(whereParts);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds token in the given string.
    /// </summary>
    /// <param name="s">string to find in</param>
    /// <param name="token">token to find</param>
    /// <returns>number of character where token started or -1 if token not found</returns>
    static public int FindToken(string s, string token)
    {
      string s1 = s.ToUpper();
      string s2 = token.ToUpper();

      int roundBracketsLevel = 0;
      int squareBracketsLevel = 0;
      for (int i = 0; i < s1.Length; i++)
      {
        char c = s1[i];
        if (c == '(')
          roundBracketsLevel++;
        else if (c == ')')
          roundBracketsLevel--;
        else if (c == '[')
          squareBracketsLevel++;
        else if (c == ']')
          squareBracketsLevel--;
        else if (squareBracketsLevel == 0 && roundBracketsLevel == 0 &&
          i + 1 >= s2.Length && (i + 1 == s2.Length || Char.IsWhiteSpace(s1[i - s2.Length])) &&
          s1.Substring(0, i + 1).EndsWith(s2))
          return i - s2.Length + 1;
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares valut to be used in data view row filters.
    /// </summary>
    /// <param name="value">value to prepare</param>
    /// <returns>string that represents the given value and is ready to be used in data view row filters</returns>
    static public string PrepareValueForDataFilter(object value)
    {
      if (CxUtils.IsNull(value))
        return "NULL";
      else if (CxType.IsBoolean(value.GetType()))
        return CxBool.Parse(value) ? "1" : "0";
      else if (CxType.IsNumber(value.GetType()))
        return CxFloat.ToConst(Convert.ToDouble(value));
      else if (CxType.IsDateTime(value.GetType()))
        return "#" + ((DateTime) value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) + "#";
      else
        return "'" + value.ToString().Replace("'", "''") + "'";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces "?" placeholders in the template expression with actual values.
    /// </summary>
    /// <param name="templateExpr">expresion with "?" parameter placeholders </param>
    /// <param name="paramNames">list of parameter names</param>
    /// <param name="valueProvider">paremeter value provider</param>
    /// <returns>expression ready to use in datatable filters or calculated field expressions</returns>
    static public string ComposeLocalExpression(
      string templateExpr,
      IList paramNames,
      IxValueProvider valueProvider)
    {
      IList paramValues = new ArrayList();
      foreach (string name in paramNames)
      {
        object value = valueProvider[name];
        string valueStr = PrepareValueForDataFilter(value);
        paramValues.Add(valueStr);
      }

      string expr = "";
      int startFrom = 0;
      foreach (string value in paramValues)
      {
        int paramIndex = templateExpr.IndexOf('?', startFrom);
        expr += templateExpr.Substring(startFrom, paramIndex - startFrom);
        expr += value;
        startFrom = paramIndex + 1;
      }
      if (startFrom < templateExpr.Length)
      {
        expr += templateExpr.Substring(startFrom);
      }

      return expr;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="templateExpr">expresion with "?" parameter placeholders </param>
    /// <param name="paramNames">list of parameter names</param>
    /// <param name="valueProvider">paremeter value provider</param>
    /// <param name="exprType">type of result expression</param>
    /// <returns>local expression value</returns>
    static public object CalculateLocalExpression(
      string templateExpr,
      IList paramNames,
      IxValueProvider valueProvider,
      Type exprType)
    {
      using (DataTable dt = new DataTable())
      {
        string expr = ComposeLocalExpression(templateExpr, paramNames, valueProvider);
        dt.Columns.Add("Result", typeof(object), expr);
        dt.Rows.Add(dt.NewRow());
        object value = dt.Rows[0][0];
        return value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="expr">expresion to calculate</param>
    /// <param name="valueProvider">paremeter value provider</param>
    /// <param name="exprType">type of the result</param>
    /// <returns>local expression value</returns>
    static public object CalculateLocalExpression(
      string expr,
      IxValueProvider valueProvider,
      Type exprType)
    {
      string[] paramNames = CxDbParamParser.GetList(expr, false);
      NameValueCollection substitutes = new NameValueCollection();
      foreach (string paramName in paramNames)
      {
        substitutes[paramName] = "?";
      }
      string templateExpr = CxDbParamParser.ReplaceParameters(expr, substitutes);
      return CalculateLocalExpression(templateExpr, paramNames, valueProvider, exprType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="expr">expresion to calculate</param>
    /// <param name="valueProvider">paremeter value provider</param>
    /// <returns>local expression value</returns>
    static public object CalculateLocalExpression(
      string expr,
      IxValueProvider valueProvider)
    {
      return CalculateLocalExpression(expr, valueProvider, typeof(object));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local boolean expression.
    /// </summary>
    /// <param name="expr">expresion to calculate</param>
    /// <param name="valueProvider">paremeter value provider</param>
    /// <returns>local expression value</returns>
    static public bool CalculateLocalBoolExpression(
      string expr,
      IxValueProvider valueProvider)
    {
      return CxBool.Parse(CalculateLocalExpression(expr, valueProvider, typeof(bool)));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes value provider for the given SQL statement from the given
    /// parameter values.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <param name="paramValues">values of parameters</param>
    /// <returns>value provider</returns>
    static public IxValueProvider GetValueProvider(string sql, object[] paramValues)
    {
      CxHashtable provider = new CxHashtable();
      if (CxUtils.NotEmpty(sql) && paramValues != null)
      {
        string[] paramNames = CxDbParamParser.GetList(sql, true);
        for (int i = 0; i < paramNames.Length && i < paramValues.Length; i++)
        {
          provider[paramNames[i]] = paramValues[i];
        }
      }
      return provider;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value cached in the DB cache (Cache table) by the given cache ID.
    /// </summary>
    static public string GetCachedValue(
      CxDbConnection connection,
      string cacheCd)
    {
      DataTable dt = connection.GetQueryResult(
        @"select c.CachedValue
            from Cache c
           where c.CacheCd = :CacheCd",
        cacheCd);
      if (dt.Rows.Count > 0)
      {
        return CxUtils.ToString(dt.Rows[0]["CachedValue"]);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes string value to the DB cache (Cache table).
    /// Returns ID of new record in the cache or null if insert was insuccessful.
    /// </summary>
    static public string SetCachedValue(
      CxDbConnection connection,
      string valueToCache,
      string cacheCd)
    {
      if (CxUtils.NotEmpty(valueToCache))
      {
        bool ownsTransaction = !connection.InTransaction;
        if (ownsTransaction)
        {
          connection.BeginTransaction();
        }
        try
        {
          string sql;

          if (CxUtils.IsEmpty(cacheCd))
          {
            cacheCd = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            sql = "insert into Cache (CacheCd, CachedValue) values (:CacheCd, :CachedValue)";
          }
          else
          {
            sql =
              @"if exists (select 1
                             from Cache c
                            where c.CacheCd = :CacheCd)
               begin
                 update Cache
                    set CachedValue = :CachedValue
                  where CacheCd = :CacheCd
               end
               else
               begin
                 insert into Cache (CacheCd, CachedValue) 
                 values (:CacheCd, :CachedValue)
               end";
          }

          CxHashtable valueProvider = new CxHashtable();
          valueProvider["CacheCd"] = cacheCd;
          valueProvider["CachedValue"] = valueToCache;

          connection.ExecuteCommand(sql, valueProvider);

          if (ownsTransaction)
          {
            connection.Commit();
          }
          return cacheCd;
        }
        catch (Exception e)
        {
          if (ownsTransaction)
          {
            connection.Rollback();
          }
          throw new ExException(e.Message, e);
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes string value to the DB cache (Cache table).
    /// Returns ID of new record in the cache or null if insert was insuccessful.
    /// </summary>
    static public string SetCachedValue(
      CxDbConnection connection,
      string valueToCache)
    {
      return SetCachedValue(connection, valueToCache, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns serializable cached in the DB cache (Cache table).
    /// </summary>
    static public object GetCachedObject(
      CxDbConnection connection,
      string cacheCd,
      Type objectType)
    {
      string cachedValue = GetCachedValue(connection, cacheCd);
      if (CxUtils.NotEmpty(cachedValue))
      {
        XmlSerializer serializer = new XmlSerializer(objectType);
        StringReader stringReader = new StringReader(cachedValue);
        object cachedObject = serializer.Deserialize(stringReader);
        return cachedObject;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes serializable object to the DB cache.
    /// Returns ID of new record in the cache or 0 if insert was insuccessful.
    /// </summary>
    static public string SetCachedObject(
      CxDbConnection connection,
      object valueToCache)
    {
      if (CxUtils.NotEmpty(valueToCache))
      {
        XmlSerializer serializer = new XmlSerializer(valueToCache.GetType());
        StringWriter stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, valueToCache);
        string stringValue = stringWriter.ToString();
        return SetCachedValue(connection, stringValue);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns name-value collection cached in the DB cache (Cache table).
    /// </summary>
    static public NameValueCollection GetCachedNameValueCollection(
      CxDbConnection connection,
      string cacheCd)
    {
      // NameValueCollection raises an exception during serialization,
      // that's why it is converted to array of strings.
      string[] array = (string[]) GetCachedObject(connection, cacheCd, typeof(string[]));
      if (array != null)
      {
        NameValueCollection collection = new NameValueCollection();
        for (int i = 0; i + 1 < array.Length; i += 2)
        {
          collection[array[i]] = array[i + 1];
        }
        return collection;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes name-value collection to the DB cache.
    /// Returns ID of new record in the cache or 0 if insert was insuccessful.
    /// </summary>
    static public string SetCachedNameValueCollection(
      CxDbConnection connection,
      NameValueCollection collection)
    {
      if (collection != null)
      {
        // NameValueCollection raises an exception during serialization,
        // that's why it is converted to array of strings.
        string[] array = new string[collection.Count * 2];
        for (int i = 0; i < collection.AllKeys.Length; i++)
        {
          array[i * 2] = collection.AllKeys[i];
          array[i * 2 + 1] = collection[array[i * 2]];
        }
        return SetCachedObject(connection, array);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds a datarow in the given table that has got values equal 
    /// to the given.
    /// </summary>
    /// <param name="table">a table to perform seek in</param>
    /// <param name="fieldNames">names of the fields to seek by</param>
    /// <param name="values">values to compare with</param>
    /// <returns>an index of a data-row found</returns>
    static public int GetDataRowIndexByValues(
      DataTable table, string[] fieldNames, object[] values)
    {
      var columnIndexes = ConvertColumnNamesToIndices(table.Columns, fieldNames);
      if (columnIndexes.Contains(-1))
        throw new Exception("Some of the columns where not found the the table");
      return GetDataRowIndexByValues(table, columnIndexes, values);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds a datarow in the given table that has got values equal 
    /// to the given.
    /// </summary>
    /// <param name="table">a table to perform seek in</param>
    /// <param name="fieldIndices">indices of the fields to seek by</param>
    /// <param name="values">values to compare with</param>
    /// <returns>an index of a data-row found</returns>
    static public int GetDataRowIndexByValues(
      DataTable table, int[] fieldIndices, object[] values)
    {
      // Todo: Should review a possibility of performing a seek 
      // using DataTable's primary keys.

      for (int i = 0; i < table.Rows.Count; i++)
      {
        DataRow row = table.Rows[i];
        bool equal = true;
        for (int j = 0; j < fieldIndices.Length; j++)
        {
          if (!CxUtils.Compare(values[j], row[fieldIndices[j]]))
          {
            equal = false;
            break;
          }
        }
        if (equal) return i;
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds a datarow in the given table that has got values equal 
    /// to the given.
    /// </summary>
    /// <param name="table">a data-table to search in</param>
    /// <param name="keyInfo">a dictionary with 
    /// field names as keys and field values as values</param>
    /// <returns>an index of a data-row found</returns>
    static public int GetDataRowIndexByValues(
      DataTable table, IDictionary<string, object> keyInfo)
    {
      string[] fieldNames;
      object[] values;
      CxDictionary.ExtractDictionaryKeysAndValues(keyInfo, out fieldNames, out values);
      return GetDataRowIndexByValues(table, fieldNames, values);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns indices of the columns with the given column names.
    /// </summary>
    /// <param name="columns">columns collection to seek in</param>
    /// <param name="columnNames">source column names</param>
    /// <returns>column indices array</returns>
    static public int[] ConvertColumnNamesToIndices(
      DataColumnCollection columns, string[] columnNames)
    {
      int[] columnIndices = new int[columnNames.Length];
      for (int i = 0; i < columnNames.Length; i++)
      {
        columnIndices[i] = columns.IndexOf(columnNames[i]);
      }
      return columnIndices;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts the given data column collection to column array.
    /// </summary>
    /// <param name="collection">a collection to be converted</param>
    /// <returns>an array of data columns</returns>
    static public DataColumn[] ConvertColumnCollectionToColumnArray(
      DataColumnCollection collection)
    {
      DataColumn[] columns = new DataColumn[collection.Count];
      for (int i = 0; i < collection.Count; i++)
      {
        columns[i] = collection[i];
      }
      return columns;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets values of a data-row by names formed as follows: 
    /// prefix + succession_index.
    /// </summary>
    /// <param name="row">a data row to get values from</param>
    /// <param name="prefix">name prefix</param>
    /// <param name="startIndex">start index of the succession</param>
    /// <param name="count">amount of items in the succession</param>
    /// <returns>an array of the data-row values</returns>
    static public object[] GetDataRowValuesByNamesSuccession(
      DataRow row, string prefix, int startIndex, int count)
    {
      object[] values = new object[count];
      for (int i = 0; i < count; i++)
      {
        values[i] = row[prefix + (startIndex + i)];
      }
      return values;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Serializes the given dataset and returns a byte array retaining 
    /// serialized data.
    /// </summary>
    /// <param name="dataset">a dataset to be serialized</param>
    /// <returns>a byte array retaining serialized data</returns>
    static public byte[] SerializeDataSet(DataSet dataset)
    {
      //dataset.RemotingFormat = SerializationFormat.Binary;
      //BinaryFormatter formatter = new BinaryFormatter();
      //MemoryStream stream = new MemoryStream();
      //formatter.Serialize(stream, dataset);
      //return stream.ToArray();
      return AdoNetHelper.SerializeDataSet(dataset);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deserializes the given byte array into a dataset.
    /// </summary>
    /// <param name="data">a byte array to be deserialized into a dataset</param>
    /// <returns>a deserialized dataset</returns>
    static public DataSet DeserializeDataSet(byte[] data)
    {
      //BinaryFormatter formatter = new BinaryFormatter();
      //MemoryStream stream = new MemoryStream(data);
      //return (DataSet) formatter.Deserialize(stream);
      return AdoNetHelper.DeserializeDataSet(data);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts the given system type to the appropriate db type.
    /// </summary>
    /// <param name="type">a type to be converted</param>
    /// <returns>a db type</returns>
    static public DbType ConvertToDbType(Type type)
    {
      if (type == typeof(string))
        return DbType.String;
      if (type == typeof(Int32))
        return DbType.Int32;
      if (type == typeof(DateTime))
        return DbType.DateTime;
      if (type == typeof(bool))
        return DbType.Boolean;
      if (type == typeof(double))
        return DbType.Double;
      if (type == typeof(Int16))
        return DbType.Int16;
      if (type == typeof(Int64))
        return DbType.Int64;
      if (type == typeof(Guid))
        return DbType.Guid;
      if (type == typeof(byte))
        return DbType.Byte;
      if (type == typeof(Single))
        return DbType.Single;
      if (type == typeof(decimal))
        return DbType.Decimal;
      if (type == typeof(sbyte))
        return DbType.SByte;
      if (type == typeof(byte[]))
        return DbType.Binary;
      if (type == typeof(UInt16))
        return DbType.UInt16;
      if (type == typeof(UInt32))
        return DbType.UInt32;
      if (type == typeof(UInt64))
        return DbType.UInt64;
      return DbType.Object;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates copies of the given columns that don't 
    /// belong to any data-table yet.
    /// </summary>
    /// <param name="sourceColumns">a data-column collection to copy from</param>
    /// <returns>an array of copied data-columns</returns>
    static public IList<DataColumn> CopyDataColumns(
      IList<DataColumn> sourceColumns)
    {
      DataColumn[] columns = new DataColumn[sourceColumns.Count];
      for (int i = 0; i < sourceColumns.Count; i++)
      {
        DataColumn column = sourceColumns[i];
        columns[i] = new DataColumn(column.ColumnName, column.DataType);
        if (column.MaxLength > -1)
          columns[i].MaxLength = column.MaxLength;
      }
      return columns;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts given parameter value to XML serializable value.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns>XML serializable value</returns>
    static public object ParamValueToSerializableValue(object value)
    {
      if (value == DBNull.Value)
      {
        return null;
      }

      string text = value as string;
      if (text != null)
      {
        string serializable;
        using (TextWriter tw = new StringWriter())
        {
          using (XmlTextWriter xw = new XmlTextWriter(tw))
          {
            xw.WriteString(text);
            StringBuilder sb = new StringBuilder(tw.ToString());
            sb.Replace("\r", "&#xD;");
            sb.Replace("\n", "&#xA;");
            serializable = sb.ToString();
          }
        }
        return serializable;
      }

      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts serializable parameter value to real parameter value.
    /// </summary>
    /// <param name="value">serializable parameter value</param>
    /// <returns>real parameter value</returns>
    static public object ParamValueFromSerializableValue(object value)
    {
      if (value == null)
      {
        return DBNull.Value;
      }

      string serializable = value as string;
      if (serializable != null)
      {
        string text;
        using (TextReader tr = new StringReader(String.Format("<text>{0}</text>", serializable)))
        using (XmlTextReader xr = new XmlTextReader(tr))
        {
          xr.Read();
          text = xr.ReadString();
        }
        return text;
      }

      return value;
    }
    //-------------------------------------------------------------------------
  }
}