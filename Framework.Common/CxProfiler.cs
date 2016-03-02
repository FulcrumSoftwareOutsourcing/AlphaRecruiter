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

namespace Framework.Utils
{
	/// <summary>
	/// Class to use for logging code block execution time
	/// </summary>
	public class CxProfiler : IDisposable
	{
    //--------------------------------------------------------------------------
    static protected int m_Level = 0; // Current level of nesting
    //--------------------------------------------------------------------------
    protected DateTime m_StartTime; // Moment when execution started
    protected string m_Name = ""; // Name of the code block
    protected bool m_LogMoments = false; // true if start and ent moment should be also logged
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">name of the code block</param>
    /// <param name="logMoments">true if start and ent moment should be also logged</param>
		public CxProfiler(string name, bool logMoments)
		{
      m_Name = name;
      m_StartTime = DateTime.Now;
      m_LogMoments = logMoments;
      if (m_LogMoments)
      {
        CxLogger.SafeWrite(GetIndent() + m_Name + " started at " + m_StartTime.ToString("HH:mm:ss.fff"));
      }
      m_Level++;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">name of the code block</param>
		public CxProfiler(string name) : this(name, false)
		{
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Logs execution time.
    /// </summary>
    public void Dispose()
    {
      m_Level--;
      DateTime endTime = DateTime.Now;
      if (m_LogMoments)
      {
        CxLogger.SafeWrite(GetIndent() + m_Name + " finished at " + endTime.ToString("HH:mm:ss.fff"));
      }
      CxLogger.SafeWrite(GetIndent() + m_Name + " took " + endTime.Subtract(m_StartTime));
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns some number of spaces to indent profiler output.
    /// </summary>
    /// <returns>some number of spaces to indent profiler output</returns>
    static protected string GetIndent()
    {
      return new string(' ', m_Level * 3);
    }
    //--------------------------------------------------------------------------
  }
}
