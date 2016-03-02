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
  /// Delegate for methods that may be called to fix some problem.
  /// </summary>
  public delegate bool DxFixingDeedDelegate();

	/// <summary>
	/// "Planned" exception to raise in case of data validation, etc..
	/// </summary>
	public class ExPlannedException : ExException
	{
    //----------------------------------------------------------------------------
    protected DxFixingDeedDelegate m_FixingDeed; // Object that contain information on fixing method invocation
    protected string m_ButtonText = null; // Text for Details button
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message"></param>
    public ExPlannedException(string message) : base(message)
		{
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="innerException">exception that is the cause of the current exception</param>
    public ExPlannedException(string message, Exception innerException) : 
      base(message, innerException)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExPlannedException(string message, string debugMessage) : base(message, debugMessage)
    {
      DebugMessage = debugMessage;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Title for the error message dialog.
    /// </summary>
    public string Title
    {
      get { return GetTitle(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns title for the error handler dialog.
    /// </summary>
    /// <returns>title for the error handler dialog</returns>
    virtual protected string GetTitle()
    {
      return "Error";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does something when error dialog is going to be shown.
    /// </summary>
    virtual public void DoOnError()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Delegate to call to fix problem
    /// </summary>
    public DxFixingDeedDelegate FixingDeed
    {
      get { return m_FixingDeed; }
      set { m_FixingDeed = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Text for Details button.
    /// </summary>
    public string ButtonText
    {
      get { return m_ButtonText; }
      set { m_ButtonText = value; }
    }
    //----------------------------------------------------------------------------
  }
}
