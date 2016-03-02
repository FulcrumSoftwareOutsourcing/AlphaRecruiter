using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Utils;

namespace Framework.Remote.Mobile
{
  public partial class CxQueryParams
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of IxValueProvider that contains values for query.
    /// </summary>
    /// <param name="names">List of parameters names.</param>
    /// <param name="values">List of parameters values.</param>
    /// <returns>Created instance of IxValueProvider that contains values for query.</returns>
    public static IxValueProvider CreateValueProvider(Dictionary<string, object> whereValues)
    {
      CxHashtable provider = new CxHashtable();
      foreach (KeyValuePair<string, object> wherePair in whereValues)
      {
        provider.Add(wherePair.Key, wherePair.Value);
      }
      return provider;
    }

    //----------------------------------------------------------------------------       
    /// <summary>
    /// Creates instance of IxValueProvider that contains values for query.
    /// </summary>
    /// <param name="entityValues"></param>
    /// <returns></returns>
    public static IxValueProvider CreateValueProvider(IDictionary<string, object> entityValues)
    {
      CxHashtable provider = new CxHashtable();
      foreach (KeyValuePair<string, object> pair in entityValues)
      {
        provider.Add(pair.Key, pair.Value);
      }
      return provider;
    }
  }
}
