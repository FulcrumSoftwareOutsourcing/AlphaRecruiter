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

using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  //------------------------------------------------------------------------------
  /// <summary>
  /// Child entities change notification mode.
  /// </summary>
  public enum NxChildChangeNotificationMode { AllRecords, EachRecord }
  //------------------------------------------------------------------------------

  //------------------------------------------------------------------------------
  /// <summary>
	/// Class that holds information about child entity usage.
	/// </summary>
	public class CxChildEntityUsageMetadata : CxMetadataObject
	{
    //----------------------------------------------------------------------------
    protected CxEntityUsageMetadata m_ParentEntityUsage = null;
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder instance</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="parentEntityUsage">parent entity usage</param>
    public CxChildEntityUsageMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxEntityUsageMetadata parentEntityUsage) : 
      base(holder, element)
    {
      m_ParentEntityUsage = parentEntityUsage;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the child entity usage.
    /// </summary>
    public string EntityUsageId
    {
      get { return CxText.ToUpper(this["entity_usage_id"]); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Child entity usage.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get { return Holder.EntityUsages[EntityUsageId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if child entity owned by parent one (rather than just referenced by).
    /// </summary>
    public bool OwnedBy
    {
      get { return (this["owned_by"] == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parent entity usage.
    /// </summary>
    override public CxMetadataObject ParentObject
    {
      get
      {
        return m_ParentEntityUsage;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if child entity usage should be visible in the hierarchical grid.
    /// </summary>
    public bool IsVisibleInHierarchy
    {
      get
      {
        if (EntityUsage != null && !EntityUsage.Visible)
          return false;
        if (CxUtils.IsEmpty(this["visible_in_hierarchy"]) &&
            Holder.Config != null &&
            Holder.Config.IsChildEntityUsageVisibleInHierarchy != NxBoolEx.Undefined)
        {
          return CxBoolEx.GetBool(Holder.Config.IsChildEntityUsageVisibleInHierarchy);
        }
        return this["visible_in_hierarchy"].ToLower() != "false";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if child entity usage should be visible on the detail page control 
    /// of the master entity grid frame.
    /// </summary>
    public bool IsVisibleInList
    {
      get
      {
        if (EntityUsage != null && (!EntityUsage.Visible || !EntityUsage.GetIsAccessGranted()))
          return false;
        return CxBool.Parse(this["visible_in_list"], true);
      }
      set { this["visible_in_list"] = value.ToString(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if child entity usage should be visible on the detail page control 
    /// of the master entity view form.
    /// </summary>
    public bool IsVisibleInView
    {
      get
      {
        if (EntityUsage != null && (!EntityUsage.Visible || !EntityUsage.GetIsAccessGranted()))
          return false;
        return CxBool.Parse(this["visible_in_view"], true);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns windows panel name to place child entity usage grid in the view form.
    /// </summary>
    public string WinViewPlacement
    { get { return this["win_view_placement"]; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns child entities change notification mode.
    /// </summary>
    public NxChildChangeNotificationMode ChangeNotificationMode
    { 
      get 
      {
        return CxEnum.Parse(
          this["change_notification_mode"], 
          NxChildChangeNotificationMode.AllRecords);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if child entity usage sould be customizable as child of another entity by user.
    /// </summary>
    public bool Customizable
    {
      get
      {
        if (EntityUsage == null)
          return false;

        return CxBool.Parse(this["customizable"], false);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if sould be shown amount of items.
    /// </summary>
    public bool IsShowAmountOfItems
    {
      get
      {
        if (EntityUsage == null)
          return false;

        return CxBool.Parse(this["show_amount_of_items"], true);
      }
    }
    //----------------------------------------------------------------------------
  }
}