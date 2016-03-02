

'use strict';
function BLoginForm(name)
{
    /// <field name='ViewLayer' type='IVLoginForm_ForBusiness'></field>
    /// <field name='DataLayer' type='DLoginForm_ForBusiness'></field>
    /// <field name='ParentLayer' type='BRoot_PublicForChildren'></field>

    BusinessLayer.call(this);

    this.Name = name;
    this.PublicForParent = new IBLoginForm_ForParent();
    this.PublicForView = new IBLoginForm_ForView();
    this.PublicForData = new IBLoginForm_ForData();
    var loginInfo = {UserName: '',  Password: '', RememberMe: false};

    var _context = null;
    var _postData = null;
    var waitingServerResp = false;

    this.GetData = function (postData)
    {
        //postData.WebRequestCallback = "DialogDataLoaded";
        //postData.WaitingId = WaitersIds.Popup;
        
        //this.DataLayer.GetData(postData);

        this.DialogDataLoaded();
    };

    this.DialogDataLoaded = function (data)
    {
        loginInfo.UserName = '';
        loginInfo.Password = '';
        loginInfo.RememberMe = true;

        this.ViewLayer.DialogDataLoaded(loginInfo, new LoginInfoValidator());
    };

    var LoginInfoValidator = function ()
    {
        this.Validate_UserName = function (value)
        {
            var errors = [];
            if (App.Utils.IsStringNullOrWhitespace(value))
                errors.push(GetTxt('The User name field is required.'));

            return errors;
        };

        this.Validate_Password = function (value)
        {
            var errors = [];
            if (App.Utils.IsStringNullOrWhitespace(value))
                errors.push(GetTxt('Password field is required.'));

            return errors;
        };

        this.ValidateAll = function (values)
        {
            for (var i = 0; i < values.length; i++)
            {
                var val = values[i];
                var vilidationMethod = this['Validate_' + val.Name];
                if (vilidationMethod)
                    val.Errors = vilidationMethod(val.Value);
            }
            return values;
        };
    };

    this.TryLogin = function (loginInfo, tokenDescr)
    {
        if (waitingServerResp == true)
            return;

        try
        {
            var postData = new PostDataObject();
            postData.PostData = loginInfo;
            postData.PostData[tokenDescr.name] = tokenDescr.value;
            postData.WebRequestCallback = 'OnLogin';
            postData.WaitingId = WaitersIds.DldWorkingWaiter;
            this.DataLayer.TryLogin(postData);
        }
        catch (ex)
        {
            waitingServerResp = true;
        }
    };

    this.OnLogin = function (a)
    {
        waitingServerResp = false;

        if (a.Errors != undefined )
            this.ViewLayer.ShowLoginServerErrors(a.Errors);
        else
        {
            this.ParentLayer.OnLoginOk(a);
            if (_context && _postData)
            {
                _context.DataLayer.Request.apply(_context.DataLayer, [_postData]);
            }
            _context = null;
            _postData = null;
        }
    };

    this.KeepLoginContext = function (context, postData)
    {
        _context = context;
        _postData = postData;
    };

   
   

};

extend(BLoginForm, BusinessLayer);












function IBLoginForm_ForData() { };
IBLoginForm_ForData.prototype.DialogDataLoaded = function (args) { };
IBLoginForm_ForData.prototype.OnLogin = function (args) { };

function IBLoginForm_ForParent() { };
IBLoginForm_ForParent.prototype.ShowDialog = function (args) { };

function IBLoginForm_ForView()
{
    this.GetData = function (args) { };
    this.TryLogin = function (loginInfo, tokenDescr) { };
    this.KeepLoginContext = function (context, postData) { };
    
};
