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
  //---------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for three-state boolean values (including 'Undefined' one)
  /// </summary>
  public enum NxBoolEx { Undefined, True, False }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Utility methods to work with three-state boolean.
  /// </summary>
  public class CxBoolEx
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns:
    /// true  if value == NxBoolEx.True,
    /// false if value == NxBoolEx.False,
    /// initialValue otherwise.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="initialValue">value to return if first argument is undefined</param>
    /// <returns>NxBoolEx value that corresponds with an argument</returns>
    static public bool GetBool(NxBoolEx value, bool initialValue)
    {
      if (value != NxBoolEx.Undefined)
      {
        return value == NxBoolEx.True;
      }
      else
      {
        return initialValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns:
    /// true  if value == NxBoolEx.True,
    /// false if value == NxBoolEx.False or NxBoolEx.Undefined
    /// initialValue otherwise.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns>NxBoolEx value that corresponds with an argument</returns>
    /// <returns></returns>
    static public bool GetBool(NxBoolEx value)
    {
      return GetBool(value, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts bool to NxBoolEx.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns>NxBoolEx value that corresponds with an argument</returns>
    static public NxBoolEx GetBoolEx(bool value)
    {
      return value ? NxBoolEx.True : NxBoolEx.False;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses string and returns corresponding boolean value, 
    /// or default value, if string is invalid.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <param name="defValue">value to return if parsing failed</param>
    /// <returns>boolean value converted from string</returns>
    static public NxBoolEx Parse(string s, NxBoolEx defValue)
    {
      bool? defaultValue = null;
      switch (defValue)
      {
        case NxBoolEx.True: defaultValue = true; break;
        case NxBoolEx.False: defaultValue = false; break;
      }
      bool? value = CxBool.ParseEx(s, defaultValue);
      if (value == true)
      {
        return NxBoolEx.True;
      }
      else if (value == false)
      {
        return NxBoolEx.False;
      }
      else
      {
        return defValue;
      }
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}