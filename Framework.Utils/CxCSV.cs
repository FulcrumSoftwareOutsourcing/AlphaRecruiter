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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Delegate to format data row value.
  /// </summary>
  public delegate string DxFormatDataRowValue(DataRow dr, string columnName);
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class to work with CSV (comma-separated values) format.
  /// </summary>
  public class CxCSV
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts datatable content to CSV string.
    /// </summary>
    /// <param name="dt">data table to convert to CSV text</param>
    /// <param name="columnsToExport">list of columns to export (null to export all columns)</param>
    /// <param name="exportHeader">true to export header (column captions in first row)</param>
    /// <param name="columnCaptions">list of column captions</param>
    /// <param name="formatDataRowValue">value formatter delegate</param>
    /// <param name="listSeparator">list separator (comma is the default)</param>
    /// <returns>text in CSV format</returns>
    static public string DataTableToCsv(
      DataTable dt,
      ICollection columnsToExport,
      bool exportHeader,
      IDictionary columnCaptions,
      DxFormatDataRowValue formatDataRowValue,
      string columnListSeparator,
      string listSeparator)
    {
      // The pure list of column names.
      List<string> columnNames = new List<string>();
      if (columnsToExport != null)
      {
        foreach (string name in columnsToExport)
        {
          if (dt.Columns.IndexOf(name) >= 0)
          {
            columnNames.Add(name);
          }
        }
      }
      else
      {
        foreach (DataColumn dc in dt.Columns)
        {
          columnNames.Add(dc.ColumnName);
        }
      }
      List<string> columnCaptionList = new List<string>();
      foreach (string columnName in columnNames)
      {
        columnCaptionList.Add(CxUtils.Nvl(columnCaptions != null ? (string) columnCaptions[columnName] : null, columnName));
      }

      StringWriter sw = new StringWriter();

      // Write the header in the stream.
      if (exportHeader)
      {
        string columnString = string.Join(columnListSeparator, columnCaptionList.ToArray());
        sw.Write(columnString);
        sw.WriteLine();
      }

      // Write the rows in the stream.
      foreach (DataRow dr in dt.Rows)
      {
        for (int i = 0; i < columnNames.Count; i++)
        {
          if (i > 0)
            sw.Write(listSeparator);

          string columnName = columnNames[i];
          string formattedValue =
            formatDataRowValue != null ? formatDataRowValue(dr, columnName) : CxUtils.ToString(dr[columnName]);
          sw.Write(CxText.GetQuotedString(formattedValue, TextQualifier, QuoteReplacer, NxGetQuotedStringMode.LeaveWhiteSpaceChars));
        }
        sw.WriteLine();
      }
      return sw.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts datatable content to CSV string.
    /// </summary>
    /// <param name="dt">data table to convert to CSV text</param>
    /// <param name="columnsToExport">list of columns to export (null to export all columns)</param>
    /// <param name="exportHeader">true to export header (column captions in first row)</param>
    /// <param name="columnCaptions">list of column captions</param>
    /// <param name="formatDataRowValue">value formatter delegate</param>
    /// <returns>text in CSV format</returns>
    static public string DataTableToCsv(
      DataTable dt,
      ICollection columnsToExport,
      bool exportHeader,
      IDictionary columnCaptions,
      DxFormatDataRowValue formatDataRowValue)
    {
      return DataTableToCsv(
        dt, columnsToExport, exportHeader, columnCaptions, formatDataRowValue, ColumnListSeparator, ListSeparator);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current culture list separator.
    /// </summary>
    static public string ListSeparator
    {
      get
      {
        return CultureInfo.CurrentCulture.TextInfo.ListSeparator;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current culture list separator.
    /// </summary>
    static public string ColumnListSeparator
    {
      get
      {
        return ",";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns CSV file text qualifier.
    /// </summary>
    static public char TextQualifier
    { get { return '"'; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns quote replacer string (double text qualifier).
    /// </summary>
    static public string QuoteReplacer
    { get { return new string(TextQualifier, 2); } }
    //-------------------------------------------------------------------------
  }
}