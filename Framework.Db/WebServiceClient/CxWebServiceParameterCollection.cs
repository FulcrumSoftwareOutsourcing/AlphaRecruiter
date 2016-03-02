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
using System.Data;
using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Class describing web service command parameters collection.
	/// </summary>
	public class CxWebServiceParameterCollection : ArrayList, IDataParameterCollection
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceParameterCollection()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of parameter with the given name.
    /// </summary>
    public int IndexOf(string parameterName)
    {
      for (int i = 0; i < Count; i++)
      {
        if (CxText.Equals(this[i].ParameterName, parameterName))
        {
          return i;
        }
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if list contains parameter with the given name.
    /// </summary>
    public bool Contains(string parameterName)
	  {
	    return IndexOf(parameterName) >= 0;
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes parameter with the given name.
    /// </summary>
    public void RemoveAt(string parameterName)
	  {
      int index = IndexOf(parameterName);
      if (index >= 0)
      {
        RemoveAt(index);
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter to the list.
    /// </summary>
    override public int Add(object value)
    {
      return Add((CxWebServiceParameter) value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter to the list.
    /// </summary>
    public int Add(CxWebServiceParameter value)
    {
      if (value == null)
      {
        throw new ExException("Web service command parameter could not be null.");
      }
      if (CxUtils.IsEmpty(value.ParameterName))
      {
        throw new ExException("Web service command parameter name could not be empty.");
      }
      return base.Add(value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter to the list.
    /// </summary>
    public int Add(string parameterName, DbType type)
    {
      return Add(new CxWebServiceParameter(parameterName, type));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter to the list.
    /// </summary>
    public int Add(string parameterName, object value)
    {
      return Add(new CxWebServiceParameter(parameterName, value));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter to the list.
    /// </summary>
    public int Add(string parameterName, DbType dbType, string sourceColumn)
    {
      return Add(new CxWebServiceParameter(parameterName, dbType, sourceColumn));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parameter by name.
    /// </summary>
    protected CxWebServiceParameter Get(string parameterName)
    {
      int index = IndexOf(parameterName);
      if (index < 0)
      {
        throw new ExException(String.Format(
          "Web service command parameter with name '{0}' is not found.", parameterName));
      }
      return this[index];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets parameter by name.
    /// </summary>
    protected void Set(string parameterName, CxWebServiceParameter value)
    {
      int index = IndexOf(parameterName);
      if (index >= 0)
      {
        if (value != null)
        {
          value.ParameterName = parameterName;
          this[index] = value;
        }
        else
        {
          RemoveAt(index);
        }
      }
      else if (value != null)
      {
        value.ParameterName = parameterName;
        Add(value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter by name.
    /// </summary>
    public CxWebServiceParameter this[string parameterName]
	  {
	    get 
      { 
        return Get(parameterName);
      }
	    set 
      { 
        Set(parameterName, value);
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter by name.
    /// </summary>
    object IDataParameterCollection.this[string parameterName]
    {
      get 
      { 
        return Get(parameterName);
      }
      set 
      { 
        Set(parameterName, (CxWebServiceParameter) value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets parameter by index.
    /// </summary>
    new public CxWebServiceParameter this[int index]
    {
      get
      {
        return (CxWebServiceParameter) base[index];
      }
      set
      {
        base[index] = value;
      }
    }
    //-------------------------------------------------------------------------
  }
}