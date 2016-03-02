function CommandHandler()
{

    this.ExecuteCommand = function (commandData)
    {
        if (commandData.Command.DisabledByExpr)
        {
            ShowMessage([commandData.Command.DisableConditionErrorText], GetTxt('Command validation'), [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning, new Delegate(ContinueAfterSaveDialog, this));
            return;
        }


        if (Navigation.CheckCurrentHasChanges())
        {
            var current = Navigation.GetCurrentFrame();
            if (current.IsNewEntity)
            {
                waitingForSavePrevious = { commandData: commandData };
                ShowMessage([GetTxt('The parent entity is not saved.'), GetTxt('To create child entity you have to save parent entity.'), GetTxt('Do you want to save it right now?')], GetTxt('Not saved entity'), [DialogButtons.Yes,  DialogButtons.Cancel], NxMessageBoxIcon.Question, new Delegate(ContinueAfterSaveDialog, this));
            }
            else
            {
                waitingForSavePrevious = { commandData: commandData};
                ShowMessage([GetTxt('Frame has changes'), GetTxt('Do you want to save changes?')], GetTxt('Question'), [DialogButtons.Yes, DialogButtons.No, DialogButtons.Cancel], NxMessageBoxIcon.Question, new Delegate(ContinueAfterSaveDialog, this));
            }
        }
        else
        {
            switch (commandData.Command.Id)
            {
                case CommandIDs.DELETE:
                    ExecuteDeleteCommand(commandData);
                    break;
                case CommandIDs.EDIT:
                    OpenDetailsFrame(commandData, NxOpenMode.Edit);
                    break;
                case CommandIDs.VIEW:
                    OpenDetailsFrame(commandData, NxOpenMode.View);
                    break;
                case CommandIDs.NEW:
                    OpenDetailsFrame(commandData, NxOpenMode.New);
                    break;
                case CommandIDs.UP:
                    //         ExecuteUpDownCommand(commandController, commandData);
                    break;
                case CommandIDs.DOWN:
                    //       ExecuteUpDownCommand(commandController, commandData);
                    break;
                default:
                    ExecuteCustomCommand(commandData);
                    return;
            }
        }

    }
    
    var waitingConfirmCommand;
    function ExecuteCustomCommand(commandData)
    {
        if (!App.Utils.IsStringNullOrWhitespace(commandData.Command.ConfirmationText))
        {
            var meta = Metadata.GetEntityUsage(commandData.EntityUsageId);
            var msg = commandData.Command.ConfirmationText.replace('%single_caption%', meta.SingleCaption);
            msg = msg.replace('%plural_caption%', meta.PluralCaption);

            waitingConfirmCommand = commandData;
            ShowMessage([msg], GetTxt('Confirmation'), [DialogButtons.Yes, DialogButtons.No], NxMessageBoxIcon.Question, new Delegate(function (dlgResult)
            {

                if (dlgResult == 'yes')
                {
                    if (Navigation.GetCurrentFrame().ExecuteCommand)
                        Navigation.GetCurrentFrame().ExecuteCommand(waitingConfirmCommand);
                    waitingConfirmCommand = null;
                }
                if (dlgResult == 'no')
                {
                    waitingConfirmCommand = null;
                }
                if (dlgResult == 'cancel')
                {
                    waitingConfirmCommand = null;
                }

            }, this));


        }
        else
        {


            if (!App.Utils.IsStringNullOrWhitespace (commandData.Command.DynamicEntityUsageAttrId) && commandData.CurrentEntity)
            {
                commandData.Command.EntityUsageId = commandData.Command.DynamicEntityUsageAttrId.toUpperCase();
                if (!App.Utils.IsStringNullOrWhitespace(commandData.Command.DynamicCommandAttrId))
                {
                    commandData.Command.TargetCommandId = commandData.Command.DynamicCommandAttrId.toUpperCase();
                }
            }

            if (!App.Utils.IsStringNullOrWhitespace(commandData.Command.EntityUsageId) &&
              !App.Utils.IsStringNullOrWhitespace(commandData.Command.TargetCommandId))
            {
                
                var targetMeta = Metadata.GetEntityUsage(commandData.Command.EntityUsageId)
                
                for(var i = 0; i < targetMeta.Commands.length; i++)
                {
                    if (targetMeta.Commands[i].Id.toUpperCase() == commandData.Command.TargetCommandId.toUpperCase())
                    {
                        commandData.Command = targetMeta.Commands[i];
                        break;
                    }
                }
                
                

                var h = new CommandHandler();
                h.ExecuteCommand(commandData);




                //ReplaceCommand(commandData);
                return;
            }




            if (Navigation.GetCurrentFrame().ExecuteCommand)
                Navigation.GetCurrentFrame().ExecuteCommand(commandData);
        }

    }

    

//    function ExecuteNewCommand ( commandData )
//    {
//        var entityUsage = Metadata.GetEntityUsage(commandData.EntityUsageId);
//        var frame = Metadata.GetDetailsFrame(entityUsage.EditFrameId);



//    CxFrameData frameData = new CxFrameData();
//    frameData.EntityUsageId = entityUsage.EntityMetadata.Id;
//    if(!string.IsNullOrEmpty(commandData.Command.EntityUsageId))
//    {
//        if (CxAppContext.Instance.EntityUsages[commandData.Command.EntityUsageId] == null)
//        {
//            frameData.EntityUsageId = commandData.Command.EntityUsageId;
//        }
       
     
//    }
//    frameData.OpenMode = NxOpenMode.New;
//    if (commandData.EditController != null && commandData.EditController.Frame != null)
//    {
//        frameData.ParentEntity = commandData.EditController.Frame.ParentEntity;
//        frameData.ParentEntityUsageId = commandData.EditController.Frame.ParentEntityUsageId;
//        if (commandData.EditController.Frame.ParentFrame != null)
//        {
//            frameData.ParentPks =
//              commandData.EditController.Frame.ParentFrame.LastQueryParams.PrimaryKeysValues;
//        }
//        else
//        {
//            frameData.ParentPks = new Dictionary<string, object>();
//        }
//    }
//    else if(commandData.ParentEntity != null)
//    {
//        frameData.ParentEntity = commandData.ParentEntity;
//        frameData.ParentEntityUsageId = commandData.ParentEntityUsageId;
//        frameData.ParentPks = commandData.ParentPks;
//    }


//    frame.InitFrame(frameData);
//    frame.SelectedEntities.Add(commandData.CurrentEntity);

//    CxCommandClickNavigation navigation = new CxCommandClickNavigation();
//    navigation.BeginFrameShowing(frame, commandController, commandData);

//}


    var waitingToDeleteData;
    function ExecuteDeleteCommand(commandData)
    {
        var objCaption = 'object(s)';
        var meta = Metadata.GetEntityUsage(commandData.EntityUsageId);
        if (!App.Utils.IsStringNullOrWhitespace(meta.SingleCaption))
            objCaption = meta.SingleCaption + '(s)';

        waitingToDeleteData = commandData;
        ShowMessage([GetTxt('Delete selected ') + objCaption + '?'], GetTxt('Question'), [DialogButtons.Yes, DialogButtons.No], NxMessageBoxIcon.Question, new Delegate(ContinueAfterDeleteQuestionDialog, this));

    }

    function ContinueAfterDeleteQuestionDialog(dlgResult)
    {
        if (dlgResult == 'yes')
        {
            waitingToDeleteData.Frame.ExecuteDeleteCommand(waitingToDeleteData);
        }
        
        waitingToDeleteData = null;
    }

    var waitingForSavePrevious;

    function ContinueAfterSaveDialog(dlgResult)
    {
        if (dlgResult == 'yes')
        {
            Navigation.GetCurrentFrame().SaveChanges(new Delegate(ContinueAfterSaveDone, this));
        }
        if (dlgResult == 'no')
        {
            Navigation.GetCurrentFrame().RevertChanges();
            this.ExecuteCommand(waitingForSavePrevious.commandData);
        }
        if (dlgResult == 'cancel')
        {
            waitingForSavePrevious = null;
        }
        
        
    }

    function ContinueAfterSaveDone()
    {
        this.ExecuteCommand(waitingForSavePrevious.commandData);
        waitingForSavePrevious = null;
    }

    function OpenDetailsFrame(commandData, openMode)
    {
        /// <param name='commandData' type='CommandData'/>

        

        

        var entityUsage = Metadata.GetEntityUsage(commandData.EntityUsageId);
        if (!App.Utils.IsStringNullOrWhitespace(commandData.Command.EntityUsageId))
        {
            entityUsage = Metadata.GetEntityUsage( commandData.Command.EntityUsageId );
        }


        var frame = Metadata.GetDetailsFrame(entityUsage.EditFrameId);

        var frameData = {};
        frameData.EntityUsageId = entityUsage.Id;
        frameData.CurrentEntityUsageId = entityUsage.Id;

        if (!App.Utils.IsStringNullOrWhitespace(commandData.Command.EntityUsageId))
        {
            frameData.CurrentEntityUsageId = commandData.Command.EntityUsageId;
        }


        frameData.CurrentEntity = commandData.CurrentEntity;
        if (!frameData.CurrentEntity && commandData.SelectedEntities && commandData.SelectedEntities.length > 0)
            frameData.CurrentEntity = commandData.SelectedEntities[0];

        if (commandData.Command.AlternativeCurrentEntity)
        {
            frameData.CurrentEntity = commandData.Command.AlternativeCurrentEntity;
        }

        

        frameData.ParentEntity = commandData.ParentEntity;
        frameData.ParentEntityUsageId = commandData.ParentEntityUsageId;
        frameData.ParentPks = commandData.ParentPrimaryKeys;
        frameData.PostCreateCommandId = commandData.Command.PostCreateCommandId;
        
        frameData.OpenMode = openMode;
        frame.InitFrame(frameData);
        //frame.SelectedEntities.Add(commandData.CurrentEntity);
        //frame.ListViewPks = commandData.EntitiesList;

        if (commandData.Command.StartedByAnotherCommandId)
            frame.OpenedByCommandId = commandData.Command.StartedByAnotherCommandId;

        OpenFrame(frame);
        //    CxCommandClickNavigation navigation = new CxCommandClickNavigation();
        //   navigation.BeginFrameShowing(frame, commandController, commandData);

    };

    

};

Metadata.AddClass("DefaultSlCommandHandler", CommandHandler);




function CommandData()
{
    this.Command = null;
    this.EntityUsageId = null;
    this.SelectedItems = [];



};