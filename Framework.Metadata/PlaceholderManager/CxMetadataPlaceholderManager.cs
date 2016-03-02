using System;
using System.Collections.Generic;
using System.Text;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxMetadataPlaceholderManager : CxPlaceholderManagerBase
  {
    //-------------------------------------------------------------------------
    private CxMetadataHolder m_Holder;
    //-------------------------------------------------------------------------
    public CxMetadataHolder Holder
    {
      get { return m_Holder; }
      set { m_Holder = value; }
    }
    //-------------------------------------------------------------------------
    public CxMetadataPlaceholderManager(CxMetadataHolder holder)
    {
      Holder = holder;
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
      // Process the entity property reference.
      if (placeholder.StartsWith("[") && placeholder.EndsWith("]"))
      {
        string entityReference = placeholder.Substring(1, placeholder.Length - 2);
        string[] partsOfReference = entityReference.Split('.');
        if (partsOfReference.Length == 2)
        {
          return ProcessEntityUsage(partsOfReference[0], partsOfReference[1], languageCd);
        }
        if (partsOfReference.Length == 3)
        {
          return ProcessAttribute(partsOfReference[0], partsOfReference[1], partsOfReference[2], languageCd);
        }
        throw new InvalidOperationException(string.Format("There should be 2 or 3 parts of entity reference, <{0}>", placeholder));
      }
      return base.ProcessPlaceholder(placeholder, languageCd);
    }
    //-------------------------------------------------------------------------
    private string ProcessAttribute(string entityUsageId, string attributeId, string propertyName, string languageCd)
    {
      CxEntityUsageMetadata entityUsage = Holder.EntityUsages.Find(entityUsageId.ToUpper());
      if (entityUsage == null)
        throw new ExException(string.Format("Entity Usage <{0}> has not been found", entityUsageId));

      CxAttributeMetadata attribute = entityUsage.GetAttribute(attributeId);
      if (attribute == null)
        throw new ExException(string.Format("Attribute <{0}> has not been found in Entity Usage <{1}>", attributeId, entityUsageId));

      string propertyValue;
      if (CxUtils.NotEmpty(languageCd))
      {
        string result = attribute.GetNonLocalizedPropertyValue(propertyName);
        string localizedValue = attribute.GetLocalizedPropertyValue(propertyName, result, languageCd);
        if (localizedValue != null)
        {
          result = localizedValue;
        }
        propertyValue = result;
      }
      else
        propertyValue = attribute[propertyName];

      return propertyValue;
    }
    //-------------------------------------------------------------------------
    private string ProcessEntityUsage(string entityUsageId, string propertyName, string languageCd)
    {
      CxEntityUsageMetadata entityUsage = Holder.EntityUsages.Find(entityUsageId.ToUpper());
      if (entityUsage == null)
        throw new ExException(string.Format("Entity Usage <{0}> has not been found", entityUsageId));
      string propertyValue;
      if (CxUtils.NotEmpty(languageCd))
      {
        string result = entityUsage.GetNonLocalizedPropertyValue(propertyName);
        string localizedValue = entityUsage.GetLocalizedPropertyValue(propertyName, result, languageCd);
        if (localizedValue != null)
        {
          result = localizedValue;
        }
        propertyValue = result;
      }
      else
        propertyValue = entityUsage[propertyName];

      return propertyValue;
    }
    //-------------------------------------------------------------------------
  }
}
