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

using System.Collections.Generic;

namespace Framework.Db
{
  //--------------------------------------------------------------------------
  /// <summary>
  /// Represents a "light" version of the database command that isn't 
  /// linked to a connection.
  /// </summary>
  public class CxQueryDescriptor
  {
    //--------------------------------------------------------------------------
    private string m_CommandText;
    private List<CxDbParameterDescription> m_Parameters;
    //--------------------------------------------------------------------------
    /// <summary>
    /// A text of the command.
    /// </summary>
    public string CommandText
    {
      get { return m_CommandText; }
      set { m_CommandText = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// An array of the command parameters.
    /// </summary>
    public List<CxDbParameterDescription> Parameters
    {
      get { return m_Parameters; }
      set { m_Parameters = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxQueryDescriptor()
    {
      Parameters = new List<CxDbParameterDescription>();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="commandText">a text of the command</param>
    /// <param name="parameters">a parameter array</param>
    public CxQueryDescriptor(string commandText, CxDbParameterDescription[] parameters)
      : this()
    {
      CommandText = commandText;
      Parameters.AddRange(parameters);
    }

    //--------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="commandText">a text of the command</param>
    /// <param name="parameters">a parameter enumeration</param>
    public CxQueryDescriptor(string commandText, IEnumerable<CxDbParameterDescription> parameters)
      : this()
    {
      CommandText = commandText;
      Parameters.AddRange(parameters);
    }
    //--------------------------------------------------------------------------
  }
}
