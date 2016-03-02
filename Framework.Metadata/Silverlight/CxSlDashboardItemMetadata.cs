using System.Xml;

namespace Framework.Metadata
{
  public class CxSlDashboardItemMetadata : CxMetadataObject
  {
     //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">dasboard item xml</param>
    public CxSlDashboardItemMetadata(
      CxMetadataHolder holder,
      XmlElement element)
      : base(holder, element)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image to display for the tree item.
    /// </summary>
    public string ImageId
    {
      get { return this["image_id"]; }
    }
    //-------------------------------------------------------------------------
    public string EntityUsageId
    {
      get { return this["entity_usage_id"]; }
    }
    //-------------------------------------------------------------------------
    public string TreeItemId
    {
      get { return this["tree_item_id"]; }
    }
    //-------------------------------------------------------------------------
    public string SectionId
    {
      get { return this["section_id"]; }
    }
  }
}
