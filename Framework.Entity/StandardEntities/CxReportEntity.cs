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
using System.Xml;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  public class CxReportEntity : CxBaseEntity
  {
    //-------------------------------------------------------------------------
    public const string ENTITY_USAGE_ID_REPORT = "Report";
    public const string ENTITY_USAGE_ID_REPORT_PARAM = "ReportParam";
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public CxReportEntity(CxEntityUsageMetadata metadata) : base(metadata)
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares report for printing, writes a record to report log.
    /// </summary>
    /// <param name="reportCd">report code</param>
    /// <param name="entityUsage">source entity usage</param>
    /// <param name="paramNames">parameter names</param>
    /// <param name="paramValues">parameter values</param>
    /// <param name="entityPkValues">entity primary key values</param>
    /// <param name="userId">current user ID</param>
    /// <returns>report log record ID</returns>
    static public object ReportStart(
      CxDbConnection connection,
      string reportCd,
      CxEntityUsageMetadata entityUsage,
      IList paramNames,
      IList paramValues,
      object[] entityPkValues,
      object userId)
    {
      XmlDocument doc = new XmlDocument();
      XmlElement rootElement = doc.CreateElement("list");
      doc.AppendChild(rootElement);

      for (int i = 0; i < paramNames.Count && i < paramValues.Count; i++)
      {
        XmlElement itemElement = doc.CreateElement("item");
        itemElement.SetAttribute("name", CxUtils.ToString(paramNames[i]));
        itemElement.SetAttribute("value", CxUtils.ToString(paramValues[i]));
        rootElement.AppendChild(itemElement);
      }

      string xmlParams = CxXml.DocToString(doc);

      object entityKey1 =
        entityPkValues != null && entityPkValues.Length > 0 ? entityPkValues[0] : DBNull.Value;
      object entityKey2 =
        entityPkValues != null && entityPkValues.Length > 1 ? entityPkValues[1] : DBNull.Value;
      object entityKey3 =
        entityPkValues != null && entityPkValues.Length > 2 ? entityPkValues[2] : DBNull.Value;

      string sql =
        @"exec dbo.p_Report_PrintStart 
            :MetadataEntityCd,
            :ReportCd,
            :UserId,
            :XmlParams,
            :EntityRecordKey1,
            :EntityRecordKey2,
            :EntityRecordKey3";

      object logId = null;
      if (!connection.InTransaction)
      {
        connection.BeginTransaction();
      }
      try
      {
        logId = connection.ExecuteScalar(
          sql,
          entityUsage.EntityId,
          reportCd,
          userId,
          xmlParams,
          entityKey1,
          entityKey2,
          entityKey3);
        connection.Commit();
      }
      catch (Exception e)
      {
        connection.Rollback();
        throw new ExException(e.Message, e);
      }
      return logId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates record in the report log, does finalization actions.
    /// </summary>
    /// <param name="reportLogId">report log record ID</param>
    /// <param name="isSuccessful">true if report is OK</param>
    static public void ReportFinish(
      CxDbConnection connection,
      object reportLogId,
      bool isSuccessful)
    {
      string sql = "exec dbo.p_Report_PrintFinish :ReportLogId, :IsReportOk";
      if (!connection.InTransaction)
      {
        connection.BeginTransaction();
      }
      try
      {
        connection.ExecuteCommand(sql, reportLogId, isSuccessful ? 1 : 0);
        connection.Commit();
      }
      catch (Exception e)
      {
        connection.Rollback();
        throw new ExException(e.Message, e);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Write Record into Batch Table to perform Batch Printing
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="BatchKey"></param>
    /// <param name="Key1_Int"></param>
    /// <param name="Value1_bit"></param>
    static public void WritoToBatchTable(      
      CxDbConnection connection,
      Guid BatchKey,
      object Key1_Int,
      bool Value1_bit)
    {
      WritoToBatchTable(
        connection,
        BatchKey,
        Key1_Int,
        Value1_bit,
        null,
        null,
        null,
        null,
        null,
        null,
        null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Write Record into Batch Table to perform Batch Printing
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="BatchKey"></param>
    /// <param name="Key1_Int"></param>
    /// <param name="Value1_bit"></param>
    static public void WritoToBatchTable(      
      CxDbConnection connection,
      Guid BatchKey,
      object Key1_Int,
      bool Value1_bit,
      bool Value2_bit,
      string Value1_nvarchar150)
    {
      WritoToBatchTable(
        connection,
        BatchKey,
        Key1_Int,
        Value1_bit,
        Value2_bit,
        null,
        null,
        null,
        null,
        Value1_nvarchar150,
        null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Write Record into Batch Table to perform Batch Printing
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="BatchKey"></param>
    /// <param name="Key1_Int"></param>
    /// <param name="Value1_int"></param>
    static public void WritoToBatchTable(
      CxDbConnection connection,
      Guid BatchKey,
      object Key1_Int,
      int Value1_int)
    {
      WritoToBatchTable(
        connection,
        BatchKey,
        Key1_Int,
        null,
        null,
        null,
        null,
        null,
        Value1_int,
        null,
        null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Write Record into Batch Table to perform Batch Printing
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="BatchKey"></param>
    /// <param name="Key1_Int"></param>
    /// <param name="Value1_bit"></param>
    /// <param name="Value2_bit"></param>
    /// <param name="Value3_bit"></param>
    /// <param name="Value4_bit"></param>
    /// <param name="Value5_bit"></param>
    /// <param name="Value1_int"></param>
    /// <param name="Value1_nvarchar150"></param>
    /// <param name="Value1_datatime"></param>
    static public void WritoToBatchTable(
      CxDbConnection connection,
      Guid BatchKey,
      object Key1_Int,
      object Value1_bit,
      object Value2_bit,
      object Value3_bit,
      object Value4_bit,
      object Value5_bit,
      object Value1_int,
      object Value1_nvarchar150,
      object Value1_datatime)
    {
      connection.ExecuteCommand(@"insert into BatchPrinting (BatchKey,  Key1_Int,  Value1_bit,  Value2_bit,  Value3_bit,  Value4_bit,  Value5_bit,  Value1_int,  Value1_nvarchar150,  Value1_datatime)
                                  values                   (:BatchKey, :Key1_Int, :Value1_bit, :Value2_bit, :Value3_bit, :Value4_bit, :Value5_bit, :Value1_int, :Value1_nvarchar150, :Value1_datatime) ",
                                BatchKey,
                                Key1_Int,
                                Value1_bit,
                                Value2_bit,
                                Value3_bit,
                                Value4_bit,
                                Value5_bit,
                                Value1_int,
                                Value1_nvarchar150,
                                Value1_datatime);
    }
    
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns full report path and file name by the given report code.
    /// </summary>
    static public string GetReportPathAndName(CxDbConnection connection, string reportCd)
    {
      return CxUtils.ToString(
        connection.ExecuteScalar("select dbo.f_Get_Report_PathAndName(:ReportCd)", reportCd));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads report entity from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    /// <param name="reportCode">report code</param>
    static public CxBaseEntity GetReportEntity(
      CxDbConnection connection,
      CxMetadataHolder holder,
      string reportCode)
    {
      CxEntityUsageMetadata entityUsage = holder.EntityUsages[ENTITY_USAGE_ID_REPORT];
      CxBaseEntity entity = CreateAndReadFromDb(entityUsage, connection, new object[] { reportCode });
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of report patameter entities.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    /// <param name="reportEntity">report entity</param>
    /// <returns>list of CxBaseEntity objects</returns>
    static public IList GetReportParameters(
      CxDbConnection connection,
      CxMetadataHolder holder,
      CxBaseEntity reportEntity)
    {
      CxEntityUsageMetadata entityUsage = holder.EntityUsages[ENTITY_USAGE_ID_REPORT_PARAM];
      DataTable dt = new DataTable();
      entityUsage.ReadData(connection, dt, "", reportEntity);
      ArrayList list = new ArrayList();
      foreach (DataRow dr in dt.Rows)
      {
        CxBaseEntity entity = CreateByDataRow(entityUsage, dr);
        list.Add(entity);
      }
      return list;
    }
    //-------------------------------------------------------------------------
  }
}