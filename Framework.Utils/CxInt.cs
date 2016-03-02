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
  public class CxInt
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to integer. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="s">string to parse as integer</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public int Parse(string s, int defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        int result;
        if (int.TryParse(s.Trim(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts object to integer. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="o">object to parse as integer</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public int Parse(object o, int defValue)
    {
      if (CxUtils.NotEmpty(o))
      {
        if (o is string)
        {
          return Parse((string) o, defValue);
        }
        try
        {
          return Convert.ToInt32(o);
        }
        catch
        {
          return defValue;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts the given string to integer. Returns defValue if conversion failed.
    /// </summary>
    public static int? ParseEx(string str, int? defValue)
    {
      if (CxUtils.NotEmpty(str))
      {
        int result;
        if (int.TryParse(str.Trim(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
  }
}