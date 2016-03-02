using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxQueryParams", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxQueryParams
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
    public List<CxFilterItem> FilterItems;
    //----------------------------------------------------------------------------
    /// <summary>
    /// The fields to sort by.
    /// </summary>
    [DataMember]
    public List<CxSortDescription>  SortDescriptions;
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
    public string OpenMode { get; set; }
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
