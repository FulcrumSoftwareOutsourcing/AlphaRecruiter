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
using System.Collections.Generic;
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with Boolean type.
  /// </summary>
  public class CxBool
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or default value, if string is invalid.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from string</returns>
    static public bool? ParseEx(string s, bool? defValue)
    {
      if (s != null)
      {
        string upper = s.ToUpper();
        if (upper == "Y" || upper == "YES" ||
            upper == "TRUE" || upper == "T" ||
            upper == "1" || upper == "ON")
        {
          return true;
        }
        else if (upper == "N" || upper == "NO" ||
                 upper == "FALSE" || upper == "F" ||
                 upper == "0" || upper == "OFF")
        {
          return false;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or default value, if string is invalid, and default value is specified.
    /// Raises an exception if string is invalid and value is null.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from string</returns>
    static public bool Parse(string s, bool? defValue)
    {
      bool? result = ParseEx(s, defValue);
      if (result == null)
      {
        throw new ExBooleanConvertException(s);
      }
      return (bool) result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or default value, if string is invalid.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from string</returns>
    static public bool Parse(string s, bool defValue)
    {
      return Parse(s, (bool?) defValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or false, if string is invalid.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <returns>boolean value converted from string</returns>
    static public bool Parse(string s)
    {
      return Parse(s, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses object and returns corresponding boolean value, 
    /// or default value, if string is invalid, and default value is specified.
    /// Raises an exception if string is invalid and value is null;
    /// </summary>
    /// <param name="o">object to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from object</returns>
    static public bool? ParseEx(object o, bool? defValue)
    {
      if (o != null)
      {
        if (o is bool)
        {
          return (bool) o;
        }
        else if (o is bool?)
        {
          return (bool?) o;
        }
        else if (o is int)
        {
          return (int) o != 0 ? true : false;
        }
        else if (o is string)
        {
          return ParseEx((string) o, defValue);
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses object and returns corresponding boolean value, 
    /// or default value, if string is invalid, and default value is specified.
    /// Raises an exception if string is invalid and value is NxBoolEx.Undefined;
    /// </summary>
    /// <param name="o">object to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from object</returns>
    static public bool Parse(object o, bool? defValue)
    {
      bool? result = ParseEx(o, defValue);
      if (result == null)
      {
        throw new ExBooleanConvertException(o != null ? o.ToString() : "null");
      }
      return (bool) result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or default value, if string is invalid.
    /// </summary>
    /// <param name="o">object to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from object</returns>
    static public bool Parse(object o, bool defValue)
    {
      return Parse(o, (bool?) defValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or false, if string is invalid.
    /// </summary>
    /// <param name="o">object to parse</param>
    /// <returns>boolean value converted from object</returns>
    static public bool Parse(object o)
    {
      return Parse(o, false);
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Boolean convert exception.
  /// </summary>
  public class ExBooleanConvertException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="text">text failed to be converted</param>
    public ExBooleanConvertException(string text) :
      base(String.Format("Could not convert '{0}' to Boolean.", text))
    {
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}