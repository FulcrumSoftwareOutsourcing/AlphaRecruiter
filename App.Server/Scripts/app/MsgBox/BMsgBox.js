'use strict';
function BMsgBox()
{
    BusinessLayer.call(this);
    this.PublicForView = new BMsgBox_forView();
    this.PublicForData = new BMsgBox_forData();

    this.GetData = function (templatesToLoad)
    {
        if (templatesToLoad.length)
        {
            var postData = new PostDataObject();
            postData.WebRequestUrl = getTemplateUrl;
            postData.WebRequestCallback = "DialogDataLoaded";
            postData.WaitingId = WaitersIds.DldWorkingWaiter;
            postData.PostData.RequiredTemplates = [HtmlTemplateIds.MessageBoxTemplate];
            this.DataLayer.Request(postData);
        }
        else
        {
            this.DialogDataLoaded();
        }

        
    };

    this.DialogDataLoaded = function (data)
    {
        this.ViewLayer.DialogDataLoaded(data);
    };

 

};

extend(BMsgBox, BusinessLayer);


function BMsgBox_forView()
{
    this.GetData = function (templatesToLoad) { };
};

function BMsgBox_forData()
{
    this.DialogDataLoaded = function (data) { };
};