'use strict';
function DLoginForm(name)
{
    Layer.apply(this);
    this.PublicForBusiness = new DLoginForm_ForBusiness();

    this.GetData = function (postData)
    {
        postData.WebRequestUrl = getTemplateUrl;
        postData.PostData.RequiredTemplates = window.HtmlProvider.GetTemplatesIdsToLoad(RequiredTemplates.LoginForm)
        this.Request(postData);
    };

    this.TryLogin = function (postData)
    {
        postData.WebRequestUrl = loginUrl;
        this.Request(postData);
      
        //try
        //{
        //    var context = this;
        //    if (postData.WebRequestUrl == getTemplateUrl && (!postData.PostData.RequiredTemplates || postData.PostData.RequiredTemplates.length == 0))
        //    {
        //        this.BusinessLayer[postData.WebRequestCallback]({ Templates: [] });
        //        return;
        //    }
        //    postData.PostData.requiredTemplates = postData.PostData.RequiredTemplates;
        //    delete postData.PostData.RequiredTemplates;

        //    var waitId = this.ShortGuid();
        //    if (postData.WaitingId)
        //        waitId = postData.WaitingId;

        //    window.Waiting.ShowWaiter(waitId);

        //    $.ajax({
        //        cache: false,
        //        async: true,
        //        type: "POST",
        //        url: postData.WebRequestUrl,
        //        context: this,
        //        data: postData.PostData,
        //        success: function (data)
        //        {
        //            window.Waiting.HideWaiter(waitId);
        //            if (data == 'unauthorized')
        //            {
        //                var waitingAuthAction = {
        //                    args: args

        //                };

        //                context.BusinessLayer.OnUnauthorized(waitingAuthAction);

        //            }
        //            context.BusinessLayer[postData.WebRequestCallback](data, context);

        //        },
        //        error: function (jqXHR, textStatus, errorThrown)
        //        {
        //            window.Waiting.HideWaiter(waitId);

        //        }
        //    });


        //}
        //catch (ex)
        //{
        //    window.Waiting.HideWaiter(waitId);
        //    throw ex;

        //}
    };


   
      
};

extend(DLoginForm, DataLayer);



function DLoginForm_ForBusiness()
{
    this.GetData = function (postData) { };
    this.TryLogin = function (postData) { };
};



