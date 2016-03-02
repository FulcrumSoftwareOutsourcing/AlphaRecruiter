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
using System.Text;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
	/// <summary>
	/// Exception to raise when validation failed on emty mandatory column.
	/// </summary>
	public class ExMandatoryViolationException : ExValidationException
	{
    //-------------------------------------------------------------------------
    protected CxBaseEntity m_Entity = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entity">entity the validation failed of</param>
    /// <param name="propertyName">name of the property that caused exception</param>
    public ExMandatoryViolationException(CxBaseEntity entity, string propertyName) 
      : base(ComposeMessage(entity, propertyName), propertyName)
		{
      m_Entity = entity;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entity">entity the validation failed of</param>
    /// <param name="propertyNames">names of the properties that caused exception</param>
    public ExMandatoryViolationException(CxBaseEntity entity, string[] propertyNames)
      : base(ComposeMessage(entity, propertyNames), propertyNames)
    {
      m_Entity = entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes error message.
    /// </summary>
    /// <param name="entity">entity the validation failed of</param>
    /// <param name="propertyName">name of the property that caused exception</param>
    /// <returns>error message</returns>
    static protected string ComposeMessage(CxBaseEntity entity, string propertyName)
    {
      string caption = CxUtils.Nvl(entity.GetAttributeCaption(propertyName), propertyName);
      return entity.Metadata.Holder.GetErr("Field \"{0}\" could not be empty.", new object[]{caption});
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes error message.
    /// </summary>
    /// <param name="entity">entity the validation failed of</param>
    /// <param name="propertyNames">names of the properties that caused exception</param>
    /// <returns>error message</returns>
    static protected string ComposeMessage(CxBaseEntity entity, string[] propertyNames)
    {
      List<string> fieldIds = new List<string>(propertyNames);

      if (fieldIds.Count > 0)
      {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < fieldIds.Count; i++)
        {
          if (i > 0)
            sb.Append(", ");
          string fieldId = fieldIds[i];
          string fieldName = null;
          CxAttributeMetadata attribute = entity.Metadata.GetAttribute(fieldId);
          if (attribute != null)
            fieldName = attribute.GetCaption(entity.Metadata);
          if (string.IsNullOrEmpty(fieldName))
            fieldName = fieldId;
          sb.AppendFormat("\"{0}\"", fieldName);
        }
        return entity.Metadata.Holder.GetErr("These fields must be non-empty: {0}", new object[] {sb.ToString()});
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity caused the exception.
    /// </summary>
    public CxBaseEntity Entity
    { get { return m_Entity; } }
    //-------------------------------------------------------------------------
  }
}