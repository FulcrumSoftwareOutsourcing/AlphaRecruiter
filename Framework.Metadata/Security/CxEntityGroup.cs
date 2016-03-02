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
using System.Collections;
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Defines entity group.
  /// </summary>
  public class CxEntityGroup : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected CxEntityGroupCondition m_Condition = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxEntityGroup(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
      XmlElement conditionElement = (XmlElement) element.SelectSingleNode("condition");
      if (conditionElement != null)
      {
        m_Condition = new CxEntityGroupCondition(Holder, conditionElement);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity group is applicable to the given metadata object.
    /// </summary>
    /// <param name="metadataObject">metadata object to check</param>
    public bool GetIsMatch(CxMetadataObject metadataObject)
    {
      if (m_Condition != null)
      {
        foreach (string propertyName in m_Condition.PropertyNames)
        {
          if (m_Condition[propertyName].ToLower() != metadataObject[propertyName].ToLower())
          {
            return false;
          }
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
  }
}