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
using System.Globalization;
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with floating point numerics (float, decimal, etc.)
  /// </summary>
  public class CxFloat
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to decimal. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="s">string to parse as decimal</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public Decimal ParseDecimal_dotSeparator(string s, Decimal defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        Decimal result;
        if (Decimal.TryParse(s.Trim(), NumberStyles.Number, GetConstantFormatProvider(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to decimal. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="s">string to parse as decimal</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public Decimal ParseDecimal(string s, Decimal defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        Decimal result;
        if (Decimal.TryParse(s.Trim(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to decimal. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="o">object to parse as decimal</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public Decimal ParseDecimal(object o, Decimal defValue)
    {
      if (CxUtils.IsEmpty(o))
      {
        return defValue;
      }
      if (o is string)
      {
        return ParseDecimal((string)o, defValue);
      }
      try
      {
        return Convert.ToDecimal(o);
      }
      catch (Exception)
      {
        return defValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to float. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="s">string to parse as float</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public float ParseFloat(string s, float defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        float result;
        if (float.TryParse(s.Trim(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to float. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="o">object to parse as float</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public float ParseFloat(object o, float defValue)
    {
      if (CxUtils.IsEmpty(o))
      {
        return defValue;
      }
      if (o is string)
      {
        return ParseFloat((string)o, defValue);
      }
      try
      {
        return Convert.ToSingle(o);
      }
      catch (Exception)
      {
        return defValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to double. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="s">string to parse as double</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public double ParseDouble(string s, double defValue)
    {
      if (CxUtils.NotEmpty(s))
      {
        double result;
        if (double.TryParse(s.Trim(), out result))
        {
          return result;
        }
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts string to double. Returns defValue if conversion failed.
    /// </summary>
    /// <param name="o">object to parse as double</param>
    /// <param name="defValue">value to return if conversion failed</param>
    /// <returns>parse value or default value if conversion failed</returns>
    static public double ParseDouble(object o, double defValue)
    {
      if (CxUtils.IsEmpty(o))
      {
        return defValue;
      }
      if (o is string)
      {
        return ParseDouble((string)o, defValue);
      }
      try
      {
        return Convert.ToDouble(o);
      }
      catch (Exception)
      {
        return defValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Return format provider for internal float constant convertation.
    /// Provides '.' decimal separator and empty group separator.
    /// </summary>
    /// <returns>format provider</returns>
    static public IFormatProvider GetConstantFormatProvider()
    {
      IFormatProvider fp = (IFormatProvider)NumberFormatInfo.CurrentInfo.Clone();
      ((NumberFormatInfo)fp).NumberDecimalSeparator = ".";
      ((NumberFormatInfo)fp).NumberGroupSeparator = "";
      return fp;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses float constant with the '.' decimal separator and empty group separator.
    /// Raises exception if convertion falied.
    /// </summary>
    /// <param name="constant">constant string</param>
    /// <returns>converted float value</returns>
    static public float ParseFloatConst(string constant)
    {
      return float.Parse(constant, GetConstantFormatProvider());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses decimal constant with the '.' decimal separator and empty group separator.
    /// Raises exception if convertion falied.
    /// </summary>
    /// <param name="constant">constant string</param>
    /// <returns>converted float value</returns>
    static public Decimal ParseDecimalConst(string constant)
    {
      return Decimal.Parse(constant, GetConstantFormatProvider());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value converted to string with '.' decimal separator and empty group separator.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns>string representation of the value</returns>
    static public string ToConst(float value)
    {
      return value.ToString(GetConstantFormatProvider());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value converted to string with '.' decimal separator and empty group separator.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns>string representation of the value</returns>
    static public string ToConst(double value)
    {
      return value.ToString(GetConstantFormatProvider());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Rounds the given value to the provided number of decimal digits.
    /// Uses "normal" method (0.5 -> 1) instead of based on IEEE 754 standard.
    /// </summary>
    /// <param name="d">value to round</param>
    /// <param name="decimals">number of decinal digits to remain</param>
    /// <returns>the given value rounded to the given number of decimal digits</returns>
    static public decimal Round(decimal d, int decimals)
    {
      decimal mult = 1;
      for (int i = decimals; i < 0; i++)
      {
        mult /= 10m;
      }
      for (int i = 0; i < decimals; i++)
      {
        mult *= 10m;
      }
      decimal value = Math.Abs(d * mult);
      decimal floor = Convert.ToDecimal(Math.Floor(Convert.ToDouble(value)));
      decimal ceiling = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(value)));
      decimal result = (value - floor < ceiling - value ? floor : ceiling) / mult * Math.Sign(d);
      return result;
    }
    //-------------------------------------------------------------------------  
  }
}