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

namespace Framework.Entity
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Log error level enumeration.
  /// </summary>
  public enum NxLogLevel
  {
    Debug = 1,
    Info  = 2,
    Warn  = 3,
    Error = 4,
    Fatal = 5
  }
  //---------------------------------------------------------------------------
  /// <summary>
	/// Logging interface.
	/// </summary>
  public interface IxLog
  {
    /// <summary>
    /// Writes record to log.
    /// </summary>
    /// <param name="level">log error level</param>
    /// <param name="message">message</param>
    /// <param name="description">description</param>
    /// <param name="parameters">additional parameters</param>
    void LogWrite(
      NxLogLevel level,
      string message,
      string description,
      NameValueCollection parameters);
    //---------------------------------------------------------------------------
    /// <summary>
    /// Writes exception info to log.
    /// </summary>
    /// <param name="e">exception to write info</param>
    void LogException(Exception e);
    //---------------------------------------------------------------------------
  }
}