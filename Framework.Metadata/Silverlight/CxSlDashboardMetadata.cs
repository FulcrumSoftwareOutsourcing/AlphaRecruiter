using System.Collections.Generic;
using System.Xml;

namespace Framework.Metadata
{
  public  class CxSlDashboardMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    public List<CxSlDashboardItemMetadata> Items { get; protected set; }
    //----------------------------------------------------------------------------
    public CxSlDashboardMetadata(CxMetadataHolder holder, XmlElement element)
      : base(holder, element)
    {
      Items = new List<CxSlDashboardItemMetadata>();
      foreach (XmlElement itemElement in element.SelectNodes("sl_dashboard_item"))
      {
        CxSlDashboardItemMetadata item = new CxSlDashboardItemMetadata(holder, itemElement);
        Items.Add(item);
      }
    }
    //----------------------------------------------------------------------------
  }
}
