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
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Summary description for CxMetadataObject.
	/// </summary>
  public class CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
	  private Dictionary<string, string> m_PropertyValues = new Dictionary<string, string>(); // Object property/value pairs
    protected string m_Id = ""; // ID of the attribute
    protected string idAttribute = ""; // Default name of the ID attribute
    protected Dictionary<string, string> m_InitialPropertyValues =
      new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    protected List<string> m_NonInheritableProperties;
    protected Dictionary<string, bool> m_IsPropertyLocalizableMap = new Dictionary<string, bool>();
    //----------------------------------------------------------------------------
    /// <summary>
    /// A list of property names that are not inheritable.
    /// </summary>
    public virtual List<string> NonInheritableProperties
    {
      get 
      {
        return new List<string>(CxNonInheritablePropertyRegistry.GetProperties(GetType()));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxMetadataObject(CxMetadataHolder holder, XmlElement element) : 
      this(holder, element, "id")
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxMetadataObject(CxMetadataHolder holder)
    {
      m_Holder = holder;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="idName">name of the ID attribute</param>
    public CxMetadataObject(
      CxMetadataHolder holder, 
      XmlElement element, 
      string idName) : this(holder)
    {
      // Invoke metadata holder event handler
      holder.DoOnMetadataObjectLoading(this, element);
      // Load properties
      foreach (XmlAttribute attribute in element.Attributes)
      {
        string name = attribute.Name;
        string value = attribute.Value;
        value = (name == idName ? CxUtils.Nvl(value.ToUpper()) : value);
        m_InitialPropertyValues[name] = value;
        PropertyValues[name] = value;
      }
      // Initialize ID
      if (CxUtils.NotEmpty(idName))
      {
        m_Id = this[idName].ToUpper();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a dictionary of property values exactly as they were initialized during XML loading.
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, string> GetInitialPropertyValues()
    {
      Dictionary<string, string> initialProperties = new Dictionary<string, string>();
      
      foreach (KeyValuePair<string, string> pair in m_InitialPropertyValues)
      {
        initialProperties.Add(pair.Key, pair.Value);
      }
      return initialProperties;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata object from the given XML element.
    /// </summary>
    /// <param name="element">the element to load from</param>
    public virtual void LoadCustomMetadata(XmlElement element)
    {
      Dictionary<string, bool> ignoreMap = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
      ignoreMap.Add("id", true);

      foreach (XmlAttribute attr in element.Attributes)
      {
        if (!ignoreMap.ContainsKey(attr.Name))
        {
          PropertyValues[attr.Name] = attr.Value;
        }
      }
    }
	  //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    virtual public void LoadOverride(XmlElement element)
    {
      Dictionary<string, bool> ignoreMap = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
      ignoreMap.Add("id", true);

      foreach (XmlAttribute attr in element.Attributes)
      {
        if (!ignoreMap.ContainsKey(attr.Name))
        {
          m_InitialPropertyValues[attr.Name] = attr.Value;
          PropertyValues[attr.Name] = attr.Value;
        }
      }
      foreach (XmlNode node in element.ChildNodes)
      {
        if (node is XmlElement && !CxXml.HasChildXmlElements(node) && !ignoreMap.ContainsKey(node.Name))
        {
          ReplacePropertyWithNode(element, node.Name);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value of the property 
    /// or null if localized value not found (or localization is disabled)
    /// </summary>
    /// <param name="propertyName">name of the property</param>
    /// <param name="propertyValue">value of the property</param>
    public string GetLocalizedPropertyValue(
      string propertyName,
      string propertyValue)
    {
      return GetLocalizedPropertyValue(
        propertyName,
        propertyValue,
        Holder.LanguageCode);
    }
	  //----------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value of the property 
    /// or null if localized value not found (or localization is disabled)
    /// </summary>
    /// <param name="propertyName">name of the property</param>
    /// <param name="propertyValue">value of the property</param>
    /// <param name="languageCode">language code</param>
    virtual public string GetLocalizedPropertyValue(
      string propertyName,
      string propertyValue,
      string languageCode)
    {
      if (IsPropertyLocalizable(propertyName))
      {
        string localizedValue = Holder.Multilanguage.GetLocalizedValue(
          languageCode,
          LocalizationObjectTypeCode,
          propertyName,
          LocalizationObjectName,
          propertyValue);

        if (localizedValue != null)
        {
          // Return successfully localized value
          return localizedValue;
        }
        else
        {
          // Try to find localized value from the inheritance list
          IList<CxMetadataObject> inheritanceList = InheritanceList;
          if (inheritanceList != null)
          {
            foreach (CxMetadataObject parentObject in inheritanceList)
            {
              // Ignore this object if somehow it is present in the list
              if (parentObject != this)
              {
                // Search only properties non-overridden in the metadata
                // Stop search when first overridden property was found
                if (GetNonLocalizedPropertyValue(propertyName) != parentObject.GetNonLocalizedPropertyValue(propertyName))
                {
                  return null;
                }
                localizedValue = parentObject[propertyName];
                if (localizedValue != propertyValue)
                {
                  // Localized value differs from the non-localized, localization OK
                  return localizedValue;
                }
              }
            }
          }
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns original value of property (without localization).
    /// </summary>
    /// <param name="propertyName">name of the property</param>
    public string GetNonLocalizedPropertyValue(string propertyName)
    {
      if (PropertyValues.ContainsKey(propertyName))
        return PropertyValues[propertyName];
      else 
        return string.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Metadata object properties.
    /// </summary>
    virtual public string this[string propertyName]
    {
      get 
      {
        string result = GetNonLocalizedPropertyValue(propertyName);
        string localizedValue = GetLocalizedPropertyValue(propertyName, result);
        if (localizedValue != null)
        {
          result = localizedValue;
        }
        return result;
      }
      set 
      { 
        PropertyValues[propertyName] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns initial property value, that was before DB override is loaded.
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <param name="getNonLocalizedValueIfEmpty">indicates if the method should return
    /// original property value when initial property value is empty.</param>
    /// <returns>initial property value</returns>
    public string GetInitialProperty(string propertyName, bool getNonLocalizedValueIfEmpty)
    {
      string value = string.Empty;
      if (!CxUtils.IsEmpty(propertyName))
      {
        if (!m_InitialPropertyValues.TryGetValue(propertyName, out value))
        {
          if (getNonLocalizedValueIfEmpty)
            value = GetNonLocalizedPropertyValue(propertyName);
          else
            value = string.Empty;
        }
      }
      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns initial property value, that was before DB override is loaded or 
    /// original property value when initial property value is empty.
    /// </summary>
    /// <param name="propertyName">property name</param>
    /// <returns>initial property value</returns>
    public string GetInitialProperty(string propertyName)
    {
      return GetInitialProperty(propertyName, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Wipes the given property out from the properties map.
    /// </summary>
    /// <returns>true if the properties map has been changed, otherwise false</returns>
    public void ResetPropertyToInitialValue(string propertyName)
    {
      if (m_InitialPropertyValues.ContainsKey(propertyName))
        PropertyValues[propertyName] = m_InitialPropertyValues[propertyName];
      else
        PropertyValues.Remove(propertyName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the metadata object contains property with the given name.
    /// </summary>
    /// <param name="propertyName">property name to check</param>
    public bool Contains(string propertyName)
    {
      return PropertyValues.ContainsKey(propertyName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Object ID.
    /// </summary>
    public string Id
    {
      get { return m_Id; }
      set { m_Id = value.ToUpper(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds contents of node with the given name to the properties list.
    /// </summary>
    /// <param name="element">element the text node located under</param>
    /// <param name="nodeName">name of the text node</param>
    protected void AddNodeToProperties(XmlElement element, string nodeName)
    {
      XmlNode node = element.SelectSingleNode(nodeName);
      if (node != null)
      {
        string nodeValue = CxText.TrimSpace(node.InnerText);
        PropertyValues[nodeName] = nodeValue;
        m_InitialPropertyValues[nodeName] = nodeValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Puts contents of node with the given name to the properties list.
    /// Replaces existing property value.
    /// </summary>
    /// <param name="element">element the text node located under</param>
    /// <param name="nodeName">name of the text node</param>
    protected void ReplacePropertyWithNode(XmlElement element, string nodeName)
    {
      XmlNode node = element.SelectSingleNode(nodeName);
      if (node != null)
      {
        string nodeValue = CxText.TrimSpace(node.InnerText);
        PropertyValues[nodeName] = nodeValue;
        m_InitialPropertyValues[nodeName] = nodeValue;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Copies all properties from the given object excluding listed ones.
    /// </summary>
    /// <param name="obj">object to copy properties from</param>
    /// <param name="excludedProps">array with names of excluded properties</param>
    public void CopyPropertiesFrom(CxMetadataObject obj, params string[] excludedProps)
    {
      foreach (string name in obj.PropertyValues.Keys)
      {
        if (name != idAttribute &&
            Array.IndexOf(excludedProps, name) == -1)
        {
          if (!PropertyValues.ContainsKey(name))
          {
            PropertyValues[name] = obj.PropertyValues[name];
          }
          if (!m_InitialPropertyValues.ContainsKey(name) && 
              obj.m_InitialPropertyValues.ContainsKey(name))
          {
            m_InitialPropertyValues[name] = obj.m_InitialPropertyValues[name];
          }
        }
      }
      DoAfterCopyProperties(obj);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Copies properties from all metadata objects present in the given list.
    /// </summary>
    /// <param name="metadataObjectList">list of metadata objects</param>
    public void CopyPropertiesFrom(IList<CxMetadataObject> metadataObjectList)
    {
      if (metadataObjectList != null)
      {
        foreach (CxMetadataObject metadataObject in metadataObjectList)
        {
          CopyPropertiesFrom(metadataObject);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Copies properties from all metadata objects present in the given list.
    /// </summary>
    /// <param name="metadataObjectList">list of metadata objects</param>
    /// <param name="excludedProperties">the properties to ignore while copying</param>
    public void CopyPropertiesFrom(
      IList<CxMetadataObject> metadataObjectList, string[] excludedProperties)
    {
      if (metadataObjectList != null)
      {
        foreach (CxMetadataObject metadataObject in metadataObjectList)
        {
          CopyPropertiesFrom(metadataObject, excludedProperties);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Inherits all the properties from the given list of metadata object into the current object.
    /// </summary>
    /// <param name="metadataObjectList">list of metadata object to inherit properties from</param>
    public void InheritPropertiesFrom(IList<CxMetadataObject> metadataObjectList)
    {
      CopyPropertiesFrom(metadataObjectList, NonInheritableProperties.ToArray());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Inherits all the properties from the given metadata object into the current object.
    /// </summary>
    /// <param name="metadataObject">the metadata object to inherit from</param>
    public void InheritPropertiesFrom(CxMetadataObject metadataObject)
    {
      InheritPropertiesFrom(new CxMetadataObject[] { metadataObject });
    }
	  //----------------------------------------------------------------------------
    /// <summary>
    /// Method is called after properties copying.
    /// </summary>
    /// <param name="sourceObj">object properties were taken from</param>
    virtual protected void DoAfterCopyProperties(CxMetadataObject sourceObj)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns object ID.
    /// </summary>
    /// <returns>metadata object id</returns>
    override public string ToString()
    {
      return Id;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parent metadata holder object.
    /// </summary>
    public CxMetadataHolder Holder
    { get {return m_Holder;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// User-friendly metadata object caption.
    /// </summary>
    virtual public string Text
    { 
      get 
      {
        return CxUtils.Nvl(this["text"], Id);
      } 
      set
      {
        this["text"] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Group name for the metadata object (if applicable).
    /// </summary>
    virtual public string GroupName
    { get {return this["group_name"];} }
    //-------------------------------------------------------------------------
    protected CxEntityUsageMetadata m_SecurityEntityUsage_Cache;
    /// <summary>
    /// Returns metadata object to find permissions for the object.
    /// </summary>
    virtual protected CxEntityUsageMetadata GetSecurityEntityUsage()
    {
      if (m_SecurityEntityUsage_Cache == null)
      {
        string id = this["security_entity_usage_id"];
        m_SecurityEntityUsage_Cache = CxUtils.NotEmpty(id) ? Holder.EntityUsages[id] : null;
      }
      return m_SecurityEntityUsage_Cache;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata object to find permissions for the object.
    /// </summary>
    public CxEntityUsageMetadata SecurityEntityUsage
    { get {return GetSecurityEntityUsage();} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns collection of all property names.
    /// </summary>
    public ICollection PropertyNames
    { get {return PropertyValues.Keys;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if metadata object is visible
    /// </summary>
    virtual protected bool GetIsVisible()
    {
      if (this["visible"].ToLower() == "false")
      {
        return false;
      }
      if (IsHiddenForUser && Holder != null && !Holder.IsDevelopmentMode)
      {
        return false;
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value that indicates if the metadata object is visible.
    /// </summary>
    public bool Visible
    { 
      get { return GetIsVisible(); }
      set { this["visible"] = value.ToString().ToLower(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if metadata object is initially visible before loading
    /// customizations from a database.
    /// </summary>
    public bool InitiallyVisible
    {
      get
      {
        string visible = GetInitialProperty("visible");
        if (visible.ToLower() == "false")
        {
          return false;
        }
        if (IsHiddenForUser && Holder != null && !Holder.IsDevelopmentMode)
        {
          return false;
        }
        return true;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if metadata object should be available only in development mode.
    /// </summary>
    public bool IsHiddenForUser
    {
      get { return CxBool.Parse(this["hidden_for_user"], false); }
      set { this["hidden_for_user"] = value.ToString(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if multilanguage is enabled.
    /// </summary>
    public bool IsMultilanguageEnabled
    {
      get
      {
        return Holder != null && Holder.IsMultilanguageEnabled;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name should be localizable.
    /// </summary>
    virtual public bool IsPropertyLocalizable(string propertyName)
    {
      if (!string.IsNullOrEmpty(LocalizationObjectTypeCode) &&
          !string.IsNullOrEmpty(propertyName) &&
          IsMultilanguageEnabled)
      {
        bool isLocalizible;
        if (m_IsPropertyLocalizableMap.TryGetValue(propertyName, out isLocalizible))
          return isLocalizible;
        return 
          m_IsPropertyLocalizableMap[propertyName] = 
            Holder.Multilanguage.IsLocalizable(LocalizationObjectTypeCode, propertyName);
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    virtual public string LocalizationObjectTypeCode
    {
      get
      {
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    virtual public string LocalizationObjectName
    {
      get
      {
        return Id;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of metadata objects properties was inherited from.
    /// </summary>
    virtual public IList<CxMetadataObject> InheritanceList
    {
      get
      {
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which are referenced by attribute properties.
    /// </summary>
    /// <returns>list of CxEntityMetadata objects or null</returns>
    virtual public IList<CxEntityMetadata> GetReferencedEntities()
    {
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parent metadata object.
    /// </summary>
    virtual public CxMetadataObject ParentObject
    {
      get
      {
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The map of property values.
    /// </summary>
    public Dictionary<string, string> PropertyValues
	  {
	    get { return m_PropertyValues; }
	    set { m_PropertyValues = value; }
	  }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the XML tag name applicable for the current metadata object.
    /// </summary>
    public virtual string GetTagName()
    {
      return string.Empty;
    }
	  //----------------------------------------------------------------------------
    /// <summary>
    /// Renders the metadata object to its XML representation.
    /// </summary>
    /// <param name="document">the document to render into</param>
    /// <param name="custom">indicates whether just customization should be rendered</param>
    /// <param name="tagNameToUse">the tag name to be used while rendering</param>
    /// <returns>the rendered object</returns>
    public virtual CxXmlRenderedObject RenderToXml(
      XmlDocument document, bool custom, string tagNameToUse)
    {
      string tagName = tagNameToUse;
      if (string.IsNullOrEmpty(tagName))
        tagName = GetTagName();

      if (string.IsNullOrEmpty(tagName))
        throw new ExException(string.Format("Current metadata object type <{0}> does not provide a valid tag name", GetType()));

      CxXmlRenderedObject result = new CxXmlRenderedObject();

      if (string.IsNullOrEmpty(tagNameToUse) && custom)
        tagName += "_custom";

      Dictionary<string, string> propertiesToRender = new Dictionary<string, string>();

      foreach (KeyValuePair<string, string> pair in PropertyValues)
      {
        bool isAttributeToRender = !IsPropertyLocalizable(pair.Key);
        if (isAttributeToRender && pair.Key != "id" && custom)
        {
          if (m_InitialPropertyValues.ContainsKey(pair.Key))
            isAttributeToRender = !CxText.Equals(pair.Value, m_InitialPropertyValues[pair.Key]);
          else 
            isAttributeToRender = !string.IsNullOrEmpty(pair.Value);
        }
        
        if (isAttributeToRender)
        {
          propertiesToRender[pair.Key] = pair.Value;
        }
      }

      if (propertiesToRender.Count == 1 && propertiesToRender.ContainsKey("id"))
        result.IsEmpty = true;

      XmlElement element = document.CreateElement(tagName, string.Empty);
      foreach (KeyValuePair<string, string> pair in propertiesToRender)
      {
        if (pair.Value != null)
        {
          XmlAttribute attribute = document.CreateAttribute(pair.Key);
          attribute.Value = pair.Value;
          element.Attributes.Append(attribute);
        }
      }
      result.Element = element;

      return result;
    }
	  //----------------------------------------------------------------------------
    #region Static methods to work with metadata objects
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of ids.
    /// </summary>
    /// <typeparam name="T">metadata object type</typeparam>
    /// <param name="list">list of metadata objects</param>
    /// <returns></returns>
    static public IList<string> ExtractIds<T>(IList<T> list) where T : CxMetadataObject
    {
      if (list == null)
        return null;

      List<string> ids = new List<string>();
      for (int i = 0; i < list.Count; i++)
      {
        ids.Add(list[i].Id);
      }
      return ids;
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}