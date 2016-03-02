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

using System.Collections;
using System.Text.RegularExpressions;

namespace Framework.Utils
{
	/// <summary>
	/// Parses words from query.
	/// </summary>
	public class CxWordParser
	{
	  private const string FindWordRegularExpression = @"\W*?(\w{2,}|\d+)\W*?";
	  
		private CxWordParser()
		{
		}
	  
    //-------------------------------------------------------------------------
    /// <summary>
    /// splits value and adds it to result if word is not an ignored one
    /// </summary>
    public static string[] SplitValue(string valueToSplit, IDictionary ignoreWords)
    {
      ArrayList values = new ArrayList();
      Match match = Regex.Match(valueToSplit, FindWordRegularExpression);

      while (match != null && match.Success)
      {
        string word = match.Groups[1].Value.ToLower();
        if (ignoreWords == null || (ignoreWords != null && !ignoreWords.Contains(word)))
        {
          values.Add(word);
        }
        match = match.NextMatch();
      }

      string[] result = new string[values.Count];
      values.CopyTo(result);
      return result;
    }
	}
}
