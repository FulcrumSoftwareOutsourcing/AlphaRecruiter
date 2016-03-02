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

using System.Collections.Generic;
using System.Runtime.Serialization;

using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Container which contains parameters for server queries.
  /// </summary>
  [DataContract]
  public class CxQueryParams
  {
    /// <summary>
    /// Entity Usage Id.
    /// </summary>
    [DataMember]
    public string EntityUsageId;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Join condition values for query.
    /// </summary>
    [DataMember]
    public Dictionary<string, object> JoinValues;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Where clause values for query.
    /// </summary>
    [DataMember]
    public Dictionary<string, object> WhereValues;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Primary keys values for query.
    /// </summary>
    [DataMember]
    public Dictionary<string, object> PrimaryKeysValues;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Filter values values for query.
    /// </summary>
    [DataMember]
    public CxFilterItem[] FilterItems;
    //----------------------------------------------------------------------------
    /// <summary>
    /// The fields to sort by.
    /// </summary>
    [DataMember]
    public CxSortDescription[] SortDescriptions;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Values of current entity.
    /// </summary>
    [DataMember]
    public Dictionary<string, object> EntityValues;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Parent entity primary keys.
    /// </summary>
    [DataMember]
    public Dictionary<string, object> ParentPks;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Parent entity usage id.
    /// </summary>
    [DataMember]
    public string ParentEntityUsageId;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute Id that had changed.
    /// </summary>
    [DataMember]
    public string ChangedAttributeId { get; set; }
    //----------------------------------------------------------------------------

    /// <summary>
    /// The starting index of the record to get.
    /// </summary>
    [DataMember]
    public int StartRecordIndex { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The max amount of the records to get (page size, in our circumstances).
    /// </summary>
    [DataMember]
    public int RecordsAmount { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Defines server method that will be used to get model.
    /// (Workaround. Because WCF with Silverligh do not supports Enums.)
    /// </summary>
    [DataMember]
    public string QueryType = CxQueryTypes.ENTITY_LIST;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity open mode
    /// </summary>
    [DataMember]
    public string OpenMode{ get; set;}

  
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

  //----------------------------------------------------------------------------
  /// <summary>
  /// Contains constants to define server method that will be used to get model.
  /// (Workaround. Because WCF with Silverligh do not supports Enums.)
  /// </summary>
  public static class CxQueryTypes
  {
    /// <summary>
    /// GetEntityList mthod. 
    /// </summary>
    public static string ENTITY_LIST = "EntityList";
    //----------------------------------------------------------------------------

    /// <summary>
    /// GetChildEntityList method.
    /// </summary>
    public static string CHILD_ENTITY_LIST = "ChildEntityList";

    //----------------------------------------------------------------------------
    /// <summary>
    /// Get EntityFromPk method.
    /// </summary>
    public static string ENTITY_FROM_PK = "EntityFromPk";

    //----------------------------------------------------------------------------
    /// <summary>
    /// Redirect posted Entity back.
    /// </summary>
    public static string DIRECT_BACK_ENTITY = "DirectBackEntity";

  }
}
