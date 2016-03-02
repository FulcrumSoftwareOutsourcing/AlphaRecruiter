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
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace Framework.Utils
{
  /// <summary>
  /// Common framework utility methods.
  /// </summary>
  public class CxCommon
  {
    //-------------------------------------------------------------------------
    public const string REG_SOFTWARE = "Software"; // Local path for folder Software in registry.
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns full stack trace text of the given exception.
    /// </summary>
    /// <param name="e"></param>
    static public string GetExceptionFullStackTrace(Exception e)
    {
      string stackTrace = "";
      Exception stackException = e;
      while (stackException != null)
      {
        string debugMessage = "";
        if (stackException is ExException)
        {
          debugMessage = ((ExException) stackException).DebugMessage;
        }
        string currentStack = stackException.StackTrace;
        if (stackException is ExWebServiceException)
        {
          currentStack = ((ExWebServiceException) stackException).WebServiceStackTrace;
        }
        stackTrace +=
          "Message: " + stackException.Message +
          (CxUtils.NotEmpty(debugMessage) ? " " + debugMessage : "") + "\r\n" +
          "Type: " + stackException.GetType().FullName + "\r\n" +
          currentStack + "\r\n\r\n";
        stackException = stackException.InnerException;
      }
      return stackTrace;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Add new row to the data table
    /// </summary>
    /// <param name="table">table to add data row</param>
    /// <param name="value">value to populate all columns except several first ones</param>
    /// <param name="firstColumnValues">values for several first columns</param>
    static public void AddDataRow(DataTable table, object value, params object[] firstColumnValues)
    {
      DataRow dataRow = table.NewRow();
      for (int i = 0; i < table.Columns.Count; i++)
      {
        dataRow[i] = (i < firstColumnValues.Length ? firstColumnValues[i] : value);
      }
      table.Rows.Add(dataRow);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to the object of the given type.
    /// </summary>
    /// <param name="s">string to convert</param>
    /// <param name="type">type to convert to</param>
    /// <returns>object of the specified type converted from the given string</returns>
    static public object StringToObject(string s, Type type)
    {
      if (type == typeof(string))
        return s;
      else if (type == typeof(int))
        return CxInt.Parse(s, 0);
      else if (type == typeof(double))
        return Convert.ToDouble(CxFloat.ParseFloat(s, 0));
      else if (type == typeof(decimal))
        return CxFloat.ParseDecimal(s, 0);
      else if (type == typeof(bool))
        return CxBool.Parse(s, false);
      else if (type == typeof(DateTime))
        return DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss.fff", null);
      else if (type == typeof(byte[]))
        return Convert.FromBase64String(s);
      else
        throw new ExInternalException("Type " + type.Name + " could not be storable");

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts object to the string according to the object type.
    /// </summary>
    /// <param name="obj">string to convert</param>
    /// <returns>string that reptesents object</returns>
    static public string ObjectToString(object obj)
    {
      if (obj is string)
        return (string)obj;
      else if (obj is int)
        return obj.ToString();
      else if (obj is double)
        return ((double)obj).ToString(CxFloat.GetConstantFormatProvider());
      else if (obj is decimal)
        return ((decimal)obj).ToString(CxFloat.GetConstantFormatProvider());
      else if (obj is bool)
        return obj.ToString();
      else if (obj is DateTime)
        return ((DateTime) obj).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
      else if (obj is byte[])
        return Convert.ToBase64String((byte[])obj);
      else
        throw new ExInternalException("Type " + obj.GetType().Name + " could not be storable");

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to byte array. Is used by the ComputeMD5Hash method.
    /// Obsolete.
    /// </summary>
    /// <param name="data">string to convert</param>
    /// <returns>array of byte</returns>
    static protected byte[] StringToByteArray(string data)
    {
      byte[] result = new byte[data.Length];
      for (int i = 0; i < data.Length; i++)
      {
        result[i] = (byte)data[i];
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates hash value from the given string. 
    /// Obsolete, for backward compatibility only.
    /// </summary>
    /// <param name="data">string to get hash from</param>
    /// <returns>hash code</returns>
    static public string ComputeMD5Hash(string data)
    {
      return Convert.ToBase64String(
        new MD5CryptoServiceProvider().ComputeHash(StringToByteArray(data)));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates email and raises validation exception if email is invalid.
    /// </summary>
    /// <param name="email">email to validate</param>
    /// <param name="isMandatory">raise exception when email is empty</param>
    /// <param name="propertyName">property (field) name to pass to validation exception</param>
    /// <param name="propertyCaption">property (field) caption to include into exception message</param>
    /// <param name="invalidErrorMessage">error message for invalid email</param>
    /// <param name="emptyErrorMessage">error message for empty email</param>
    static public void ValidateEmail(
      string email,
      bool isMandatory,
      string propertyName,
      string propertyCaption,
      string invalidErrorMessage,
      string emptyErrorMessage)
    {
      string caption = CxUtils.NotEmpty(propertyCaption) ? propertyCaption : "Email";
      if (CxUtils.NotEmpty(email))
      {
        if (!CxEmail.IsValid(email))
        {
          throw new ExValidationException(String.Format(invalidErrorMessage, caption), propertyName);
        }
      }
      else if (isMandatory)
      {
        throw new ExValidationException(String.Format(emptyErrorMessage, caption), propertyName);
      }
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Validates email and raises validation exception if email is invalid.
    /// </summary>
    /// <param name="email">email to validate</param>
    /// <param name="isMandatory">raise exception when email is empty</param>
    /// <param name="propertyName">property (field) name to pass to validation exception</param>
    /// <param name="propertyCaption">property (field) caption to include into exception message</param>
    static public void ValidateEmail(
      string email,
      bool isMandatory,
      string propertyName,
      string propertyCaption)
    {
      ValidateEmail(
        email, isMandatory, propertyName, propertyCaption,
        "{0} is invalid.", "{0} could not be empty.");
    }
    //--------------------------------------------------------------------------  
  }
}