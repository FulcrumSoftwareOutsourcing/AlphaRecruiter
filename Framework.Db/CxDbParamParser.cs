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
using System.Collections.Specialized;
using System.Text;
using Framework.Utils;

namespace Framework.Db
{
	/// <summary>
	/// Utility class to parse and replace database parameters.
	/// </summary>
	public class CxDbParamParser
	{
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parses SQL and returns list of parameters from it (denoted as :PARAM_NAME).
    /// </summary>
    /// <param name="sql">SQL statement to parse</param>
    /// <param name="unique">true if list should not include duplicate parameter names</param>
    /// <returns>list of parameter names</returns>
    static public string[] GetList(string sql, bool unique)
    {
      int len = sql != null ? sql.Length : 0;
      StringBuilder sb = new StringBuilder(len);
      ArrayList list = new ArrayList();
      int state = 0;
      for (int i = 0; i < len; i++)
      {
        char c = sql[i];
        state = m_States[GetStateRow(c)][state];
        if (state == 9)
          sb.Append(c);
        else
          AddToList(list, sb, unique);
      }
      AddToList(list, sb, unique);
      string[] result = new string[list.Count];
      list.CopyTo(result);
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given SQL statement contains parameter placeholders.
    /// </summary>
    /// <param name="sql">SQL statement to check</param>
    static public bool HasParameters(string sql)
    {
      if (CxUtils.NotEmpty(sql) && sql.IndexOf(":") >= 0)
      {
        IList paramList = GetList(sql, true);
        return paramList != null && paramList.Count > 0;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given SQL statement contains parameter placeholders
    /// (except application parameters).
    /// </summary>
    /// <param name="sql">SQL statement to check</param>
    static public bool HasNonApplicationParameters(string sql)
    {
      if (CxUtils.NotEmpty(sql) && sql.IndexOf(":") >= 0)
      {
        IList paramList = GetList(sql, true);
        if (paramList != null)
        {
          foreach (string name in paramList)
          {
            if (name != null && !name.StartsWith("Application$", StringComparison.OrdinalIgnoreCase))
            {
              return true;
            }
          }
        }
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Replaces parameter placeholders with the given substitues.
    /// </summary>
    /// <param name="sql">SQL statement to replace paremeters</param>
    /// <param name="substitutes">paremeter name vs. substitute dictionary</param>
    /// <returns>SQL statement with replaced parameter placeholders</returns>
    static public string ReplaceParameters(string sql, NameValueCollection substitutes)
    {
      int len = sql.Length;
      StringBuilder sb = new StringBuilder(len);
      StringBuilder sb2 = new StringBuilder(len);
      int state = 0;
      for (int i = 0; i < len; i++)
      {
        char c = sql[i];
        sb2.Append(c);
        state = m_States[GetStateRow(c)][state];
        if (state == 9)
          sb.Append(c);
        else
          AddToStringBuffer(substitutes, sb, sb2);
      }
      AddToStringBuffer(substitutes, sb, sb2);
      return sb2.ToString();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds current string builder contents to the list (as string).
    /// </summary>
    /// <param name="list">list to add</param>
    /// <param name="sb">string builder to ad contents of</param>
    /// <param name="unique">true if list should not include duplicate parameter names</param>
    static protected void AddToList(ArrayList list, StringBuilder sb, bool unique)
    {
      if (sb.Length > 0)
      {
        string s = sb.ToString().ToUpper();
        if ( ! unique || ! list.Contains(s) ) list.Add(s);
        sb.Remove(0, sb.Length);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Replaces parameter name with substitute.
    /// </summary>
    /// <param name="substitutes">paremeter name vs. substitute dictionary</param>
    /// <param name="sb">string builder with parameter name</param>
    /// <param name="sb2">string builder to replace paremeter name with substitute in</param>
    static protected void AddToStringBuffer(NameValueCollection substitutes, 
                                            StringBuilder sb,
                                            StringBuilder sb2)
    {
      if (sb.Length == 0) return;

      string paramName = sb.ToString();
      string paramValue = substitutes[paramName.ToUpper()];
      int paramLen = paramName.Length + 1;
      string s = sb2.ToString();
      int paramStart = s.LastIndexOf(":" + paramName);
      sb2.Remove(paramStart, paramName.Length + 1);
      sb2.Insert(paramStart, paramValue);
      sb.Remove(0, sb.Length);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Makes step in finite automata.
    /// </summary>
    /// <param name="c">character to analyze</param>
    /// <returns>new automata state</returns>
    static protected int GetStateRow(char c)
    {
      switch(c)
      {
        case '/'  : return 0;
        case '*'  : return 1;
        case '-'  : return 2;
        case '\n' : return 3;
        case '\r' : return 3;
        case '\'' : return 4;
        case '"'  : return 5;
        case ':'  : return 6;
        default   : return (IsDbIdentifierChar(c) ? 7 : 8);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if character is a valid part of the database identifier.
    /// </summary>
    /// <param name="c">character to analyze</param>
    /// <returns>true if character is a valid part of the database identifier or false otherwise</returns>
    static protected bool IsDbIdentifierChar(char c)
    {
      return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
             (c == '$') || (c == '.') || (c == '_') || (c == '"') || (c == '#');
    }
    //----------------------------------------------------------------------------
    // Table of finite automata jumps
    static protected int[][] m_States = new int[][]
     { new int[] {1, 0, 2, 0, 1, 5, 6, 7, 1, 1},
       new int[] {0, 2, 3, 2, 0, 5, 6, 7, 0, 0},
       new int[] {4, 4, 2, 2, 5, 5, 6, 7, 4, 4},
       new int[] {0, 0, 2, 2, 0, 0, 6, 7, 0, 0},
       new int[] {6, 6, 2, 2, 6, 5, 0, 7, 6, 6},
       new int[] {7, 7, 2, 2, 7, 5, 6, 0, 7, 7},
       new int[] {8, 0, 2, 2, 0, 5, 6, 7, 0, 0},
       new int[] {0, 0, 2, 2, 0, 5, 6, 7, 9, 9},
       new int[] {0, 0, 2, 2, 0, 5, 6, 7, 0, 0}
     };
    //----------------------------------------------------------------------------
	}
}
