'use strict';
function BCommandBar()
{
    /// <field name='ViewLayer' type='VCommandBar_ForBusiness'/>
    /// <field name='ParentLayer' type='BGridFrame_ForChild'/>
    BusinessLayer.call(this);
    this.PublicForParent = new BCommandBar_ForParent();
    this.PublicForView = new BCommandBar_ForView();

    var Commands = null;
    var selectedItems;
    var usageId;

    this.Run = function (entityUsageId, selectedItem, openMode, frame)
    {
        usageId = entityUsageId;
        if (selectedItem)
        {
            selectedItems = selectedItem;
        }
        
        var hasAvialableCommands = false;

        Commands = [];
        var metadata = Metadata.GetEntityUsage(entityUsageId);
        if(metadata || metadata.Commands)
        {
            for (var i = 0; i < metadata.Commands.length; i++)
            {
                if (metadata.Commands[i].AvailableOnEditform)
                {
                    hasAvialableCommands = true;
                    break;
                }
            }


            for (var i = 0; i < metadata.Commands.length; i++)
            {
                if (openMode && metadata.Commands[i].Id.toUpperCase() == openMode.toUpperCase())
                    continue;

                if (frame && frame.IsCommandAllowed && !frame.IsCommandAllowed(metadata.Commands[i].Id))
                    continue;

                Commands.push( metadata.Commands[i] ) ;
                Commands[metadata.Commands[i].Id] = metadata.Commands[i];
            }
        }

        if (frame.IsGridFrame ||  hasAvialableCommands || !openMode ||
            (   openMode &&
                openMode.toUpperCase() != NxOpenMode.Edit.toUpperCase() &&
            //    openMode.toUpperCase() != NxOpenMode.View.toUpperCase() &&
                openMode.toUpperCase() != NxOpenMode.New.toUpperCase() &&
                openMode.toUpperCase() != NxOpenMode.ChildEdit.toUpperCase() &&
             //   openMode.toUpperCase() != NxOpenMode.ChildView.toUpperCase() &&
                openMode.toUpperCase() != NxOpenMode.ChildNew.toUpperCase())
            )
        {
            this.ViewLayer.CreateCommands(Commands, selectedItem, openMode, frame.IsGridFrame);
        }
        else
        {
            if (openMode && (
                openMode.toUpperCase() == NxOpenMode.Edit.toUpperCase() ||
                //openMode.toUpperCase() == NxOpenMode.View.toUpperCase() ||
                openMode.toUpperCase() == NxOpenMode.New.toUpperCase() ||
                openMode.toUpperCase() == NxOpenMode.ChildEdit.toUpperCase() ||
                //openMode.toUpperCase() == NxOpenMode.ChildView.toUpperCase() ||
                openMode.toUpperCase() == NxOpenMode.ChildNew.toUpperCase()))
            {
                this.ViewLayer.HideCommandBar();
            }
        }

      

        CheckCommandsStates(this);
    };

    this.CommandsStateChanged = function (commandsStates)
    {

    };

    this.SelectedItemChaged = function (items)
    {
        selectedItems = items;
        CheckCommandsStates(this);
        
    };

    function CheckCommandsStates(context)
    {
        if (!Commands)
            return;

        var toDisable = [];
        var toEnable = [];

        for (var i = 0; i < Commands.length; i++)
        {
            var wasProcessed = false;
            var command = Commands[i];
            command.DisabledByExpr = false;

            if (command.Id == CommandIDs.NEW)
            {
                toEnable.push(command.Id);
                continue;
            }

            if (!selectedItems) return;

            if (selectedItems.length == 0 && command.IsEnabled && command.IsEntityInstanceRequired)
            {
                toDisable.push(command.Id);
                var wasProcessed = true;
            }
            if (selectedItems.length > 1 && command.IsEnabled && command.IsMultiple == false)
            {
                toDisable.push(command.Id);
                var wasProcessed = true;
            }
            if (selectedItems.length == 1 && command.IsEnabled && command.IsEntityInstanceRequired )
            {
                toEnable.push(command.Id);
                var wasProcessed = true;
            }
            if (selectedItems.length == 1 && command.IsEnabled && command.IsMultiple == false)
            {
                toEnable.push(command.Id);
                var wasProcessed = true;
            }

            if (selectedItems && selectedItems.length && selectedItems.length > 0)
            {

                for (var a = 0; a < selectedItems.length; a++)
                {
                    var item = selectedItems[a];

                    for (var k = 0; k < item.length; k++)
                    {
                        if (item[k].DisabledCommandIds && item[k].DisabledCommandIds[command.Id])
                        {
                            if (command.HiddenWhenDisabled)
                            {
                                toDisable.push(command.Id);
                                var wasProcessed = true;
                            }
                            else
                            {
                                command.DisabledByExpr = true;
                                var wasProcessed = true;
                            }

                        }
                    }
                }

            }
            if (selectedItems && selectedItems.length == undefined)
            {
                var meta = Metadata.GetEntityUsage(usageId);
               

                    for (var b = 0; b < meta.AttributesList.length; b++)
                    {
                        var attr = meta.AttributesList[b];
                        var val = selectedItems[attr.Id];
                        if (val && val.DisabledCommandIds && val.DisabledCommandIds[command.Id])
                        {
                            if (command.HiddenWhenDisabled)
                            {
                                toDisable.push(command.Id);
                                var wasProcessed = true;
                            }
                            else
                            {
                                command.DisabledByExpr = true;
                                var wasProcessed = true;
                            }

                        }
                    }
                


                
                

            }

            if (!wasProcessed)
            {
                toEnable.push(command.Id);
            }


        }
        context.ViewLayer.ChangeCommandsState(toDisable, toEnable);
    }

    this.ExecuteCommand = function (command)
    {
      

        if ((command == 'SaveAndClose' || command == 'SaveAndStay') && this.ParentLayer.SaveAndClose)
        {
            this.ParentLayer.SaveAndClose();
            return;
        }
       
        if (command == 'Cancel' )
        {
            Navigation.CloseCurrentFrame();
            return;
        }


        var handlerObj = Metadata.CreateClassInstance(command.SlHandlerClassId);
        if (!handlerObj)
            handlerObj = Metadata.CreateClassInstance("DefaultSlCommandHandler");
        var commandData = this.ParentLayer.GetCommandData(command);
        handlerObj.ExecuteCommand(commandData)


        
    };
   
    this.CreateFrameButtons = function (metadata, openMode)
    {
        var meta = Metadata.GetEntityUsage(metadata);
        var isViewMode = openMode == NxOpenMode.View || openMode == NxOpenMode.ChildView;

        if (!isViewMode)
        {
            if (meta.SaveAndStayCommand)
            {
                this.ViewLayer.ShowSaveAndStayCmd();
            }
            this.ViewLayer.ShowSaveAndCloseCmd();
        }
        this.ViewLayer.ShowCancelCmd();
        
        var descrs = this.ViewLayer.GetAdtButtonsDescr();
        this.ParentLayer.FrameButtonsCreated(descrs[0], descrs[1], descrs[2]);

    };

    
};

extend(BCommandBar, BusinessLayer);

function BCommandBar_ForParent()
{
    this.Run = function (commands, selectedItem) { };
    this.CommandsStateChanged = function (commandsStates) { };
    this.SelectedItemChaged = function (item) { };
    this.CreateFrameButtons = function (metadata, openMode) { };
    this.ExecuteCommand = function (command){ }
};

function BCommandBar_ForView()
{
    this.ExecuteCommand = function (command) { };
    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr) { };
};

