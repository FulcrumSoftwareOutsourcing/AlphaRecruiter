'use strict';
function BAutoLayoutFrame()
{
    /// <field name='ViewLayer' type='VAutoLayoutFrame_ForBusiness'/>
    /// <field name='Commands' type='BCommandBar_ForParent'/>

    BBaseFrame.call(this);
    this.PublicForView = new BAutoLayoutFrame_ForView();
    this.PublicForData = new BAutoLayoutFrame_ForData();
    this.PublicForChildren = new BAutoLayoutFrame_ForChild();
    this.Layout = null;

    this.OpenedByCommandId = null;
    this.OpenMode = null;
   
    this.CurrentEntityUsageId = null;
    

    var children = [];
    var tabControld = []; 

    this.DataLoaded = false;

    

    this.InitFrame = function (frameData)
    {
        this.EntityMetadataId = frameData.EntityUsageId;
        this.CurrentEntity = frameData.CurrentEntity;
        this.CurrentEntityUsageId = frameData.CurrentEntityUsageId;

        this.ParentEntity = frameData.ParentEntity;
        this.ParentEntityUsageId = frameData.ParentEntityUsageId;
        this.ParentPrimaryKeys = frameData.ParentPks;




        if (this.ParentEntity && this.ParentEntity['ROLEID'] && this.CurrentEntity)
        {
            this.CurrentEntity['ROLEID'] = this.ParentEntity['ROLEID'].Value;
        }



        this.OpenMode = frameData.OpenMode;
        this.ViewLayer.SetFrameHolderId(frameData.FrameHolderId);

    };

    this.EntityMetadataLoaded = function (_data)
    {
        if (_data)
        {
            Metadata.AddEntityUsage(_data.Metadata);

            for (var i = 0; i < _data.RelatedMetadata.length; i++)
            {
                Metadata.AddEntityUsage(_data.RelatedMetadata[i]);
            }
        }


       


        var data = new PostDataObject();
        data.WebRequestUrl = getEntityFromPkUrl;
        data.WebRequestCallback = 'FrameDataLoaded';
        data.PostData.entityUsageId = this.EntityMetadataId;
        data.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplatesIds();
        data.PostData.clientEntityUsagesRequired = Metadata.GetEntityUsage(this.EntityMetadataId) == null;
        if (this.CurrentEntity)
        {
            data.PostData.pkVals = JSON.stringify(this.GetPrimaryKeys(this.CurrentEntity, Metadata.GetEntityUsage(this.CurrentEntityUsageId)));
            data.PostData.entityValues = JSON.stringify(this.GetObjectValues(this.CurrentEntity, Metadata.GetEntityUsage(this.CurrentEntityUsageId)));
        }
        if (this.ParentPrimaryKeys)
        {
            data.PostData.parentPrimaryKeys = JSON.stringify(this.ParentPrimaryKeys);
            data.PostData.parentEntityUsageId = this.ParentEntityUsageId;
        }
       
        data.PostData.openMode = this.OpenMode;
        

        this.DataLayer.Request(data);
    };

    this.Run = function ()
    {
        if (!Metadata.GetEntityUsage( this.EntityMetadataId ))
        {
            var data = new PostDataObject();
            data.WebRequestUrl = getMetadataUrl;
            data.WebRequestCallback = 'EntityMetadataLoaded';
            data.PostData.entityUsageId = this.EntityMetadataId;

            this.DataLayer.Request(data);
        }
        else
        {
            var missedRelated = [];
            var meta = Metadata.GetEntityUsage(this.EntityMetadataId);
            for (var i = 0; i < meta.Commands.length; i++)
            {
                if (!App.Utils.IsStringNullOrWhitespace( meta.Commands[i].EntityUsageId ) && 
                    !Metadata.GetEntityUsage(meta.Commands[i].EntityUsageId.toUpperCase() ) )
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

   

    function GetPrimaryKeys(entity, metadata)
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
                    if(App.Utils.IsObject(val) )
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id].Value;
                    else
                        pks[metadata.AttributesList[i].Id] = entity[metadata.AttributesList[i].Id];
                }
            }
        }
        return pks;
    };

    function GetPrimaryKeysFromObjEntity(entity, metadata)
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

    this.base_FrameDataLoaded = this.FrameDataLoaded;
    this.FrameDataLoaded = function (data)
    {
        this.base_FrameDataLoaded(data);
        this.Data = data;
        
        


        this.CurrentEntity = data.EntityList.Rows[0];
        

        if (this.ParentEntity && this.ParentEntity['ROLEID'] && this.CurrentEntity && this.CurrentEntity['ROLEID'] && !this.CurrentEntity['ROLEID'].Value )
        {

            this.CurrentEntity['ROLEID'].Value = this.ParentEntity['ROLEID'].Value;
        }


        //bData, validator, templates, openMode
        this.ViewLayer.FrameDataLoaded(data, new EntityValidator(this.EntityMetadataId), data.Templates, this.OpenMode);
        this.ViewLayer.CreateLayout(this.Layout);
        this.Commands.Run(data.EntityMetadataId, this.CurrentEntity, this.OpenMode, this);
        this.Commands.CreateFrameButtons(this.EntityMetadataId, this.OpenMode);

        //CreateLayout(this);

        


        //this.ViewLayer.FrameDataLoaded(data, new RowValidator(), data.Templates);
        //this.Commands.Run(data.EntityMetadataId);
        //this.PagingPnl.Run(data.EntityMetadataId, data)
        this.DataLoaded = true;
    };

    function CreateLayout(context)
    {
        children = []
        tabControld = [];
        
        


        var frameBuilder = new FrameBuilder (this);

        var rootLayout = frameBuilder.GetUiElement(
            context.Layout,
            Metadata.GetEntityUsage( context.EntityMetadataId) ,
            context.CurrentEntity,
            context.OpenMode);

        rootLayout.SetValue(Grid.RowProperty, 0);
        m_RootGrid.Children.push(rootLayout);
        OnLayoutCompleted();
    //    foreach (CxTabControl tabControl in TabControls)
    //    {
    //      tabControl.SelectedTabGhanged += tabControl_SelectedTabGhanged;
    //}



    }

    


   // function UploadFile

    var saveDoneDelegate;
    this.SaveCurentEntity = function (data, doneDelegate, filesToUpload)
    {
        saveDoneDelegate = doneDelegate;


        var hasFiles = false;
        if (filesToUpload)
        {
            for (var file in filesToUpload)
            {
                if (filesToUpload.hasOwnProperty(file))
                {
                    hasFiles = true;
                    break;
                }
            }
        }

        if (hasFiles)
        {
            var upDlg = new BUploadDlg();
            upDlg.ViewLayer = new VUploadDlg();
            upDlg.ConnectLayers();
            upDlg.ShowDialog(filesToUpload, new Delegate(this.SaveCurentEntity, this, [data, doneDelegate, filesToUpload]), this.EntityMetadataId, this.CurrentEntity);

            return;
        }



        if (this.ParentEntity && this.ParentEntity['ROLEID'] && this.CurrentEntity)
        {
            this.CurrentEntity['ROLEID'] = this.ParentEntity['ROLEID'].Value;
        }

        var postData = new PostDataObject();

        postData.WebRequestCallback = "SaveCurentEntityResponse";
        postData.WebRequestUrl = executeCommandUrl;
        postData.PostData.commandId = 'SAVE';
        this.LastExecutedCommandId = postData.PostData.commandId;
        postData.PostData.entityUsageId = this.EntityMetadataId;
        postData.PostData.currentEnt = JSON.stringify(this.CurrentEntity);
        postData.PostData.pkVals = JSON.stringify(GetPrimaryKeys(this.CurrentEntity, Metadata.GetEntityUsage(this.CurrentEntityUsageId)));
       // postData.PostData.selectedEnts = JSON.stringify( [this.CurrentEntity]);
        postData.PostData.startRecordIndex = 0;
        postData.PostData.recordsAmount = 1;
        postData.PostData.queryType = QueryTypes.ENTITY_FROM_PK;
        postData.PostData.openMode = this.OpenMode;
        postData.PostData.isNewEntity = this.OpenMode == NxOpenMode.ChildNew || this.OpenMode == NxOpenMode.New;

        postData.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplatesIds();
        if (this.ParentEntity)
        {
            postData.PostData.parentPrimaryKeys = JSON.stringify(this.GetPrimaryKeys(this.ParentEntity, Metadata.GetEntityUsage(this.ParentEntityUsageId)));
            postData.PostData.parentEntityUsageId = this.ParentEntityUsageId;

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
                postData.PostData.joins = JSON.stringify(joinVals);
            }
        }

      
        
        this.DataLayer.Request(postData);
    };

    this.SaveCurentEntityResponse = function (data)
    {
        if (data.ValidationErrors.length > 0)
        {
            ShowMessage(data.ValidationErrors, 'Validation', [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning);
            saveDoneDelegate = null;
        }
        else
        {
            if (this.OpenMode == NxOpenMode.New)
                this.OpenMode = NxOpenMode.Edit;
            if (this.OpenMode == NxOpenMode.ChildNew)
                this.OpenMode = NxOpenMode.ChildEdit;

            this.ViewLayer.AllChangesSaved();
            if (!saveDoneDelegate)
            {
                
                Navigation.CloseCurrentFrame();

                var meta = Metadata.GetEntityUsage(this.EntityMetadataId);

                if (this.LastExecutedCommandId == 'SAVE' && !App.Utils.IsStringNullOrWhitespace(meta.PostCreateCommandId))
                {
                    var cmd;
                    for (var i = 0; i < meta.Commands.length; i++)
                    {
                        if (meta.Commands[i].Id.toUpperCase() == meta.PostCreateCommandId.toUpperCase())
                            cmd = meta.Commands[i];
                    }

                    var handlerObj = Metadata.CreateClassInstance("DefaultSlCommandHandler");
                    var commandData = this.GetCommandData(cmd);
                    handlerObj.ExecuteCommand(commandData)
                }

            }
            else
            {
                saveDoneDelegate.Invoke();
            }
            saveDoneDelegate = null;
        }
        
        

    };

    this.GetCommandData = function (command)
    {
        var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
        var cmdData = new CommandData();
        cmdData.Command = command;
        cmdData.EntityUsageId = this.EntityMetadataId;
        cmdData.SelectedEntities = [this.GetEntityValues(this.CurrentEntity, metadata)];
        cmdData.Frame = this;

        cmdData.CurrentEntity = this.GetEntityValues(this.CurrentEntity, metadata);
        
        this.SetQueryParamsToObject.call(this, cmdData);

        return cmdData;


    };

   
    this.base_ExecuteCommand = this.ExecuteCommand;
    this.ExecuteCommand = function (commandData)
    {
        if (this.EntityMetadataId != commandData.EntityUsageId)
        {
            var childFrame = this.ViewLayer.GetChildFrame(commandData.EntityUsageId);
            childFrame.ExecuteCommand(commandData);
        }
        else
            this.base_ExecuteCommand(commandData);

      

    };



    this.Refresh = function ()
    {
        this.Run();
    };

    this.GetCurrentEntity = function ()
    {
        return this.CurrentEntity;
    };

    this.GetCurrentEntityPks = function ()
    {
        var pks = {};
        var metadata = Metadata.GetEntityUsage(this.EntityMetadataId);
        for (var i = 0; i < metadata.AttributesList.length; i++)
        {
            if (metadata.AttributesList[i].PrimaryKey)
            {
                var val = this.CurrentEntity[metadata.AttributesList[i].Id];
                if (App.Utils.IsObject(val))
                    pks[metadata.AttributesList[i].Id] = this.CurrentEntity[metadata.AttributesList[i].Id].Value;
                else
                    pks[metadata.AttributesList[i].Id] = this.CurrentEntity[metadata.AttributesList[i].Id];
            }
        }

        return pks;
    };

    this.DeleteCommandResponse = function (data)
    {
        if (data.ValidationErrors && data.ValidationErrors.length > 0)
        {
            ShowMessage(data.ValidationErrors, GetTxt('Error'), [DialogButtons.OK], NxMessageBoxIcon.Question);
        }

        Navigation.CloseCurrentFrame();
    };

    this.IsCommandAllowed = function (commandId)
    {
        if (commandId == CommandIDs.NEW)
            return false;
        return true;
    }


    this.CalculateExpressionsResponse = function (data)
    {
        this.ViewLayer.ExpressionValuesChanged(data)
    };

    this.DymmicCommandClick = function (item, attr)
    {
        var meta = Metadata.GetEntityUsage(this.EntityMetadataId);
       

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

};
extend(BAutoLayoutFrame, BBaseFrame);

BAutoLayoutFrame.CreateInstance = function ()
{
    var bFrame = new BAutoLayoutFrame();
    bFrame.ViewLayer = new VAutoLayoutFrame();
    bFrame.ViewBorder = new AutoLayoutFrameViewBorder();
    bFrame.DataBorder = new AutoLayoutFrameDataBorder();

    var bCommands = new BCommandBar();
    bCommands.ViewLayer = new VCommandBar();
    bCommands.ViewBorder = new CommandBarVBorder();
    bFrame.AddChild(bCommands, "Commands");

    bFrame.ConnectLayers();
    return bFrame;
};


function BAutoLayoutFrame_ForView()
{
    BBaseFrame_ForView.call(this);
    this.SaveCurentEntity = function (data, doneDelegate) { };
    this.Refresh = function () { };
    this.GetCurrentEntity = function () { };
    this.GetCurrentEntityPks = function () { };
    this.DymmicCommandClick = function (item, attr) { }
};

function BAutoLayoutFrame_ForData()
{
    BBaseFrame_ForData.call(this);
    this.SaveCurentEntityResponse = function (data) { };
};

function BAutoLayoutFrame_ForChild()
{
    BBaseFrame_ForChild.call(this);

};

