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
using System.Text;

namespace Framework.Db
{
  //--------------------------------------------------------------------------
  /// <summary>
  /// Represents a where clause fragment in the statement.
  /// </summary>
  public class CxWhereClause
  {
    //--------------------------------------------------------------------------
    private string m_Text;
    private List<CxDbParameterDescription> m_Parameters;
    //--------------------------------------------------------------------------

    #region Properties
    //--------------------------------------------------------------------------
    /// <summary>
    /// A text string of the where clause.
    /// </summary>
    public string Text
    {
      get { return m_Text; }
      set { m_Text = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// A list of parameters used in the where clause text.
    /// </summary>
    public List<CxDbParameterDescription> Parameters
    {
      get { return m_Parameters; }
      set { m_Parameters = value; }
    }
    //--------------------------------------------------------------------------
    #endregion

    #region Ctors
    //--------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxWhereClause()
    {
      Text = string.Empty;
      Parameters = new List<CxDbParameterDescription>();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="text">a text to initialize clause with</param>
    /// <param name="parameters">a list of parameters</param>
    public CxWhereClause(
      string text, CxDbParameterDescription[] parameters)
    {
      Text = text;
      Parameters = new List<CxDbParameterDescription>(parameters);
    }
    //--------------------------------------------------------------------------
    #endregion

    //--------------------------------------------------------------------------
    /// <summary>
    /// Joins the given where clauses into one where clause 
    /// using the given separator.
    /// </summary>
    /// <param name="separator">a separator for where clause texts</param>
    /// <param name="clauses">an array of where clauses to be joined</param>
    /// <returns>joined where clause</returns>
    static public CxWhereClause Join(string separator, CxWhereClause[] clauses)
    {
      CxWhereClause result = new CxWhereClause();
      StringBuilder sb = new StringBuilder();
      int nonEmptyI = 0;
      for (int i = 0; i < clauses.Length; i++)
      {
        if (!string.IsNullOrEmpty(clauses[i].Text))
        {
          if (nonEmptyI > 0)
            sb.Append(separator);
          sb.Append(clauses[i].Text);
          nonEmptyI++;
        }
        result.Parameters.AddRange(clauses[i].Parameters);
      }
      result.Text = sb.ToString();
      return result;
    }
    //--------------------------------------------------------------------------
  }
}
