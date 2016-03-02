'use strict';
function DataLayer()
{
    Layer.call(this);
    /// <field name='BusinessLayer' type='BusinessLayer'></field>
};

extend(DataLayer, Layer);

DataLayer.prototype.Request = function (postData)
{
    /// <param name='postData' type='PostDataObject'></field>
    try
    {
        var context = this;
        if ( postData.WebRequestUrl == getTemplateUrl && (! postData.PostData.RequiredTemplates || postData.PostData.RequiredTemplates.length == 0 ) )
        {
            this.BusinessLayer[postData.WebRequestCallback]({ Templates: [] });
            return;
        }
        if (postData.PostData.RequiredTemplates)
            postData.PostData.requiredTemplates = postData.PostData.RequiredTemplates;
        delete postData.PostData.RequiredTemplates;

        var waitId = this.ShortGuid();
        if ( postData.WaitingId )
            waitId = postData.WaitingId;

        var changedSettings = Settings.GetSettingsToSave();
        if (changedSettings)
            postData.PostData.settingsToSave = JSON.stringify(changedSettings);
        

        window.Waiting.ShowWaiter(waitId);

        


        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            //dataType: "json",
            url: postData.WebRequestUrl,
            context: this,
            //contentType: "application/json;charset=utf-8",
            //data: JSON.stringify(postData.PostData),
            data: postData.PostData,
            traditional: true,
            success: function (data, status, xhr)
            {
               // try
              //  {
                if (data.Error)
                {
                    window.Waiting.HideWaiter(waitId);
                    ShowMessage([data.Error], GetTxt('Error'), [DialogButtons.OK], NxMessageBoxIcon.Error);
                    //alert(data.Error);
                }
                else if ($.type(data) === "string" && data.indexOf('unauthorized') > -1)
                    {
                        var waitingAuthAction = {
                            postData: postData
                        };

                        data = data.replace( 'unauthorized', '');

                        context.BusinessLayer.OnUnauthorized(postData, data);
                    }
                    else
                {
                    if (data.Error)
                    {
                        ShowMessage([data.Error], GetTxt('Error'), [DialogButtons.OK], NxMessageBoxIcon.Error);
                    }
                    
                    var filename = GetFileName(data, status, xhr);
                    if (!App.Utils.IsStringNullOrWhitespace(filename))
                    {
                        window.Waiting.HideWaiter(waitId);
                        DownloadFile(filename, data, xhr);
                        return;
                    }
                    else
                    {
                        if(data.Settings)
                            Settings.SettingsLoaded(data.Settings);
                        context.BusinessLayer[postData.WebRequestCallback](data, context);
                    }
                    
                    }
             //   }
               // catch (ex)
              //  {
                //   window.Waiting.HideWaiter(waitId);
                 //   throw ex;
            //    }

                window.Waiting.HideWaiter(waitId);

            },
            error: function (jqXHR, textStatus, errorThrown)
            {
                window.Waiting.HideWaiter(waitId);
            }
        });


    }
    catch (ex)
    {
        window.Waiting.HideWaiter(waitId);
        throw ex;

    }


    function GetFileName(response, status, xhr)
    {
        var filename = "";
        var disposition = xhr.getResponseHeader("Content-Disposition");
        if (disposition && disposition.indexOf("attachment") !== -1)
        {
            var filenameRegex = /filename[^;=\n]*=(([""]).*?\2|[^;\n]*)/;
            var matches = filenameRegex.exec(disposition);
            if (matches != null && matches[1])
                filename = matches[1].replace(/[""]/g, "");
        }

        return filename;
    }

    function DownloadFile(filename, response, xhr)
    {
        var type = xhr.getResponseHeader("Content-Type");
        var blob = new Blob([response], { type: type });

        if (typeof window.navigator.msSaveBlob !== "undefined")
        {
            // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed.
            window.navigator.msSaveBlob(blob, filename);
        } else
        {
            var URL = window.URL || window.webkitURL;
            var downloadUrl = URL.createObjectURL(blob);

            if (filename)
            {
                // Use HTML5 a[download] attribute to specify filename.
                var a = document.createElement("a");
                // Safari doesn"t support this yet.
                if (typeof a.download === "undefined")
                {
                    window.location = downloadUrl;
                } else
                {
                    a.href = downloadUrl;
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                }
            } else
            {
                window.location = downloadUrl;
            }

            setTimeout(function ()
            {
                URL.revokeObjectURL(downloadUrl);
            }, 100); // Cleanup
        }

    }

};

DataLayer.prototype.ShortGuid = function ()
{
    return App.Utils.ShortGuid();
};

DataLayer.prototype.BusinessLayer = null;
DataLayer.prototype.PublicForBusiness = null;


