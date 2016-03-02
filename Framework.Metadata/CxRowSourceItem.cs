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
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Static row source item (specified in the metadata)
	/// </summary>
	public class CxRowSourceItem : CxComboItem
	{
    //-------------------------------------------------------------------------
    protected CxRowSourceMetadata m_RowSource = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rowSource">parent row source metadata</param>
    /// <param name="value">value of the item</param>
    /// <param name="description">text description of the item</param>
		public CxRowSourceItem(
      CxRowSourceMetadata rowSource,
      object value,
      string description) : base(value, description)
		{
      m_RowSource = rowSource;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rowSource">parent row source metadata</param>
    /// <param name="value">value of the item</param>
    /// <param name="description">text description of the item</param>
    /// <param name="imageReference">image reference to the item image (optional)</param>
    public CxRowSourceItem(
      CxRowSourceMetadata rowSource,
      object value,
      string description,
      string imageReference) : this(rowSource, value, description)
    {
      m_ImageReference = imageReference;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent row source metadata.
    /// </summary>
    public CxRowSourceMetadata RowSource
    { get { return m_RowSource; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if row source item should be localizable.
    /// </summary>
    public bool IsLocalizable
    {
      get
      {
        return RowSource != null && 
               RowSource.HardCoded &&
               RowSource.IsLocalizable &&
               RowSource.Holder != null && 
               RowSource.Holder.IsMultilanguageEnabled &&
               RowSource.Holder.Multilanguage.IsLocalizable(LocalizationObjectTypeCode, LocalizationPropertyCode) &&
               CxText.ContainsLetters(OriginalDescription);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns description (item display text)
    /// </summary>
    override public string Description
    {
      get
      {
        if (IsLocalizable)
        {
          return RowSource.Holder.Multilanguage.GetValue(
            LocalizationObjectTypeCode,
            LocalizationPropertyCode,
            LocalizationObjectName,
            OriginalDescription);
        }
        return base.Description;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns original description, specified in the metadata.
    /// </summary>
    public string OriginalDescription
    {
      get
      {
        return base.Description;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Unique object name for localization.
    /// </summary>
    public string LocalizationObjectName
    {
      get
      {
        if (RowSource != null)
        {
          bool isItemUnique = true;
          foreach (CxComboItem item in RowSource.List)
          {
            if (item != this && CxUtils.ToString(item.Value) == CxUtils.ToString(Value))
            {
              isItemUnique = false;
            }
          }
          return RowSource.Id + "." + CxUtils.ToString(Value) + (isItemUnique ? "" : "." + OriginalDescription);
        }
        return CxUtils.ToString(Value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Object type code for localization.
    /// </summary>
    static public string LocalizationObjectTypeCode
    { get { return "Metadata.RowSource.Item"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Description property code for localization.
    /// </summary>
    static public string LocalizationPropertyCode
    { get { return "text"; } }
    //-------------------------------------------------------------------------
  }
}