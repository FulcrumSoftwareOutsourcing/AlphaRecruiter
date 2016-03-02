'use strict';
function VMsgBox()
{
    ViewLayer.call(this);
    this.PublicForBusiness = new VMsgBox_forBusiness();

    this.Messages = ko.observableArray();
    this.ImageUrl = ko.observable();
    var resultDelegate = null;
    var dialogsCount = 0;

    this.Show = function ( text, caption, buttons, icon, msgResultDelegat)
    {
        dialogsCount++;
        resultDelegate = msgResultDelegat;
        this.Messages.removeAll();

        //for (var i = 0; i < 5; i++)
        //{

        //}

        this.Messages.pushAll(text);
        

      


        var imageMeta = Metadata.GetImage(icon);
        if (imageMeta && imageMeta.Folder && imageMeta.FileName)
            this.ImageUrl( imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName );

        if (VPopup.ContentModel != this)
        {
            window.VPopup.ShowDialog({
                templateId: HtmlTemplateIds.MessageBoxTemplate,
                model: this,
                caption: GetTxt(caption),
                Buttons: buttons,
            });
        }
        else
        {
            window.VPopup.ChangeButtons(buttons);
        }
        
    };

    

    this.DialogDataLoaded = function (data)
    {
        if (data)
        {
            HtmlProvider.AddTemplates(data.Templates);
        }

        

        ko.applyBindings(this, document.getElementById('DlgContentHolder'));
        window.VPopup.OnDialogDataLoaded.call(window.VPopup);

    };

    this.LoadData = function ()
    {
        this.BusinessLayer.GetData(HtmlProvider.GetTemplatesIdsToLoad([HtmlTemplateIds.MessageBoxTemplate]));
    };

    this.DoYes = function ()
    {
        if (resultDelegate)
            resultDelegate.Invoke('yes');
        return CanClose();
    };

    this.DoNo = function ()
    {
        if (resultDelegate)
            resultDelegate.Invoke('no');
        return CanClose();

    };

    this.DoCancel = function ()
    {
        if (resultDelegate)
            resultDelegate.Invoke('cancel');
        return CanClose();
    };

    this.DoOk = function ()
    {
        if (resultDelegate)
            resultDelegate.Invoke('cancel');
        return CanClose();
    };

    function CanClose()
    {
        if (dialogsCount <= 1)
        {
            dialogsCount = 0;
            return true;
        }
        else
        {
            dialogsCount = 0;
            return false;
        }
    };
};

extend(VMsgBox, ViewLayer);

function VMsgBox_forBusiness()
{
    this.DialogDataLoaded = function (data) { };
};