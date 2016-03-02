'use strict';
function VUploadDlg()
{
    ViewLayer.call(this);
    this.PublicForBusiness = new VUploadDlg_ForBusiness();


    this.UploadedText = ko.observable('');
    this.UploadedSpeed = ko.observable('');
    this.FileName = ko.observable('');
    this.UploadPercent = ko.observable('0%');

    var isDataLoaded = false;
    var fileSize = 0;
    
    this.FileChanged = function (fileName, size)
    {
        fileSize = size;

        this.UploadedText(GetTxt('Uploaded:') + ' ' + App.Utils.HumanFileSize(0, true) + ' from ' + App.Utils.HumanFileSize(fileSize, true));
        this.UploadedSpeed(GetTxt('Upload speed kb/s') + ': 0');
        this.FileName(fileName);
        this.UploadPercent ('0%');
        $('#progressColorBar').width(0);

    };

    this.NextChunkUploaded = function (bytesUploaded, speed, percentUploaded)
    {
        this.UploadedText(GetTxt('Uploaded:') + ' ' + App.Utils.HumanFileSize(bytesUploaded, true) + ' from ' + App.Utils.HumanFileSize(fileSize, true));
        this.UploadedSpeed(GetTxt('Upload speed kb/s') + ': ' + speed);
        this.UploadPercent(percentUploaded + '%');

        var barWidth = $('#progressBar').width();
        
        $('#progressColorBar').width(barWidth * percentUploaded / 100);
    };

    this.ShowDialog = function ()
    {
        
        this.UploadedText('');
        this.UploadedSpeed('');

        

        window.VPopup.ShowDialog({
            templateId: HtmlTemplateIds.UploadDlgTemplate,
            model: this,
            caption: GetTxt('Uploadng file') + ' ' + file.name,
            Buttons: [DialogButtons.Cancel],
            width: 250,
            height: 200
        });
    };

    this.LoadData = function ()
    {
        if (!isDataLoaded)
        {
            var postData = new PostDataObject();
            //postData.WaitingId = WaitersIds.DialogDataLoading;
            postData.PostData.RequiredTemplates = window.HtmlProvider.GetTemplatesIdsToLoad(RequiredTemplates.UploadDlg);
            this.BusinessLayer.GetData(postData);
        }
        else
            this.DialogDataLoaded();
    };

    this.DialogDataLoaded = function (data)
    {
        if (!isDataLoaded)
        {
            window.HtmlProvider.AddTemplates(data.Templates);
            isDataLoaded = true;
        }

        ko.applyBindings(this, document.getElementById('DlgContentHolder'));


        window.VPopup.OnDialogDataLoaded.call(window.VPopup);






    };


  

    this.DoCancel = function ()
    {
        this.BusinessLayer.StopUpload();
        this.Clear();
        return true;
    };



    this.Clear = function ()
    {
        this.UploadedText('') ;
        this.UploadedSpeed('');
        this.FileName('');
        this.UploadPercent('');
    };

    this.Close = function ()
    {
      
    };


}

extend(VUploadDlg, ViewLayer);

function VUploadDlg_ForBusiness()
{
    this.ShowDialog = function () { };
    this.DialogDataLoaded = function (data) { };
    this.LoadData = function () { };
    this.Clear = function () { };


    this.Close = function () { };

    this.FileChanged = function (fileName, size) { };
    this.NextChunkUploaded = function (bytesUploaded, speed) { };
    this.DoCancel = function (){ }
};