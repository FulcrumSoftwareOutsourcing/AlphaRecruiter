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
	/// Summary description for CxInfoException.
	/// </summary>
	public class ExInfoException : ExPlannedException
	{
    //----------------------------------------------------------------------------
    protected string m_Title = ""; // Dialog title
    protected string m_Details = ""; // Information to show in the Details pane
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">information message</param>
    /// <param name="title">dialog title</param>
		public ExInfoException(string message, string title) : base(message)
		{
      m_Title = title;
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">information message</param>
    /// <param name="title">dialog title</param>
    /// <param name="details">information to show in the Details pane</param>
		public ExInfoException(string message, string title, string details) : base(message)
		{
      m_Title = title;
      m_Details = details;
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns title for the error handler dialog.
    /// </summary>
    /// <returns>title for the error handler dialog</returns>
    override protected string GetTitle()
    {
      return m_Title;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Information to show in the Details pane.
    /// </summary>
    public string Details
    {
      get { return m_Details; }
    }
    //----------------------------------------------------------------------------
	}
}
