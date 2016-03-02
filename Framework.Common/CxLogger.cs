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
using System.Diagnostics;

namespace Framework.Utils
{
	/// <summary>
	/// Class that provides logging facilities for application
	/// </summary>
	public class CxLogger
	{
    //--------------------------------------------------------------------------
    static protected TextWriterTraceListener m_Listener = null; // Trace listener to write log
    //--------------------------------------------------------------------------
    /// <summary>
    /// Writes message to the log.
    /// </summary>
    /// <param name="message">message to write to the log.</param>
    static public void Write(string message)
    {
      Initialize();
      string s = new String('-', 80) + "\r\n" + 
                 "[" + CxDate.NowAsString() + "] " + message;
      m_Listener.WriteLine(s);
      m_Listener.Flush();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Writes message to the log and suppresses any exception thrown during this process.
    /// </summary>
    /// <param name="message">message to write to the log.</param>
    static public void SafeWrite(string message)
    {
      try
      {
        Write(message);
      }
      catch
      {
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Initializes logger.
    /// </summary>
    static protected void Initialize()
    {
      lock (typeof(CxLogger))
      {
        if (m_Listener == null) 
        {
          string logFileName = CxPath.GetUserSettingsFolder() + @"\Exceptions.log";
          m_Listener = new TextWriterTraceListener(logFileName);
        }
      }
    }
    //--------------------------------------------------------------------------
  }
}