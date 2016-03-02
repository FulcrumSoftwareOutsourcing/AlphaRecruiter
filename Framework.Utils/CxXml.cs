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
using System.Xml;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace Framework.Utils
{
	/// <summary>
	/// Class with helper functions to work with XML.
	/// </summary>
	public class CxXml
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute with the given name from the element.
    /// If attribute absent returns empty string.
    /// </summary>
    /// <param name="element">element to find attribute for</param>
    /// <param name="attrName">name of the attribute</param>
    /// <returns>attribute with the given name from the element</returns>
    static public string GetAttr(XmlElement element, string attrName)
    {
      return GetAttr(element, attrName, "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute with the given name from the element.
    /// If attribute absent returns defaut value.
    /// </summary>
    /// <param name="element">element to find attribute for</param>
    /// <param name="attrName">name of the attribute</param>
    /// <param name="defValue">value to return if attribute not found</param>
    /// <returns>attribute with the given name from the element</returns>
    static public string GetAttr(XmlElement element, string attrName, string defValue)
    {
      if (HasAttr(element, attrName))
      {
        return element.Attributes[attrName].Value;
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute with the given name exists in the element.
    /// </summary>
    /// <param name="element">element to find attribute for</param>
    /// <param name="attrName">name of the attribute</param>
    /// <returns>true if attribute with the given name exists in the element or false otherwise</returns>
    static public bool HasAttr(XmlElement element, string attrName)
    {
      return (element != null && element.Attributes[attrName] != null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends node like <name>text</name> to the parent node.
    /// </summary>
    /// <param name="parent">element to add new one</param>
    /// <param name="name">name of the new node</param>
    /// <param name="text">text new node should contain</param>
    /// <returns>created element</returns>
    static public XmlElement AppendTextElement(XmlNode parent, string name, string text)
    {
      XmlElement element = CreateElement(parent, name);
      element.InnerText = text;
      return element;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads text from the text node with the given name under parent.
    /// </summary>
    /// <param name="parent">node to find the text one</param>
    /// <param name="name">name of the node to find</param>
    /// <returns>trimmed text of the node with the given name or empty string if such node not found</returns>
    static public string ReadTextElement(XmlNode parent, string name)
    {
      if (parent == null) return "";
      XmlNode node = parent.SelectSingleNode(name);
      string value = (node != null ? CxText.TrimSpace(node.InnerText) : "");
      return value;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates element with the given tag name under parent.
    /// </summary>
    /// <param name="parent">node to create new element under</param>
    /// <param name="tagName">tag name of the new element</param>
    /// <returns>created element</returns>
    static public XmlElement CreateElement(XmlNode parent, string tagName)
    {
      XmlElement element = parent.OwnerDocument.CreateElement(tagName);
      parent.AppendChild(element);
      return element;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if argument is a valid XML node name.
    /// </summary>
    /// <param name="name">name to check</param>
    /// <returns>true if argument is a valid XML node name or false otherwise</returns>
    static public bool IsValidNodeName(string name)
    {
      XmlDocument doc = new XmlDocument();
      try
      {
        doc.CreateElement(name);
        return true;
      }
      catch (XmlException)
      {
        return false;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates XML document drom the file.
    /// </summary>
    /// <param name="fileName">name of file with XML document</param>
    /// <returns>loaded document</returns>
    static public XmlDocument LoadDocument(string fileName)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(fileName);
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates XML document from the string.
    /// </summary>
    /// <param name="xml">string with XML document</param>
    /// <returns>loaded document</returns>
    static public XmlDocument StringToDoc(string xml)
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xml);
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates XML document from the byte array.
    /// </summary>
    /// <param name="contents">byte array with document contents</param>
    /// <returns>loaded document</returns>
    static public XmlDocument BytesToDoc(byte[] contents)
    {
      XmlDocument doc = new XmlDocument();
      MemoryStream stream = new MemoryStream(contents);
      doc.Load(stream);
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes XML document to the file.
    /// </summary>
    /// <param name="doc">document to write</param>
    /// <param name="fileName">name of file to write</param>
    static public void SaveDocument(XmlDocument doc, string fileName)
    {
      StreamWriter fw = new StreamWriter(fileName, false, Encoding.UTF8);
      WriteDocTo(doc, fw);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns document contents as a string.
    /// </summary>
    /// <param name="doc">document to convert to string</param>
    /// <returns>document contents as a string</returns>
    static public string DocToString(XmlDocument doc)
    {
      StringWriter sw = new StringWriter();
      WriteDocTo(doc, sw);
      return sw.ToString();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes document to the given writer.
    /// </summary>
    /// <param name="doc">document to convert to string</param>
    /// /<param name="tw">text writer to write document to</param>
    static public void WriteDocTo(XmlDocument doc, TextWriter tw)
    {
      XmlTextWriter writer = new XmlTextWriter(tw);
      writer.Formatting = Formatting.Indented;
      doc.WriteContentTo(writer);
      writer.Flush();
      writer.Close();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Deletes all children of node with the given name and parent.
    /// </summary>
    /// <param name="parent">parent of node to delete children</param>
    /// <param name="nodeName">name of node to delete children</param>
    static public void DeleteNodeChildren(XmlNode parent, string nodeName)
    {
      XmlNode node = parent.SelectSingleNode(nodeName);
      if (node != null)
      {
        node.RemoveAll();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates XML document and appends root element.
    /// </summary>
    /// <param name="rootName">name of the root element</param>
    /// <returns>crated document</returns>
    static public XmlDocument CreateDocument(string rootName)
    {
      XmlDocument doc = new XmlDocument();
      doc.AppendChild(doc.CreateElement(rootName));
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds attributes to the given element.
    /// </summary>
    /// <param name="element">element to add attributes</param>
    /// <param name="attributes">array with attribute names and values 
    /// to add to the script element (even array elements are names and odd ones are values)</param>
    static public void AddAttributes(XmlElement element, params string[] attributes)
    {
      for (int i = 0 ; i < attributes.Length / 2; i++)
      {
        string name = attributes[i * 2];
        string value = attributes[i * 2 + 1];
        element.SetAttribute(name, value);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads XML document from resources of the given assembly.
    /// </summary>
    /// <param name="assembly">assembly to load from</param>
    /// <param name="fileName">XML file name to load</param>
    /// <param name="nameSpace">namespace for the resource (optional)</param>
    /// <returns>XML document object or null, if not found</returns>
    static public XmlDocument LoadXmlFromResource(
      Assembly assembly, 
      string fileName,
      string nameSpace)
    {
      XmlDocument doc = null;
      if (assembly != null)
      {
        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
          bool isFound;
          if (CxUtils.NotEmpty(nameSpace))
          {
            isFound = string.Equals(
              resourceName, nameSpace + "." + fileName, StringComparison.OrdinalIgnoreCase);
          }
          else
          {
            isFound = resourceName.ToUpper().EndsWith(fileName.ToUpper());
          }
          if (isFound)
          {
            doc = new XmlDocument();
            Stream xmlStream = assembly.GetManifestResourceStream(resourceName);
            XmlTextReader reader = new XmlTextReader(xmlStream);
            doc.Load(reader);
            break;
          }
        }
      }
      return doc;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads XML document from resources of the given assembly.
    /// </summary>
    /// <param name="assembly">assembly to load from</param>
    /// <param name="fileName">XML file name to load</param>
    /// <returns>XML document object or null, if not found</returns>
    static public XmlDocument LoadXmlFromResource(
      Assembly assembly, 
      string fileName)
    {
      return LoadXmlFromResource(assembly, fileName, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Combines the given xml documents into one document.
    /// Does not include root nodes.
    /// </summary>
    /// <param name="documents">documents to be combined</param>
    /// <returns>an XML document that contains all the content 
    /// of the given documents (except root nodes)</returns>
    static public XmlDocument Combine(XmlDocument[] documents)
    {
      XmlDocument resultDocument = new XmlDocument();
      resultDocument.AppendChild(resultDocument.CreateNode(XmlNodeType.Element, "Root", ""));
      
      foreach (XmlDocument document in documents)
      {
        foreach (XmlNode node in document.DocumentElement.ChildNodes)
        {
          resultDocument.DocumentElement.AppendChild(resultDocument.ImportNode(node, true));
        }
      }
      return resultDocument;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given node contains some child XML elements (not text or cdata values).
    /// </summary>
    /// <param name="node">a node to perform check on</param>
    /// <returns>true if the given node contains some child XML elements</returns>
    static public bool HasChildXmlElements(XmlNode node)
    {
      foreach (XmlNode childNode in node)
      {
        if (childNode.NodeType == XmlNodeType.Element)
          return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    public static byte[] Serialize<T>(T container)
    {
      if (container == null)
        throw new ArgumentNullException("container");
      using (MemoryStream contentStream = new MemoryStream())
      {
        DataContractSerializer serializer = new DataContractSerializer(container.GetType());
        serializer.WriteObject(contentStream, container);
        return contentStream.ToArray();
      }
    }
    //-------------------------------------------------------------------------
    public static T Deserialize<T>(byte[] bytes)
    {
      using (MemoryStream contentStream = new MemoryStream(bytes))
      {
        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
        return (T) serializer.ReadObject(contentStream);
      }
    }
    //----------------------------------------------------------------------------
    public static string SerializeToString<T>(T container)
    {
      return Encoding.UTF8.GetString(Serialize(container));
    }
    //----------------------------------------------------------------------------
    public static T DeserializeFromString<T>(string str)
    {
      return Deserialize<T>(Encoding.UTF8.GetBytes(str));
    }
    //----------------------------------------------------------------------------
  }
}