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
  public class CxEnum
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to enumeration element of the given type.
    /// </summary>
    /// <typeparam name="T">type of the enumeration</typeparam>
    /// <param name="s">string to convert</param>
    /// <param name="defValue">default value to return if convertion fails</param>
    /// <returns>converted value or default value</returns>
    static public T Parse<T>(string s, T defValue)
    {
      if (CxUtils.IsEmpty(s))
      {
        return defValue;
      }
      else
      {
        try
        {
          return (T) Enum.Parse(typeof(T), s, true);
        }
        catch
        {
          return defValue;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to enumeration element of the given type.
    /// </summary>
    /// <typeparam name="T">type of the enumeration</typeparam>
    /// <param name="s">string to convert</param>
    /// <param name="value">converted value</param>
    /// <returns>true if value was converted, false if string is invalid</returns>
    static public bool Parse<T>(string s, out T value)
    {
      value = default(T);
      if (CxUtils.IsEmpty(s))
      {
        return false;
      }
      else
      {
        try
        {
          value = (T) Enum.Parse(typeof(T), s, true);
          return true;
        }
        catch
        {
          return false;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to flags enumeration of the given type.
    /// String must contain the integer representation of the flags enumeration value.
    /// </summary>
    /// <typeparam name="T">type of the enumeration</typeparam>
    /// <param name="s">string to convert</param>
    /// <param name="defValue">default value to return if convertion fails</param>
    /// <returns>converted value or default value</returns>
    static public T ParseFlags<T>(string s, T defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        UInt64 intValue;
        if (UInt64.TryParse(s, out intValue))
        {
          try
          {
            return (T) (object) intValue;
          }
          catch
          {
            return defValue;
          }
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to flags enumeration of the given type.
    /// String must contain the integer representation of the flags enumeration value.
    /// </summary>
    /// <typeparam name="T">type of the enumeration</typeparam>
    /// <param name="s">string to convert</param>
    /// <param name="value">converted value</param>
    /// <returns>true if value was converted</returns>
    static public bool ParseFlags<T>(string s, out T value)
    {
      value = default(T);
      if (CxUtils.NotEmpty(s))
      {
        UInt64 intValue;
        if (UInt64.TryParse(s, out intValue))
        {
          try
          {
            value = (T) (object) intValue;
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
  }
}