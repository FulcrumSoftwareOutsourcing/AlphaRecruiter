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
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Globalization;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class to contain general utility functions.
  /// </summary>
  public class CxUtils
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Logically compares two objects.
    /// Objects are equal if:
    /// - both are null or DBNull or empty string
    /// - both are the same type and equals
    /// - object x converted to the y type equals to y
    /// </summary>
    /// <param name="x">first object</param>
    /// <param name="y">second object</param>
    /// <returns>true if objects are equal or false otherwise</returns>
    public static bool Compare(object x, object y)
    {
      if (NotEmpty(x) && NotEmpty(y))
      {
        if (x.GetType() == y.GetType())
        {
          return x.Equals(y);
        }
        else
        {
          try
          {
            return (Convert.ChangeType(x, y.GetType()).Equals(y));
          }
          catch
          {
            return false;
          }
        }
      }
      else if (IsEmpty(x) && IsEmpty(y))
      {
        return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns color converted from the string in the following format:
    /// R,G,B where R, G and B are integer values.
    /// </summary>
    /// <param name="colorString">string representing RGB color</param>
    /// <returns>color or empty color if falied</returns>
    static public Color ColorFromRGBString(string colorString)
    {
      if (NotEmpty(colorString))
      {
        string[] strArray = colorString.Split(',');
        if (strArray.Length >= 3)
        {
          int[] intArray = new int[3];
          for (int i = 0; i < 3; i++)
          {
            intArray[i] = CxInt.Parse(strArray[i], -1);
            if (intArray[i] < 0)
            {
              return Color.Empty;
            }
          }
          return Color.FromArgb(intArray[0], intArray[1], intArray[2]);
        }
      }
      return Color.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares one object to another.
    /// Objects must be the same type.
    /// </summary>
    /// <param name="x">first object</param>
    /// <param name="y">second object</param>
    /// <returns>-1 if x less y, 0 if equals, 1 if x greater y</returns>
    static public int CompareWithComparer(object x, object y)
    {
      object a = x == DBNull.Value ? null : x;
      object b = y == DBNull.Value ? null : y;
      Comparer comparer = new Comparer(CultureInfo.InvariantCulture);
      return comparer.Compare(a, b);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given string is null or empty one or false otherwise.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>true if the given string is null or empty one or false otherwise</returns>
    static public bool IsEmpty(string s)
    {
      return string.IsNullOrEmpty(s);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given object is null or empty one or false otherwise.
    /// </summary>
    /// <param name="o">object to check</param>
    /// <returns>true if the given object is null or empty one or false otherwise</returns>
    static public bool IsEmpty(object o)
    {
      return IsNull(o) || IsEmpty(o.ToString());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns false if the given string is null or empty one or true otherwise.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>false if the given string is null or empty one or true otherwise.</returns>
    static public bool NotEmpty(string s)
    {
      return !string.IsNullOrEmpty(s);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns false if the given object is null or empty one or true otherwise.
    /// </summary>
    /// <param name="o">object to check</param>
    /// <returns>false if the given object is null or empty one or true otherwise.</returns>
    static public bool NotEmpty(object o)
    {
      return !IsEmpty(o);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns empty string ("") if string equals to null or the same string otherwise.
    /// </summary>
    /// <param name="s">string to check/return</param>
    /// <returns>empty string ("") if the given string equals to null of the given string otherwise</returns>
    static public string Nvl(string s)
    {
      return Nvl(s, "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value if string equals to null or empty or the same string otherwise.
    /// </summary>
    /// <param name="s">string to check/return</param>
    /// <param name="defaultValue">default value to use when string is null</param>
    /// <returns>default value if the given string equals to null of the given string otherwise</returns>
    static public string Nvl(string s, string defaultValue)
    {
      return string.IsNullOrEmpty(s) ? defaultValue : s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value if object equals to null or the same object otherwise.
    /// </summary>
    /// <param name="obj">object to check/return</param>
    /// <param name="defaultValue">default value to use when object is null</param>
    /// <returns>default value if the given object equals to null of the given object otherwise</returns>
    static public object Nvl(object obj, object defaultValue)
    {
      return Nvl<object>(obj, defaultValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value if object equals to null or the same object otherwise.
    /// </summary>
    /// <param name="obj">object to check/return</param>
    /// <param name="defaultValue">default value to use when object is null</param>
    /// <returns>default value if the given object equals to null of the given object otherwise</returns>
    static public T Nvl<T>(T obj, T defaultValue) where T : class
    {
      return IsNull(obj) ? defaultValue : obj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given object is DBNull or null.
    /// </summary>
    /// <param name="obj">object to check</param>
    /// <returns>true if given object is DBNull or null</returns>
    static public bool IsNull(object obj)
    {
      return (obj == null || Convert.IsDBNull(obj));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns original exception.
    /// </summary>
    /// <param name="e">exception that may be incapsulate an original one</param>
    /// <returns>original exception</returns>
    static public Exception GetOriginalException(Exception e)
    {
      if (e is TargetInvocationException)
        return GetOriginalException(e.InnerException);
      else
        return e;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the root exception.
    /// </summary>
    /// <param name="e">exception that may be incapsulate an original one</param>
    /// <returns>root exception</returns>
    static public Exception GetRootException(Exception e)
    {
      if (e.InnerException != null)
        return GetOriginalException(e.InnerException);
      else
        return e;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls Dispose method for the given object.
    /// (if object supports IDisposable interface)
    /// </summary>
    /// <param name="obj">object to dispose</param>
    static public void Dispose(object obj)
    {
      if (obj is IDisposable)
      {
        ((IDisposable) obj).Dispose();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls Dispose method for the given object (if object supports
    /// IDisposable interface) and sets object variable to null.
    /// </summary>
    /// <param name="obj">object to dispose</param>
    static public void Dispose(ref object obj)
    {
      Dispose(obj);
      obj = null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string representation of the given value along with the type.
    /// </summary>
    /// <param name="value">value to get string representation of</param>
    /// <returns>string representation of the given value along with the type</returns>
    static public string GetObjectTypeAndValueText(object value)
    {
      if (IsNull(value))
      {
        return "Null";
      }
      else
      {
        Type t = value.GetType();
        string s = (value is DateTime ? ((DateTime) value).ToString("yyyy/MM/dd HH:mm:ss.ffff", CultureInfo.InvariantCulture) :
             value is string   ? "\"" + (string) value + "\"" :
                                 value.ToString());
        return "(" + t.Name + ") <" + s + ">";
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Validates if value lies between min and max.
    /// </summary>
    /// <param name="value">value to check</param>
    /// <param name="min">left range bound</param>
    /// <param name="max">right range bound</param>
    /// <param name="caption">control caption</param>
    /// <returns>error message if value is not in range or emprt string if it is OK</returns>
    static public string ValidateRange(decimal value, decimal min, decimal max, string caption)
    {
      string s = (value >= min && value <= max ? "" : ComposeValueNotInRangeMessage(caption, min, max));
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates if value lies between min and max.
    /// </summary>
    /// <param name="value">value to check</param>
    /// <param name="min">left range bound</param>
    /// <param name="max">right range bound</param>
    /// <param name="caption">control caption</param>
    /// <returns>error message if value is not in range or emprt string if it is OK</returns>
    static public string ValidateRange(
      decimal value, 
      decimal min, 
      decimal max, 
      string caption,
      string betweenText,
      string notLessText,
      string notGreaterText)
    {
      string s = (value >= min && value <= max ? "" : 
        ComposeValueNotInRangeMessage(caption, min, max, betweenText, notLessText, notGreaterText));
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes message about value not in range.
    /// </summary>
    /// <param name="caption">control caption</param>
    /// <param name="min">minimal valid value</param>
    /// <param name="max">maximal valid value</param>
    /// <returns>message about value not in range</returns>
    static public string ComposeValueNotInRangeMessage(string caption, decimal min, decimal max)
    {
      return ComposeValueNotInRangeMessage(
        caption,
        min, 
        max,
        "{0} should be between {1} and {2}",
        "{0} should be not less than {1}",
        "{0} should be not greater than {2}");
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Composes message about value not in range.
    /// </summary>
    /// <param name="caption">control caption</param>
    /// <param name="min">minimal valid value</param>
    /// <param name="max">maximal valid value</param>
    /// <returns>message about value not in range</returns>
    static public string ComposeValueNotInRangeMessage(
      string caption, 
      decimal min, 
      decimal max,
      string betweenText,
      string notLessText,
      string notGreaterText)
    {
      string text = "";
      if (min > decimal.MinValue && max < decimal.MaxValue)
      {
        text = CxText.Format(betweenText, caption, min, max);
      }
      else if (min > decimal.MinValue)
      {
        text = CxText.Format(notLessText, caption, min, max);
      }
      else if (max > decimal.MinValue)
      {
        text = CxText.Format(notGreaterText, caption, min, max);
      }
      return text;
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Composes human readable description that contains SQL and parameter values.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <param name="parameters">statement parameters</param>
    /// <returns>human readable description that contains SQL and parameter values</returns>
    static public string ComposeSqlDescription(string sql, object[] parameters)
    {
      if (IsEmpty(sql)) return "";

      StringBuilder sb = new StringBuilder();
      sb.Append("SQL:\r\n");
      sb.Append(sql);
      if (parameters != null && parameters.Length > 0)
      {
        sb.Append("\r\nParameters:\r\n");
        for (int i = 0; i < parameters.Length; i++)
        {
          object p = parameters[i];
          sb.Append("[" + i + "]=");
          sb.Append(GetObjectTypeAndValueText(p) + "\r\n");
        }
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts object to string.
    /// </summary>
    /// <param name="o">object to convert</param>
    static public string ToString(object o)
    {
      return o != null ? o.ToString() : "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts object to string.
    /// </summary>
    /// <param name="o">object to convert</param>
    /// <param name="AddToTheBegin">will be added to the end if 'o' is not empty</param>
    /// <param name="AddToTheEnd">will be added to the end if 'o' is not empty</param>
    /// <returns></returns>
    static public string ToString(object o, string AddToTheBegin, string AddToTheEnd)
    {
      string res = ToString(o);
      return IsEmpty(res) ? res : (IsEmpty(AddToTheBegin) ? "" : AddToTheBegin) 
                                        + res 
                                          + (IsEmpty(AddToTheEnd) ? "" : AddToTheEnd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if strings are equal (or both strings are empty or null).
    /// Performs case-insensitive compare.
    /// </summary>
    static public bool Equals(string s1, string s2)
    {
      return CxText.Equals(s1, s2);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns HTTP content type by file extension.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    static public string GetContentTypeByFileExtension(string extension)
    {
      extension = Nvl(extension).ToUpper();
      if (extension.StartsWith("."))
      {
        extension = extension.Substring(1);
      }
      switch (extension)
      {
        case "GIF":
          return "image/gif";
        case "JPG":
          return "image/jpeg";
        case "PNG":
          return "image/png";
        case "CSV":
          return "application/vnd.ms-excel";
        case "XLS":
          return "application/vnd.ms-excel";
        default:
          return "text/html";
      }
    }
    //-------------------------------------------------------------------------
  }
}