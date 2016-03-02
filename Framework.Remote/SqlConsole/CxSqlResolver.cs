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
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Framework.Db;

namespace Framework.Remote
{
  /// <summary>
  /// Class exequtes SQL statements and returns results as XML.
  /// </summary>
  public class CxSqlResolver
  {
    /// <summary>
    /// Splits sql script at the single statements using 'GO' word as splitter. 
    /// </summary>
    /// <param name="sqlScript">Sql script to proccess.</param>
    /// <returns>Array of single sql statements.</returns>
    private string[] GetScriptSequence(string sqlScript)
    {
      if (string.IsNullOrEmpty(sqlScript))
        return new string[0];
      //split on 'GO' word
      string[] splittedSql = Regex.Split(sqlScript, @"\sGO\s", RegexOptions.IgnoreCase);
      if (splittedSql.Length > 0)
      {
        //remove 'GO' at the end of all statement, if exists
        splittedSql[splittedSql.Length - 1] =
          Regex.Replace(splittedSql[splittedSql.Length - 1], @"\sGO\z*", "", RegexOptions.IgnoreCase);
      }
      return splittedSql;
    }

    /// <summary>
    /// Executes given SQL statement and return result as XML.
    /// </summary>
    /// <param name="sqlScript"></param>
    /// <returns></returns>
    public string ExecuteDeploymentStatement(string sqlScript, SqlConnection connection)
    {
      XElement result = new XElement("result");// result XML for client
      string[] splittedSql = GetScriptSequence(sqlScript);
      using (connection)
      {


        foreach (string sql in splittedSql)
        {
          if (!Regex.Match(sql, @"\S").Success) //ignore witespace(empty sql) 
            continue;




      
            if (connection.State == ConnectionState.Closed)
              connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);

            using (IDataReader reader = command.ExecuteReader())
            {
              do
              {
                List<CxCoumnDescriptor> descriptors = new List<CxCoumnDescriptor>();
                SetColumns(descriptors, reader.GetSchemaTable()); // get info about columns and columns ordinals
                while (reader.Read())
                {
                  foreach (CxCoumnDescriptor descriptor in descriptors)
                  {
                    descriptor.AddData(reader[descriptor.Ordinal]); // get data
                  }
                }

                // successful statement without results
                if (descriptors.Count == 1 && descriptors[0].Name == "null_table")
                {
                  descriptors[0].Name = string.Empty;
                  descriptors[0].AddData("Command(s) completed successfully.");
                }

                // add tables XML in resut XML
                XElement xTable = new XElement("t");
                foreach (CxCoumnDescriptor descriptor in descriptors)
                {
                  xTable.Add(descriptor.XmlData);
                }
                result.Add(xTable);

              } while (reader.NextResult());

            }
            //     connection.Close();

        }
      }

      return result.ToString();
    }


    /// <summary>
    /// Executes given SQL statement and return result as XML.
    /// </summary>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
    public string ExecuteStatement(string sqlStatement)
    {
      XElement result = new XElement("result");// result XML for client
      string[] splittedSql = GetScriptSequence(sqlStatement);
      foreach (string sql in splittedSql)
      {
        if (!Regex.Match(sql, @"\S").Success)//ignore witespace(empty sql) 
          continue;

        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          try
          {
            conn.BeginTransaction();
            using (IDataReader reader = conn.ExecuteReader(sql))
            {
              do
              {
                List<CxCoumnDescriptor> descriptors = new List<CxCoumnDescriptor>();
                SetColumns(descriptors, reader.GetSchemaTable());// get info about columns and columns ordinals
                while (reader.Read())
                {
                  foreach (CxCoumnDescriptor descriptor in descriptors)
                  {
                    descriptor.AddData(reader[descriptor.Ordinal]);// get data
                  }
                }

                // successful statement without results
                if (descriptors.Count == 1 && descriptors[0].Name == "null_table")
                {
                  descriptors[0].Name = string.Empty;
                  descriptors[0].AddData("Command(s) completed successfully.");
                }

                // add tables XML in resut XML
                XElement xTable = new XElement("t");
                foreach (CxCoumnDescriptor descriptor in descriptors)
                {
                  xTable.Add(descriptor.XmlData);
                }
                result.Add(xTable);

              }
              while (reader.NextResult());

            }
            conn.Commit();
          }
          catch (Exception ex)
          {
            conn.Rollback();
            result.Add(CreateErrorXml(ex.Message, ex.StackTrace));
          }
        }
      }

      return result.ToString();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and fills XML by given error message and stack trace.
    /// </summary>
    /// <param name="message">Message of occured Exception.</param>
    /// <param name="stackTrace">Stack trace of occured Exception.</param>
    /// <returns>Created XML.</returns>
    private XElement CreateErrorXml(string message, string stackTrace)
    {
      XElement xTable = new XElement("t");
      XElement xClm = new XElement("c");
      XAttribute name = new XAttribute("n", "error");
      XAttribute isError = new XAttribute("isError", "true");
      xTable.Add(xClm);
      xClm.Add(name);
      xTable.Add(isError);
      XElement xErrorCell = new XElement("i");
      xErrorCell.Value = string.Concat("Message:\n\t\t\t", message, "\n", "StackTrace:\n", stackTrace);
      xClm.Add(xErrorCell);
      return xTable;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Initializes given CxCoumnDescriptor collection using given schema table.
    /// </summary>
    /// <param name="descriptors">List of CxCoumnDescriptor to initialize.</param>
    /// <param name="resultTable">Table with schema information.</param>
    private void SetColumns(ICollection<CxCoumnDescriptor> descriptors, DataTable resultTable)
    {
      if (resultTable == null)
      {
        //successful statement without results
        // add default column
        descriptors.Add(
          new CxCoumnDescriptor
          {
            Name = "null_table",
            Ordinal = 0
          });
      }
      else
      {
        foreach (DataRow row in resultTable.Rows)
        {
          descriptors.Add(
            new CxCoumnDescriptor
            {
              Name = row["ColumnName"].ToString(),
              Ordinal = Convert.ToInt32(row["ColumnOrdinal"])
            });
        }
      }


    }
  }
}
