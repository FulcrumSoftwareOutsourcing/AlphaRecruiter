
function VPopup()
{
    ViewLayer.call(this);
    this.Body = document.getElementById('htmlBody');
    this.IsBinded = false;
    this.Visible = ko.observable(false);
    this.NoCloseButton = ko.observable(false);
    
    this.PublicForParent = new VPopup_ForParent();

    
    this.Waiter = null;
    this.MainWaiter = null;

    window.Waiting.AddIndividualWaiter(WaitersIds.DialogDataLoading, this.ShowMainWaitSpinner, this.HideMainWaitSpinner, this);
    
    Resizer.AddResizable('popup', new Delegate(this.OnClientResized, this));

    this.FullScreenMode = false;

    this.OverDisplayStyle = ko.computed(function ()
    {

            if (this.Visible())
            {
                
                $('#popupOver').show();
                $('#popupDlgBorder').show();
                if (VPopup.ContentModel && VPopup.ContentModel.OnDialogVisible)
                {
                    VPopup.ContentModel.OnDialogVisible.call(VPopup.ContentModel);
                }
                //$('#popupDlgBorder').fadeIn(200, function ()
                //{
                //    if (VPopup.ContentModel && VPopup.ContentModel.OnDialogVisible)
                //    {
                //        VPopup.ContentModel.OnDialogVisible.call(VPopup.ContentModel);
                //    }
                //}); 
                
            }
            else
            {
                $('#popupOver').hide();
                //$('#popupDlgBorder').fadeOut(150);
                $('#popupDlgBorder').hide();
                
            }

        //return this.Visible() == true ? "block" : "none";
    }, this);

    this.Caption = ko.observable('');

    VPopup.prototype.Top = ko.observable(0);
    VPopup.prototype.Left = ko.observable(0);
    VPopup.prototype.Width = ko.observable(0);
    VPopup.prototype.Height = ko.observable(0);

    VPopup.prototype.TopStyle = ko.computed(function ()
    {
        return (this.Top() + 'px');
    }, this);

    VPopup.prototype.LeftStyle = ko.computed(function ()
    {
        return (this.Left() + 'px');
    }, this);

    VPopup.prototype.WidthStyle = ko.computed(function ()
    {
        return (this.Width() > 0 ? (this.Width() + 'px') : 'auto');
    }, this);

    VPopup.prototype.HeightStyle = ko.computed(function ()
    {
        return (this.Height() > 0 ? (this.Height() + 'px') : 'auto');
    }, this);

    this.CloseImageUrl = ko.observable(skinFolderUrl + '/images/framework/dlg_close_btn.png');

    VPopup.prototype.OnCloseBtnMouseDown = this.OnCloseBtnMouseDown;
    VPopup.prototype.OnCloseBtnMouseOver = this.OnCloseBtnMouseOver;
    VPopup.prototype.OnCloseBtnMouseOut = this.OnCloseBtnMouseOut;

    this.OkBtn = ko.observable(false);
    this.YesBtn = ko.observable(false);
    this.NoBtn = ko.observable(false);
    this.CancelBtn = ko.observable(false);
    this.ExButtons = ko.observableArray();

    VPopup.prototype.OkBtnVisible = ko.computed(function ()
    {
        return this.OkBtn() ? 'block' : 'none';
    }, this);

    VPopup.prototype.YesBtnVisible = ko.computed(function ()
    {
        return this.YesBtn() ? 'block' : 'none';
    }, this);

    VPopup.prototype.NoBtnVisible = ko.computed(function ()
    {
        return this.NoBtn() ? 'block' : 'none';
    }, this);

    VPopup.prototype.CancelBtnVisible = ko.computed(function ()
    {
        return this.CancelBtn() ? 'block' : 'none';
    }, this);

    this.Waiting = ko.observable(false);

    //VPopup.prototype.WaitSpinnerStyle = ko.computed(function ()
    //{
    //    if (this.Waiting())
    //    {
    //        if (this.Waiter)
    //            this.Waiter.Show();
    //        //$('#dlgButtons').hide();
    //    }
    //    else
    //    {
    //        if (this.Waiter)
    //            this.Waiter.Hide();
    //        //$('#dlgButtons').show();
    //    }
    //    return this.Waiting() ? 'MainWaiterImg fade-in-img' : 'MainWaiterImg';
    //}, this);


    window.Waiting.AddIndividualWaiter(WaitersIds.Popup, this.ShowWaitSpinner, this.HideWaitSpinner, this);
    window.Waiting.AddIndividualWaiter(WaitersIds.DldWorkingWaiter, this.ShowDldWorking, this.HideDldWorking, this);
    this.DlgIsWorkingVisible = ko.observable('none');


    this.ErrorListElementId = App.Utils.ShortGuid();

    
    //window.ResizeDispatcher.AddResizeListener( new Delegate( function ()
    //{
    //    if (!window.VPopup.IsBinded)
    //        return;
    //    window.VPopup.Left((window.GetClientWidth() / 2) - (window.VPopup.DlgBorder.clientWidth / 2));
    //    window.VPopup.Top((window.GetClientHeight() / 2) - (window.VPopup.DlgBorder.clientHeight / 2));
    //}, this), 5);

};

extend(VPopup, ViewLayer);

VPopup.prototype.Body = null;
VPopup.prototype.IsBinded = null;
VPopup.prototype.Visible = null;

VPopup.prototype.OverDisplayStyle = null;
VPopup.prototype.OverHeightStyle = null;
VPopup.prototype.OverWidthStyle = null;

VPopup.prototype.DlgBorder = null;
VPopup.prototype.Caption = null;
VPopup.prototype.Top = null;
VPopup.prototype.Left = null;
VPopup.prototype.TopStyle = null;
VPopup.prototype.LeftStyle = null;
VPopup.prototype.Width = null;
VPopup.prototype.Height = null;

VPopup.prototype.CloseImageUrl = null;

VPopup.prototype.DlgContentHolder = null;

VPopup.prototype.ContentModel = null;

VPopup.prototype.OkBtn = null;
VPopup.prototype.YesBtn = null;
VPopup.prototype.NoBtn = null;
VPopup.prototype.CancelBtn = null;

VPopup.prototype.DlgIsWorkingVisible = null

VPopup.prototype.ChangeButtons = function (buttons, exButtons, noCloseButton)
{
    this.OkBtn(false);
    this.YesBtn(false);
    this.NoBtn(false);
    this.CancelBtn(false);
    if (buttons )
    {
        for (var i = 0; i < buttons.length; i++)
        {
            switch (buttons[i])
            {
                case DialogButtons.OK:
                    this.OkBtn(true);
                    break;
                case DialogButtons.Yes:
                    this.YesBtn(true);
                    break;
                case DialogButtons.No:
                    this.NoBtn(true);
                    break;
                case DialogButtons.Cancel:
                    this.CancelBtn(true);
                    break;
            }
        }
    }
    else
    {
        if (!noCloseButton)
            this.CancelBtn(true);
    }


    this.NoCloseButton(noCloseButton);


    this.ExButtonClick = function (item)
    {
        item.handler.apply(item.handlerContext);
    };


    if (exButtons)
    {
        for (var i = 0; i < exButtons.length; i++)
        {
            this.ExButtons.push(exButtons[i]);
        }
    }

}

VPopup.prototype.ShowDialog = function (args)
{
    this.FullScreenMode = args.FullScreen;

    if (!this.IsBinded)
    {


        ko.applyBindings(this, document.getElementById('popupHolder'));
        this.DlgBorder = document.getElementById('popupDlgBorder');
        this.IsBinded = true;
        this.DlgContentHolder = document.getElementById('DlgContentHolder');

        
    }

    this.OkBtn(false);
    this.YesBtn(false);
    this.NoBtn(false);
    this.CancelBtn(false);
    if (args.Buttons != undefined)
    {
        for (var i = 0; i < args.Buttons.length; i++)
        {
            switch (args.Buttons[i])
            {
                case DialogButtons.OK:
                    this.OkBtn(true);
                    break;
                case DialogButtons.Yes:
                    this.YesBtn(true);
                    break;
                case DialogButtons.No:
                    this.NoBtn(true);
                    break;
                case DialogButtons.Cancel:
                    this.CancelBtn(true);
                    break;
            }
        }
    }
    else
    {
        if (!args.NoCloseButton)
            this.CancelBtn(true);
    }


    this.NoCloseButton(args.NoCloseButton);


    this.ExButtonClick = function (item)
    {
        item.handler.apply(item.handlerContext);
    };


    if (args.ExButtons)
    {
        for (var i = 0; i < args.ExButtons.length; i++)
        {
            this.ExButtons.push(args.ExButtons[i]);
        }
    }

   

    
  
   
    if (typeof args.caption !== 'undefined')
        this.Caption(GetTxt( args.caption) );

    if (typeof args.width !== 'undefined')
        this.Width(args.width);
    else
        this.Width(0);

    if (typeof args.height !== 'undefined')
        this.Height(args.height);
    else
        this.Height(0);

  

    window.VPopup.OnClientResized();

    if (typeof args.templateId !== 'undefined')
    {
        var dlgContentHolder = this.DlgContentHolder;
        var errListHolder = '';
        if (args.model.HasErrors && args.model.ValidationErrors)
        {
            errListHolder = '';
        }
        dlgContentHolder.innerHTML = '<div data-bind=" template: { name: \'' + args.templateId + '\'}" ></div>';
        this.ContentModel = args.model;
        args.model.SelfCloseDelegate = { context: this, func: this.OnCancel };

        args.model.LoadData.call(args.model);

    }

   
     
};

VPopup.prototype.OnDialogDataLoaded = function ()
{
    this.Visible(true);
    this.OnClientResized();
};

VPopup.prototype.OnClientResized = function ()
{
    if (!window.VPopup.DlgBorder)
        return;
    if (!this.FullScreenMode)
    {
        window.VPopup.Left((window.GetClientWidth() / 2) - (window.VPopup.DlgBorder.clientWidth / 2));
        window.VPopup.Top((window.GetClientHeight() / 2) - (window.VPopup.DlgBorder.clientHeight / 2));

        $('#popupDlgBorder').removeClass('PopupDlgBorderFullScr');
        $('#popupCaption').show();
        $('#dlgButtons').show();
        $('#DlgContentHolder').css('padding', '0');

     
    }
    else
    {
        window.VPopup.Left(0);
        window.VPopup.Top(0);
        window.VPopup.Width($(document).width());
        window.VPopup.Height($(document).height());

        $('#popupDlgBorder').addClass('PopupDlgBorderFullScr');
        $('#popupCaption').hide();
        $('#dlgButtons').hide();
        $('#login-page-holder').height($(document).height());
        $('#DlgContentHolder').css('padding', '0');
        
    }

    var mainWaitImage = $('#dlgMainWaitingImage');
    var imgWidth = $(mainWaitImage).width();
    var imgHeight = $(mainWaitImage).height();
    $(mainWaitImage).offset(
        {
            top: window.VPopup.Top() + (window.VPopup.Height() / 2) - imgHeight / 2,
            left: window.VPopup.Left() + (window.VPopup.Width() / 2) - imgWidth / 2
        })

    if (this.ContentModel && this.ContentModel.OnResize)
    {
        this.ContentModel.OnResize();
    }

   
    
};



VPopup.prototype.OnCloseBtnMouseOver = function OnCloseBtnMouseOver(model)
{
    this.CloseImageUrl(skinFolderUrl + '/images/framework/dlg_close_btn_h.png');
};

VPopup.prototype.OnCloseBtnMouseOut = function OnCloseBtnMouseOut(model)
{
    this.CloseImageUrl(skinFolderUrl + '/images/framework/dlg_close_btn.png');
};

VPopup.prototype.OnOk = function ()
{
    if (this.ContentModel != null && this.ContentModel.DoOk != undefined)
    {
        if (this.ContentModel.DoOk())
            this.CloseInternal();
        else
        {
            if (this.ContentModel.HasErrors && this.ContentModel.HasErrors() == true)
            {
                $('#popupErrosList').addClass('flash_err_baloon');
                setTimeout(function ()
                {
                    $('#popupErrosList').removeClass('flash_err_baloon');
                }, 400);

                return;
            }
        }
    }
    else
        this.CloseInternal();
};

VPopup.prototype.OnYes = function ()
{
    if (this.ContentModel != null && this.ContentModel.DoYes != undefined)
    {
        if (this.ContentModel.DoYes())
            this.CloseInternal();
    }
    else
        this.CloseInternal();
};

VPopup.prototype.OnNo = function ()
{
    if (this.ContentModel != null && this.ContentModel.DoNo != undefined)
    {
        if (this.ContentModel.DoNo())
            this.CloseInternal();
    }
    else
        this.CloseInternal();
};

VPopup.prototype.OnCancel = function ()
{
    if (this.ContentModel && this.ContentModel.DoCancel)
    {
        if (this.ContentModel.DoCancel())
            this.CloseInternal();
    }
    else
        this.CloseInternal();
};

VPopup.prototype.CloseInternal = function ()
{
    if (this.ContentModel )
    {
        ko.cleanNode(document.getElementById('DlgContentHolder'));
        if (this.ContentModel.HasErrors)
            this.ContentModel.HasErrors(false);
        if (this.ContentModel.ValidationErrors)
            this.ContentModel.ValidationErrors.splice(0, this.ContentModel.ValidationErrors().length);

        if (this.ExButtons)
            this.ExButtons.removeAll();
   
        //setTimeout(function ()
        //{
        //    var dlgContentHolder = document.getElementById('DlgContentHolder');
        //    dlgContentHolder.innerHTML = '';
        //}, 300)
    }

   
    
    this.ContentModel = null;
    this.Visible(false);
};

VPopup.prototype.ShowMainWaitSpinner = function ()
{
    
    if (!this.MainWaiter)
        this.MainWaiter = new BusyIndicator('WorkingDlgIndicator', 55, '#FFFFFF');
    this.MainWaiter.Show();
};

VPopup.prototype.HideMainWaitSpinner = function ()
{
    this.MainWaiter.Hide();
};

VPopup.prototype.ShowDldWorking = function ()
{
    


    if (!this.Waiter)
        this.Waiter = new BusyIndicator('WorkingDlgIndicator', 25, '#000000');
    this.Waiting(true);
    this.Waiter.Show();
    

    //this.DlgIsWorkingVisible('block');
};

VPopup.prototype.HideDldWorking = function ()
{
    this.Waiter.Hide();
    this.Waiting(false);

    
    //this.DlgIsWorkingVisible('none');
};


function VPopup_ForParent() 
{
    this.CloseInternal = function () { };
};