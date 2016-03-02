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
using System.Collections.Specialized;
using System.Web;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Log source.
  /// </summary>
  public enum NxLogSource
  {
    Commidea,
    System,
    Google,
    DHL
  }
  //---------------------------------------------------------------------------
  /// <summary>
	/// Utility methods for work with application log stored in the database.
	/// </summary>
	public class CxDbLog : IxLog
	{
    //-------------------------------------------------------------------------
    public const string ENTITY_USAGE_ID_LOG_RECORD        = "LogRecord";
    public const string ENTITY_USAGE_ID_LOG_RECORD_DETAIL = "LogRecordDetail";
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    protected NxLogSource m_LogSource = NxLogSource.System;
    protected CxBaseEntity m_Entity = null;
    protected string m_EntityCode = null;
    protected string m_EntityPkValue1 = null;
    protected string m_EntityPkValue2 = null;
    protected string m_EntityPkValue3 = null;
    //-------------------------------------------------------------------------
    private int _logEntityId = -1;
    public int LogEntityId
    {
      get
      {
        return _logEntityId;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="logSource"></param>
    /// <param name="entity"></param>
    /// <param name="entityCode"></param>
    /// <param name="entityPkValue1"></param>
    /// <param name="entityPkValue2"></param>
    /// <param name="entityPkValue3"></param>
    public CxDbLog(
      CxMetadataHolder holder,
      NxLogSource logSource,
      CxBaseEntity entity,
      string entityCode,
      string entityPkValue1,
      string entityPkValue2,
      string entityPkValue3)
    {
      m_Holder = holder;
      m_LogSource = logSource;
      m_Entity = entity;
      m_EntityCode = entityCode;
      m_EntityPkValue1 = entityPkValue1;
      m_EntityPkValue2 = entityPkValue2;
      m_EntityPkValue3 = entityPkValue3;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes record to log.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    /// <param name="description"></param>
    /// <param name="parameters"></param>
    public void LogWrite(
      NxLogLevel level,
      string message,
      string description,
      NameValueCollection parameters)
    {
      using (CxDbConnection connection = m_Holder.CreateDbConnection())
      {
        // Get entity code and entity primary key value.
        if (m_Entity != null && CxUtils.IsEmpty(m_EntityCode))
        {
          m_EntityCode = CxUtils.ToString(connection.ExecuteScalar(
            @"select me.EntityCd 
              from EntityUsageEntityCodes me
             where me.MetadataEntityCd = :MetadataEntityCd",
            m_Entity.Metadata.EntityId));
          if (CxUtils.NotEmpty(m_EntityCode))
          {
            string[] pkNames;
            object[] pkValues;
            m_Entity.GetPrimaryKeyValues(out pkNames, out pkValues);
            if (pkValues != null && pkValues.Length >= 1)
            {
              m_EntityPkValue1 = CxUtils.ToString(pkValues[0]);
            }
            if (pkValues != null && pkValues.Length >= 2)
            {
              m_EntityPkValue2 = CxUtils.ToString(pkValues[1]);
            }
            if (pkValues != null && pkValues.Length >= 3)
            {
              m_EntityPkValue3 = CxUtils.ToString(pkValues[2]);
            }
            if (CxUtils.IsEmpty(m_EntityPkValue1))
            {
              m_EntityCode = null;
            }
          }
        }
        // Write log record.
        connection.BeginTransaction();
        try
        {
          CxBaseEntity logEntity = CxBaseEntity.CreateWithDefaults(
            m_Holder.EntityUsages[ENTITY_USAGE_ID_LOG_RECORD],
            null,
            null,
            connection);
          logEntity.IsNew = true;

          logEntity["LogLevelPriority"] = (int) level;
          logEntity["LogSource_CD"] = m_LogSource.ToString().ToUpper();
          logEntity["CreateDate"] = DateTime.Now;
          logEntity["Message"] = message;
          logEntity["Description"] = description;
          logEntity["EntityCd"] = m_EntityCode;
          logEntity["EntityRecordKey1"] = m_EntityPkValue1;
          logEntity["EntityRecordKey2"] = m_EntityPkValue2;
          logEntity["EntityRecordKey3"] = m_EntityPkValue3;

          InitLogEntity(logEntity, connection);

          logEntity.WriteChangesToDb(connection);
          if (logEntity["LogRecordId"] != null)
          {
            _logEntityId = CxInt.Parse(logEntity["LogRecordId"], -1);
          }
          // Write log record parameters
          if (parameters != null)
          {
            foreach (string paramName in parameters.AllKeys)
            {
              string paramValue = parameters[paramName];

              CxBaseEntity logDetailEntity = CxBaseEntity.CreateWithDefaults(
                m_Holder.EntityUsages[ENTITY_USAGE_ID_LOG_RECORD_DETAIL],
                null,
                null,
                connection);
              logDetailEntity.IsNew = true;

              logDetailEntity["LogRecordId"] = logEntity["LogRecordId"];
              logDetailEntity["ParamName"] = paramName;
              logDetailEntity["ParamValue"] = paramValue;

              logDetailEntity.WriteChangesToDb(connection);
            }
          }

          connection.Commit();
        }
        catch (Exception e)
        {
          connection.Rollback();
          throw new ExIncapsulatedException(e);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes exception info to log.
    /// </summary>
    /// <param name="e">exception to write info</param>
    public void LogException(Exception e)
    {
      Exception actualException = CxUtils.GetOriginalException(e);
      string message = e.Message;
      string description = CxCommon.GetExceptionFullStackTrace(actualException);
      LogWrite(NxLogLevel.Error, message, description, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Performs additional log entity initialization before saving to DB.
    /// </summary>
    /// <param name="logEntity">log entity to initialize</param>
    /// <param name="connection">database connection</param>
    virtual protected void InitLogEntity(CxBaseEntity logEntity, CxDbConnection connection)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes record to log.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="source">source</param>
    /// <param name="level">error level</param>
    /// <param name="message">message</param>
    /// <param name="description">deacription</param>
    /// <param name="entityCode">related entity code</param>
    /// <param name="entityPkValue1">entity primary key first field value</param>
    /// <param name="entityPkValue2">entity primary key second field value</param>
    /// <param name="entityPkValue3">entity primary key third field value</param>
    /// <param name="parameters">additional parameters</param>
    static public void LogWrite(
      CxMetadataHolder holder,
      NxLogSource source,
      NxLogLevel level,
      string message,
      string description,
      string entityCode,
      string entityPkValue1,
      string entityPkValue2,
      string entityPkValue3,
      NameValueCollection parameters)
    {
      CxDbLog dbLog = new CxDbLog(
        holder,
        source,
        null,
        entityCode,
        entityPkValue1,
        entityPkValue2,
        entityPkValue3);

      dbLog.LogWrite(level, message, description, parameters);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes record to log.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="source">source</param>
    /// <param name="level">error level</param>
    /// <param name="message">message</param>
    /// <param name="description">deacription</param>
    /// <param name="entity">related entity</param>
    /// <param name="parameters">additional parameters</param>
    static public void LogWrite(
      CxMetadataHolder holder,
      NxLogSource source,
      NxLogLevel level,
      string message,
      string description,
      CxBaseEntity entity,
      NameValueCollection parameters)
    {
      CxDbLog dbLog = new CxDbLog(
        holder,
        source,
        entity,
        null,
        null,
        null,
        null);

      dbLog.LogWrite(level, message, description, parameters);
    }
    //-------------------------------------------------------------------------
  }
}