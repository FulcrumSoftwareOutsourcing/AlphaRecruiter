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

namespace Framework.Metadata
{
  public class CxCustomizationUtils
  {
    //-------------------------------------------------------------------------
    public static bool CompareLanguageDictionaries(
      Dictionary<string, string> dictionary1,
      Dictionary<string, string> dictionary2)
    {
      bool result = dictionary1.Keys.Count == dictionary2.Keys.Count;
      if (result)
      {
        foreach (KeyValuePair<string, string> pair in dictionary1)
        {
          result =
            dictionary2.ContainsKey(pair.Key) &&
            string.Equals(pair.Value, dictionary2[pair.Key], StringComparison.InvariantCulture);
          if (!result)
            break;
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    public static bool CompareLanguageDictionaries(
      Dictionary<CxEntityUsageMetadata, Dictionary<string, string>> dictionary1,
      Dictionary<CxEntityUsageMetadata, Dictionary<string, string>> dictionary2)
    {
      foreach (KeyValuePair<CxEntityUsageMetadata, Dictionary<string, string>> pair in dictionary1)
      {
        if (!dictionary2.ContainsKey(pair.Key))
        {
          if ((pair.Value.Count == 0))
            continue;
          else
            return false;
        }
        if (!CompareLanguageDictionaries(dictionary2[pair.Key], pair.Value))
          return false;
      }
      return true;
    }
    //-------------------------------------------------------------------------
  }
}
