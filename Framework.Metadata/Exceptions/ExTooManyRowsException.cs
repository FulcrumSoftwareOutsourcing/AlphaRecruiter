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

namespace Framework.Metadata
{
	/// <summary>
	/// Exception to raise when there are more than one entity with the given primary
	/// key values.
	/// </summary>
	public class ExTooManyRowsException : ExException
	{
    //--------------------------------------------------------------------------
    protected string m_Caption = ""; // Name of the table/view/entity where rows were found
    protected string[] m_PKNames = null; // List of primary key names
    protected object[] m_PKValues = null; // List of primary key values
    protected int m_Count = 0; // Number of rows with such primary key
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="caption">name of the table/view/entity where rows were found</param>
    /// <param name="pkNames">list of primary key names</param>
    /// <param name="pkValues">list of primary key values</param>
    /// <param name="count">number of rows with such primary key</param>
		public ExTooManyRowsException(string caption, 
                                  string[] pkNames, 
                                  object[] pkValues, 
                                  int count)
      : base(ComposeMessage(caption, pkNames, pkValues, count))
		{
      m_Caption = caption;
      m_PKNames = PKNames;
      m_PKValues = pkValues;
      m_Count = count;
		}
    //--------------------------------------------------------------------------
    /// <summary>
    /// Name of the table/view/entity where rows were found.
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
    ///  Number of rows with such primary key.
    /// </summary>
    public int Count
    {
      get { return m_Count; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Composes error message.
    /// </summary>
    /// <param name="caption">name of the table/view/entity where rows were not found</param>
    /// <param name="pkNames">list of primary key names</param>
    /// <param name="pkValues">list of primary key values</param>
    /// <param name="count">number of rows with such primary key</param>
    /// <returns>detailed error message</returns>
		static protected string ComposeMessage(string caption, 
                                           string[] pkNames, 
                                           object[] pkValues, 
                                           int count)
    {
      string s = string.Format("There are {0} rows in the {1} with such primary key values:",
                               count, caption);
      for (int i = 0; i < pkNames.Length; i++)
      {
        s += "\r\n" + pkNames[i] + "=" + CxUtils.GetObjectTypeAndValueText(pkValues[i]);
      }
      return s;
    }
    //--------------------------------------------------------------------------
	}
}
