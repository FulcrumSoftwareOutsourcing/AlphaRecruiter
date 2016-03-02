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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Framework.Common;

namespace Framework.Utils
{
  public class CxBaseTextLogger : IxLogger
  {
    //-------------------------------------------------------------------------
    static protected CxBaseTextLogger m_Instance = null;
    //-------------------------------------------------------------------------
    protected UniqueList<string> m_ExceptionTypeNamesToIgnore =
      new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxBaseTextLogger()
		{
      string exceptionTypeNamesToIgnore = 
        CxUtils.Nvl(CxConfigurationHelper.ErrorLogIgnoreExceptions);
      if (CxUtils.NotEmpty(exceptionTypeNamesToIgnore))
      {
        IList<string> typeNameList = CxText.DecomposeWithSeparator(exceptionTypeNamesToIgnore, ",");
        foreach (string typeName in typeNameList)
        {
          m_ExceptionTypeNamesToIgnore.Add(typeName);
        }
      }
      DeleteExpiredLogFiles();
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns log file folder.
    /// </summary>
    /// <returns></returns>
    virtual protected string GetLogFileFolder()
    {
      return CxPath.GetUserSettingsFolder();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns log file name.
    /// </summary>
    /// <returns></returns>
    protected string GetLogFileName()
    {
      return "Log_" + DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".txt";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns log record header text.
    /// </summary>
    virtual protected string GetHeaderText()
    {
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns log record footer text.
    /// </summary>
    virtual protected string GetFooterText()
    {
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes string to log.
    /// </summary>
    /// <param name="message"></param>
    public void Write(string message)
	  {
      lock (typeof(CxBaseTextLogger))
      {
        string logFolder = GetLogFileFolder();
        if (!Directory.Exists(logFolder))
        {
          Directory.CreateDirectory(logFolder);
        }
        string logFileName = GetLogFileName();
        string logFileFullName = Path.Combine(logFolder, logFileName);

        string header = GetHeaderText();
        string footer = GetFooterText();
        using (StreamWriter sw = File.AppendText(logFileFullName))
        {
          if (CxUtils.NotEmpty(header))
          {
            sw.WriteLine(header);
          }

          sw.WriteLine(message);

          if (CxUtils.NotEmpty(footer))
          {
            sw.WriteLine(footer);
          }
        }
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes expired log files.
    /// </summary>
    protected void DeleteExpiredLogFiles()
    {
      string logFolder = GetLogFileFolder();
      if (Directory.Exists(logFolder))
      {
        string[] files = Directory.GetFiles(logFolder, "Log_????????.txt");
        foreach (string fileName in files)
        {
          string strDate = Path.GetFileNameWithoutExtension(fileName).Substring(4);
          if (CxText.RegexValidate(strDate, "\\d*"))
          {
            try
            {
              DateTime fileDate = DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.CurrentCulture.DateTimeFormat);
              if ((DateTime.Now - fileDate).TotalDays > CxConfigurationHelper.ErrorLogDaysToKeep)
              {
                File.Delete(fileName);
              }
            }
            catch
            {
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given exception should be ignored and not logged.
    /// </summary>
    /// <param name="e">exception to check</param>
    /// <returns>true if the given exception should be not logged</returns>
    public bool IsExceptionIgnored(Exception e)
    {
      if (e == null)
      {
        return true;
      }
      Type type = e.GetType();
      while (type != null && type != typeof(Exception))
      {
        if (m_ExceptionTypeNamesToIgnore.Contains(type.FullName))
        {
          return true;
        }
        type = type.BaseType;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current logger instance.
    /// </summary>
    static public CxBaseTextLogger Instance
    {
      get
      {
        lock (typeof(CxBaseTextLogger))
        {
          if (m_Instance == null)
          {
            m_Instance = new CxBaseTextLogger();
          }
          return m_Instance;
        }
      }
    }
    //-------------------------------------------------------------------------
   }
}