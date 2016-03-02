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
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinPanelMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected CxWinTabMetadata m_Tab = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="tab">tab metadata</param>
    /// <param name="element">XML element to load data from</param>
    public CxWinPanelMetadata(
      CxMetadataHolder holder, 
      CxWinTabMetadata tab,
      XmlElement element) : base(holder, element)
		{
      Tab = tab;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Creates empty panel metadata.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="tab">tab metadata</param>
    /// <param name="id">ID of the panel</param>
    public CxWinPanelMetadata(
      CxMetadataHolder holder,
      CxWinTabMetadata tab,
      string id) : base(holder)
    {
      Tab = tab;
      Id = id;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tab metadata.
    /// </summary>
    public CxWinTabMetadata Tab
    { 
      get { return m_Tab; }
      protected set { m_Tab = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text of the panel.
    /// </summary>
    public string DisplayText
    { get { return Id != Text ? Text : ""; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if panel should be placed starting with the new line.
    /// </summary>
    public bool IsNewLine
    { get { return this["new_line"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns panel layout column count.
    /// </summary>
    public int ColumnCount
    { get { return CxInt.Parse(this["column_count"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns panel layout control width.
    /// </summary>
    public int ControlWidth
    { get { return CxInt.Parse(this["control_width"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the panel is customizable thru the Enterprise Customization.
    /// </summary>
    public bool Customizable
    { get { return CxBool.Parse(this["customizable"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns panel layout Caption (Label) width.
    /// </summary>
    public int LabelWidth
    { get { return CxInt.Parse(this["label_width"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if panel should be placed starting with the new line.
    /// </summary>
    public bool IsBorderVisible
    {
      get { return CxBool.Parse(this["border"], true); }
      set { this["border"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if panel should be placed starting with the new line.
    /// </summary>
    public string PlaceBeforeDetailTab
    { get { return this["place_before_detail_tab"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if panel should be placed starting with the new line.
    /// </summary>
    public string PlaceAfterDetailTab
    { get { return this["place_after_detail_tab"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates if the panel appearance should be like a caption with an underlining at the top
    /// and no border around.
    /// </summary>
    public bool IsShownAsSeparator
    { 
      get { return CxBool.Parse(this["is_shown_as_separator"], false); }
      set { this["is_shown_as_separator"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the panel caption is visible.
    /// </summary>
    public bool IsCaptionVisible
    {
      get { return CxBool.Parse(this["is_caption_visible"], false); }
      set { this["is_caption_visible"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Panel height.
    /// </summary>
    public int Height
    {
      get { return CxInt.Parse(this["height"], 0); }
      set { this["height"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WinPanel";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent metadata object.
    /// </summary>
    public override CxMetadataObject ParentObject
    {
      get 
      {
        return Tab != null ? Tab.Form : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets the panel index inside the parent's tab if any.
    /// </summary>
    /// <returns>panel index</returns>
    public int GetIndex()
    {
      if (Tab == null)
        throw new ExException(
          string.Format("Tab is undefined for the current panel <{0}>", Id));
      for (int i = 0; i < Tab.Panels.Count; i++)
      {
        if (this == Tab.Panels[i])
          return i;
      }
      return -1;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return Tab != null ? Tab.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns tab caption translated in the context of the entity usage.
    /// </summary>
    /// <param name="entityMetadata">entity or entity usage context</param>
    /// <param name="languageCd">language code to be used, null if default</param>
    /// <returns>tab caption translated in the context of the entity usage</returns>
    public string GetCaption(CxEntityMetadata entityMetadata, string languageCd)
    {
      string localizedCaption = null;
      if (Holder.Multilanguage != null)
      {
        string languageCode = languageCd ?? Holder.Multilanguage.LanguageCode;
        string originalCaption = GetNonLocalizedPropertyValue("text");
        CxEntityUsageMetadata entityUsage = entityMetadata as CxEntityUsageMetadata;
        while (localizedCaption == null && entityUsage != null)
        {
          localizedCaption = Holder.Multilanguage.GetLocalizedValue(
            languageCode,
            LocalizationObjectTypeCode,
            "text",
            entityUsage.Id + "." + Tab.Form.Id + "." + Id,
            originalCaption);
          entityUsage = entityUsage.InheritedEntityUsage;
        }
      }
      if (localizedCaption != null)
      {
        return localizedCaption;
      }
      else
      {
        return DisplayText;
      }
    }
    //----------------------------------------------------------------------------
  }
}