'use strict';
function BGridFrame()
{
    /// <field name='Commands' type='BCommandBar_ForParent'/>
    /// <field name='PagingPnl' type='BCommandBar_ForParent'/>
    /// <field name='Filters' type='BFilters_ForParent'/>
    /// <field name='DataGrid' type='BDataGrid_ForParent'/>

    BBaseFrame.call(this);

    this.IsGridFrame = true;
   

    this.ViewBorder = new GridFrameViewBorder();
    this.DataBorder = new GridFrameDataBorder();

  

    this.PublicForParent = new BGridFrame_ForParent();
    this.PublicForView = new BGridFrame_ForView();

    this.PublicForChildren = new BGridFrame_ForChild();
    
    var selectedItems = [];
    
    this.Data = null;
    this.DataLoaded = false;
    this.FilterItems = [];
    var dataOnceLoaded = false;
    var applyDefaultFilterLoaded = false;

    var rowsPerPage = 50;
    var startRecordIndex = 0;
    var sortings = [];

    this.InitFrame = function (frameData)
    {
        this.EntityMetadataId = frameData.EntityUsageId;
       
        this.ParentEntity = frameData.ParentEntity;
        this.ParentEntityUsageId = frameData.ParentEntityUsageId;
        this.ParentPrimaryKeys = frameData.ParentPks;
        

        
        this.OpenMode = frameData.OpenMode;
        this.ViewLayer.SetFrameHolderId(frameData.FrameHolderId);
    };

    this.EntityMetadataLoaded = function (data)
    {
        var sortFromSettings = Settings.GetSettings('grid_' + this.EntityMetadataId, 'sorts');
        if (!App.Utils.IsStringNullOrWhitespace(sortFromSettings))
            sortings = JSON.parse(sortFromSettings);


        if (data)
        {
            Metadata.AddEntityUsage(data.Metadata);

            for (var i = 0; i < data.RelatedMetadata.length; i++)
            {
                Metadata.AddEntityUsage(data.RelatedMetadata[i]);
            }
        }

        this.base_Run(waitingTreeItem);
        waitingTreeItem = null;

        var meta = Metadata.GetEntityUsage(this.EntityMetadataId);


        
        if (!meta)
        {
            return;
            
        }
            

        var url =  getEntityListUrl;

        var data = new PostDataObject();
        data.WebRequestUrl = url;
        data.WebRequestCallback = 'FrameDataLoaded';
        data.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplatesIds();
        data.PostData.entityUsageId = this.EntityMetadataId;
        data.PostData.clientEntityUsagesRequired = Metadata.GetEntityUsage(this.EntityMetadataId) == null;
        data.PostData.getWithoutData = false;
       
        if (meta && !dataOnceLoaded && meta.SlFilterOnStart)
            data.PostData.getWithoutData = true;

        data.PostData.filters = JSON.stringify(this.FilterItems);

        data.PostData.sorts = JSON.stringify(sortings);

        data.PostData.startRecordIndex = startRecordIndex;
        rowsPerPage = Settings.GetSettings('grid_' + this.EntityMetadataId, 'page_size');
        if (!rowsPerPage)
            rowsPerPage = 50;
        data.PostData.recordsAmount = rowsPerPage;

        //data.PostData.primaryKeysValues = JSON.stringify(GetPrimaryKeys(CurrentEntity, Metadata.GetEntityUsage(this.CurrentEntityUsageId)));

        if (this.ParentEntity)
        {
            data.PostData.parentPrimaryKeys = JSON.stringify(this.GetPrimaryKeys(this.ParentEntity, Metadata.GetEntityUsage(this.ParentEntityUsageId)));
            data.PostData.parentEntityUsageId = this.ParentEntityUsageId;
            var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
            if (metadata.JoinParamsNames.length > 0)
            {
                data.WebRequestUrl = getChildEntityListUrl;
                var joinVals = {};
                for (var i = 0; i < metadata.JoinParamsNames.length; i++)
                {
                    var name = metadata.JoinParamsNames[i];
                    var val = this.ParentEntity[name].Value;
                    if (App.Utils.IsObject(val))
                    {
                        joinVals[name] = val.Value;
                    }
                    else
                    {
                        joinVals[name] = val;
                    }


                    
                }
                data.PostData.joins = JSON.stringify(joinVals);
            }
        }

        data.PostData.entityValues = JSON.stringify({});
        data.PostData.openMode = this.OpenMode;
        data.PostData.requiredSettings = 'grid_' + this.EntityMetadataId;
        this.DataLayer.Request(data);


       
    };

    var waitingTreeItem = null;
    this.base_Run = this.Run;
    this.Run = function (treeItem)
    {
        if (treeItem)
        {
            waitingTreeItem = treeItem;
            this.EntityMetadataId = treeItem.EntityMetadataId;
        }
        
        if (!App.Utils.IsStringNullOrWhitespace( this.EntityMetadataId)  &&   !Metadata.GetEntityUsage(this.EntityMetadataId))
        {
            var data = new PostDataObject();
            data.WebRequestUrl = getMetadataUrl;
            data.WebRequestCallback = 'EntityMetadataLoaded';
            data.PostData.entityUsageId = this.EntityMetadataId;
            data.PostData.requiredSettings = 'grid_' + this.EntityMetadataId;
            this.DataLayer.Request(data);
        }
        else
        {

            var missedRelated = [];
            var meta = Metadata.GetEntityUsage(this.EntityMetadataId);
            for (var i = 0; i < meta.Commands.length; i++)
            {
                if (!App.Utils.IsStringNullOrWhitespace(meta.Commands[i].EntityUsageId) &&
                    !Metadata.GetEntityUsage(meta.Commands[i].EntityUsageId.toUpperCase()))
                {
                    missedRelated.push(meta.Commands[i].EntityUsageId.toUpperCase());
                }
                if (!App.Utils.IsStringNullOrWhitespace(meta.Commands[i].DynamicEntityUsageAttrId) &&
                   !Metadata.GetEntityUsage(meta.Commands[i].DynamicEntityUsageAttrId.toUpperCase()))
                {
                    missedRelated.push(meta.Commands[i].DynamicEntityUsageAttrId.toUpperCase());
                }
            }

            if (missedRelated.length > 0)
            {
                var data = new PostDataObject();
                data.WebRequestUrl = getMetadataUrl;
                data.WebRequestCallback = 'EntityMetadataLoaded';
                data.PostData.relaredEntityUsageIds = missedRelated;
               

                this.DataLayer.Request(data);
                return;
            }


            this.EntityMetadataLoaded();
        }
       
    };
    
    this.Refresh = function ()
    {
        var url = getEntityListUrl;

        var data = new PostDataObject();
        data.WebRequestUrl = url;
        data.WebRequestCallback = 'FrameDataLoaded';
        data.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplatesIds();
        data.PostData.entityUsageId = this.EntityMetadataId;
        data.PostData.clientEntityUsagesRequired = Metadata.GetEntityUsage(this.EntityMetadataId) == null;
        data.PostData.filters = JSON.stringify(this.FilterItems);
        data.PostData.sorts = JSON.stringify(sortings);
        data.PostData.getWithoutData = false;
      
     

        if (this.ParentEntity)
        {
            data.PostData.parentPrimaryKeys  = JSON.stringify(this.GetPrimaryKeys(this.ParentEntity, Metadata.GetEntityUsage(this.ParentEntityUsageId)));
            data.PostData.parentEntityUsageId = this.ParentEntityUsageId;
            var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
            if (metadata.JoinParamsNames.length > 0)
            {
                data.WebRequestUrl = getChildEntityListUrl;
                var joinVals = {};
                for(var i = 0; i < metadata.JoinParamsNames.length; i++)
                {
                    var name = metadata.JoinParamsNames[i];
                    var val = this.ParentEntity[name].Value;
                    if (App.Utils.IsObject(val))
                    {
                        joinVals[name] = val.Value;
                    }
                    else
                    {
                        joinVals[name] = val;
                    }
                }
                data.PostData.joins = JSON.stringify(joinVals);
            }
        }

       

        data.PostData.sortDescriptions = [];


        data.PostData.startRecordIndex = startRecordIndex;
        rowsPerPage = Settings.GetSettings('grid_' + this.EntityMetadataId, 'page_size');
        if (!rowsPerPage)
            rowsPerPage = 50;
        data.PostData.recordsAmount = rowsPerPage;

        data.PostData.openMode = this.OpenMode;

        this.DataLayer.Request(data);
    };

    this.base_FrameDataLoaded = this.FrameDataLoaded;
    this.FrameDataLoaded = function (data)
    {
        this.base_FrameDataLoaded(data);

        

        this.Data = data;
      
        var meta = Metadata.GetEntityUsage(this.EntityMetadataId);
        
        this.ViewLayer.FrameDataLoaded(data, new EntityValidator(this.EntityMetadataId), data.Templates, this.OpenMode);



        var foundNewSelected = [];
        if (selectedItems && selectedItems.length > 0 && data.EntityList.Rows.length > 0)
        {

            var oldFirstSelected = selectedItems[0];
            var match = true;
            for (var k = 0; k < data.EntityList.Rows.length ; k++)
            {
                var item = data.EntityList.Rows[k];

                match = true;
                for (var m = 0; m < meta.PrimaryKeysIds.length ; m++)
                {
                    var pkName = meta.PrimaryKeysIds[m];
                    var pkIndex = data.EntityList.PksIndexesInSet[pkName];
                    if (oldFirstSelected[pkIndex].Value != item[pkIndex].Value)
                    {
                        match = false;
                        break;
                    }

                }
                if (match)
                {
                    selectedItems = [item];
                    break;
                }
                
            }

            if (!match && data.EntityList.Rows.length > 0)
            {
                selectedItems = [data.EntityList.Rows[0]];
            }
            if (data.EntityList.Rows.length == 0)
            {
                selectedItems = [];
            }
        }

        if (selectedItems.length == 0 && data.EntityList.Rows.length > 0)
        {
            selectedItems = [data.EntityList.Rows[0]];
        }
        if (data.EntityList.Rows.length == 0)
        {
            selectedItems = [];
        }



        this.DataGrid.LoadData(data, selectedItems, this.OpenMode);


        this.Commands.Run(data.EntityMetadataId, selectedItems, this.OpenMode, this );
        
        if (!dataOnceLoaded)
        {
            var pageSize = Settings.GetSettings('grid_' + this.EntityMetadataId, 'page_size')
            if (!App.Utils.IsStringNullOrWhitespace(pageSize))
                data.RecordsAmount = new Number(pageSize);

        }
        this.PagingPnl.Run(data.EntityMetadataId, data);
        this.Filters.Run(this.EntityMetadataId, data.RowSources);

        this.DataLoaded = true;

        dataOnceLoaded = true;

        
        if (meta.ApplyDefaultFilter && !applyDefaultFilterLoaded)
        {
            this.FilterItems = this.Filters.GetFilters();
            applyDefaultFilterLoaded = true;
            this.Refresh();
            return;
        }

    };


    var RowValidator = function ()
    {
        //this.Validate_UserName = function (value)
        //{
        //    var errors = [];
        //    if (App.Utils.IsStringNullOrWhitespace(value))
        //        errors.push(GetTxt('The User name field is required.'));

        //    return errors;
        //};

        

        //this.ValidateAll = function (values)
        //{
        //    for (var i = 0; i < values.length; i++)
        //    {
        //        var val = values[i];
        //        var vilidationMethod = this['Validate_' + val.Name];
        //        if (vilidationMethod)
        //            val.Errors = vilidationMethod(val.Value);
        //    }
        //    return values;
        //};
    };

    this.SelectedItemChanged = function (items)
    {
        selectedItems = items;
        this.Commands.SelectedItemChaged(items);
    };

    this.GetCommandData = function (command)
    {
        var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);


        var cmdData = new CommandData();
        cmdData.Command = command;
       // if (!App.Utils.IsStringNullOrWhitespace(command.EntityUsageId))
         //   cmdData.EntityUsageId = command.EntityUsageId;
       // else
            cmdData.EntityUsageId = this.EntityMetadataId;


        var selected = [];
        for (var i = 0; i < selectedItems.length; i++)
        {
            selected.push(this.GetEntityValues(selectedItems[i], metadata));
        }

        cmdData.SelectedEntities = selected;
        cmdData.CurrentEntity = selected.length > 0 ? selected[0] : null
        cmdData.Frame = this;
        this.SetQueryParamsToObject(cmdData, selectedItems);

        
        return cmdData;
    };

   

   

    this.DeleteCommandResponse = function (data)
    {
        if (data.ValidationErrors && data.ValidationErrors.length > 0)
        {
            ShowMessage(data.ValidationErrors, GetTxt('Error'), [DialogButtons.OK], NxMessageBoxIcon.Question);
        }

        this.Refresh();
    };

    

    this.Find = function (filters)
    {
        this.FilterItems = filters;
        startRecordIndex = 0;
        this.PagingPnl.Reset();
        this.Refresh();
    };

    this.RowsPerPageChanged = function (rpp)
    {
        rowsPerPage = rpp;
        startRecordIndex = 0;
        this.PagingPnl.Reset();
        this.Refresh();
    };

    this.PageIndexChanged = function (index)
    {
        startRecordIndex = index;
        this.Refresh();
    };

    this.SortingChanged = function (sorts)
    {
        sortings = sorts;
        Settings.AddChangedSettings('grid_' + this.EntityMetadataId, 'sorts', sorts);
        if (this.Data && this.Data.EntityList.Rows.length > 0)
        {
            this.Refresh();
        }
    };

    this.ExportToCsv = function ()
    {
        var url = eportToCsvUrl;

        var data = new PostDataObject();
        data.WebRequestUrl = url;
        data.WebRequestCallback = 'FrameDataLoaded';
        data.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplatesIds();
        data.PostData.entityUsageId = this.EntityMetadataId;
        data.PostData.clientEntityUsagesRequired = Metadata.GetEntityUsage(this.EntityMetadataId) == null;
        data.PostData.filters = JSON.stringify(this.FilterItems);
        data.PostData.sorts = JSON.stringify(sortings);
        data.PostData.getWithoutData = false;


        if (this.ParentEntity)
        {
            data.PostData.parentPrimaryKeys = JSON.stringify(this.GetPrimaryKeys(this.ParentEntity, Metadata.GetEntityUsage(this.ParentEntityUsageId)));
            data.PostData.parentEntityUsageId = this.ParentEntityUsageId;
            var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
            if (metadata.JoinParamsNames.length > 0)
            {
                
                var joinVals = {};
                for (var i = 0; i < metadata.JoinParamsNames.length; i++)
                {
                    var name = metadata.JoinParamsNames[i];
                    var val = this.ParentEntity[name].Value;
                    if (App.Utils.IsObject(val))
                    {
                        joinVals[name] = val.Value;
                    }
                    else
                    {
                        joinVals[name] = val;
                    }
                }
                data.PostData.joins = JSON.stringify(joinVals);
            }
        }



        data.PostData.sortDescriptions = [];


        data.PostData.startRecordIndex = startRecordIndex;
        data.PostData.recordsAmount = rowsPerPage;

        data.PostData.openMode = this.OpenMode;

        this.DataLayer.Request(data);
    };

    
};

extend(BGridFrame, BBaseFrame);

//====================================================================
//====================================================================
//====================================================================

function BGridFrame_ForParent ()
{
    
};

 
BGridFrame.CreateInstance = function ()
{
    var bGridFrame = new BGridFrame();
    bGridFrame.ViewLayer = new VGridFrame();


    var bCommands = new BCommandBar();
    bCommands.ViewLayer = new VCommandBar();
    bCommands.ViewBorder = new CommandBarVBorder();
    bGridFrame.AddChild(bCommands, "Commands");


    var bPagingPnl = new BPagingPnl();
    bPagingPnl.ViewLayer = new VPagingPnl();
    bGridFrame.AddChild(bPagingPnl, 'PagingPnl');

    var bFilters = new BFilters();
    bFilters.ViewLayer = new VFilters();
    bFilters.ViewBorder = new FiltersViewBorder();
    bGridFrame.AddChild(bFilters, 'Filters');

    var bDataGrid = new BDataGrid();
    bDataGrid.ViewLayer = new VDataGrid();
    bDataGrid.ViewBorder = new DataGridViewBorder();
    bGridFrame.AddChild(bDataGrid, 'DataGrid');


    bGridFrame.ConnectLayers();
    return bGridFrame;
};

Metadata.AddClass ("CxGridFrame", BGridFrame);


function BGridFrame_ForView()
{
    BBaseFrame_ForView.call(this);
    this.SelectedItemChanged = function (item) { };
    this.Refresh = function (){};
};

function BGridFrame_ForChild()
{
    BBaseFrame_ForChild.call(this);
    this.Find = function (filters) { };
    this.SelectedItemChanged = function (items) { };
    this.RowsPerPageChanged = function (rowsPerPage) { };
    this.PageIndexChanged = function (startRecordindex) { };
    this.SortingChanged = function (sorts) { };
    this.ExportToCsv = function () { };
};
