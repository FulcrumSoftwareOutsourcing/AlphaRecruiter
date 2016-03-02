function QueryParams()
{

    this.EntityUsageId;


    this.JoinValues = {};


    this.WhereValues = {};


    this.PrimaryKeysValues = {};


    this.FilterItems = [];
    
    this.SortDescriptions = [];
    
    this.EntityValues = {};

    
    this.ParentPks = {};

    
    this.ParentEntityUsageId ;

    
    this.ChangedAttributeId;
    
    this.StartRecordIndex;
    
    this.RecordsAmount;
    
    this.QueryType = QueryTypes.ENTITY_LIST;
    
    this.OpenMode;
};

    ////----------------------------------------------------------------------------
    ///// <summary>
    ///// Contains constants to define server method that will be used to get model.
    ///// (Workaround. Because WCF with Silverligh do not supports Enums.)
    ///// </summary>
    function QueryTypes() { }
    
    //    /// <summary>
    //    /// GetEntityList mthod. 
    //    /// </summary>
    QueryTypes.ENTITY_LIST = "EntityList";
    //    //----------------------------------------------------------------------------

    //    /// <summary>
    //    /// GetChildEntityList method.
    //    /// </summary>
    QueryTypes.CHILD_ENTITY_LIST = "ChildEntityList";

    //    //----------------------------------------------------------------------------
    //    /// <summary>
    //    /// Get EntityFromPk method.
    //    /// </summary>
    QueryTypes.ENTITY_FROM_PK = "EntityFromPk";

    //    //----------------------------------------------------------------------------
    //    /// <summary>
    //    /// Redirect posted Entity back.
    //    /// </summary>
    QueryTypes.DIRECT_BACK_ENTITY = "DirectBackEntity";

    //}
//};