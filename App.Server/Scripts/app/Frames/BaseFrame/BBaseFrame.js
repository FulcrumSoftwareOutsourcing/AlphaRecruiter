'use strict';
function BBaseFrame()
{
    /// <field name='Commands' type='BCommandBar_ForParent'/>
    BusinessLayer.call(this);
  
    this.CurrentEntity = null;

    this.ViewBorder = new BaseFrameViewBorder();
    this.DataBorder = new BaseFrameDataBorder();
    this.EntityMetadataId = null;
    this.PublicForData = new BBaseFrame_ForData();
    this.PublicForChildren = new BBaseFrame_ForChild();
    this.PublicForView = new BBaseFrame_ForView();
    
    this.ParentEntity = null;
    this.ParentEntityUsageId = null;
    this.ParentPrimaryKeys = null;
    this.LastExecutedCommandId = null;
    

    this.Run = function (treeItem)
    {
        if (treeItem)
        {
            this.EntityMetadataId = treeItem.EntityMetadataId;
        }

    };

    this.FrameDataLoaded = function (data)
    {
        data.EntityMetadataId = this.EntityMetadataId;
        if(data.Metadata)
        {
            Metadata.AddEntityUsage(data.Metadata);
        }
    };

    this.SaveAndClose = function ()
    {
        this.ViewLayer.SaveAndClose();
    };

    this.ExecuteDeleteCommand = function (commandData)
    {
        var data = new PostDataObject();
        data.WebRequestUrl = executeCommandUrl;
        data.WebRequestCallback = 'DeleteCommandResponse';

        data.PostData.commandId = CommandIDs.DELETE;
        data.PostData.entityUsageId = this.EntityMetadataId;
        data.PostData.currentEnt = JSON.stringify(commandData.CurrentEntity);
        data.PostData.selectedEnts = JSON.stringify(commandData.SelectedEntities);
        data.PostData.pkVals = JSON.stringify(commandData.PrimaryKeysValues);
        data.PostData.parentPrimaryKeys = JSON.stringify(commandData.ParentPrimaryKeys);
        data.PostData.parentEntityUsageId = commandData.ParentEntityUsageId;
        data.PostData.startRecordIndex = 0;
        data.PostData.recordsAmount = 1;
        data.PostData.queryType = QueryTypes.ENTITY_FROM_PK;
        data.PostData.openMode = this.OpenMode;
        data.PostData.isNewEntity = this.OpenMode == NxOpenMode.ChildNew || this.OpenMode == NxOpenMode.New;


        this.DataLayer.Request(data);
    }

    this.DeleteCommandResponse = function (data)
    {
       
        
    };

    this.SetQueryParamsToObject = function (target, selectedItems)
    {
        var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
        if (this.ParentEntity)
        {
            target.ParentEntity = this.ParentEntity;
            target.ParentPrimaryKeys = this.GetPrimaryKeys(this.ParentEntity, Metadata.GetEntityUsage(this.ParentEntityUsageId));
            target.ParentEntityUsageId = this.ParentEntityUsageId;

            var parentMetadata = Metadata.GetEntityUsage(this.ParentEntityUsageId);
            if (parentMetadata.JoinParamsNames.length > 0)
            {
                var joinVals = {};
                for (var i = 0; i < parentMetadata.JoinParamsNames.length; i++)
                {
                    var name = parentMetadata.JoinParamsNames[i];
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
                target.Joins = JSON.stringify(joinVals);
            }
        }
        if (selectedItems && selectedItems.length > 0)
        {
            target.PrimaryKeysValues = this.GetPrimaryKeys(selectedItems[0], metadata);
        }


    };

    this.GetEntityValues = function (entity, metadata)
    {
        var vals = {};
        if (entity.length)
        {
            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
                var val = entity[i].Value;
                if (App.Utils.IsObject(val))
                    vals[metadata.AttributesList[i].Id] = val.Value;
                else
                    vals[metadata.AttributesList[i].Id] = val;
            }
        }
        else
        {
            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
              
                    var val = entity[metadata.AttributesList[i].Id];
                    if (App.Utils.IsObject(val))
                        vals[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id].Value;
                    else
                        vals[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id];
                
            }
        }
        return vals;
    };

    this.GetPrimaryKeys = function(entity, metadata)
    {
        var pks = {};

        if (entity.length)
        {

            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
                if (metadata.AttributesList[i].PrimaryKey)
                {
                    pks[metadata.AttributesList[i].Id] = entity[i].Value;
                }
            }
        }
        else
        {
            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
                if (metadata.AttributesList[i].PrimaryKey)
                {
                    var val = entity[metadata.AttributesList[i].Id];
                    if (App.Utils.IsObject(val))
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id].Value;
                    else
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id];
                }
            }
        }
        return pks;
    };

    this.GetObjectValues = function (entity, metadata)
    {
        var pks = {};

        if (entity.length)
        {

            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
               
                    pks[metadata.AttributesList[i].Id] = entity[i].Value;
               
            }
        }
        else
        {
            for (var i = 0; i < metadata.AttributesList.length; i++)
            {
                
                    var val = entity[metadata.AttributesList[i].Id];
                    if (App.Utils.IsObject(val))
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id].Value;
                    else
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id];
                
            }
        }
        return pks;
    };

    this.GetPrimaryKeysFromObjEntity = function(entity, metadata)
    {
        var pks = {};

        for (var i = 0; i < metadata.AttributesList.length; i++)
        {
            if (metadata.AttributesList[i].PrimaryKey)
            {
                pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id];
            }
        }
        return pks;
    };

    this.OnPropertyChanged = function (attr, value) 
    {
        if (attr.DependentAttributesIds.length > 0 || attr.DependentMandatoryAttributesIds.length > 0 || attr.DependentStateIds.length > 0)
        {
            var data = new PostDataObject();
            data.WebRequestUrl = calculateExpressionsUrl;
            data.WebRequestCallback = 'CalculateExpressionsResponse';

           
            data.PostData.entityUsageId = this.EntityMetadataId;
            
            var oldVal = this.CurrentEntity[attr.Id].Value;
            this.CurrentEntity[attr.Id].Value = value;
            data.PostData.currentEnt = JSON.stringify(this.GetEntityValues(this.CurrentEntity, Metadata.GetEntityUsage(this.EntityMetadataId)));
            
            
            //data.PostData.pkVals = JSON.stringify(commandData.PrimaryKeysValues);
            //data.PostData.parentPrimaryKeys = JSON.stringify(commandData.ParentPrimaryKeys);
            //data.PostData.parentEntityUsageId = commandData.ParentEntityUsageId;
            data.PostData.startRecordIndex = 0;
            data.PostData.recordsAmount = 1;
            //data.PostData.queryType = QueryTypes.ENTITY_FROM_PK;
            //data.PostData.openMode = this.OpenMode;
            //data.PostData.isNewEntity = this.OpenMode == NxOpenMode.ChildNew || this.OpenMode == NxOpenMode.New;
            data.PostData.changedAttributeId = attr.Id;
            data.PostData.dependentAttributesIds = attr.DependentAttributesIds;
            data.PostData.dependentMandatoryAttributesIds = attr.DependentMandatoryAttributesIds;
            data.PostData.dependentStateIds = attr.DependentStateIds;

            this.DataLayer.Request(data);
            this.CurrentEntity[attr.Id].Value = oldVal;
        }
    };

    this.CalculateExpressionsResponse = function (data)
    {
        this.ViewLayer.ExpressionValuesChanged(data)
    };

    this.ExecuteCommand = function (commandData)
    {
        var data = new PostDataObject();
        data.WebRequestUrl = executeCommandUrl;
        data.WebRequestCallback = 'ExecuteCommandResponse';

        data.PostData.commandId = commandData.Command.Id;
        data.PostData.entityUsageId = this.EntityMetadataId;
        //data.PostData.entityUsageId = App.Utils.IsStringNullOrWhitespace(commandData.EntityUsageId) ? this.EntityMetadataId : commandData.EntityUsageId;
        data.PostData.currentEnt = JSON.stringify(commandData.CurrentEntity);
        data.PostData.selectedEnts = JSON.stringify(commandData.SelectedEntities);
        data.PostData.pkVals = JSON.stringify(commandData.PrimaryKeysValues);
        data.PostData.parentPrimaryKeys = JSON.stringify(commandData.ParentPrimaryKeys);
        data.PostData.parentEntityUsageId = this.ParentEntityUsageId;//  commandData.ParentEntityUsageId;
        data.PostData.startRecordIndex = 0;
        data.PostData.recordsAmount = 1;
        
        data.PostData.openMode = this.OpenMode;
        data.PostData.isNewEntity = this.OpenMode == NxOpenMode.ChildNew || this.OpenMode == NxOpenMode.New;


        this.DataLayer.Request(data);

    };

    this.ExecuteCommandResponse = function (data)
    {
        if (data.ValidationErrors && data.ValidationErrors.length > 0)
        {
            ShowMessage(data.ValidationErrors, GetTxt('Command validation'), [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning);
        }

        if (data.Command && data.Command.RefreshPage)
        {
            this.Refresh();
        }
    };

    this.Download = function (attrId, item)
    {
        //window.IsDownload = true;

        document.location = downloadUrl + '?entityUsageId=' + 
            this.EntityMetadataId + '&attributeId=' + 
            attrId + '&pkVals=' + JSON.stringify(this.GetPrimaryKeys(item, Metadata.GetEntityUsage(this.EntityMetadataId)));
      
    };

    this.DymmicCommandClick = function (attrId, item)
    {
        var meta = Metadata.GetEntityUsage(this.EntityMetadataId);
        var attr = meta.Attributes[attrId];

        var dynCmdId = attr.HyperlinkCommandId.toUpperCase();
        var dynCmd;
        for (var i = 0; i < meta.Commands.length; i++)
        {
            if (meta.Commands[i].Id.toUpperCase() == dynCmdId)
            {
                dynCmd = meta.Commands[i];
                break;
            }
        }

        this.Commands.ExecuteCommand(dynCmd);
    }

    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr)
    {
        this.ViewLayer.FrameButtonsCreated(saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr);
    };

};

extend(BBaseFrame, BusinessLayer);



function BBaseFrame_ForData()
{
    this.FrameDataLoaded = function (data) { };
    this.Run = function (treeItemId) { };
    this.EntityMetadataLoaded = function (data) { };
    this.DeleteCommandResponse = function () { };
    this.CalculateExpressionsResponse = function (data) { };
    this.ExecuteCommandResponse = function (data) {};
};

function BBaseFrame_ForChild()
{
    this.GetCommandData = function (command) { };
    this.SaveAndClose = function () { };
    this.Download = function (attrId, item) { };
    this.DymmicCommandClick = function (attrId, item) { }
    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr) { };

};

function BBaseFrame_ForView()
{
    this.OnPropertyChanged = function (attr, value) { };
    this.ExecuteCommand = function (commandData){};
    this.GetPrimaryKeys = function (entity, metadata) { };
};