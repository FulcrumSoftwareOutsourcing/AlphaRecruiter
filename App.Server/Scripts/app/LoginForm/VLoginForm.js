'use strict';
function VLoginForm(name)
{
    /// <field name='LoginInfo' type='ObservableObject'></field>
    /// <field name='BusinessLayer' type='IBLoginForm_ForView'></field>
    this.PublicForBusiness = new IVLoginForm_ForBusiness();
    this.PublicForParent = new VLoginForm_ForParent();
    
    this.FormSubject = window.GetTxt("Log in")
    this.ValidationErrors = ko.observableArray([]);
    this.HasErrors = ko.observable(false);
    
    

    var isDataLoaded = false;

    this.LoginInfo = null;
    
   

    this.DialogDataLoaded = function (data)
    {
        if (!isDataLoaded)
        {
            //window.HtmlProvider.AddTemplates(data.Templates);
            this.LoginInfo = data.LoginInfo;
            this.LoginInfo.HasErrorsChangedDelegate = new Delegate(ValidationErrorsChanged, this);
            isDataLoaded = true;

            

            
        }
            
        $(document).on( 'keypress', KeyPress);


        ko.applyBindings(this, document.getElementById('DlgContentHolder'));
        window.VPopup.OnDialogDataLoaded.call(window.VPopup);
        
        
    };

    function KeyPress(e)
    {
        if (e.which == 13)
        {
            $('#loginBtn').focus();
            VPopup.OnOk.call(VPopup);
        }
    }

    this.OnDialogVisible = function ()
    {
        this.LoginInfo.FocusFirstField();
    };

    this.Show = function (context, postData)
    {
        $('#top').hide();

        if (this.LoginInfo)
        {
            this.LoginInfo.Validate = false;
            this.LoginInfo.UserName('');
            this.LoginInfo.Password('');
            this.LoginInfo.RememberMe(true);
        }


        this.BusinessLayer.KeepLoginContext(context, postData);

        window.VPopup.ShowDialog({
            templateId: HtmlTemplateIds.LoginFormTemplate,
            model: this,
            caption: GetTxt( 'Log in' ),
            ExButtons: [{ text: GetTxt('Log in'), handler: this.LoginClick, handlerContext: this, id: 'btnLogin' }],
            NoCloseButton: true,
            _width: 100,
            _height: 380,
            FullScreen : true

        });

        this.LoginInfo.Validate = true;
    };

    this.LoginClick = function ()
    {
        VPopup.OnOk.call(VPopup);
    };

    this.LoadData = function ()
    {
        if (!isDataLoaded)
        {
            var postData = new PostDataObject();
            postData.PostData.RequiredTemplates = window.HtmlProvider.GetTemplatesIdsToLoad(RequiredTemplates.LoginForm);
            this.BusinessLayer.GetData(postData);
        }
        else
            this.DialogDataLoaded();
    };

    this.DoCancel = function ()
    {
        ko.cleanNode(document.getElementById('DlgContentHolder'));
        ClearAll(this);

        return true;
    };

    this.Close = function ()
    {
        ClearAll(this);
        this.SelfCloseDelegate.func.call(this.SelfCloseDelegate.context);
    };

    function ClearAll(context)
    {
        $(document).off ('keypress');
        if (context && context.LoginInfo)
        {
           

            context.LoginInfo.Password('');
            context.LoginInfo.Password.HasErrors(false);
            context.LoginInfo.Password.HasFocus(false);
            context.LoginInfo.Password.ErrorMessages.removeAll();

            context.LoginInfo.RememberMe(false);

            context.HasErrors(false);
            context.ValidationErrors.removeAll();
        }
    };
    
    function ValidationErrorsChanged()
    {
       

        this.ValidationErrors.removeAll();
        this.HasErrors(false);
        
        for (var i = 0; i < this.LoginInfo.ObservablePropsNames.length; i++)
        {
            var propName = this.LoginInfo.ObservablePropsNames[i];
            if (this.LoginInfo[propName].HasErrors())
            {
                this.LoginInfo[propName].HasFocus(false);
                this.ValidationErrors.pushAll(this.LoginInfo[propName].ErrorMessages());
                this.HasErrors(true);
            }   
        }
        this.LoginInfo.FocusFirstInvalidField();
        

    };

    this.DoOk = function ()
    {
        if (!this.LoginInfo.ValidateAll())
        {
            this.LoginInfo.FocusFirstInvalidField();
            return false;
        }

        this.BusinessLayer.TryLogin( this.LoginInfo, App.Utils.GetRequestVerificationToken() );
        

        return false;
    };

    this.ShowLoginServerErrors = function  (errors) 
    {
        this.LoginInfo.ShowErrors(errors);
        //$('#popupErrosList').addClass('flash_err_baloon');
        //setTimeout(function ()
        //{
        //    $('#popupErrosList').removeClass('flash_err_baloon');
        //}, 400);
    };

    var me = this;
    $(document).keydown(function (e)
    {
        if (e.keyCode == 13)
        {
            $('#btnLogin').focus();
        }
        

    });

  

    this.OnResize = function ()
    {
        
        $('#login-column').css({ height: $(window).innerHeight() });

        //if ($(window).innerHeight() > 630)
        //{
        //    $('#login-column').css({ height: $(window).innerHeight() });
        //}
        //else
        //{
        //    if ($(window).innerWidth() > 320)
        //    {
        //        $('#login-column').css({ height: 'auto' });
        //    }
        //}
        
        $('#login-column').css({ left: $(window).innerWidth() - $('#login-column').width() });

    };


};

extend(VLoginForm, ViewLayer);

function IVLoginForm_ForBusiness()
{
    this.DialogDataLoaded = function (data) { };
    this.ShowLoginServerErrors = function (errors) { };
};

//IVLoginForm_ForBusiness.prototype.ShowVilidationErrors = function (a) { };
//IVLoginForm_ForBusiness.prototype.Close = function (a) { };


function VLoginForm_ForParent()
{
    this.Show = function () { };
};