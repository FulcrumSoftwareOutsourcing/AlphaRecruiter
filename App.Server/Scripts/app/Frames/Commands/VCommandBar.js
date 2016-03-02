function VCommandBar()
{

    ViewLayer.call(this);
    this.PublicForBusiness = new VCommandBar_ForBusiness();
    this.Commands = ko.observableArray();
    this.SaveAndStayCmdVisible = ko.observable(false);
    this.SaveAndCloseCmdVisible = ko.observable(false);
    this.CancelCmdVisible = ko.observable(false);

    
    
    this.SaveAndStayCmdImageUrl ;
    this.SaveAndCloseCmdImageUrl;
    this.CancelCmdImageUrl;

    var imageMeta = Metadata.GetImage('BlueDisk_48x48');
    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        this.SaveAndCloseCmdImageUrl = this.SaveAndStayCmdImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;

    var imageMeta = Metadata.GetImage('Undo_48x48');
    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        this.CancelCmdImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;


    var isBinded = false;
    var tempId;

    this.CreateCommands = function (commands)
    {
        this.Commands.removeAll();
        this.Commands.pushAll(commands);

        this.Commands.ById = {};

        for (var i = 0; i < commands.length; i++)
        {
            this.Commands.ById[commands[i].BCommand.Id] = commands[i];
        }

        if (!isBinded)
        {
            tempId = this.ParentLayer.GetTempId();
            var commandsBar = document.getElementById('commandsBar_' + this.ParentLayer.GetTempId());
            $(commandsBar).html('');
            $(commandsBar).append('<div data-bind="template:  {name: \'Commands/CommandsBar\'}" ></div>');
            

            
            ko.applyBindings(this, $(commandsBar).children()[0]);
            isBinded = true;
        }
        SetCommandBarVisibility.call(this);
    };

    this.ChangeCommandsState = function (toDisable, toEnable)
    {
      
        if (this.Commands().length == 0)
        {
            return;
        }

        for (var i = 0; i < toEnable.length; i++)
        {
            if (this.Commands.ById[toEnable[i]].BCommand.Visible)
                this.Commands.ById[toEnable[i]].Visible(true);
        }

        for (var i = 0; i < toDisable.length; i++)
        {
            this.Commands.ById[toDisable[i]].Visible(false);
        }

        SetCommandBarVisibility.call(this);
    };

    this.CommandClick = function (command, context)
    {
        context.BusinessLayer.ExecuteCommand(command);
    };

    this.HideInternalCommands = function ()
    {
        this.SaveAndCloseCmdVisible(false);
        this.SaveAndStayCmdVisible(false);
        this.CancelCmdVisible(false);
        SetCommandBarVisibility.call(this);
    };

    this.ShowSaveAndCloseCmd = function ()
    {
        this.SaveAndCloseCmdVisible(true);
        SetCommandBarVisibility.call(this);
    };

    this.ShowSaveAndStayCmd = function ()
    {
        this.SaveAndStayCmdVisible(true);
        SetCommandBarVisibility.call(this);
    };

    this.ShowCancelCmd = function ()
    {
        this.CancelCmdVisible(true);
        SetCommandBarVisibility.call(this);
    };

    function SetCommandBarVisibility()
    {
        var hasVisible = false;
        for (var i = 0; i < this.Commands().length; i++)
        {
            if (this.Commands()[i].Visible())
            {
                hasVisible = true;
            }
        }
        if(hasVisible || this.SaveAndCloseCmdVisible() ||
                this.SaveAndStayCmdVisible() ||
                this.CancelCmdVisible() )
              $('#commandsBar_' + this.ParentLayer.GetTempId()).show();
        else
            $('#commandsBar_' + this.ParentLayer.GetTempId()).hide();


      
    };


    this.GetAdtButtonsDescr = function ()
    {
        return [
           {
               Visible: this.SaveAndStayCmdVisible(),
               ExecDelegate: new Delegate(this.CommandClick, this, ['SaveAndStay', this])
           },
           {
               Visible: this.SaveAndCloseCmdVisible(),
               ExecDelegate: new Delegate(this.CommandClick, this, ['SaveAndClose', this])
           },
           {
               Visible: this.CancelCmdVisible(),
               ExecDelegate: new Delegate(this.CommandClick, this, ['Cancel', this])
           },
        ];

    };

    this.HideCommandBar = function ()
    {
        $('#commandsBar_' + this.ParentLayer.GetTempId()).css('height', '0px');
    };
   

};

extend(VCommandBar, ViewLayer);

function VCommandBar_ForBusiness()
{
    this.CreateCommands = function (commands) { };
    this.ChangeCommandsState = function (toDisable, toEnable) { };


    this.HideInternalCommands = function () { };
    this.ShowSaveAndCloseCmd = function () { };
    this.ShowSaveAndStayCmd = function () { };
    this.ShowCancelCmd = function () { };
    this.GetAdtButtonsDescr = function () { };
    this.HideCommandBar = function () { };

};
