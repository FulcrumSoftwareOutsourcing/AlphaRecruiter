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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Framework.Utils
{
	/// <summary>
	/// Summary description for CxHashtable.
	/// </summary>
  [Serializable]
  public class CxHashtable : Hashtable, IxValueProvider, IXmlSerializable
	{

        private Dictionary<string, string> valueTypes = new Dictionary<string, string>();
        IDictionary<string, string> IxValueProvider.ValueTypes
        {
            get
            {
                return valueTypes;
            }
        }

        //--------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        public CxHashtable() : base()
		{
		}
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected CxHashtable(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxHashtable(params object[] keysAndValues) : this()
    {
      for (int i = 0; i < keysAndValues.Length / 2; i++)
      {
        this[(string) keysAndValues[2 * i]] = keysAndValues[2 * i + 1];
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Indexed property to get value by name.
    /// </summary>
    public object this [string name] 
    { 
      get { return base[name.ToUpper()];  }
      set { base[name.ToUpper()] = value; } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Serializes this object and writes the XML document to a file using the specified stream.
    /// </summary>
    /// <param name="stream">stream to which the object is serialized</param>
    public void Serialize(Stream stream)
    {
      XmlSerializer xs = new XmlSerializer(typeof(CxHashtable));
      xs.Serialize(stream, this);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deserializes the XML document contained by the specified stream.
    /// </summary>
    /// <param name="stream">the Stream that contains the XML document to deserialize</param>
    /// <returns>IDictionary being deserialized</returns>
    public IDictionary Deserialize(Stream stream)
    {
      XmlSerializer xs = new XmlSerializer(typeof(CxHashtable));
      CxHashtable ds = (CxHashtable)xs.Deserialize(stream);
      return ds;
    }
    //-------------------------------------------------------------------------

	  #region Implementation of IXmlSerializable
    //-------------------------------------------------------------------------
	  /// <summary>
	  /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead. 
	  /// </summary>
	  /// <returns>
	  /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
	  /// </returns>
	  public XmlSchema GetSchema()
	  {
      return null;
	  }
    //-------------------------------------------------------------------------
	  /// <summary>
	  /// Generates an object from its XML representation.
	  /// </summary>
	  /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized. </param>
	  public void ReadXml(XmlReader reader)
	  {
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");
        string key = reader.ReadElementString("key");
        string value = reader.ReadElementString("value");
        reader.ReadEndElement();
        reader.MoveToContent();
        Add(key, value);
      }
      reader.ReadEndElement();
	  }
    //-------------------------------------------------------------------------
	  /// <summary>
	  /// Converts an object into its XML representation.
	  /// </summary>
	  /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized. </param>
	  public void WriteXml(XmlWriter writer)
	  {
      foreach (object key in Keys)
      {
        object value = this[key];
        writer.WriteStartElement("item");
        writer.WriteElementString("key", key.ToString());
        writer.WriteElementString("value", value.ToString());
        writer.WriteEndElement();
      }
	  }
    //-------------------------------------------------------------------------
	  #endregion
	}
}
