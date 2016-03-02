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
using System.Xml;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  public class CxOptionGroupLocalizer : IxMultilanguageItemProvider
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of multilanguage items.
    /// </summary>
    public CxMultilanguageItem[] GetMultilanguageItems()
    {
      List<CxMultilanguageItem> items = new List<CxMultilanguageItem>();
      XmlDocument doc = GetOptionGroupXml();
      if (doc != null && doc.DocumentElement != null)
      {
        AppendMutlilanguageItems(items, doc.DocumentElement);
      }
      return items.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates options class instance.
    /// </summary>
    /// <returns></returns>
    virtual protected CxOptions CreateOptions()
    {
      return new CxOptions();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns option group XML document.
    /// </summary>
    /// <returns></returns>
    protected XmlDocument GetOptionGroupXml()
    {
      CxOptions options = CreateOptions();
      return options != null ? options.GetOptionGroupsXml() : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends multilanguage items from the option groups XML element.
    /// </summary>
    /// <param name="items">list to append</param>
    /// <param name="element">XML element to append from</param>
    protected void AppendMutlilanguageItems(List<CxMultilanguageItem> items, XmlElement element)
    {
      foreach (XmlElement groupElement in element.SelectNodes("group"))
      {
        CxMultilanguageItem item = new CxMultilanguageItem(
          CxMultilanguage.OTC_TEXT,
          CxMultilanguage.PC_TEXT,
          "OptionGroup." + CxXml.GetAttr(groupElement, "id"),
          CxXml.GetAttr(groupElement, "text"));
        items.Add(item);
        AppendMutlilanguageItems(items, groupElement);
      }
    }
    //-------------------------------------------------------------------------
  }
}