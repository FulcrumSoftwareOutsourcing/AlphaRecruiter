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
using System.Reflection;
using System.Data;
using System.Xml;

using Framework.Utils;
using System.ComponentModel;
using System.Collections.Generic;

namespace Framework.Entity
{
	/// <summary>
	/// Abstract definition class.
	/// </summary>
	abstract public class CxAbstractDefinition : IxDefinition, ICloneable
	{
    //----------------------------------------------------------------------------
    public const string NULL_INI_FILE_VALUE = "_NULL_INI_FILE_VALUE_"; // Value to write instead of INI files as NULLs
    //----------------------------------------------------------------------------
    protected CxAbstractDefinition m_Copy = null; // Copy of the defininition to use during update
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected CxAbstractDefinition()
		{
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Indexed property to get and set field value
    /// </summary>
    public object this [string name]
    {
      get { return GetProperty(name); }
      set { SetProperty(name, value); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value of the property.
    /// </summary>
    /// <param name="name">name of the field</param>
    /// <returns>value of the edit field by the field name</returns>
    virtual protected object GetProperty(string name)
    {
      PropertyInfo property;
      Object owner;
      FindPropertyOrThrow(name, out property, out owner);
      return property.GetValue(owner, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets property value.
    /// </summary>
    /// <param name="name">name of the property</param>
    /// <param name="value">value of the property</param>
    virtual protected void SetProperty(string name, object value)
    {
      PropertyInfo property;
      Object owner;
      FindPropertyOrThrow(name, out property, out owner);
      object val = (value is string ? CxCommon.StringToObject((string) value, property.PropertyType) : value);
      property.SetValue(owner, val, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds property by the property name.
    /// </summary>
    /// <param name="name">name of the property</param>
    /// <param name="property">found property or null</param>
    /// <param name="owner">object found property belongs to or null</param>
    virtual public void FindProperty(string name, out PropertyInfo property, out Object owner)
    {
      property = GetType().GetProperty(name, 
                                            BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                            null,
                                            null, 
                                            new Type[0], 
                                            null);
      owner = (property != null ? this : null);
      return;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds property by the property name or throw exception if it is not found.
    /// </summary>
    /// <param name="name">name of the property</param>
    /// <param name="property">found property</param>
    /// <param name="owner">object found property belongs to</param>
    virtual public void FindPropertyOrThrow(string name, out PropertyInfo property, out Object owner)
    {
      FindProperty(name, out property, out owner);
      if (property == null)
      {
        throw new ExPropertyNotFoundException(name);
      }
      return;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of all properies.
    /// </summary>
    [Browsable(false)]
    public IList<string> AllProperties
    { 
      get { return GetAllProperties(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of editable properies.
    /// </summary>
    [Browsable(false)]
    public IList<string> EditableProperties
    { 
      get { return GetEditableProperties(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of storable properies.
    /// </summary>
    [Browsable(false)]
    public IList<string> StorableProperties
    { 
      get { return GetStorableProperties(); }
    }
        private Dictionary<string, string> valueTypes = new Dictionary<string, string>();
        public IDictionary<string, string> ValueTypes
        {
            get
            {
                return valueTypes;
            }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns list of all properties.
        /// </summary>
        /// <returns>list of all properties</returns>
        virtual protected IList<string> GetAllProperties()
    {
      List<string> props = new List<string>();
      PropertyInfo[] properties = GetType().GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      foreach (PropertyInfo property in properties)
      {
        if (property.PropertyType == typeof(string))
        {
          props.Add(property.Name);
        }
      }
      return props;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of editable properties.
    /// </summary>
    /// <returns>list of editable properties</returns>
    virtual protected IList<string> GetEditableProperties()
    {
      List<string> props = new List<string>();
      PropertyInfo[] properties = GetType().GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      foreach (PropertyInfo property in properties)
      {
        if (IsEditableType(property.PropertyType) && IsEditable(property.Name))
        {
          props.Add(property.Name);
        }
      }
      return props;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name is editable or false otherwise.
    /// </summary>
    /// <param name="name">name of property to check</param>
    /// <returns>true if property with the given name is editable or false otherwise.</returns>
    virtual protected bool IsEditable(string name)
    {
      return (name != "Item" && name != "TypeName");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of storable properties.
    /// </summary>
    /// <returns>list of storable properties</returns>
    virtual protected IList<string> GetStorableProperties()
    {
      List<string> props = new List<string>();
      PropertyInfo[] properties = GetType().GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
      foreach (PropertyInfo property in properties)
      {
        if (IsEditableType(property.PropertyType) && IsStorable(property.Name))
        {
          props.Add(property.Name);
        }
      }
      return props;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name is storable or false otherwise.
    /// </summary>
    /// <param name="name">name of property to check</param>
    /// <returns>true if property with the given name is storable or false otherwise.</returns>
    virtual protected bool IsStorable(string name)
    {
      return (name != "Item" && name != "TypeName");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns title for definition views (dialog, widget, etc.)
    /// </summary>
    /// <returns>title for definition views (dialog, widget, etc.)</returns>
    virtual public string GetViewTitle()
    {
      return "";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of the XML node to read/write property with the given name.
    /// </summary>
    /// <param name="propertyName">name of property</param>
    /// <returns>name of the XML node to read/write property with the given name</returns>
    virtual protected string GetNodeName(string propertyName)
    {
      return propertyName;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name and value should be saved or false otherwise.
    /// </summary>
    /// <param name="name">name of property</param>
    /// <param name="value">property value</param>
    /// <returns>true if property with the given name and value should be saved or false otherwise</returns>
    virtual protected bool ShouldBeWritten(string name, ref object value)
    {
      return CxUtils.NotEmpty(value);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default property value to use if XML node is absent.
    /// </summary>
    /// <param name="name">name of property</param>
    /// <returns>default property value to use if XML node is absent</returns>
    virtual protected string GetDefaultValue(string name)
    {
      return "";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates deep copy of this object.
    /// </summary>
    /// <returns></returns>
    virtual public Object Clone()
    {
      return MemberwiseClone();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Clears all properties.
    /// </summary>
    virtual public void Clear()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Restores properties from the application INI file.
    /// </summary>
    /// <param name="sectionName">name of the section in the INI file</param>
    virtual public void ReadIni(string sectionName)
    {
      ReadIni(CxIniFile.GetAppIniFileName(), sectionName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Restores properties from the specified INI file.
    /// </summary>
    /// <param name="iniFileName">name of INI file to save</param>
    /// <param name="sectionName">name of the section in the INI file</param>
    virtual public void ReadIni(string iniFileName, string sectionName)
    {
      foreach (string name in StorableProperties)
      {
        string nodeName = GetNodeName(name);
        string value = CxIniFile.GetPrivateProfileString(sectionName, 
                                                         nodeName, 
                                                         NULL_INI_FILE_VALUE, 
                                                         iniFileName);
        if (value != NULL_INI_FILE_VALUE)
        {
          this[name] = value;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Saves properties to the application INI file.
    /// </summary>
    /// <param name="sectionName">name of the section in the INI file</param>
    virtual public void WriteIni(string sectionName)
    {
      WriteIni(CxIniFile.GetAppIniFileName(), sectionName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Saves properties to the specified INI file.
    /// </summary>
    /// <param name="iniFileName">name of INI file to save</param>
    /// <param name="sectionName">name of the section in the INI file</param>
    virtual public void WriteIni(string iniFileName, string sectionName)
    {
      CxIniFile.DeletePrivateProfileSection(sectionName, iniFileName);
      foreach (string name in StorableProperties)
      {
        object value = this[name];
        if (ShouldBeWritten(name, ref value))
        {
          string keyName = GetNodeName(name);
          CxIniFile.WritePrivateProfileString(sectionName, keyName, value.ToString(), iniFileName);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Restores properties from the datarow.
    /// </summary>
    /// <param name="dataRow">datarow with definition data</param>
    virtual public void ReadDataRow(DataRow dataRow)
    {
      if (dataRow != null)
      {
        foreach (DataColumn column in dataRow.Table.Columns)
        {
          // Here we're writing a value directly by 
          // the column name (not by attribute id) because of performance issue
          // connected with getting an appropriate attribute by column name.
          this[column.ColumnName] = CxData.GetColumnValue(dataRow, column);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes properties into the datarow.
    /// </summary>
    /// <param name="dataRow">datarow with definition data</param>
    virtual public void WriteDataRow(DataRow dataRow)
    {
      if (dataRow != null)
      {
        foreach (string name in AllProperties)
        {
          object value = this[name];
          if (ShouldBeWritten(name, ref value))
          {
            CxData.SetValue(dataRow, name, value);
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Restores properties from value provider.
    /// </summary>
    /// <param name="provider">value provider with definition data</param>
    virtual public void ReadValueProvider(IxValueProvider provider)
    {
      foreach (string name in AllProperties)
      {
        this[name] = provider[name];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes properties into value provider.
    /// </summary>
    /// <param name="provider">value provider with definition data</param>
    virtual public void WriteValueProvider(IxValueProvider provider)
    {
      foreach (string name in AllProperties)
      {
        provider[name] = this[name];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Starts update of the definition.
    /// </summary>
    virtual public void BeginUpdate()
    {
      m_Copy = (CxAbstractDefinition)(Clone());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finishes update of the definition.
    /// </summary>
    virtual public void ApplyUpdate()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Cancels update of the definition.
    /// </summary>
    virtual public void CancelUpdate()
    {
      if (m_Copy != null)
      {
        m_Copy.CopyTo(this);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Copies definition properties into another one.
    /// </summary>
    /// <param name="definition">definition to copy properties to</param>
    public void CopyTo(IxDefinition definition)
    {
      if (definition != null)
      {
        IList<string> propList = definition.EditableProperties;
        foreach (string name in EditableProperties)
        {
          if (propList.Contains(name))
          {
            definition[name] = this[name];
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads definition from the XML document.
    /// </summary>
    /// <param name="parent">parent XML node</param>
    virtual public void Read(XmlNode parent)
    {
      foreach (string name in StorableProperties)
      {
        string nodeName = GetNodeName(name);
        XmlNode node = parent[nodeName];
        string value = (node == null ? GetDefaultValue(name) : node.InnerText);
        this[name] = value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads definition from the string with XML.
    /// </summary>
    /// <param name="xml">xml with definition</param>
    virtual public void Read(String xml)
    {
      XmlDocument doc = CxXml.StringToDoc(xml);
      Read(doc.DocumentElement);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes definition into the XML document.
    /// </summary>
    /// <param name="parent">parent XML node</param>
    virtual public void Write(XmlNode parent)
    {
      foreach (string name in StorableProperties)
      {
        object value = this[name];
        if (ShouldBeWritten(name, ref value) && CxUtils.NotEmpty(value))
        {
          string nodeName = GetNodeName(name);
          string s = CxCommon.ObjectToString(value);
          CxXml.AppendTextElement(parent, nodeName, s);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes definition into the XML document and returns it string representation.
    /// </summary>
    /// <param name="rootName">name of root node in the XNL document</param>
    /// <returns>string representation of XML document with definition</returns>
    virtual public String Write(String rootName)
    {
      return CxXml.DocToString(WriteToDoc(rootName));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads definition from the XML document.
    /// </summary>
    /// <param name="parent">parent XML node</param>
    virtual public void ReadAll(XmlNode parent)
    {
      foreach (string name in AllProperties)
      {
        string nodeName = GetNodeName(name);
        XmlNode node = parent[nodeName];
        string value = (node == null ? GetDefaultValue(name) : node.InnerText);
        this[name] = value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes definition into the XML document.
    /// </summary>
    /// <param name="parent">parent XML node</param>
    virtual public void WriteAll(XmlNode parent)
    {
      foreach (string name in AllProperties)
      {
        object value = this[name];
        if (ShouldBeWritten(name, ref value) && CxUtils.NotEmpty(value))
        {
          string nodeName = GetNodeName(name);
          string s = CxCommon.ObjectToString(value);
          CxXml.AppendTextElement(parent, nodeName, s);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes definition into the XML document and returns it.
    /// </summary>
    /// <param name="rootName">name of root node in the XNL document</param>
    /// <returns>XML document with definition</returns>
    virtual public XmlDocument WriteToDoc(String rootName)
    {
      XmlDocument doc = CxXml.CreateDocument(rootName);
      Write(doc.DocumentElement);
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given type are editable.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if property with the given type are editable</returns>
    virtual protected bool IsEditableType(Type type)
    {
      return IsSimpleType(type);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given type are storable.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if property with the given type are storable</returns>
    virtual protected bool IsStorableType(Type type)
    {
      return IsSimpleType(type);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given type are storable.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if property with the given type are storable</returns>
    virtual protected bool IsSimpleType(Type type)
    {
      return type == typeof(string)   || 
             type == typeof(int)      ||
             type == typeof(double)   ||
             type == typeof(decimal)  ||
             type == typeof(bool)  ||
             type == typeof(DateTime);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Renders a dictionary from the current definition.
    /// </summary>
    /// <returns>the dictionary rendered</returns>
    public virtual Dictionary<string, object> ToDictionary()
    {
      var result = new Dictionary<string, object>();
      foreach (var property in AllProperties)
      {
        result[property] = this[property];
      }
      return result;
    }
    //----------------------------------------------------------------------------
  }

  public static class CxAbstractDefinitionExtension
  {
    public static IList<Dictionary<string, object>> ToDictionaries(this IList<CxAbstractDefinition> definitions)
    {
      var result = new List<Dictionary<string, object>>();
      foreach (var definition in definitions)
      {
        result.Add(definition.ToDictionary());
      }
      return result;
    }
  }
}
