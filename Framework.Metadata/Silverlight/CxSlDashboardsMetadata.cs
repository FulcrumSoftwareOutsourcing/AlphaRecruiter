using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Framework.Metadata
{
  public class CxSlDashboardsMetadata : CxMetadataCollection
  {
    //-------------------------------------------------------------------------
    protected List<CxSlDashboardMetadata> m_Items = new List<CxSlDashboardMetadata>();
    protected Dictionary<string, CxSlDashboardMetadata> m_ItemsMap = new Dictionary<string, CxSlDashboardMetadata>();
    //-------------------------------------------------------------------------
    public CxSlDashboardsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "SlDashboads.xml"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    protected override void Load(XmlDocument doc)
    {
      foreach (XmlElement dashboard in doc.DocumentElement.SelectNodes("sl_dashboard"))
      {
        CxSlDashboardMetadata dm = new CxSlDashboardMetadata(Holder, dashboard);
        m_Items.Add(dm);
        m_ItemsMap.Add(dm.Id, dm);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Section with the given ID.
    /// </summary>
    public CxSlDashboardMetadata this[string id]
    {
      get
      {
        if (m_ItemsMap.ContainsKey(id.ToUpper()))
          return m_ItemsMap[id.ToUpper()];
        throw new ExMetadataException(string.Format("Dashboard with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
  }
}
