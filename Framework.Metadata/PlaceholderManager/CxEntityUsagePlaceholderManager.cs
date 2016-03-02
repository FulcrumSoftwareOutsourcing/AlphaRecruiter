using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Metadata
{
  public class CxEntityUsagePlaceholderManager: CxMetadataPlaceholderManager
  {
    //-------------------------------------------------------------------------
    private CxEntityUsageMetadata m_EntityUsage;
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata EntityUsage
    {
      get { return m_EntityUsage; }
      set { m_EntityUsage = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityUsagePlaceholderManager(CxEntityUsageMetadata entityUsage)
      : base(entityUsage.Holder)
    {
      EntityUsage = entityUsage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes the given placeholder. Returns null if no procession was done - placeholder was not recognized.
    /// </summary>
    /// <param name="placeholder">placeholder</param>
    /// <param name="languageCd">language code to export (current if empty)</param>
    /// <returns>value to replace placeholder with; null if the placeholder was invalid</returns>
    protected override string ProcessPlaceholder(string placeholder, string languageCd)
    {
      if (EntityUsage.PropertyValues.ContainsKey(placeholder))
        return EntityUsage[placeholder];
      return base.ProcessPlaceholder(placeholder, languageCd);
    }
    //-------------------------------------------------------------------------
  }
}
