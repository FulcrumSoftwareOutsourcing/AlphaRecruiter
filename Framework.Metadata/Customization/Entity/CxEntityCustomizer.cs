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
using System.Collections.Generic;
using System.Xml;

using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class to hold customization data for an entity usage
  /// </summary>
  public class CxEntityCustomizer : CxCustomizerBase, IxStorableInIdOrder
  {
    //-------------------------------------------------------------------------
    private CxEntityUsageMetadata m_Metadata;
    private CxAttributeCustomizerList m_AttributeCustomizers;
    private List<CxChildEntityCustomizer> m_ChildEntityCustomizers;
    private CxEntityCustomizerData m_CurrentData;
    private CxEntityCustomizerData m_InitialData;
    private CxEntityCustomizerLocalization m_CurrentLocalization;
    private CxEntityCustomizerLocalization m_InitialLocalization;
    //-------------------------------------------------------------------------
    
    #region Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxEntityUsageMetadata Metadata
    {
      get { return m_Metadata; }
      protected set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current data snapshot, non-saved data.
    /// </summary>
    public CxEntityCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    public override string Id
    {
      get { return Metadata != null ? Metadata.Id : null; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initial data snapshot, the data which is actually applied to the application.
    /// </summary>
    public CxEntityCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsVisibilityModified
    {
      get { return CurrentData.Visible != InitialData.Visible; }
    }
    //-------------------------------------------------------------------------
    public bool IsVisibilityToAdministratorModified
    {
      get { return CurrentData.VisibleToAdministrator != InitialData.VisibleToAdministrator; }
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerList AttributeCustomizers
    {
      get { return m_AttributeCustomizers; }
      set { m_AttributeCustomizers = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizer FormCustomizer
    {
      get
      {
        if (Metadata.WinForm != null)
          return Manager.GetFormCustomizer(Metadata.WinForm);
        else
          return null;
      }
    }
    //-------------------------------------------------------------------------
    public CxEntityCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public List<CxChildEntityCustomizer> ChildEntityCustomizers
    {
      get { return m_ChildEntityCustomizers; }
      set { m_ChildEntityCustomizers = value; }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityCustomizer(
      CxCustomizationManager manager, CxEntityUsageMetadata metadata)
      : base(manager)
    {
      Metadata = metadata;
      Context = manager.Context;

      AttributeCustomizers = new CxAttributeCustomizerList();
      ChildEntityCustomizers = new List<CxChildEntityCustomizer>();

      Initialize();
    }
    //-------------------------------------------------------------------------
    #endregion

    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares attribute name and attribute lists
    /// </summary>
    /// <param name="entity">an entity context</param>
    /// <param name="l1">first list of attribute names</param>
    /// <param name="l2">second list of attribute entity</param>
    /// <returns>true if lists are equal</returns>
    static public bool CompareAttrLists(
      CxEntityMetadata entity, IList<string> l1, IList<CxAttributeMetadata> l2)
    {
      UniqueList<string> list1 =
        new UniqueList<string>(StringComparer.OrdinalIgnoreCase);

      foreach (string attrId in l1)
      {
        CxAttributeMetadata attribute = entity.GetAttribute(attrId);
        CxAttributeMetadata valueAttribute = entity.GetValueAttribute(attribute);
        if (valueAttribute != null)
          list1.Add(valueAttribute.Id);
        else
          list1.Add(attribute.Id);
      }

      UniqueList<string> list2 = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
      foreach (CxAttributeMetadata attribute in l2)
      {
        CxAttributeMetadata valueAttribute = entity.GetValueAttribute(attribute);
        if (valueAttribute != null)
          list2.Add(valueAttribute.Id);
        else
          list2.Add(attribute.Id);
      }
      return CxList.CompareOrdered(list1, list2);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves customization data to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Save(CxDbConnection connection)
    {
      SaveData(connection);
      SaveLocalization(connection);
    }
    //-------------------------------------------------------------------------
    private void SaveData(CxDbConnection connection)
    {
      if (GetIsModifiedData())
      {
        // Entity management.
        DbInsertOrUpdate(connection);

        // Perform descending notification to fire the SaveData action in descendants.
        foreach (CxAttributeCustomizer attributeCustomizer in AttributeCustomizers)
        {
          attributeCustomizer.SaveData(connection);
        }

        InitialData = CurrentData.Clone();
      }
    }
    //-------------------------------------------------------------------------
    private void SaveLocalization(CxDbConnection connection)
    {
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = Context.Holder.Multilanguage;
        if (multilanguage != null)
        {
          foreach (KeyValuePair<string, string> langCaption in CurrentLocalization.LanguageSingleCaptionMap)
          {
            string localizationObjectName = Metadata.Id;
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              Metadata.LocalizationObjectTypeCode,
              "single_caption",
              localizationObjectName,
              CurrentLocalization.NonLocalizedSingleCaption,
              langCaption.Value);
          }
          foreach (KeyValuePair<string, string> langCaption in CurrentLocalization.LanguagePluralCaptionMap)
          {
            string localizationObjectName = Metadata.Id;
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              Metadata.LocalizationObjectTypeCode,
              "plural_caption",
              localizationObjectName,
              CurrentLocalization.NonLocalizedPluralCaption,
              langCaption.Value);
          }
        }
      }

      // Perform descending notification to fire the SaveLocalization action in descendants.
      foreach (CxAttributeCustomizer attributeCustomizer in AttributeCustomizers)
      {
        attributeCustomizer.SaveLocalization(connection);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves list of objects to database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="list">lists of objects to save</param>
    static public void SaveList(CxDbConnection connection, IList<CxEntityCustomizer> list)
    {
      connection.ExecuteInTransaction(
        delegate
        {
          foreach (CxEntityCustomizer customizer in list)
          {
            customizer.Save(connection);
          }
        });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedData()
    {
      bool isModified = !CurrentData.Compare(InitialData);
      if (!isModified)
      {
        foreach (CxAttributeCustomizer attributeCustomizer in AttributeCustomizers)
        {
          if (attributeCustomizer.GetIsModifiedData())
          {
            isModified = true;
            break;
          }
        }
      }
      return isModified;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedLocalization()
    {
      return !CurrentLocalization.Compare(InitialLocalization);
    }
    //-------------------------------------------------------------------------
    private static IList<string> GetEditFilterQueryOrderStringList(CxStorableInIdOrderList gridOrder)
    {
      List<string> result = new List<string>();
      foreach (CxAttributeCustomizer attributeCustomizer in gridOrder)
      {
        string attributeId =
          attributeCustomizer.TextMetadata != null && attributeCustomizer.TextMetadata.HyperLinkComposeXml ?
          attributeCustomizer.TextMetadata.Id :
          attributeCustomizer.Id;
        result.Add(attributeId);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    private static IList<string> GetGridOrderStringList(CxStorableInIdOrderList gridOrder)
    {
      List<string> result = new List<string>();
      foreach (CxAttributeCustomizer attributeCustomizer in gridOrder)
      {
        string attributeId =
          attributeCustomizer.TextMetadata != null ?
          attributeCustomizer.TextMetadata.Id :
          attributeCustomizer.Id;
        result.Add(attributeId);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies latest version of the customization to the current metadata.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      base.ApplyToMetadata();
      // Here we compare the customized order to the one defined now in the real metadata.
      // Grid Visible Order
      CxAttributeOrder orderGrid = Metadata.GetAttributeOrder(NxAttributeContext.GridVisible);
      IList<string> orderGridStrings = GetGridOrderStringList(CurrentData.GridVisibleOrder);
      if (!CxList.CompareOrdered(orderGridStrings, orderGrid.XmlOrderIds))
      {
        orderGrid.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(orderGridStrings));
      }
      else
      {
        orderGrid.SetCustomOrder(null);
      }

      // Edit Order
      CxAttributeOrder orderEdit = Metadata.GetAttributeOrder(NxAttributeContext.Edit);
      IList<string> orderEditStrings = GetEditFilterQueryOrderStringList(CurrentData.EditOrder);
      if (!CxList.CompareOrdered(
            orderEditStrings,
            orderEdit.XmlOrderIds))
      {
        orderEdit.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(orderEditStrings));
      }
      else
      {
        orderEdit.SetCustomOrder(null);
      }

      // Filter Order
      CxAttributeOrder orderFilter = Metadata.GetAttributeOrder(NxAttributeContext.Filter);
      IList<string> orderFilterStrings = GetEditFilterQueryOrderStringList(CurrentData.FilterOrder);
      if (!CxList.CompareOrdered(
            orderFilterStrings,
            orderFilter.XmlOrderIds))
      {
        orderFilter.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(orderFilterStrings));
      }
      else
      {
        orderFilter.SetCustomOrder(null);
      }

      // Query Order
      CxAttributeOrder orderQuery = Metadata.GetAttributeOrder(NxAttributeContext.Queryable);
      IList<string> orderQueryStrings = GetEditFilterQueryOrderStringList(CurrentData.QueryOrder);
      if (!CxList.CompareOrdered(
            orderQueryStrings,
            orderQuery.XmlOrderIds))
      {
        orderQuery.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(orderQueryStrings));
      }
      else
      {
        orderQuery.SetCustomOrder(null);
      }

      // Child Entity Usage Order
      
      // 1. Main form
      CxChildEntityUsageOrder orderChildEntityUsage_InList = Metadata.GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InList);
      if (!CxList.CompareOrdered(
            CurrentData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList).ToStringList(),
            orderChildEntityUsage_InList.XmlOrderIds))
      {
        orderChildEntityUsage_InList.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(CurrentData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList).ToStringList()));
      }
      else
      {
        orderChildEntityUsage_InList.SetCustomOrder(null);
      }
      // 2. Edit form
      CxChildEntityUsageOrder orderChildEntityUsage_InView = Metadata.GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InView);
      if (!CxList.CompareOrdered(
            CurrentData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView).ToStringList(),
            orderChildEntityUsage_InView.XmlOrderIds))
      {
        orderChildEntityUsage_InView.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(CurrentData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView).ToStringList()));
      }
      else
      {
        orderChildEntityUsage_InView.SetCustomOrder(null);
      }



      foreach (CxAttributeCustomizer attributeCustomizer in AttributeCustomizers)
      {
        attributeCustomizer.ApplyToMetadata();
      }

      if (Metadata.Visible != CurrentData.Visible)
      {
        Metadata.Visible = CurrentData.Visible;
        Metadata.ApplyVisibilityToInheritedEntityUsages();
      }

      if (Metadata.IsHiddenForUser != (!CurrentData.VisibleToAdministrator))
      {
        Metadata.IsHiddenForUser = !CurrentData.VisibleToAdministrator;
      }


      CxMultilanguage multilanguage = Context.Holder.Multilanguage;
      string localizationObjectName = Metadata.Id;
      if (CurrentLocalization.LanguageSingleCaptionMap.ContainsKey(multilanguage.LanguageCode))
      {
        multilanguage.SetLocalizedValueInMemory(
          multilanguage.LanguageCode,
          Metadata.LocalizationObjectTypeCode,
          "single_caption",
          localizationObjectName,
          CurrentLocalization.LanguageSingleCaptionMap[multilanguage.LanguageCode]);
      }
      if (CurrentLocalization.LanguagePluralCaptionMap.ContainsKey(Metadata.Holder.LanguageCode))
      {
        multilanguage.SetLocalizedValueInMemory(
          multilanguage.LanguageCode,
          Metadata.LocalizationObjectTypeCode,
          "plural_caption",
          localizationObjectName,
          CurrentLocalization.LanguagePluralCaptionMap[multilanguage.LanguageCode]);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets customizer data to defaults.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();

      // Grid Visible Order
      CxAttributeOrder order = Metadata.GetAttributeOrder(NxAttributeContext.GridVisible);
      order.ResetToDefault();
      CurrentData.GridVisibleOrder.Clear();
      CxList.AddRange(
        CurrentData.GridVisibleOrder, AttributeCustomizers.GetSublistBy(order.OrderAttributes));

      // Edit Order
      order = Metadata.GetAttributeOrder(NxAttributeContext.Edit);
      order.ResetToDefault();
      CurrentData.EditOrder.Clear();
      CxList.AddRange(
        CurrentData.EditOrder, AttributeCustomizers.GetSublistBy(order.OrderAttributes));

      // Filter Order
      order = Metadata.GetAttributeOrder(NxAttributeContext.Filter);
      order.ResetToDefault();
      CurrentData.FilterOrder.Clear();
      CxList.AddRange(
        CurrentData.FilterOrder, AttributeCustomizers.GetSublistBy(order.OrderAttributes));

      // Query Order
      order = Metadata.GetAttributeOrder(NxAttributeContext.Queryable);
      order.ResetToDefault();
      CurrentData.QueryOrder.Clear();
      CxList.AddRange(
        CurrentData.QueryOrder, AttributeCustomizers.GetSublistBy(order.OrderAttributes));

      // Secondary Tabs Order
      ResetToDefaultChildOrder(NxChildEntityUsageOrderType.InList);
      ResetToDefaultChildOrder(NxChildEntityUsageOrderType.InView);

      CurrentData.Visible = CxBool.Parse(Metadata.GetInitialProperty("visible", false), true);
      if (Context.Holder.IsDevelopmentMode)
        CurrentData.VisibleToAdministrator = !CxBool.Parse(Metadata.GetInitialProperty("hidden_for_user", false), false);

      // Reset all the child customizers.
      foreach (CxAttributeCustomizer attributeCustomizer in AttributeCustomizers)
      {
        attributeCustomizer.ResetToDefault();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the given type of the children order.
    /// </summary>
    private void ResetToDefaultChildOrder(NxChildEntityUsageOrderType orderType)
    {
      CxChildEntityUsageOrder childEntityUsageOrder = Metadata.GetChildEntityUsageOrder(orderType);
      childEntityUsageOrder.ResetToDefault();
      CurrentData.InitOrder_SecondaryTabs(orderType);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the customizer.
    /// </summary>
    public void Initialize()
    {
      InitializeAttributeCustomizers();
      InitializeChildEntityCustomizers();

      // Initializing the Current Data
      CurrentData = new CxEntityCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      // Setting the current data as the initial one.
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxEntityCustomizerLocalization(this);
      InitialLocalization = CurrentLocalization.Clone();
    }
    //-------------------------------------------------------------------------
    protected void InitializeChildEntityCustomizers()
    {
      ChildEntityCustomizers.Clear();
      foreach (CxChildEntityUsageMetadata childEntityUsageMetadata in Metadata.ChildEntityUsagesList)
      {
        ChildEntityCustomizers.Add(
          new CxChildEntityCustomizer(Manager, this, childEntityUsageMetadata));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the attribute customizers.
    /// </summary>
    protected void InitializeAttributeCustomizers()
    {
      AttributeCustomizers.Clear();
      //List<string> blackList = new List<string>();
      
      foreach (CxAttributeMetadata attribute in Metadata.GetAttributesWithoutText())
      {
        //CxAttributeMetadata valueAttribute = Metadata.GetValueAttribute(attribute) ?? attribute;
        
        //CxAttributeMetadata textAttribute = null;
        //if (valueAttribute == attribute)
        //    textAttribute = Metadata.GetTextAttribute(attribute);

        //if (!blackList.Contains(valueAttribute.Id))
        //{
          CxAttributeCustomizer attributeCustomizer =
            new CxAttributeCustomizer(this, attribute.Id);
          AttributeCustomizers.Add(attributeCustomizer);
        //}

        //if (attribute != valueAttribute)
        //  blackList.Add(attribute.Id);
        //if (valueAttribute != null)
        //  blackList.Add(valueAttribute.Id);
        //if (textAttribute != null)
        //  blackList.Add(textAttribute.Id);

      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    ///                     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///                     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      return Metadata != null ? Metadata.ToString() : base.ToString();
    }
    //-------------------------------------------------------------------------

    #region Database related
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider for a data operation.
    /// </summary>
    /// <returns></returns>
    protected override IxValueProvider GetValueProvider()
    {
      CxHashtable provider = new CxHashtable();

      provider["ApplicationCd"] = Metadata.Holder.ApplicationCode;
      provider["MetadataObjectId"] = Metadata.Id;
      provider["MetadataObjectTypeCd"] = "entity_usage";

      CxXmlRenderedObject renderedObject = Metadata.RenderToXml(new XmlDocument(), true, null);
      provider["MetadataContent"] = renderedObject.ToString();
      return provider;
    }
    //-------------------------------------------------------------------------
    #endregion
    
    //-------------------------------------------------------------------------
    public void InitializeForLanguage(string languageCd)
    {
      CurrentLocalization.InitializeForLanguage(languageCd);
      InitialLocalization.InitializeForLanguage(languageCd);
    }
    //-------------------------------------------------------------------------

  }
}