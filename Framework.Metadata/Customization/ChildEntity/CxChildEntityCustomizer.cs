using System;

namespace Framework.Metadata
{
  public class CxChildEntityCustomizer : CxCustomizerBase, IxStorableInIdOrder
  {
    //-------------------------------------------------------------------------
    private CxChildEntityUsageMetadata m_ChildMetadata;
    private CxEntityCustomizer m_ParentCustomizer;
    private CxEntityCustomizer m_ChildCustomizer;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    public CxChildEntityCustomizer(
      CxCustomizationManager manager,
      CxEntityCustomizer parentCustomizer, CxChildEntityUsageMetadata childMetadata)
      : base(manager)
    {
      ParentCustomizer = parentCustomizer;
      ChildMetadata = childMetadata;
      Visible = childMetadata != null ? childMetadata.Visible : false;
      VisibleToAdministrator = childMetadata != null ? !childMetadata.IsHiddenForUser : false;
      EntityCustomizer = new CxEntityCustomizer(manager, childMetadata.EntityUsage);
    }
    //-------------------------------------------------------------------------

    #region Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxChildEntityUsageMetadata ChildMetadata
    {
      get { return m_ChildMetadata; }
      protected set { m_ChildMetadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent customizer.
    /// </summary>
    public CxEntityCustomizer ParentCustomizer
    {
      get { return m_ParentCustomizer; }
      set { m_ParentCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityCustomizer EntityCustomizer
    {
      get { return m_ChildCustomizer; }
      set { m_ChildCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    public override string Id
    {
      get
      {
        if (ChildMetadata != null)
          return ChildMetadata.Id;
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer is visible to the end-user.
    /// This type of visibility is not storable for now. Change the comment upon implementing.
    /// </summary>
    public bool Visible { get; protected set; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer is visible to the administrator.
    /// This type of visibility is not storable for now. Change the comment upon implementing.
    /// </summary>
    public bool VisibleToAdministrator { get; protected set; }
    //-------------------------------------------------------------------------
    #endregion

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      return Id ?? base.ToString();
    }
    //-------------------------------------------------------------------------
  }
}
