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

using System.Xml.Linq;

namespace Framework.Remote
{
  /// <summary>
  /// Descriptor for table column
  /// </summary>
  public class CxCoumnDescriptor
  {
    private string m_ColumnName = string.Empty;
    private string m_Name = string.Empty;
    private readonly XElement m_ClmXml = new XElement("c");
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default .ctor
    /// </summary>
    public CxCoumnDescriptor()
    {
      XAttribute clmNameAttr = new XAttribute("n", "");
      m_ClmXml.Add(clmNameAttr);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets column name;
    /// </summary>
    public string Name
    {
      get { return m_Name; }
      set
      {
        m_Name = value;
        if (value != null)
          m_ClmXml.Attribute("n").Value = value;
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets column ordinal
    /// </summary>
    public int Ordinal { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds column data in internal store.
    /// </summary>
    /// <param name="data"></param>
    public void AddData(object data)
    {
      XElement xData = new XElement("i") {Value = data.ToString()};
      m_ClmXml.Add(xData);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets columns data as XML.
    /// </summary>
    public XElement XmlData
    {
      get { return m_ClmXml; }
    }
  }
}
