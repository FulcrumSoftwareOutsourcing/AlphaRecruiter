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

namespace Framework.Utils
{
	/// <summary>
	/// Collection of IxValueProvider objects.
	/// Acts as IxValueProvider. 
	/// Searches for value in all value providers.
	/// </summary>
	public class CxValueProviderCollection : CollectionBase, IxValueProvider
	{
    //-------------------------------------------------------------------------
    protected CxHashtable m_InternalProvider = new CxHashtable();

        public IDictionary<string, string> ValueTypes { get; private set; }
        
           
        

        //-------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        public CxValueProviderCollection() : base()
		{
            ValueTypes = new Dictionary<string, string>();

        }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns value provider collection for the given set of value providers.
    /// Returns null if given set is empty.
    /// </summary>
    /// <param name="valueProviders">set of value providers</param>
    static public CxValueProviderCollection Create(params IxValueProvider[] valueProviders)
    {
      if (valueProviders != null && valueProviders.Length > 0)
      {
        CxValueProviderCollection collection = new CxValueProviderCollection();
        foreach (IxValueProvider provider in valueProviders)
        {
          collection.AddIfNotEmpty(provider);
        }
        return collection.Count > 0 ? collection : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets IxValueProvider list item.
    /// </summary>
    public IxValueProvider this[int index]
    {
      get
      {
        return (IxValueProvider) List[index];
      }
      set
      {
        List[index] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets value by name.
    /// </summary>
    public object this[string name]
    {
      get
      {
        object result = m_InternalProvider[name];
        for (int i = 0; i < Count && result == null; i++)
        {
          IxValueProvider provider = this[i];
          try
          {
            result = provider[name];
          }
          catch
          {
          }
        }
        return result;
      }
      set
      {
        m_InternalProvider[name] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds value provider to the list.
    /// </summary>
    /// <param name="provider">provider to add</param>
    /// <returns>index of added element</returns>
    public int Add(IxValueProvider provider)
    {
      return List.Add(provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds value provider to the list.
    /// </summary>
    /// <param name="provider">provider to add</param>
    public void AddIfNotEmpty(IxValueProvider provider)
    {
      if (provider != null)
      {
        Add(provider);
      }
    }
        ////temp fix for 'file' and 'photo' fields
        //private IDictionary<string, string> m_objTypes = new Dictionary<string, string>();
        //public IDictionary<string, string> ObjTypes
        //{
        //    get
        //    {
        //        return m_objTypes;
        //    }
        //}

        //public void AddObjTypes(IDictionary<string, string> objTypes)
        //{
        //    m_objTypes = objTypes;
        //}


    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts value provider at the specified index.
    /// </summary>
    /// <param name="index">position to insert to</param>
    /// <param name="provider">provider to insert</param>
    public void Insert(int index, IxValueProvider provider)
    {
      List.Insert(index, provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the given value provider object.
    /// </summary>
    /// <param name="provider">provider to get index of</param>
    /// <returns>index or -1 if provider is not in list</returns>
    public int IndexOf(IxValueProvider provider)
    {
      return List.IndexOf(provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if list contains the given provider.
    /// </summary>
    /// <param name="provider">provider to check</param>
    public bool Contains(IxValueProvider provider)
    {
      return List.Contains(provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes given provider from the list
    /// </summary>
    /// <param name="provider">provider to remove</param>
    public void Remove(IxValueProvider provider)
    {
      List.Remove(provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies list elements to the given array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(IxValueProvider[] array, int index)
    {
      List.CopyTo(array, index);
    }
    //-------------------------------------------------------------------------
  }
}