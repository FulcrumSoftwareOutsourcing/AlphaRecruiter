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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with strings.
  /// </summary>
  public class CxText
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given string is null or empty one or false otherwise.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>true if the given string is null or empty one or false otherwise</returns>
    static public bool IsEmpty(string s)
    {
      return CxUtils.IsEmpty(s);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given string is null or empty one or false otherwise.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>true if the given string is null or empty one or false otherwise</returns>
    static public bool NotEmpty(string s)
    {
      return CxUtils.NotEmpty(s);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reverses characters in the given string.
    /// </summary>
    /// <param name="s">source string</param>
    /// <returns>reversed string</returns>
    static public string Reverse(string s)
    {
      if (s != null)
      {
        StringBuilder sb = new StringBuilder();
        for (int i = s.Length - 1; i >= 0; i--)
        {
          sb.Append(s[i]);
        }
        return sb.ToString();
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces characters from the charsToReplace with characters from the same position in 
    /// replacementChars.
    /// </summary>
    /// <param name="s">string to process</param>
    /// <param name="charsToReplace">characters to replace</param>
    /// <param name="replacementChars">replacement characters</param>
    /// <returns>string with repaced characters</returns>
    static public string ReplaceChars(string s, string charsToReplace, string replacementChars)
    {
      string result = s;
      for (int i = 0; i < charsToReplace.Length; i++)
      {
        string s1 = new string(charsToReplace[i], 1);
        string s2 = (i < replacementChars.Length - 1 ? new string(replacementChars[i], 1) : "");
        result = result.Replace(s1, s2);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces characters from the charsToReplace with characters from the same position in 
    /// replacementChars.
    /// </summary>
    /// <param name="s">string to process</param>
    /// <param name="charsToReplace">characters to replace</param>
    /// <param name="replacementChars">replacement characters</param>
    /// <returns>string with repaced characters</returns>
    static public string ReplaceCharsCI(string s, string charsToReplace, string replacementChars)
    {
      string result = s;
      for (int i = 0; i < charsToReplace.Length; i++)
      {
        string s1 = new string(charsToReplace[i], 1);
        string s2 = (i < replacementChars.Length - 1 ? new string(replacementChars[i], 1) : "");
        result = result.Replace(s1.ToLower(), s2);
        result = result.Replace(s1.ToUpper(), s2);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces characters from the charsToReplace with replacementChar.
    /// </summary>
    /// <param name="s">string to process</param>
    /// <param name="charsToReplace">characters to replace</param>
    /// <param name="replacementChar">replacement character</param>
    /// <returns>string with repaced characters</returns>
    static public string ReplaceChars(string s, string charsToReplace, char replacementChar)
    {
      string result = s;
      for (int i = 0; i < charsToReplace.Length; i++)
      {
        result = result.Replace(charsToReplace[i], replacementChar);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces characters from the charsToReplace with replacementChar.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to process</param>
    /// <param name="charsToReplace">characters to replace</param>
    /// <param name="replacementChar">replacement character</param>
    /// <returns>string with repaced characters</returns>
    static public string ReplaceCharsCI(string s, string charsToReplace, char replacementChar)
    {
      string result = s;
      for (int i = 0; i < charsToReplace.Length; i++)
      {
        result = result.Replace(Char.ToLower(charsToReplace[i]), replacementChar);
        result = result.Replace(Char.ToUpper(charsToReplace[i]), replacementChar);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces in given string all substrings from replaceWhat array with 
    /// corresponding strings from replaceWith array.
    /// </summary>
    /// <param name="s">string to process</param>
    /// <param name="replaceWhat">list of substrings to replace</param>
    /// <param name="replaceWith">list of substrings to replace with</param>
    /// <returns>string after replacement</returns>
    static public string Replace(
      string s,
      string[] replaceWhat,
      string[] replaceWith)
    {
      string result = s;
      for (int i = 0; i < replaceWhat.Length && i < replaceWith.Length; i++)
      {
        result = result.Replace(replaceWhat[i], replaceWith[i]);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds prefix to given string if string is not empty
    /// </summary>
    /// <param name="s">string to add prefix to</param>
    /// <param name="prefix">prefix to add</param>
    /// <returns>string with added prefix</returns>
    static public string AddPrefix(string s, string prefix)
    {
      return IsEmpty(s) ? s : String.Concat(prefix, s);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds suffix to given string if string is not empty
    /// </summary>
    /// <param name="s">string to add suffix to</param>
    /// <param name="suffix">suffix to add</param>
    /// <returns>string with added suffix</returns>
    static public string AddSuffix(string s, string suffix)
    {
      return IsEmpty(s) ? s : String.Concat(s, suffix);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds prefix and suffix to given string if string is not empty
    /// </summary>
    /// <param name="s">string to add prefix and suffix to</param>
    /// <param name="prefix">prefix to add before string</param>
    /// <param name="suffix">suffix to add after string</param>
    /// <returns>string with prefix before and suffix after</returns>
    static public string AddPrefixAndSuffix(string s, string prefix, string suffix)
    {
      return AddSuffix(AddPrefix(s, prefix), suffix);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string after given prefix.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without prefix</returns>
    static public string ExtractStringAfterPrefix(
      string s, 
      string prefix,
      StringComparison comparisonType)
    {
      string result = "";
      if (NotEmpty(s) && NotEmpty(prefix))
      {
        int index = s.IndexOf(prefix, comparisonType);
        if (index >= 0)
        {
          result = s.Substring(index + prefix.Length);
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string after given prefix.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <returns>string without prefix</returns>
    static public string ExtractStringAfterPrefix(string s, string prefix)
    {
      return ExtractStringAfterPrefix(s, prefix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string after given prefix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <returns>string without prefix</returns>
    static public string ExtractStringAfterPrefixCI(string s, string prefix)
    {
      return ExtractStringAfterPrefix(s, prefix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string before given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to extract</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without suffix</returns>
    static public string ExtractStringBeforeSuffix(
      string s, 
      string suffix,
      StringComparison comparisonType)
    {
      string result = "";
      if (NotEmpty(s) && NotEmpty(suffix))
      {
        int index = s.IndexOf(suffix, comparisonType);
        if (index >= 0)
        {
          result = s.Substring(0, index);
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string before given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to extract</param>
    /// <returns>string without suffix</returns>
    static public string ExtractStringBeforeSuffix(string s, string suffix)
    {
      return ExtractStringBeforeSuffix(s, suffix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string before given suffix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to extract</param>
    /// <returns>string without suffix</returns>
    static public string ExtractStringBeforeSuffixCI(string s, string suffix)
    {
      return ExtractStringBeforeSuffix(s, suffix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string enclosed between given prefix and suffix
    /// </summary>
    /// <param name="s">string to check for prefix and suffix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <param name="suffix">suffix to extract</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without prefix and suffix</returns>
    static public string ExtractString(
      string s, 
      string prefix, 
      string suffix,
      StringComparison comparisonType)
    {
      return ExtractStringBeforeSuffix(
        ExtractStringAfterPrefix(s, prefix, comparisonType), suffix, comparisonType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string enclosed between given prefix and suffix
    /// </summary>
    /// <param name="s">string to check for prefix and suffix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <param name="suffix">suffix to extract</param>
    /// <returns>string without prefix and suffix</returns>
    static public string ExtractString(string s, string prefix, string suffix)
    {
      return ExtractString(s, prefix, suffix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string enclosed between given prefix and suffix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check for prefix and suffix</param>
    /// <param name="prefix">prefix to extract</param>
    /// <param name="suffix">suffix to extract</param>
    /// <returns>string without prefix and suffix</returns>
    static public string ExtractStringCI(string s, string prefix, string suffix)
    {
      return ExtractString(s, prefix, suffix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string starts with the given prefix.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="prefix">prefix to check</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>true if string starts with the given prefix</returns>
    static public bool StartsWith(string s, string prefix, StringComparison comparisonType)
    {
      if (NotEmpty(s) && NotEmpty(prefix))
      {
        return s.StartsWith(prefix, comparisonType);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string starts with the given prefix.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="prefix">prefix to check</param>
    /// <returns>true if string starts with the given prefix</returns>
    static public bool StartsWith(string s, string prefix)
    {
      return StartsWith(s, prefix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string starts with the given prefix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="prefix">prefix to check</param>
    /// <returns>true if string starts with the given prefix</returns>
    static public bool StartsWithCI(string s, string prefix)
    {
      return StartsWith(s, prefix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string ends with the given suffix.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="suffix">suffix to check</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>true if string ends with the given suffix</returns>
    static public bool EndsWith(string s, string suffix, StringComparison comparisonType)
    {
      if (NotEmpty(s) && NotEmpty(suffix))
      {
        return s.EndsWith(suffix, comparisonType);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string ends with the given suffix.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="suffix">suffix to check</param>
    /// <returns>true if string ends with the given suffix</returns>
    static public bool EndsWith(string s, string suffix)
    {
      return EndsWith(s, suffix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string ends with the given suffix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="suffix">suffix to check</param>
    /// <returns>true if string ends with the given suffix</returns>
    static public bool EndsWithCI(string s, string suffix)
    {
      return EndsWith(s, suffix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix from given string, if given string starts with given prefix.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without prefix</returns>
    static public string TrimPrefix(string s, string prefix, StringComparison comparisonType)
    {
      if (StartsWith(s, prefix, comparisonType))
      {
        return s.Substring(prefix.Length);
      }
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix from given string, if given string starts with given prefix.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <returns>string without prefix</returns>
    static public string TrimPrefix(string s, string prefix)
    {
      return TrimPrefix(s, prefix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix from given string, if given string starts with given prefix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check for prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <returns>string without prefix</returns>
    static public string TrimPrefixCI(string s, string prefix)
    {
      return TrimPrefix(s, prefix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes suffix from given string, if given string ends with given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to trim</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without suffix</returns>
    static public string TrimSuffix(string s, string suffix, StringComparison comparisonType)
    {
      if (EndsWith(s, suffix, comparisonType))
      {
        return s.Substring(0, s.Length - suffix.Length);
      }
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes suffix from given string, if given string ends with given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to trim</param>
    /// <returns>string without suffix</returns>
    static public string TrimSuffix(string s, string suffix)
    {
      return TrimSuffix(s, suffix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes suffix from given string, if given string ends with given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix</param>
    /// <param name="suffix">suffix to trim</param>
    /// <returns>string without suffix</returns>
    static public string TrimSuffixCI(string s, string suffix)
    {
      return TrimSuffix(s, suffix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix and suffix from given string, 
    /// if given string starts with given prefix and ends with given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix and prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <param name="suffix">suffix to trim</param>
    /// <param name="comparisonType">comparison type</param>
    /// <returns>string without prefix and suffix</returns>
    static public string TrimPrefixAndSuffix(
      string s, 
      string prefix, 
      string suffix, 
      StringComparison comparisonType)
    {
      string result = s;
      if (StartsWith(s, prefix, comparisonType) && EndsWith(s, suffix, comparisonType))
      {
        result = s.Substring(prefix.Length, s.Length - prefix.Length - suffix.Length);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix and suffix from given string, 
    /// if given string starts with given prefix and ends with given suffix.
    /// </summary>
    /// <param name="s">string to check for suffix and prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <param name="suffix">suffix to trim</param>
    /// <returns>string without prefix and suffix</returns>
    static public string TrimPrefixAndSuffix(string s, string prefix, string suffix)
    {
      return TrimPrefixAndSuffix(s, prefix, suffix, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes prefix and suffix from given string, 
    /// if given string starts with given prefix and ends with given suffix.
    /// Case-insensitive.
    /// </summary>
    /// <param name="s">string to check for suffix and prefix</param>
    /// <param name="prefix">prefix to trim</param>
    /// <param name="suffix">suffix to trim</param>
    /// <returns>string without prefix and suffix</returns>
    static public string TrimPrefixAndSuffixCI(string s, string prefix, string suffix)
    {
      return TrimPrefixAndSuffix(s, prefix, suffix, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates given string with the specified regular expression
    /// </summary>
    /// <param name="value">string to validate</param>
    /// <param name="pattern">regex patter to validate with</param>
    /// <returns>true if string conforms to the given pattern</returns>
    public static bool RegexValidate(string value, string pattern)
    {
      Match m = Regex.Match(value, pattern);
      return (m.Success && m.Index == 0 && m.Length == value.Length);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates given string with the specified regular expression
    /// </summary>
    /// <param name="value">string to validate</param>
    /// <param name="pattern">regex patter to validate with</param>
    /// <param name="options">regex options</param>
    /// <returns>true if string conforms to the given pattern</returns>
    public static bool RegexValidate(string value, string pattern, RegexOptions options)
    {
      Match m = Regex.Match(value, pattern, options);
      return (m.Success && m.Index == 0 && m.Length == value.Length);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Trims spaces, CRs, LFs and tabulations from the string.
    /// </summary>
    /// <param name="s">string to trim</param>
    /// <returns>trimmed string</returns>
    static public string TrimSpace(string s)
    {
      return s != null ? s.Trim(new char[] { ' ', '\n', '\r', '\t' }) : s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces LFs with CR/LF pairs.
    /// </summary>
    /// <param name="s">string to normalize</param>
    /// <returns>source string with CRs replaced with CRs with CR/LF</returns>
    static public string NormalizeCrLf(string s)
    {
      int len = s.Length;
      StringBuilder sb = new StringBuilder(len);
      for (int i = 0; i < len; i++)
      {
        char c = s[i];
        if ((i == 0 || s[i - 1] != '\r') && c == '\n')
          sb.Append("\r\n");
        else
          sb.Append(c);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Pads string to the specified length, trims string to the specified length,
    /// if string is longer.
    /// </summary>
    /// <param name="s">string to pad</param>
    /// <param name="padChar">character to add</param>
    /// <param name="len">length of resulting string</param>
    /// <returns>padded (or trimmed) string</returns>
    static public string FixedPadLeft(string s, char padChar, int len)
    {
      if (s.Length > len)
      {
        return s.Substring(s.Length - len);
      }
      else
      {
        return s.PadLeft(len, padChar);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Pads string to the specified length, trims string to the specified length,
    /// if string is longer.
    /// </summary>
    /// <param name="s">string to pad</param>
    /// <param name="padChar">character to add</param>
    /// <param name="len">length of resulting string</param>
    /// <returns>padded (or trimmed) string</returns>
    static public string FixedPadRight(string s, char padChar, int len)
    {
      if (s.Length > len)
      {
        return s.Substring(0, len);
      }
      else
      {
        return s.PadRight(len, padChar);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given string contains at least one substring from substr array.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <param name="substrList">list of substrings to find</param>
    /// <returns>true if given string contains at least one substring from substr array</returns>
    static public bool Contains(string s, IEnumerable substrList)
    {
      if (NotEmpty(s) && substrList != null)
      {
        foreach (string substr in substrList)
        {
          if (CxUtils.NotEmpty(substr) && s.IndexOf(substr) >= 0)
          {
            return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes string into array of parts divided with separator.
    /// </summary>
    /// <param name="str">string to decompose</param>
    /// <param name="separator">string that separates parts</param>
    /// <param name="textQualifier">character to enclose strings containing separator itself</param>
    /// <returns>list with separated parts</returns>
    static public List<string> DecomposeWithSeparator(
      string str,
      string separator,
      char textQualifier)
    {
      List<string> list = new List<string>();
      String s = CxUtils.Nvl(str).Trim();
      if (IsEmpty(s)) return list;

      int strLen = s.Length;
      int sepLen = separator.Length;
      int prevSepIndex = -sepLen;
      while (prevSepIndex < strLen - 1)
      {
        int pos = prevSepIndex + sepLen;
        while (pos < strLen && Char.IsWhiteSpace(s[pos])) pos++; // Eat spaces
        if (pos >= strLen) break;
        if (textQualifier != '\x0' && s[pos] == textQualifier)
        {
          int tokenEnd;
          String token = ExtractQuotedString(s.Substring(pos), textQualifier, out tokenEnd);
          list.Add(token);
          int tokenEnd2 = tokenEnd + 1 + pos;
          prevSepIndex = (tokenEnd2 == strLen ? strLen :
            tokenEnd2 < s.Length &&
            s.IndexOf(separator, tokenEnd2) == tokenEnd2 ? tokenEnd2 :
            tokenEnd2 - sepLen);
        }
        else
        {
          int sepIndex = s.IndexOf(separator, pos);
          if (sepIndex == -1)
          {
            list.Add(s.Substring(prevSepIndex + sepLen).Trim());
            break;
          }
          else
          {
            list.Add(s.Substring(prevSepIndex + sepLen, sepIndex - prevSepIndex - sepLen).Trim());
            prevSepIndex = sepIndex;
          }
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes string into array of parts divided with separator.
    /// </summary>
    /// <param name="str">string to decompose</param>
    /// <param name="separator">string that separates parts</param>
    /// <returns>list with separated parts</returns>
    static public List<string> DecomposeWithSeparator(string str, string separator)
    {
      return DecomposeWithSeparator(str, separator, '\"');
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes string into a list of sub-strings divided by the separators
    /// such as white space characters or the comma character.
    /// </summary>
    /// <param name="str">the string to be decomposed</param>
    /// <returns>a list of sub-strings</returns>
    static public IList<string> DecomposeWithWhiteSpaceAndComma(string str)
    {
      return DecomposeWithSeparators(str, new char[] { ' ', '\t', '\r', '\n', ','});
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes the given string into a list of sub-strings divided by the given
    /// separators.
    /// </summary>
    /// <param name="str">the string to be decomposed</param>
    /// <param name="separators">the list of separators to decompose by</param>
    /// <returns>a list of sub-strings</returns>
    static public IList<string> DecomposeWithSeparators(string str, char[] separators)
    {
      List<string> resultList = new List<string>();
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.Length; i++)
      {
        char currentChar = str[i];
        if (((IList) separators).Contains(currentChar))
        {
          var substr = sb.ToString();
          if (!string.IsNullOrEmpty(substr))
            resultList.Add(substr);
          sb = new StringBuilder();
        }
        else
        {
          sb.Append(currentChar);
        }
      }
      
      {
        var substr = sb.ToString();
        if (!string.IsNullOrEmpty(substr))
          resultList.Add(substr);
      }
      return resultList.AsReadOnly();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns comma-separated list of the strings.
    /// </summary>
    /// <param name="list">string list</param>
    /// <returns>comma-separated text</returns>
    static public string ComposeCommaSeparatedString(IList<string> list)
    {
      return ComposeWithSeparator(list, ",");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes string array into string divided with separator.
    /// If a string contains separator, quotes the string.
    /// </summary>
    /// <param name="list">list with strings to compose</param>
    /// <param name="separator">string that separates parts</param>
    /// <returns>composed string</returns>
    static public string ComposeWithSeparator(IList<string> list, String separator)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < list.Count; i++)
      {
        string s = CxUtils.ToString(list[i]);
        if (s.IndexOfAny((separator + "\"").ToCharArray()) != -1) s = GetQuotedString(s, "\"\"");
        sb.Append(s);
        if (i != list.Count - 1) sb.Append(separator);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes empty or null strings from the given string list.
    /// </summary>
    /// <param name="lines">given strings collection</param>
    /// <returns>cleaned up result</returns>
    static public IList<string> RemoveEmptyStrings(IList<string> lines)
    {
      IList<string> result = new List<string>();
      foreach (string line in lines)
      {
        if (!string.IsNullOrEmpty(line))
          result.Add(line);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Concatenates parts into one string, parts are divided by separator.
    /// Ignores empty strings.
    /// </summary>
    /// <param name="separator">separator to divide strings</param>
    /// <param name="parts">parts to concatenate</param>
    /// <returns>concatenated string or null</returns>
    static public string Join(string separator, params string[] parts)
    {
      string result = "";
      foreach (string part in parts)
      {
        if (NotEmpty(part))
        {
          result += (result != "" ? separator : "") + part;
        }
      }
      return result != "" ? result : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes string into array of parts divided with separator.
    /// Concatenates decomposed parts into one string, decomposed parts are divided by separator.
    /// Ignores empty strings.
    /// </summary>
    /// <param name="separator">separator to divide strings</param>
    /// <param name="parts">parts to concatenate</param>
    /// <returns>concatenated string or null</returns>
    static public string Join2(string separator, params string[] parts)
    {
      List<string> listStr = new List<string>();
      foreach (string part in parts)
      {
        listStr.AddRange(DecomposeWithSeparator(part, separator));
      }
      return Join(separator, listStr.ToArray());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encloses string into quotes and replaces internal quotes with given string.
    /// </summary>
    /// <param name="s">string to enclose in quotes</param>
    /// <param name="quoteChar">char representing a quote</param>
    /// <param name="quoteReplacer">string to replace quotes</param>
    /// <returns>string enclosed into quotes</returns>
    static public string GetQuotedString(string s, char quoteChar, string quoteReplacer)
    {
      return GetQuotedString(s, quoteChar, quoteReplacer, NxGetQuotedStringMode.None);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encloses string into quotes and replaces internal quotes with given string.
    /// </summary>
    /// <param name="s">string to enclose in quotes</param>
    /// <param name="quoteChar">char representing a quote</param>
    /// <param name="quoteReplacer">string to replace quotes</param>
    /// <returns>string enclosed into quotes</returns>
    static public string GetQuotedString(
      string s, char quoteChar, string quoteReplacer, NxGetQuotedStringMode mode)
    {
      bool doLeaveWhiteSpacesAsIs = 
        (mode & NxGetQuotedStringMode.LeaveWhiteSpaceChars) == NxGetQuotedStringMode.LeaveWhiteSpaceChars;
      int len = CxUtils.Nvl(s).Length;
      StringBuilder sb = new StringBuilder(len * 2);
      sb.Append(quoteChar);
      for (int i = 0; i < len; i++)
      {
        char c = s[i];
        if (char.IsWhiteSpace(c))
        {
          if (doLeaveWhiteSpacesAsIs)
            sb.Append(c);
          else
            sb.Append(' ');
        }
        else if (c < 32)
        {
          // Do nothing
        }
        else if (c == quoteChar)
          sb.Append(quoteReplacer);
        else
          sb.Append(c);
      }
      sb.Append(quoteChar);
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encloses string into quotes and replaces internal quotes with given string.
    /// </summary>
    /// <param name="s">string to enclose in quotes</param>
    /// <param name="quoteReplacer">string to replace quotes</param>
    /// <returns>string enclosed into quotes</returns>
    static public string GetQuotedString(string s, string quoteReplacer)
    {
      return GetQuotedString(s, '\"', quoteReplacer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encloses string into double quotes.
    /// </summary>
    static public string GetQuotedString(string s)
    {
      return GetQuotedString(s, '\"', "\"\"");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encloses string into single quotes.
    /// </summary>
    static public string GetSingleQuotedString(string s)
    {
      return GetQuotedString(s, '\'', "''");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes quotes from the quoted string.
    /// </summary>
    /// <param name="s">string to extract quotes</param>
    /// <param name="textQualifier">quote character</param>
    /// <param name="tokenEnd">output parameter with index of char where token ends</param>
    /// <returns>string with removed quotes</returns>
    static protected string ExtractQuotedString(
      string s,
      char textQualifier,
      out int tokenEnd)
    {
      tokenEnd = 0;
      if (IsEmpty(s) || s[0] != textQualifier) return s;

      int len = s.Length;
      tokenEnd = len;
      StringBuilder sb = new StringBuilder(len);
      int curPos = 1;
      int nextQuotePos = s.IndexOf(textQualifier, curPos);
      while (nextQuotePos != -1)
      {
        sb.Append(s.Substring(curPos, nextQuotePos - curPos));
        curPos = nextQuotePos;
        if (nextQuotePos + 1 == len || s[nextQuotePos + 1] != textQualifier)
        {
          tokenEnd = nextQuotePos;
          break;
        }
        else
        {
          sb.Append(textQualifier);
          curPos += 2;
        }
        nextQuotePos = s.IndexOf(textQualifier, curPos);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes quotes from the quoted string.
    /// </summary>
    /// <param name="s">string to extract quotes</param>
    /// <param name="tokenEnd">output parameter with index of char where token ends</param>
    /// <returns>string with removed quotes</returns>
    static protected string ExtractQuotedString(string s, out int tokenEnd)
    {
      return ExtractQuotedString(s, '\"', out tokenEnd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Extracts quoted string. Removes 'quoteStr' from the beginning and 
    /// from the end of the string, replaces double 'quoteStr' occurrencies 
    /// inside the string with the single 'quoteStr'.
    /// </summary>
    /// <param name="s">quoted string</param>
    /// <param name="quoteStr">quote character (or string)</param>
    /// <returns>extracted string</returns>
    static public string ExtractQuotedString(string s, string quoteStr)
    {
      if (NotEmpty(quoteStr) && NotEmpty(s) &&
          s.StartsWith(quoteStr) && s.EndsWith(quoteStr))
      {
        if (s.Length > quoteStr.Length * 2)
        {
          return s.Substring(quoteStr.Length, s.Length - (quoteStr.Length * 2)).Replace(quoteStr + quoteStr, quoteStr);
        }
      }
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Extracts quoted string. Removes " from the beginning and 
    /// from the end of the string, replaces double " occurrencies 
    /// inside the string with the single ".
    /// </summary>
    /// <param name="s">quoted string</param>
    /// <returns>extracted string</returns>
    static public string ExtractQuotedString(string s)
    {
      return ExtractQuotedString(s, "\"");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns upper-cased string
    /// </summary>
    static public string ToUpper(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return s;
      return s.ToUpper();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns lower-cased string
    /// </summary>
    static public string ToLower(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return s;
      return s.ToLower();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Makes the first character of the string uppercase.
    /// </summary>
    /// <param name="s">string to change case</param>
    /// <returns>string with the first character uppercase and other character as is</returns>
    static public string ToTitleCase(string s)
    {
      return NotEmpty(s) ? s.Substring(0, 1).ToUpper() + (s.Length > 1 ? s.Substring(1) : "") : s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if strings are equal (or both strings are empty or null).
    /// Performs case-insensitive compare.
    /// </summary>
    static public bool Equals(string s1, string s2)
    {
      return StringComparer.OrdinalIgnoreCase.Compare(s1 ?? String.Empty, s2 ?? String.Empty) == 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns number of lines of the given string.
    /// </summary>
    /// <param name="s">string to count lines</param>
    /// <returns>number of lines of the given string</returns>
    static public int GetLineCount(string s)
    {
      int count = 0;
      if (NotEmpty(s))
      {
        for (int i = 0; i < s.Length; i++)
        {
          if (s[i] == '\n')
          {
            count++;
          }
        }
      }
      return count;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds back slash to the end of the given string, 
    /// if string is not empty and is not already ended with a slash.
    /// </summary>
    /// <param name="s">string to add backslash</param>
    /// <returns>string with backslash in the end</returns>
    static public string AddBackSlash(string s)
    {
      if (NotEmpty(s) && !s.EndsWith("\\") && !s.EndsWith("/"))
      {
        return s + "\\";
      }
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds slash to the end of the given string, 
    /// if string is not empty and is not already ended with a slash.
    /// </summary>
    /// <param name="s">string to add backslash</param>
    /// <returns>string with backslash in the end</returns>
    static public string AddSlash(string s)
    {
      if (NotEmpty(s) && !s.EndsWith("\\") && !s.EndsWith("/"))
      {
        return s + "/";
      }
      return s;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns part of string located before the given separator.
    /// </summary>
    /// <param name="srcStr">source string</param>
    /// <param name="separator">separator  string</param>
    /// <returns>substring before the separator</returns>
    static public string SubstringBeforeSeparator(string srcStr, string separator)
    {
      string tmpStr = srcStr.Trim();
      int curPos = tmpStr.IndexOf(separator);
      if ((curPos >= 0) && (tmpStr.Length > separator.Length))
      {
        tmpStr = tmpStr.Substring(0, curPos);
      }
      else
      {
        tmpStr = "";
      }
      return tmpStr;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns part of string located after the given separator.
    /// </summary>
    /// <param name="srcStr">source string</param>
    /// <param name="separator">separator  string</param>
    /// <returns>substring after the separator</returns>
    static public string SubstringAfterSeparator(string srcStr, string separator)
    {
      string tmpStr = srcStr.Trim();
      int curPos = tmpStr.IndexOf(separator);
      if ((curPos >= 0) && (tmpStr.Length > separator.Length))
      {
        tmpStr = tmpStr.Substring(curPos + separator.Length);
      }
      else
      {
        tmpStr = "";
      }
      return tmpStr;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Safe version of the string Substring() method.
    /// </summary>
    /// <param name="s">string to return substring of</param>
    /// <param name="start">number of character to start substring</param>
    /// <param name="length">number of character to include in the substring</param>
    /// <returns>substring of the given string that starts from the given character
    /// and has the given length</returns>
    static public string Substring(string s, int start, int length)
    {
      string s2 = CxUtils.Nvl(s);
      int start2 = Math.Min(Math.Max(0, start), s2.Length);
      int length2 = Math.Max(Math.Min(length, s2.Length - start2), 0);
      return s2.Substring(start2, length2);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes placeholders from the string.
    /// </summary>
    /// <param name="text">text to remove placeholders from</param>
    /// <param name="placeholder">a character placeholder starts with and ends with</param>
    /// <returns>string with removed placeholders</returns>
    static public string RemovePlaceholders(string text, char placeholder)
    {
      if (NotEmpty(text) && text.IndexOf(placeholder) >= 0)
      {
        StringBuilder source = new StringBuilder(text);
        StringBuilder target = new StringBuilder();
        bool append = true;
        while (source.Length > 0)
        {
          char c = source[0];
          source.Remove(0, 1);
          if (append)
          {
            if (c == placeholder)
            {
              if (source.Length > 0 && source[0] == placeholder)
              {
                source.Remove(0, 1);
              }
              else
              {
                append = false;
              }
            }
          }
          else
          {
            if (c == placeholder)
            {
              append = true;
              if (source.Length > 0)
              {
                c = source[0];
                source.Remove(0, 1);
              }
              else
              {
                c = Char.MinValue;
              }
            }
          }
          if (append && c != Char.MinValue)
          {
            target.Append(c);
          }
        }
        return target.ToString().Trim();
      }
      return text;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts (serializes) string to array of bytes.
    /// </summary>
    /// <param name="s">string to serialize</param>
    /// <returns>array of bytes</returns>
    static public byte[] ToByteArray(string s)
    {
      if (s != null)
      {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, s);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds in string rightmost digit characters, converts them to int, 
    /// increments value and replaces digits with a new value.
    /// </summary>
    /// <param name="s">string containing number to increment</param>
    /// <param name="increment">increment by value (value to add)</param>
    /// <param name="maxLength">string length constraint, length must be not more than maxLength</param>
    static public string IncrementSequence(
      string s,
      int increment,
      int maxLength)
    {
      if (NotEmpty(s))
      {
        int i = s.Length - 1;
        while (i >= 0 && (s[i] < '0' || s[i] > '9'))
        {
          i--;
        }
        if (i >= 0)
        {
          int endIndex = i;
          while (i >= 0 && s[i] >= '0' && s[i] <= '9')
          {
            i--;
          }
          int startIndex = i + 1;
          string digits = s.Substring(startIndex, endIndex - startIndex + 1);
          int newValue = CxInt.Parse(digits, 0) + increment;
          s = s.Remove(startIndex, endIndex - startIndex + 1);
          s = s.Insert(startIndex, newValue.ToString());
          if (maxLength > 0 && s.Length > maxLength)
          {
            int letterIndex = -1;
            for (int j = startIndex - 1; j >= 0; j--)
            {
              if ((s[j] >= 'a' && s[j] <= 'z') ||
                  (s[j] >= 'A' && s[j] <= 'Z') ||
                  (s[j] >= '0' && s[j] <= '9'))
              {
                letterIndex = j;
                break;
              }
            }
            int removeIndex = letterIndex - (s.Length - maxLength) + 1;
            if (letterIndex >= 0 && removeIndex >= 0)
            {
              s = s.Remove(removeIndex, s.Length - maxLength);
            }
            else if (startIndex > 0)
            {
              removeIndex = startIndex - (s.Length - maxLength);
              if (removeIndex >= 0)
              {
                s = s.Remove(removeIndex, s.Length - maxLength);
              }
            }
          }
          return s;
        }
      }
      return s;
    }
    //--------------------------------------------------------------------------- 
    /// <summary>
    /// Returns proposedName, if such name is not used yet, or proposedName
    /// appended by sequence number, if such name was already used.
    /// </summary>
    /// <param name="proposedName">proposed name for the object</param>
    /// <param name="usedNameList">list of used names</param>
    /// <param name="usedNameDictionary">dictionary of used names</param>
    /// <param name="maxLength">maximum name length</param>
    static protected string GetUniqueName(
      string proposedName,
      IList usedNameList,
      IDictionary usedNameDictionary,
      int maxLength)
    {
      if (NotEmpty(proposedName))
      {
        string result = proposedName;
        if (usedNameList != null ||usedNameDictionary != null)
        {
          if ((usedNameDictionary != null && usedNameDictionary.Contains(result)) &&
              (usedNameList != null && usedNameList.Contains(result)))
          {
            if (!RegexValidate(result, ".* \\(\\d+\\)"))
            {
              result = IncrementSequence(
                result + " (0)",
                1,
                maxLength);
            }
            while ((usedNameDictionary != null && usedNameDictionary.Contains(result)) &&
                   (usedNameList != null && usedNameList.Contains(result)))
            {
              result = IncrementSequence(result, 1, maxLength);
            }
          }
        }
        return result;
      }
      return proposedName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns proposedName, if such name is not used yet, or proposedName
    /// appended by sequence number, if such name was already used.
    /// </summary>
    /// <param name="proposedName">proposed name for the object</param>
    /// <param name="usedNameList">list of used names</param>
    /// <param name="maxLength">maximum name length</param>
    static public string GetUniqueName(
      string proposedName,
      IList usedNameList,
      int maxLength)
    {
      return GetUniqueName(proposedName, usedNameList, null, maxLength);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns proposedName, if such name is not used yet, or proposedName
    /// appended by sequence number, if such name was already used.
    /// </summary>
    /// <param name="proposedName">proposed name for the object</param>
    /// <param name="usedNameDictionary">dictionary of used names</param>
    /// <param name="maxLength">maximum name length</param>
    static public string GetUniqueName(
      string proposedName,
      IDictionary usedNameDictionary,
      int maxLength)
    {
      return GetUniqueName(proposedName, null, usedNameDictionary, maxLength);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Trims string. If result is empty returns null
    /// </summary>
    /// <param name="val">original string value</param>
    /// <returns>trimmed value or null</returns>
    static public string Clean(string val)
    {
      if (val != null)
      {
        val = val.Trim();
        if (val == "")
          val = null;
      }
      return val;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Serializes string to base-64 string.
    /// </summary>
    /// <param name="source">string to serialize</param>
    /// <returns>serialized base-64 string</returns>
    static public string ToBase64(string source)
    {
      IFormatter formatter = new BinaryFormatter();
      MemoryStream stream = new MemoryStream();
      formatter.Serialize(stream, source);
      return Convert.ToBase64String(stream.ToArray());
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Deserializes string from base-64 string.
    /// </summary>
    /// <param name="source">base-64 string</param>
    /// <returns>deserialized string</returns>
    static public string FromBase64(string source)
    {
      byte[] bytes = Convert.FromBase64String(source);
      MemoryStream stream = new MemoryStream(bytes);
      IFormatter formatter = new BinaryFormatter();
      string result = (string)formatter.Deserialize(stream);
      return result;
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Replaces {0}, {1}, ... placeholders with string values of given parameters.
    /// Maximum count of parameters is 10 (from 0 to 9).
    /// </summary>
    /// <param name="s">string with placeholders</param>
    /// <param name="parameters">array of parameters</param>
    /// <returns>string with replaced placeholders</returns>
    static public string FormatArray(string s, object[] parameters)
    {
      if (NotEmpty(s))
      {
        int paramLength = parameters != null ? parameters.Length : 0;
        for (int i = 0; i < 10; i++)
        {
          string placeholder = "{" + i + "}";
          string paramValue = i < paramLength ? CxUtils.ToString(parameters[i]) : "";
          s = s.Replace(placeholder, paramValue);
        }
      }
      return s;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Replaces {0}, {1}, ... placeholders with string values of given parameters.
    /// Maximum count of parameters is 10 (from 0 to 9).
    /// </summary>
    /// <param name="s">string with placeholders</param>
    /// <param name="parameters">array of parameters</param>
    /// <returns>string with replaced placeholders</returns>
    static public string Format(string s, params object[] parameters)
    {
      return FormatArray(s, parameters);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string is in C# representation.
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>true if string starts and ends with double quote</returns>
    static public bool IsCSharpString(string s)
    {
      return NotEmpty(s) && s.Length > 2 && s.StartsWith("\"") && s.EndsWith("\"");
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Parses C# string representation to the normal string value.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <returns>string value</returns>
    static public string ParseCSharpString(string s)
    {
      if (IsCSharpString(s))
      {
        s = s.Substring(1).Substring(0, s.Length - 2);
        s = Replace(s,
          new string[] { "\\\"", "\\n", "\\r", "\\t" },
          new string[] { "\"", "\n", "\r", "\t" });
      }
      return s;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given string contains at least one letter character.
    /// </summary>
    /// <param name="s">string to check for letters</param>
    /// <returns>true if given string contains at least one letter character</returns>
    static public bool ContainsLetters(string s)
    {
      if (NotEmpty(s))
      {
        for (int i = 0; i < s.Length; i++)
        {
          if (Char.IsLetter(s, i))
          {
            return true;
          }
        }
      }
      return false;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns an amount of the entries of the given substring encountered 
    /// in the given string.
    /// </summary>
    /// <param name="s">a string to perform the calculation in</param>
    /// <param name="subString">the sub-string which entries should be calculated</param>
    /// <returns>amount of entries</returns>
    static public int CalcEntriesAmount(string s, string subString)
    {
      // Validate the code applicability.
      if (string.IsNullOrEmpty(s))
        return 0;
      if (string.IsNullOrEmpty(subString))
        return 0;

      int amount = 0;
      int startIndex = 0;
      int indexOf = s.IndexOf(subString, startIndex);
      while (indexOf > -1)
      {
        amount++;
        startIndex = indexOf + 1;

        indexOf = s.IndexOf(subString, startIndex);
      }
      return amount;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Appends source text with the text to append (if text to append is not empty). 
    /// Inserts separator between source text and text to append.
    /// </summary>
    /// <param name="sourceText">source text</param>
    /// <param name="textToAppend">text to append</param>
    /// <param name="separator">separator to insert between</param>
    /// <returns>appended source text or original source text</returns>
    static public string Append(
      string sourceText,
      string textToAppend,
      string separator)
    {
      return CxUtils.Nvl(sourceText) +
             (NotEmpty(sourceText) && NotEmpty(textToAppend) ? CxUtils.Nvl(separator) : "") +
             CxUtils.Nvl(textToAppend);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes HTML tags from the HTML and returns pure text.
    /// </summary>
    /// <param name="html">HTML content to strip tags from</param>
    /// <returns>pure text</returns>
    static public string RemoveHtmlTagsAndStyles(string html)
    {
      if (html != null)
      {
        do
        {
          int start = html.IndexOf("<style");
          if (start <= 0) break;
          int end = html.IndexOf("</style");
          int count = 0;
          if (end > 0)
            count = html.IndexOf(">", end) - start + 1;
          html = html.Remove(start, count);
        } while (true);

        return RemoveHtmlTags(html);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes HTML tags from the HTML and returns pure text.
    /// </summary>
    /// <param name="html">HTML content to strip tags from</param>
    /// <returns>pure text</returns>
    static public string RemoveHtmlTags(string html)
    {
      if (html != null)
      {
        StringBuilder sb = new StringBuilder();
        bool isInTag = false;
        bool isPrevSpace = false;
        foreach (char c in html)
        {
          if (c == '<')
          {
            isInTag = true;
          }
          else if (c == '>')
          {
            isInTag = false;
          }
          else if (!isInTag)
          {
            bool isSpace = Char.IsWhiteSpace(c) || Char.IsControl(c);
            if (isSpace)
            {
              if (!isPrevSpace)
              {
                sb.Append(' ');
              }
            }
            else
            {
              sb.Append(c);
            }
            isPrevSpace = isSpace;
          }
        }
        return HttpUtility.HtmlDecode(sb.ToString().Trim());
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Splits multiline text into particular lines.
    /// Empty lines are ignored.
    /// </summary>
    /// <param name="text">text to split</param>
    /// <returns>array of non-empty lines</returns>
    static public string[] SplitLines(string text)
    {
      List<string> list = new List<string>();
      Regex r = new Regex(@"([^\n\r]+)[\s\n\r]*");
      Match m = r.Match(text);
      while (m != null && m.Success)
      {
        if (m.Groups.Count > 1 && NotEmpty(m.Groups[1].Value))
        {
          list.Add(m.Groups[1].Value);
        }
        m = m.NextMatch();
      }
      return list.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Splits text into particular words.
    /// </summary>
    /// <param name="text">text to split</param>
    /// <returns>array of non-empty words</returns>
    static public string[] SplitWords(string text)
    {
      List<string> list = new List<string>();
      Regex r = new Regex(@"(\w+)\W*");
      Match m = r.Match(text);
      while (m != null && m.Success)
      {
        if (m.Groups.Count > 1 && NotEmpty(m.Groups[1].Value))
        {
          list.Add(m.Groups[1].Value);
        }
        m = m.NextMatch();
      }
      return list.ToArray();
    }
    //-------------------------------------------------------------------------
  }
}