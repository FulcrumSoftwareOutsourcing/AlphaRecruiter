'use strict';
function VLoginPanel()
{
    /// <field name='BusinessLayer' type='BLoginPanel_PublicForView'></field>
    /// <field name='ParentLayer' type='VHome_ForChildren'></field>
    

    this.PublicForBusiness = new VLoginPanel_PublicForBusiness();
    this.PublicForParent = new VLoginPanel_PublicForParent();

    this.UserName = ko.observable('');
    var RootElement = document.getElementById('loginPanelHolder');
    var waitingServerResp = false;

    var me = this;

    this.CalcUserNameVisible = function ()
    {
        if ((me.UserName() != null && me.UserName() != '') &&
            App.Utils.IsMobile() == false && $(document).width() > 1000)
        {
            me.UserLogedVisibilityUserName(true);
            $('#miUserName').hide();
            
        }
        else
        {
            me.UserLogedVisibilityUserName(false);
            $('#miUserName').show();
            if (!App.Utils.IsStringNullOrWhitespace(me.UserName()))
            {
                $('#miUserName').text(me.UserName());
            }
        }
    };

    $(window).click(function ()
    {
        $('#logoutMenuHolder').hide();
        
    })


    this.ShowWaiter = function()
    {
        waitingServerResp = true;
        waiter.Show();
    };

    this.HideWaiter = function()
    {
        waiter.Hide();
        waitingServerResp = false;
    };

    window.Waiting.AddIndividualWaiter('Logoff', this.ShowWaiter, this.HideWaiter, this);

    

    this.UserLogedVisibility = ko.computed(function ()
    {
        if (me.CalcUserNameVisible)
            me.CalcUserNameVisible();
        return (this.UserName() != null && this.UserName() != '') ? "block" : "none";
    }, this);

    

    

    this.GuestVisibility = ko.computed(function ()
    {
        return (this.UserName() == null || this.UserName() == '') ? "block" : "none";
    }, this);

    
   

  //  this.CreateLoginPanel = function (a)
  //  {
        
        //AppSizes.LoginPanelHeight = RootElement.clientHeight;
        //AppSizes.LoginPanelWidth = RootElement.clientWidth;
        //this.ParentLayer.OnChildPartCreated('loginPnl');
   // };

    this.OnLoginClick = function ()
    {
        this.ParentLayer.OnLoginClick();
    };

    this.OnRegisterClick = function ()
    {
        this.ParentLayer.OnRegisterClick();
    };

    this.GetClientWidth = function ()
    {
        return RootElement.clientWidth;
    };

    this.OnLogoffClick = function ()
    {
        if (waitingServerResp)
            return;

        
        var token = App.Utils.GetRequestVerificationToken();


        var postData = new PostDataObject();
        postData.PostData[token.name] = token.value;
        postData.WaitingId = 'Logoff';
        this.BusinessLayer.DoLogoff(postData);
    };

    this.OnUserMenuClick = function ()
    {
        $('[class="GridColMenu"]').hide();
        $('#settingsMenuHolder').hide();
        $('#mmHolder').hide();

        var holder = $('#logoutMenuHolder');
        $(holder).css('display') == 'none' ? $(holder).css('display', 'block') : $(holder).css('display', 'none');

        var menu = $(holder).children().first();
        var w = $(menu).width();

        $(menu).css('margin-left', '-' + (w + 10) + 'px');

      

    };

    this.OnLoginOk = function (userName)
    {
        this.UserName(userName);
    };

    this.OnLoggedOff = function ()
    {
        document.cookie = '';
        location.reload(true);
    };

    window.Resizer.AddResizable('LoginPanel', new Delegate(function ()
    {
        if (me.CalcUserNameVisible)
            me.CalcUserNameVisible();

        $('#logoutMenuHolder').hide();

        
    }, this));

    

    ko.applyBindings(this, document.getElementById('loginPanelHolder'));

    var waiter = new BusyIndicator('WorkingLogoutIndicator', 16, '#FFFFFF');

    $('#miLogout').text(GetTxt('Logout'));
    $('#miAccount').text(GetTxt('My Account'));

    $('#miLogout').click(function ()
    {
        me.OnLogoffClick.call(me);
    });

    $('#miAccount').click(function (e)
    {
        e.stopPropagation();
    });

};

extend(VLoginPanel, ViewLayer);

VLoginPanel.prototype.UserName = null;
VLoginPanel.prototype.UserLogedVisibility = null;
VLoginPanel.prototype.UserLogedVisibilityUserName = ko.observable(false);




function VLoginPanel_PublicForBusiness()
{
    this.OnLoginOk = function (userName) { };
    this.OnLoggedOff = function (){};
};

function VLoginPanel_PublicForParent()
{
    this.GetClientWidth = function () { };
};