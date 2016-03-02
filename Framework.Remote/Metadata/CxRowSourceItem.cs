/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System.Runtime.Serialization;

namespace Framework.Remote
{
  [DataContract]
  public class CxRowSourceItem
  {
    //-------------------------------------------------------------------------
    [DataMember]
    public object Key { get; set; }
    //-------------------------------------------------------------------------
    [DataMember]
    public string Text { get; set; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="key">a key to initialize with</param>
    /// <param name="text">a text value that corresponds to the key</param>
    public CxRowSourceItem(object key, string text)
    {
      Key = key;
      Text = text;
    }
    //-------------------------------------------------------------------------
  }
}
