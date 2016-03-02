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

using Framework.Utils;

namespace Framework.Entity
{
	/// <summary>
	/// Exception to raise when there were no entity with the given primary
	/// key values found.
	/// </summary>
	public class ExNoDataFoundException : ExException
	{
    //--------------------------------------------------------------------------
    protected string m_Caption = ""; // Name of the not found entity
    protected string[] m_PKNames = null; // List of primary key names
    protected object[] m_PKValues = null; // List of primary key values
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="caption">name of the not found entity</param>
    /// <param name="pkNames">list of primary key names</param>
    /// <param name="pkValues">list of primary key values</param>
		public ExNoDataFoundException(string caption, 
                                  string[] pkNames, 
                                  object[] pkValues)
      : base(ComposeMessage(caption, pkNames, pkValues))
		{
      m_Caption = caption;
      m_PKNames = PKNames;
      m_PKValues = pkValues;
		}
    //--------------------------------------------------------------------------
    /// <summary>
    /// Name of the not found entity.
    /// </summary>
    public string Caption
    {
      get { return m_Caption; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// List of primary key names.
    /// </summary>
    public string[] PKNames 
    {
      get { return m_PKNames; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// List of primary key values.
    /// </summary>
    public object[] PKValues 
    {
      get { return m_PKValues; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Composes error message.
    /// </summary>
    /// <param name="caption">name of the not found entity</param>
    /// <param name="pkNames">list of primary key names</param>
    /// <param name="pkValues">list of primary key values</param>
    /// <returns>detailed error message</returns>
		static protected string ComposeMessage(string caption, 
                                           string[] pkNames, 
                                           object[] pkValues)
    {
      string s = string.Format("{0} could not be found. Probably it was deleted.\r\n" + 
                               "Primary key values are:",
                               caption);
      for (int i = 0; i < pkNames.Length; i++)
      {
        s += "\r\n" + pkNames[i] + "=" + CxUtils.GetObjectTypeAndValueText(pkValues[i]);
      }
      return s;
    }
    //--------------------------------------------------------------------------
	}
}
