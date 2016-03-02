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
using Framework.Db;

namespace Framework.Metadata
{
  public class CxCustomizationManager
  {
    //-------------------------------------------------------------------------
    private IxCustomizationContext m_Context;
    private Dictionary<CxEntityUsageMetadata, CxEntityCustomizer> m_EntityCustomizerMap =
      new Dictionary<CxEntityUsageMetadata, CxEntityCustomizer>();

    private Dictionary<CxRowSourceMetadata, CxLookupCustomizer> m_LookupCustomizerMap =
      new Dictionary<CxRowSourceMetadata, CxLookupCustomizer>();

    private Dictionary<CxWinFormMetadata, CxFormCustomizer> m_FormCustomizerMap =
      new Dictionary<CxWinFormMetadata, CxFormCustomizer>();

    private CxWinSectionsCustomizer m_SectionsCustomizer;
    //-------------------------------------------------------------------------
    public IxCustomizationContext Context
    {
      get { return m_Context; }
      set { m_Context = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<CxEntityUsageMetadata, CxEntityCustomizer> EntityCustomizerMap
    {
      get { return m_EntityCustomizerMap; }
      set { m_EntityCustomizerMap = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<CxRowSourceMetadata, CxLookupCustomizer> LookupCustomizerMap
    {
      get { return m_LookupCustomizerMap; }
      set { m_LookupCustomizerMap = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<CxWinFormMetadata, CxFormCustomizer> FormCustomizerMap
    {
      get { return m_FormCustomizerMap; }
      set { m_FormCustomizerMap = value; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionsCustomizer SectionsCustomizer
    {
      get { return m_SectionsCustomizer; }
      set { m_SectionsCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    public CxCustomizationManager(IxCustomizationContext context)
    {
      Context = context;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets a customizer by the given entity usage.
    /// </summary>
    /// <returns>a customizer found</returns>
    public CxEntityCustomizer GetEntityCustomizer(CxEntityUsageMetadata entity)
    {
      if (EntityCustomizerMap.ContainsKey(entity))
        return EntityCustomizerMap[entity];
      else
        return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets a customizer by the given form metadata.
    /// </summary>
    /// <returns>a customizer found</returns>
    public CxFormCustomizer GetFormCustomizer(CxWinFormMetadata form)
    {
      if (FormCustomizerMap.ContainsKey(form))
        return FormCustomizerMap[form];
      else
        return null;
    }
    //-------------------------------------------------------------------------
    public void ApplyToMetadata()
    {
      foreach (CxEntityCustomizer entityUsageCustomizer in EntityCustomizerMap.Values)
      {
        entityUsageCustomizer.ApplyToMetadata();
      }
      foreach (CxLookupCustomizer lookupCustomizer in LookupCustomizerMap.Values)
      {
        lookupCustomizer.ApplyToMetadata();
      }
      foreach (CxFormCustomizer formCustomizer in FormCustomizerMap.Values)
      {
        formCustomizer.ApplyToMetadata();
      }
      SectionsCustomizer.ApplyToMetadata();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Registers an entity customizer.
    /// </summary>
    public void RegisterEntityCustomizer(
      CxEntityCustomizer customizer)
    {
      EntityCustomizerMap[customizer.Metadata] = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Registers a lookup customizer.
    /// </summary>
    public void RegisterLookupCustomizer(
      CxLookupCustomizer customizer)
    {
      LookupCustomizerMap[customizer.Metadata] = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Registers a form customizer.
    /// </summary>
    public void RegisterFormCustomizer(
      CxFormCustomizer customizer)
    {
      FormCustomizerMap[customizer.Metadata] = customizer;
    }
    //-------------------------------------------------------------------------
    public void Save(CxDbConnection connection)
    {
      foreach (CxEntityCustomizer entityCustomizer in EntityCustomizerMap.Values)
      {
        entityCustomizer.Save(connection);
      }

      foreach (CxLookupCustomizer lookupCustomizer in LookupCustomizerMap.Values)
      {
        lookupCustomizer.Save(connection);
      }

      foreach (CxFormCustomizer formCustomizer in FormCustomizerMap.Values)
      {
        formCustomizer.Save(connection);
      }
      SectionsCustomizer.Save(connection);
    }
    //-------------------------------------------------------------------------
    public void InitializeForLanguage(string languageCd)
    {
      foreach (CxEntityCustomizer entityCustomizer in EntityCustomizerMap.Values)
      {
        entityCustomizer.InitializeForLanguage(languageCd);
      }
      SectionsCustomizer.InitializeForLanguage(languageCd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether something has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModified(CxEntityCustomizer entityCustomizer)
    {
      if (entityCustomizer != null)
      {
        if(entityCustomizer.GetIsModifiedData() || entityCustomizer.GetIsModifiedLocalization())
          return true;

        if (entityCustomizer.FormCustomizer != null && entityCustomizer.FormCustomizer.GetIsModified())
          return true;
      }
      
      return false;
    }
    //-------------------------------------------------------------------------
  }
}
