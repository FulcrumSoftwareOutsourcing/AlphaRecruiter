using System;
using System.Collections.Generic;
using System.Xml;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinSectionsCustomizer : CxCustomizerBase
  {
    //-------------------------------------------------------------------------
    private CxWinSectionsMetadata m_Metadata;
    private IList<CxWinSectionCustomizer> m_SectionCustomizersList;
    private IDictionary<string, CxWinSectionCustomizer> m_SectionCustomizers;
    private CxWinSectionsCustomizerData m_CurrentData;
    private CxWinSectionsCustomizerData m_InitialData;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public override string Id
    {
      get { return Metadata != null ? Metadata.ToString(): string.Empty; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current data snapshot, non-saved data.
    /// </summary>
    public CxWinSectionsCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initial data snapshot, the data which is actually applied to the application.
    /// </summary>
    public CxWinSectionsCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxWinSectionsMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    public IList<CxWinSectionCustomizer> SectionCustomizersList
    {
      get { return m_SectionCustomizersList; }
      set { m_SectionCustomizersList = value; }
    }
    //-------------------------------------------------------------------------
    public IDictionary<string, CxWinSectionCustomizer> SectionCustomizers
    {
      get { return m_SectionCustomizers; }
      set { m_SectionCustomizers = value; }
    }
    //-------------------------------------------------------------------------

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxWinSectionsCustomizer(CxCustomizationManager manager, CxWinSectionsMetadata metadata, IxCustomizationContext context)
      : base(manager)
    {
      Metadata = metadata;
      Context = context;

      SectionCustomizersList = new List<CxWinSectionCustomizer>();
      SectionCustomizers = new Dictionary<string, CxWinSectionCustomizer>();

      Initialize();
    }
    //-------------------------------------------------------------------------
    #endregion

    //-------------------------------------------------------------------------
    /// <summary>
    /// Initialize sections customizer.
    /// </summary>
    private void Initialize()
    {
      InitializeSectionCustomizers();

      // Initializing the Current Data
      CurrentData = new CxWinSectionsCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      // Setting the current data as the initial one.
      InitialData = CurrentData.Clone();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initialize list of section customizer.
    /// </summary>
    private void InitializeSectionCustomizers()
    {
      SectionCustomizers.Clear();

      foreach (CxWinSectionMetadata section in Metadata.AllItemsList)
      {
        CxWinSectionCustomizer sectionCustomizer = 
          new CxWinSectionCustomizer(this, section);
        SectionCustomizersList.Add(sectionCustomizer);
        SectionCustomizers.Add(section.Id, sectionCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets customizer data to defaults.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();

      CxWinSectionOrder order = Metadata.WinSectionOrder;
      order.ResetToDefault();
      CurrentData.VisibleOrder.Clear();
      foreach (string id in order.OrderIds)
      {
        CurrentData.VisibleOrder.Add(SectionCustomizers[id]);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies latest version of the customization to the current metadata.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      base.ApplyToMetadata();

      CxWinSectionOrder orderSection = Metadata.WinSectionOrder;
      if (!CxList.CompareOrdered(
            CurrentData.VisibleOrder.ToStringList(),
            orderSection.XmlOrderIds))
      {
        orderSection.SetCustomOrder(
          CxText.ComposeCommaSeparatedString(CurrentData.VisibleOrder.ToStringList()));
      }
      else
      {
        orderSection.SetCustomOrder(null);
      }

      foreach (CxWinSectionCustomizer sectionCustomizer in SectionCustomizersList)
      {
        sectionCustomizer.ApplyToMetadata();
      }

      return true;
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
        DbInsertOrUpdate(connection);

        foreach (CxWinSectionCustomizer sectionCustomizer in SectionCustomizersList)
        {
          sectionCustomizer.SaveData(connection);
        }

        InitialData = CurrentData.Clone();
      }
    }
    //-------------------------------------------------------------------------
    private void SaveLocalization(CxDbConnection connection)
    {
      foreach (CxWinSectionCustomizer sectionCustomizer in SectionCustomizersList)
      {
        sectionCustomizer.SaveLocalization(connection);
      }
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
        foreach (CxWinSectionCustomizer sectionCustomizer in SectionCustomizersList)
        {
          if (sectionCustomizer.GetIsModifiedData())
          {
            isModified = true;
            break;
          }
        }
      }
      return isModified;
    }
    //-------------------------------------------------------------------------
    public void InitializeForLanguage(string languageCd)
    {
      foreach (CxWinSectionCustomizer sectionCustomizer in SectionCustomizersList)
      {
        sectionCustomizer.CurrentLocalization.InitializeForLanguage(languageCd);
        sectionCustomizer.InitialLocalization.InitializeForLanguage(languageCd);
      }
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
      provider["MetadataObjectId"] = "singleton";
      provider["MetadataObjectTypeCd"] = "win_sections";

      CxXmlRenderedObject renderedObject = Metadata.RenderToXml(new XmlDocument(), true, null);
      provider["MetadataContent"] = renderedObject.ToString();
      return provider;
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}
