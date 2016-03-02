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
using System.Reflection;
using System.Text;
using Framework.Metadata;

namespace Framework.Entity
{
  /// <summary>
  /// Scrambler class to perform scrambling for the scrambled_text web controls.
  /// </summary>
  static public class CxScrambler
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Performs scrambling.
    /// Invokes scrambling method specified for the attribute.
    /// </summary>
    /// <param name="attribute">attribute to scramble value for</param>
    /// <param name="text">text to scramble</param>
    /// <returns>scrambled text</returns>
    static public string Scramble(CxAttributeMetadata attribute, string text)
    {
      if (attribute == null)
      {
        return null;
      }
      if (!attribute.IsScrambled)
      {
        return text;
      }
      if (attribute.ScramblerClass == null || String.IsNullOrEmpty(attribute.ScramblerMethod))
      {
        return Scramble(text);
      }
      return Scramble(attribute.ScramblerClass.Class, attribute.ScramblerMethod, text);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Performs scrambling.
    /// Invokes scrambling method from the given class.
    /// </summary>
    /// <param name="scrambler">scrambler class</param>
    /// <param name="scrambleMethod">scrambler method</param>
    /// <param name="text">text to scramble</param>
    /// <returns>scrambled text</returns>
    static public string Scramble(Type scrambler, string scrambleMethod, string text)
    {
      if (scrambler == null || String.IsNullOrEmpty(scrambleMethod) || String.IsNullOrEmpty(text))
      {
        return text;
      }
      MethodInfo methodInfo = scrambler.GetMethod(
        scrambleMethod,
        BindingFlags.Static | BindingFlags.Public,
        null,
        new Type[] { typeof(string) },
        null);
      return (string) methodInfo.Invoke(null, new object[] { text });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default scrambling method.
    /// </summary>
    /// <param name="text">text to scramble</param>
    /// <returns>scrambled text</returns>
    static public string Scramble(string text)
    {
      if (!String.IsNullOrEmpty(text))
      {
        return new string('*', text.Length);
      }
      return String.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Credit card number scrambling method.
    /// </summary>
    /// <param name="text">credit card number</param>
    /// <returns>scrambled text</returns>
    static public string ScrambleCreditCardNumber(string text)
    {
      if (!String.IsNullOrEmpty(text))
      {
        if (text.Length > 4)
        {
          StringBuilder sb = new StringBuilder();
          sb.Append(new string('*', text.Length - 4));
          sb.Append(text.Substring(text.Length - 4));
          return sb.ToString();
        }
        else
        {
          return text;
        }
      }
      return String.Empty;
    }
    //-------------------------------------------------------------------------
  }
}