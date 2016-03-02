'use strict';
function BLoginPanel(viewLayer, dataLayer, viewBorderLine, dataBorderLine, parent)
{
    /// <field name='ParentLayer' type='BRoot_PublicForChildren'></field>
    /// <field name='ViewLayer' type='VLoginPanel_PublicForBusiness'></field>

    this.PublicForData = new BLoginPanel_PublicForData();
    this.PublicForView = new BLoginPanel_PublicForView();
    this.PublicForParent = new BLoginPanel_PublicForParent();
    
    BusinessLayer.call(this, viewLayer, dataLayer, viewBorderLine, dataBorderLine, parent);

    this.CreateLoginPanel = function (a)
    {
        this.ViewLayer.CreateLoginPanel(a);
        //this.View.CreateLoginPanel(a);
    };

    this.DoLogoff = function (postData)
    {
        
        postData.WebRequestCallback = 'OnLoggedOff';
        postData.WebRequestUrl = logoffUrl;
        this.DataLayer.Request(postData);
    };

    this.OnLoggedOff = function (args)
    {
        if(args.Logoff && args.Logoff == 'ok')
        {
            if (window.pingTimerId)
            {
                clearInterval(window.pingTimerId);
                window.pingTimerId = null;
            }

            this.ViewLayer.OnLoggedOff();
        }
    };

    this.OnLogin = function (a)
    {
        this.View.OnLogin(a.UserName);
    };


    this.DoLogin = function (a)
    {
        this.Parent.ShowDialog(DialogIds.Login);
    };

    this.OnLoginOk = function (userName)
    {
        this.ViewLayer.OnLoginOk(userName);
    };
};

extend(BLoginPanel, BusinessLayer); 




function BLoginPanel_PublicForData()
{
    this.OnLogin = function (a) { };
    this.OnLoggedOff = function (a){ };
};

function BLoginPanel_PublicForView()
{
    this.DoLogin = function () { };
    this.DoLogoff = function (args) { };
};

function BLoginPanel_PublicForParent () 
{
    this.CreateLoginPanel = function () { };
    this.OnLoginOk = function (userName) { };
};