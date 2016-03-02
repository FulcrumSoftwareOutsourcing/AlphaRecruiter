using System;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  public abstract class CxCustomizerBase: IxCustomizer
  {
    private IxCustomizationContext m_Context;
    private CxCustomizationManager m_Manager;
    //-------------------------------------------------------------------------
    public abstract string Id { get; }
    //-------------------------------------------------------------------------
    public IxCustomizationContext Context
    {
      get { return m_Context; }
      set { m_Context = value; }
    }
    //-------------------------------------------------------------------------
    public CxCustomizationManager Manager
    {
      get { return m_Manager; }
      set { m_Manager = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="manager">customization manager the customizer belongs to</param>
    protected CxCustomizerBase(CxCustomizationManager manager)
    {
      Manager = manager;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies latest version of the customization to the current metadata.
    /// </summary>
    public virtual bool ApplyToMetadata()
    {
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets customizer data to defaults.
    /// </summary>
    public virtual void ResetToDefault()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider for a data operation.
    /// </summary>
    protected virtual IxValueProvider GetValueProvider()
    {
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts customization record to the table.
    /// </summary>
    protected void DbInsertOrUpdate(CxDbConnection connection)
    {
      string sql =
        @"exec [p_Framework_MetadataCustomization_InsertOrUpdate]
            @MetadataObjectId 						 = :MetadataObjectId 			, 
            @ApplicationCd 				    		 = :ApplicationCd 			  , 
            @MetadataObjectTypeCd  				 = :MetadataObjectTypeCd  ,
            @MetadataContent               = :MetadataContent       ";
      IxValueProvider provider = GetValueProvider();
      connection.ExecuteCommand(sql, provider);
    }
    //-------------------------------------------------------------------------
  }
}
