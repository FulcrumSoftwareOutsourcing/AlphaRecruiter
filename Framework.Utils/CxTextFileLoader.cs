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
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Framework.Utils
{
  /// <summary>
  /// Class for load data from text file into DataTable.
  /// </summary>
  public class CxTextFileLoader
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// File format enumeration
    /// </summary>
    public enum NxFileFormat { Delimited, Fixed }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Class containing column description.
    /// </summary>
    protected class CxColumnDescription
    {
      //----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      public CxColumnDescription()
      {
      }
      //----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      public CxColumnDescription(string name, Type dataType)
      {
        Name = name;
        DataType = dataType;
      }
      //----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      public CxColumnDescription(XmlElement element)
      {
        if (element != null)
        {
          Name = CxXml.GetAttr(element, "name");
          switch (CxXml.GetAttr(element, "type"))
          {
            case "longstring": DataType = typeof(string); break;
            case "string": DataType = typeof(string); break;
            case "int": DataType = typeof(int); break;
            case "bool": DataType = typeof(bool); break;
            case "decimal": DataType = typeof(decimal); break;
            case "float": DataType = typeof(float); break;
            case "double": DataType = typeof(double); break;
            case "datetime": DataType = typeof(DateTime); break;
            default: DataType = typeof(string); break;
          }
          FixedSize = CxInt.Parse(CxXml.GetAttr(element, "fixed_size"), 0);
          Format = CxXml.GetAttr(element, "format");
          SourceColumnName = CxXml.GetAttr(element, "source_column_name");
        }
      }
      //----------------------------------------------------------------------
      /// <summary>
      /// Converts text value to the data value.
      /// </summary>
      public object GetDataValue(string textValue)
      {
        if (CxUtils.NotEmpty(textValue))
        {
          try
          {
            if (DataType == typeof(string))
            {
              return textValue;
            }
            else if (DataType == typeof(int))
            {
              return int.Parse(textValue);
            }
            else if (DataType == typeof(bool))
            {
              return CxBool.Parse(textValue);
            }
            else if (DataType == typeof(decimal))
            {
              return decimal.Parse(textValue, NumberFormatInfo.InvariantInfo);
            }
            else if (DataType == typeof(float))
            {
              return float.Parse(textValue, NumberFormatInfo.InvariantInfo);
            }
            else if (DataType == typeof(double))
            {
              return double.Parse(textValue, NumberFormatInfo.InvariantInfo);
            }
            else if (DataType == typeof(DateTime))
            {
              if (CxUtils.IsEmpty(Format))
              {
                return DateTime.Parse(textValue);
              }
              else
              {
                return DateTime.ParseExact(textValue, Format, null);
              }
            }
          }
          catch
          {
            return DBNull.Value;
          }
        }
        return DBNull.Value;
      }
      //----------------------------------------------------------------------
      /// <summary>
      /// Name of the column
      /// </summary>
      public string Name;
      //----------------------------------------------------------------------
      /// <summary>
      /// Type of the column
      /// </summary>
      public Type DataType;
      //----------------------------------------------------------------------
      /// <summary>
      /// Fixed size of the column for the fixed-width column file
      /// </summary>
      public int FixedSize = 0;
      //----------------------------------------------------------------------
      /// <summary>
      /// DateTime or Number format to convert value from text
      /// </summary>
      public string Format;
      //----------------------------------------------------------------------
      /// <summary>
      /// Name of the source column mapped to the target (this) column.
      /// </summary>
      public string SourceColumnName;
      //----------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    protected string m_DataFileName = null;
    protected NxFileFormat m_FileFormat = NxFileFormat.Delimited;
    protected string m_Separator = ",";
    private string m_ColumnNameSeparator = ",";
    protected char m_TextQualifier = '\"';
    protected bool m_IsFirstRowHasColumnNames = false;
    protected char m_DecimalSeparator = '.';
    protected ArrayList m_SourceColumnList = new ArrayList();
    protected Hashtable m_SourceColumnMap = new Hashtable();
    protected ArrayList m_TargetColumnList = new ArrayList();
    protected Hashtable m_TargetColumnMap = new Hashtable();
    protected DataTable m_SourceTable = new DataTable();
    protected DataTable m_TargetTable = new DataTable();
    private string m_CsvContent;
    //---------------------------------------------------------------------------
    public string CsvContent
    {
      get { return m_CsvContent; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxTextFileLoader(string dataFileName, string xmlDescFileName)
    {
      m_DataFileName = dataFileName;
      LoadXmlDescription(CxXml.LoadDocument(xmlDescFileName));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxTextFileLoader(string dataFileName, XmlDocument xmlDoc)
    {
      m_DataFileName = dataFileName;
      LoadXmlDescription(xmlDoc);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxTextFileLoader(XmlDocument xmlDoc)
    {
      LoadXmlDescription(xmlDoc);
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads file description from the XML document.
    /// </summary>
    protected void LoadXmlDescription(XmlDocument xmlDoc)
    {
      if (xmlDoc != null && xmlDoc.DocumentElement != null)
      {
        XmlElement rootElement = xmlDoc.DocumentElement;

        m_FileFormat = CxEnum.Parse<NxFileFormat>(
          CxXml.GetAttr(rootElement, "format"), NxFileFormat.Delimited);

        m_Separator = CxXml.GetAttr(rootElement, "separator");

        string textQualifier = CxXml.GetAttr(rootElement, "text_qualifier");
        m_TextQualifier = CxUtils.NotEmpty(textQualifier) ? textQualifier[0] : '\x0';

        m_IsFirstRowHasColumnNames =
          CxBool.Parse(CxXml.GetAttr(rootElement, "first_row_has_column_names"));

        string decimalSeparator = CxXml.GetAttr(rootElement, "decimal_separator");
        m_DecimalSeparator = CxUtils.NotEmpty(decimalSeparator) ? decimalSeparator[0] : '.';

        XmlElement columnsElement = (XmlElement) rootElement.SelectSingleNode("columns");
        if (columnsElement != null)
        {
          foreach (XmlElement element in columnsElement.SelectNodes("column"))
          {
            CxColumnDescription columnDesc = new CxColumnDescription(element);
            m_SourceColumnList.Add(columnDesc);
            m_SourceColumnMap.Add(columnDesc.Name, columnDesc);
          }
        }

        XmlElement targetColumnsElement = (XmlElement) rootElement.SelectSingleNode("target_columns");
        if (targetColumnsElement != null)
        {
          foreach (XmlElement element in targetColumnsElement.SelectNodes("column"))
          {
            CxColumnDescription columnDesc = new CxColumnDescription(element);
            m_TargetColumnList.Add(columnDesc);
            m_TargetColumnMap.Add(columnDesc.Name, columnDesc);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads the full CSV line from the reader, taking into account any possible 
    /// caret returns inside it.
    /// </summary>
    /// <param name="reader">the reader to read by</param>
    /// <returns>the full line read</returns>
    public string ReadFullLine(StreamReader reader)
    {
      string line = reader.ReadLine();

      while (CxText.CalcEntriesAmount(line, "\"") % 2 == 1)
      {
        line += Environment.NewLine + reader.ReadLine();
      }
      return line;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads data from text file into the data table.
    /// </summary>
    public DataTable LoadData(string csvContent)
    {
      m_CsvContent = csvContent;
      return LoadData();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads data from text file into the data table.
    /// </summary>
    public DataTable LoadData()
    {
      m_SourceTable.Clear();
      m_SourceTable.Columns.Clear();
      m_TargetTable.Clear();
      m_TargetTable.Columns.Clear();

      foreach (CxColumnDescription columnDesc in m_SourceColumnList)
      {
        m_SourceTable.Columns.Add(columnDesc.Name, columnDesc.DataType);
      }

      foreach (CxColumnDescription columnDesc in m_TargetColumnList)
      {
        m_TargetTable.Columns.Add(columnDesc.Name, columnDesc.DataType);
      }

      // Read source file into the source data table.
      StreamReader reader;
      if (CsvContent == null)
        reader = File.OpenText(m_DataFileName);
      else
      {
        MemoryStream ms = new MemoryStream();
        byte[] csvBytes = Encoding.UTF8.GetBytes(CsvContent);
        ms.Write(csvBytes, 0, csvBytes.Length);
        ms.Position = 0;
        reader = new StreamReader(ms);
      }
      try
      {
        Hashtable columnNameMap = null;
        int lineIndex = 0;
        string line = ReadFullLine(reader);
        while (line != null)
        {
          line = GetPreProcessedLine(line);
          if (lineIndex == 0 && m_IsFirstRowHasColumnNames)
          {
            // Get column names map from the first line
            string[] names = GetSplittedLineData(line, m_ColumnNameSeparator);
            if (names != null && names.Length > 0)
            {
              columnNameMap = new Hashtable();
              for (int i = 0; i < names.Length; i++)
              {
                columnNameMap[names[i]] = i;
              }
            }
          }
          else
          {
            // Read values from the text line
            string[] values = GetSplittedLineData(line);
            if (values != null && values.Length > 0)
            {
              DataRow dr = m_SourceTable.NewRow();
              for (int i = 0; i < m_SourceTable.Columns.Count; i++)
              {
                DataColumn dc = m_SourceTable.Columns[i];
                int valueIndex = i;
                if (columnNameMap != null)
                {
                  valueIndex = CxInt.Parse(columnNameMap[dc.ColumnName], -1);
                }
                if (valueIndex >= 0 && valueIndex < values.Length)
                {
                  CxColumnDescription columnDesc = (CxColumnDescription) m_SourceColumnMap[dc.ColumnName];
                  if (columnDesc != null)
                  {
                    string textValue = GetPreProcessedTextValue(columnDesc, values[valueIndex]);
                    dr[dc] = columnDesc.GetDataValue(textValue);
                  }
                }
              }
              m_SourceTable.Rows.Add(dr);
            }
          }
          lineIndex++;
          line = ReadFullLine(reader);
        }
      }
      finally
      {
        reader.Close();
      }

      if (m_SourceTable.Columns.Count > 0)
      {
        if (m_TargetTable.Columns.Count > 0)
        {
          // Map from source data table to the target data table
          foreach (DataRow sourceRow in m_SourceTable.Rows)
          {
            DataRow targetRow = m_TargetTable.NewRow();
            foreach (DataColumn targetColumn in m_TargetTable.Columns)
            {
              CxColumnDescription columnDesc = (CxColumnDescription) m_TargetColumnMap[targetColumn.ColumnName];
              if (columnDesc != null && CxUtils.NotEmpty(columnDesc.SourceColumnName))
              {
                int sourceIndex = m_SourceTable.Columns.IndexOf(columnDesc.SourceColumnName);
                if (sourceIndex >= 0)
                {
                  targetRow[targetColumn] = sourceRow[sourceIndex];
                }
              }
            }
            m_TargetTable.Rows.Add(targetRow);
          }
          return m_TargetTable;
        }
        else
        {
          return m_SourceTable;
        }
      }

      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns line data splitted to text values of columns.
    /// </summary>
    protected string[] GetSplittedLineData(string line, string separator)
    {
      if (CxUtils.NotEmpty(line))
      {
        IList<string> list = null;
        if (m_FileFormat == NxFileFormat.Delimited)
        {
          list = CxText.DecomposeWithSeparator(line, separator, m_TextQualifier);
        }
        else if (m_FileFormat == NxFileFormat.Fixed)
        {
          list = new List<string>();
          int currentIndex = 0;
          foreach (CxColumnDescription columnDesc in m_SourceColumnList)
          {
            string s = null;
            if (columnDesc.FixedSize > 0 && currentIndex + columnDesc.FixedSize <= line.Length)
            {
              s = line.Substring(currentIndex, columnDesc.FixedSize);
            }
            if (s != null)
            {
              list.Add(s.Trim());
            }
            currentIndex += columnDesc.FixedSize;
          }
        }
        if (list != null)
        {
          string[] array = new string[list.Count];
          list.CopyTo(array, 0);
          return array;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns line data splitted to text values of columns.
    /// </summary>
    protected string[] GetSplittedLineData(string line)
    {
      return GetSplittedLineData(line, m_Separator);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns pre-processed line value.
    /// Strips special characters from the line.
    /// </summary>
    protected string GetPreProcessedLine(string line)
    {
      if (CxUtils.NotEmpty(line))
      {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
          char ch = line[i];
          if (ch >= 32 || char.IsWhiteSpace(ch))
          {
            sb.Append(line[i]);
          }
        }
        return sb.ToString().Trim();
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns pre-processed text value.
    /// </summary>
    protected string GetPreProcessedTextValue(
      CxColumnDescription columnDesc,
      string textValue)
    {
      if (CxUtils.NotEmpty(textValue))
      {
        if (CxType.IsNumber(columnDesc.DataType))
        {
          return textValue.Replace(
            new String(m_DecimalSeparator, 1),
            NumberFormatInfo.InvariantInfo.CurrencyDecimalSeparator);
        }
      }
      return textValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets data file name.
    /// </summary>
    public string DataFileName
    { get { return m_DataFileName; } set { m_DataFileName = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets data file format.
    /// </summary>
    public NxFileFormat FileFormat
    { get { return m_FileFormat; } set { m_FileFormat = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets field separator.
    /// </summary>
    public string Separator
    { get { return m_Separator; } set { m_Separator = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets field text qualifier.
    /// </summary>
    public char TextQualifier
    { get { return m_TextQualifier; } set { m_TextQualifier = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if first row should contain column names.
    /// </summary>
    public bool IsFirstRowHasColumnNames
    { get { return m_IsFirstRowHasColumnNames; } set { m_IsFirstRowHasColumnNames = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets decimal separator.
    /// </summary>
    public char DecimalSeparator
    { get { return m_DecimalSeparator; } set { m_DecimalSeparator = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Separator for column names.
    /// </summary>
    public string ColumnNameSeparator
    {
      get { return m_ColumnNameSeparator; }
      set { m_ColumnNameSeparator = value; }
    }
    //-------------------------------------------------------------------------
  }
}