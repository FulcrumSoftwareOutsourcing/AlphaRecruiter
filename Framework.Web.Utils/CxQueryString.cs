/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Framework.Utils;

namespace Framework.Web.Utils
{
	/// <summary>
	/// Class incapsulating operations with HTTP request query string.
	/// </summary>
	public class CxQueryString
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Enumeration for hash code source options.
    /// </summary>
    public enum NxHashSignSource {QueryString, QueryStringAndSession};
    //-------------------------------------------------------------------------
    /// <summary>
    /// Query string hash code sign parameter.
    /// </summary>
    protected const string QS_HASH_CODE = "QSCODE";
    //-------------------------------------------------------------------------
    private string m_StartSeparator = "?";
    private string m_PairSeparator = "&";
    private string m_ValueSeparator = "=";
    private string m_OriginalQueryString = "";
    private NameValueCollection m_Map = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Default constructor
    /// </summary>
    public CxQueryString()
    {
      m_Map = new NameValueCollection(0);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from a string that already contains a querystring. No url encoding is done. Standard separation characters are used.
    /// </summary>
    /// <param name="queryString">Querystring string source</param>
    public CxQueryString(string queryString) : this(queryString, false)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from a string that already contains a querystring. Standard separation characters are used.
    /// </summary>
    /// <param name="queryString">Querystring string source</param>
    /// <param name="urlEncode">Whether or not standard Http url encoding should be performed on the values</param>
    public CxQueryString(
      string queryString, 
      bool urlEncode) : this(queryString, urlEncode, "?", "&", "=")
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from a string that already contains a querystring. Custom separation characters can be used.
    /// </summary>
    /// <param name="queryString">Querystring string source</param>
    /// <param name="urlEncode">Whether or not standard Http url encoding should be performed on the values</param>
    /// <param name="startSeparator">String that appears in the front of the query string. Used in the ToString() output.</param>
    /// <param name="pairSeparator">String that appears between pairs of the query string. Used in the ToString() output.</param>
    /// <param name="valueSeparator">String that appears between name and value in each pair of the query string. Used in the ToString() output.</param>
    public CxQueryString(
      string queryString, 
      bool urlEncode, 
      string startSeparator, 
      string pairSeparator, 
      string valueSeparator)
    {
      m_OriginalQueryString = queryString;

      m_StartSeparator = startSeparator;
      m_PairSeparator = pairSeparator;
      m_ValueSeparator = valueSeparator;

      queryString = queryString.Replace(m_StartSeparator, "");

      string[] parts = Regex.Split(queryString, m_PairSeparator);

      m_Map = new NameValueCollection(parts.Length);

      foreach (string part in parts) 
      {
        string[] subparts = Regex.Split(part, m_ValueSeparator);
        if (subparts.Length == 0)
        {
          Set(subparts[0], "");
        }
        else if (subparts.Length > 0)
        {
          Set(subparts[0], subparts[1], urlEncode);
        }
      }   
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from another CxQueryString object. Deep copy is performed to make sure the new object becomes completely independent of its source.
    /// </summary>
    /// <param name="source">Another object whose state will be used to create this new object.</param>
    public CxQueryString(CxQueryString source)
    {
      m_OriginalQueryString = source.ToString();

      m_StartSeparator = source.StartSeparator;
      m_PairSeparator = source.PairSeparator;
      m_ValueSeparator = source.ValueSeparator;

      string [] keys = source.AllKeys;

      m_Map = new NameValueCollection(keys.Length);

      foreach (string key in keys)
      {
        Set(key, source.Get(key));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from a NameValueCollection, such as one usually found in System.Web.UI.Page.Request.Querystring objects. No url encoding is done.
    /// </summary>
    /// <param name="queryStringMap">Querystring collection source</param>
    public CxQueryString(NameValueCollection queryStringMap) : this(queryStringMap, false)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor from a NameValueCollection, such as one usually found in System.Web.UI.Page.Request.Querystring objects. 
    /// </summary>
    /// <param name="queryStringMap">Querystring collection source</param>
    /// <param name="urlEncode">Whether or not standard Http url encoding should be performed on the values</param>
    public CxQueryString(NameValueCollection queryStringMap, bool urlEncode)
    {
      m_Map = new NameValueCollection(queryStringMap.Count);
      // copy each pair
      foreach (string q in queryStringMap)
      {
        Set(q, queryStringMap.Get(q), urlEncode);
      }
      m_OriginalQueryString = ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes pair with given name.
    /// </summary>
    /// <param name="name">Name of the pair to be removed.</param>
    public void Remove(string name)
    {
      m_Map.Remove(name.ToUpper());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Determines whether pair with given name exists in this query string. Case insensitive.
    /// </summary>
    /// <param name="name">Name of the desired pair.</param>
    /// <returns>Whether or not given pair exists in this query string.</returns>
    public bool Contains(string name)
    {
      return m_Map[name.ToUpper()] != null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets the value of the pair with given name. Adds new pair, if doesn't already exist. Url encoding is performed on NewValue.
    /// </summary>
    /// <param name="name">Name of the desired pair.</param>
    /// <param name="newValue">New value to be assigned to this pair. Will be forced to undergo url encoding.</param>
    public void Set(string name, string newValue)
    {
      Set(name, newValue, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Retreives the value of the pair with given name.
    /// </summary>
    /// <param name="name">Name of the desired pair.</param>
    public string Get(string name)
    {
      return CxUtils.Nvl(m_Map[name.ToUpper()]);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all keys in this query string
    /// </summary>
    public string[] AllKeys
    {
      get 
      {
        return m_Map.AllKeys;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Allows for square-bracket referencing of elements
    /// </summary>
    public string this[string name]
    {
      get 
      {
        return Get(name);
      }
      set 
      {
        Set(name, value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets the value of the pair with given name. Adds new pair, if doesn't already exist. No url encoding is done.
    /// </summary>
    /// <param name="name">Name of the desired pair.</param>
    /// <param name="newValue">New value to be assigned to this pair.</param>
    /// <param name="urlEncode">Whether or not standard Http url encoding should be performed on the values</param>
    public void Set(string name, string newValue, bool urlEncode)
    {
      if (CxUtils.NotEmpty(name))
      {
        if (urlEncode)
        {
          newValue = HttpContext.Current.Server.UrlEncode(newValue);
        }
        m_Map[name.ToUpper()] = newValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines the string that preceeds the actual pairs in the string output of this query string. "?" by default.
    /// </summary>
    public string StartSeparator
    {
      get 
      {
        return m_StartSeparator;
      }
      set 
      {
        m_StartSeparator = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines the string that separates pairs. "&amp;" by default.
    /// </summary>
    public string PairSeparator
    {
      get 
      {
        return m_PairSeparator;
      }
      set 
      {
        m_PairSeparator = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines the string that separates name from value within a pair. "=" by default.
    /// </summary>
    public string ValueSeparator
    {
      get 
      {
        return m_ValueSeparator;
      }
      set 
      {
        m_ValueSeparator = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// An instance of the querystring object that is based on the querystring in the current context
    /// </summary>
    public static CxQueryString Current
    {
      get 
      {
        return new CxQueryString(HttpContext.Current.Request.QueryString);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Represenation of the original string which was used to initialize this object
    /// </summary>
    public string OriginalQueryString
    {
      get 
      {
        return m_OriginalQueryString;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// String representation of contained querystring.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      if (m_Map.Count < 1)
      {
        return "";
      }

      NameValueCollection map = new NameValueCollection();
      for (int i = 0; i < m_Map.Count; i++)
      {
        string paramName = m_Map.Keys[i];
        string paramValue = m_Map.Get(paramName);
        if (CxUtils.NotEmpty(paramValue))
        {
          map[paramName] = paramValue;
        }
      }

      StringBuilder b = new StringBuilder();
      b.Append(m_StartSeparator);

      for (int i = 0; i < map.Count; i++) 
      {
        string paramName = map.Keys[i];
        string paramValue = map.Get(paramName);

        b.Append(paramName);
        b.Append(ValueSeparator);
        b.Append(paramValue);
			
        if (i < (map.Count - 1))
        {
          b.Append(PairSeparator);
        }
      }
      return b.ToString();
    }
    //-------------------------------------------------------------------------
    public string SetToUri(Uri uri)
    {
      return uri.Scheme + Uri.SchemeDelimiter + uri.Host + uri.AbsolutePath + this.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears the query string.
    /// </summary>
    public void Clear()
    {
      if (m_Map != null)
      {
        m_Map.Clear();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates hash code for the query string.
    /// </summary>
    protected string CalculateHashCode(NxHashSignSource hashSignSource)
    {
      ArrayList keys = new ArrayList();
      foreach (string key in AllKeys)
      {
        if (key.ToUpper() != QS_HASH_CODE && CxUtils.NotEmpty(this[key]))
        {
          keys.Add(key.ToUpper());
        }
      }
      keys.Sort();

      string hashSource = "";
      foreach (string key in keys)
      {
        hashSource += key + "=" + CxText.ToUpper(DecodeFull(this[key])) + ";";
      }
      if (hashSignSource == NxHashSignSource.QueryStringAndSession)
      {
        hashSource += "SessionId=" + HttpContext.Current.Session.SessionID + ";";
      }

      return CxCommon.ComputeMD5Hash(hashSource);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Signs query string with a hashcode.
    /// </summary>
    public void SignWithHashCode(NxHashSignSource hashSignSource)
    {
      this[QS_HASH_CODE] = CalculateHashCode(hashSignSource);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Signs query string with a hashcode.
    /// </summary>
    public void SignWithHashCode()
    {
      SignWithHashCode(NxHashSignSource.QueryStringAndSession);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates hashcode sign. Raises an exception if invalid.
    /// </summary>
    public void ValidateHashCodeSign(NxHashSignSource hashSignSource)
    {
      string actualCode = this[QS_HASH_CODE];
      string validCode = CalculateHashCode(hashSignSource);
      if (CxUtils.IsEmpty(actualCode) || actualCode != validCode)
      {
        string errorMessage = 
          hashSignSource == NxHashSignSource.QueryString ?
          "Access denied. Query string is invalid." :
          "Access denied. Query string or session is invalid.";
        throw new ExValidationException(errorMessage);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates hashcode sign. Raises an exception if invalid.
    /// </summary>
    public void ValidateHashCodeSign()
    {
      //ValidateHashCodeSign(NxHashSignSource.QueryStringAndSession);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of query string parameters.
    /// </summary>
    public int Count
    { get {return m_Map != null ? m_Map.Count : 0;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if query string is empty.
    /// </summary>
    public bool IsEmpty
    { get {return Count == 0;} }
    //-------------------------------------------------------------------------
    public string DecodeFull(string url)
    {
      string result = url;
      string oldResult = "";
      int counter = 0;
      while (oldResult != result && result.Contains("%") && counter < 5)
      {
        counter++;
        oldResult = result;
        result = HttpContext.Current.Server.UrlDecode(result);
      }
      return result;
    }
	}
}