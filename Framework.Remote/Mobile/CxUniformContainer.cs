using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{

  [DataContract(Name = "CxUniformContainer", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]


  [KnownType(typeof(object[]))]
  [KnownType(typeof(CxAssemblyContainer))]
  [KnownType(typeof(byte[]))]
  [KnownType(typeof(CxClientAssemblyMetadata))]
  [KnownType(typeof(CxClientAttributeMetadata))]
 // [KnownType(typeof(List<string>))]
  [KnownType(typeof(CxClientClassMetadata))]
  [KnownType(typeof(CxClientCommandMetadata))]
  [KnownType(typeof(CxClientEntityMark))]
 // [KnownType(typeof(List<object>))]
  [KnownType(typeof(CxClientEntityMarks))]
  [KnownType(typeof(List<CxClientEntityMark>))]
  [KnownType(typeof(CxClientEntityMetadata))]
  [KnownType(typeof(Dictionary<string, CxClientAttributeMetadata>))]
  [KnownType(typeof(CxClientEntityMetadata[]))]
  [KnownType(typeof(CxClientCommandMetadata[]))]
  [KnownType(typeof(CxExceptionDetails))]
  [KnownType(typeof(Dictionary<string, object>))]
  [KnownType(typeof(List<CxClientParentEntity>))]
  [KnownType(typeof(CxClientImageMetadata))]
  [KnownType(typeof(CxClientMultilanguageItem))]
  [KnownType(typeof(CxClientParentEntity))]
  [KnownType(typeof(string[]))]
  [KnownType(typeof(CxClientPortalMetadata))]
  [KnownType(typeof(List<CxClientSectionMetadata>))]
  [KnownType(typeof(List<CxClientRowSource>))]
  [KnownType(typeof(List<CxClientAssemblyMetadata>))]
  [KnownType(typeof(List<CxClientClassMetadata>))]
  [KnownType(typeof(CxClientImageMetadata[]))]
  [KnownType(typeof(List<CxLayoutElement>))]
  [KnownType(typeof(Dictionary<string, byte[]>))]
  [KnownType(typeof(List<CxClientMultilanguageItem>))]
  [KnownType(typeof(List<CxLanguage>))]
  [KnownType(typeof(CxSkin))]
  [KnownType(typeof(List<CxSkin>))]
  [KnownType(typeof(Dictionary<string, string>))]
  [KnownType(typeof(CxClientRowSource))]
  [KnownType(typeof(CxClientRowSourceItem))]
  [KnownType(typeof(List<CxClientRowSourceItem>))]
  [KnownType(typeof(CxClientSectionMetadata))]
  [KnownType(typeof(CxClientTreeItemMetadata))]
  [KnownType(typeof(List<CxClientTreeItemMetadata>))]
  [KnownType(typeof(CxCommandParameters))]
  [KnownType(typeof(List<Dictionary<string, object>>))]
  [KnownType(typeof(CxDataItem))]
  [KnownType(typeof(CxExportToCsvInfo))]
  [KnownType(typeof(CxExpressionResult))]
  [KnownType(typeof(Dictionary<string, CxClientRowSource>))]
  [KnownType(typeof(CxFilterItem))]
 // [KnownType(typeof(IList))]
  [KnownType(typeof(CxLanguage))]
  [KnownType(typeof(CxLayoutElement))]
  [KnownType(typeof(List<CxLayoutElement>))]
  [KnownType(typeof(CxModel))]
  [KnownType(typeof(CxSortDescription[]))]
  [KnownType(typeof(CxQueryParams))]
  [KnownType(typeof(CxSettingsContainer))]
  [KnownType(typeof(CxUploadData))]
  [KnownType(typeof(CxUploadParams))]
  [KnownType(typeof(CxUploadResponse))]
  [KnownType(typeof(CxClientDashboardData))]
  [KnownType(typeof(CxClientDashboardItem))]
  [KnownType(typeof(CxClientDashboardItem[]))]


  public class CxUniformContainer 
  {
    /// <summary>
    /// Gets or sets the inner dictionary.
    /// </summary>
    /// <value>The inner dictionary.</value>
    [DataMember]
    public Dictionary<string, object> InnerDictionary { get; set; }

    public object this[string key]
    {
      get
      {
        if (InnerDictionary.ContainsKey(key))
          return InnerDictionary[key];
        else
          return null;
      }
      set { InnerDictionary[key] = value; }
    }

    /// <summary>
    /// Gets the list of keys available in the container.
    /// </summary>
    /// <value>The keys.</value>
    public IList<string> Keys
    {
      get { return new List<string>(InnerDictionary.Keys); }
    }

    /// <summary>
    /// Gets the list of values available within the container.
    /// </summary>
    /// <value>The values.</value>
    public IList<object> Values
    {
      get { return new List<object>(InnerDictionary.Values); }
    }

    /// <summary>
    /// Gets the first key.
    /// </summary>
    /// <value>The first key.</value>
    public string FirstKey
    {
      get { return Keys.Count > 0 ? Keys[0] : null; }
    }

    /// <summary>
    /// Gets the first value.
    /// </summary>
    /// <value>The first value.</value>
    public object FirstValue
    {
      get { return FirstKey != null ? InnerDictionary[FirstKey] : null; }
    }

    public CxUniformContainer()
    {
      InnerDictionary = new Dictionary<string, object>();
    }

    public CxUniformContainer(string key, object value)
      : this()
    {
      InnerDictionary[key] = value;
    }

    /// <summary>
    /// Determines whether the container contains a value under the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// 	<c>true</c> if the specified key contains key; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsKey(string key)
    {
      return InnerDictionary.ContainsKey(key);
    }


    /// <summary>
    /// Clears the data in the current container.
    /// </summary>
    public void Clear()
    {
      InnerDictionary.Clear();
    }
  }
}
