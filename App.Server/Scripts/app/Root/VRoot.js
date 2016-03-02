function VRoot()
{
    /// <field name='BusinessLayer' type='BRoot_PublicForView'></field>
    /// <field name='Popup' type='VPopup_ForParent'></field>
    /// <field name='LoginPanel' type='VLoginPanel_PublicForParent'></field>
    /// <field name='Tree' type='VTree_ForParent'></field>
    /// <field name='Workspaces' type='VWorkspacePnl_ForParent'></field>

    ViewLayer.call(this);
   
    

    

    this.PublicForChildren = new VRoot_ForChildren();
    this.PublicForBusiness = new VRoot_PublicForBusiness();

    var topElement = document.getElementById('top');
    var leftElement = document.getElementById('left');
    var splitterElement = document.getElementById('splitter');
    var rightElement = document.getElementById('right');
    var me = this;
    
    Resizer.AddResizable("Root", new Delegate(function ()
    {
        $('#settingsMenuHolder').hide();



    }, this));

    $(window).click(function ()
    {
        $('#settingsMenuHolder').hide();
        $('[class="GridColMenu"]').hide();
    });

    var binded = false;
    this.OnStartObjectLoaded = function (args)
    {
        window.HtmlProvider.AddTemplates(args.Templates);
        //this.Workspaces.CreatePanel();

        if (!binded)
        {
            $('#miLang').text('Language');
            $('#miWorkspaces').text('Workspaces');
            $('#settingsBtn').click(function (e)
            {

                var lpW = $('#loginPanelHolder').width();
                var sbW = $('#settingsBtn').width();

                e.stopPropagation();
                $('#logoutMenuHolder').hide();
                $('#mmHolder').hide();

                var holder = $('#settingsMenuHolder');
                $(holder).css('display') == 'none' ? $(holder).css('display', 'block') : $(holder).css('display', 'none');

                var menu = $(holder).children().first();
                var w = $(menu).width();


                $(menu).css('margin-left', '-' + (w + lpW + sbW - 30) + 'px');

                var arr = $(holder).children()[1];
                var w = $(arr).width();
                $(arr).css('margin-left', '-' + (w + lpW + sbW - 20 ) + 'px');

            })

            

            binded = true;
        }

        CreateWorkspacesList();

        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1)
        {
            $('#loginPanelHolder').css("margin-top", -57);
            $('#settingsBtn').css("margin-top", -57);
            $('#settingsBtn').css("padding-top", 13);

            
            
        }

        $('#top').show();

    };

    var langMenuCreated = false;
    function CreateWorkspacesList()
    {
        for (var i = 0; i < Metadata.APPLICATION$WORKSPACEAVAILABLE.length; i++)
        {
            var w = Metadata.APPLICATION$WORKSPACEAVAILABLE[i];
            $('#wrkspc_' + w.WorkspaceId).remove();
        }


        var selW = Metadata.GetWorkspace(Metadata.APPLICATION$CURRENTWORKSPACEID);
        
        var menu = $('#miWorkspaces');
        for (var i = 0; i < Metadata.APPLICATION$WORKSPACEAVAILABLE.length; i++)
        {
            var w = Metadata.APPLICATION$WORKSPACEAVAILABLE[i];
            var classSelected = w.WorkspaceId == selW.WorkspaceId ? 'UserMenuItemSelected' : '';
            var html = '<div id="wrkspc_' + w.WorkspaceId + '" class="UserMenuItem ' + classSelected + '" >' + w.Name + '</div>';
            $(menu).after(html);
            $('#wrkspc_' + w.WorkspaceId).click(WorkspaceClicked);
        }

        var menu = $('#miLang');
        if (!langMenuCreated)
        {
            for (var i = 0; i < Metadata.Languages.length; i++)
            {
                var w = Metadata.Languages[i];
                var classSelected = w.IsSelected ? 'UserMenuItemSelected' : '';
                var html = '<div id="lang_' + w.LanguageCd + '" class="UserMenuItem ' + classSelected + '" >' + w.Name + '</div>';
                $(menu).after(html);
                $('#lang_' + w.LanguageCd).click(LanguageClicked);
            }
            langMenuCreated = true;
        }
        else
        {
            for (var i = 0; i < Metadata.Languages.length; i++)
            {
                var w = Metadata.Languages[i];
                var classSelected = w.IsSelected ? 'UserMenuItemSelected' : '';
                var mi = $('#lang_' + w.LanguageCd);
                $(mi).removeClass('UserMenuItemSelected');
                if(w.IsSelected)
                    $(mi).addClass('UserMenuItemSelected');
            }
            langMenuCreated = true;
        }

    }

    function WorkspaceClicked(e)
    {
        var id = e.target.id.replace('wrkspc_', '');
        var selW = Metadata.GetWorkspace(Metadata.APPLICATION$CURRENTWORKSPACEID);
        if (('' + id) == ('' == selW.WorkspaceId))
            return;

        Metadata.APPLICATION$CURRENTWORKSPACEID = new Number( id );
        me.BusinessLayer.WorkspaceChanged();

        
    };

    function LanguageClicked(e)
    {
        var id = e.target.id.replace('lang_', '');
      
        me.BusinessLayer.LanguageChanged(id);


    };


    this.OnLoginClick = function ()
    {
        this.LoginForm.Show();
        
    };

    this.OnChildPartCreated = function (partName)
    {
        
    };

    //this.OnResize = function (appSizes)
    //{
    //    ///<param name='appSizes' type='AppSizes' />
    //    appSizes.TopHeight = $(topElement).height();

    //};
    //window.ResizeDispatcher.AddResizeListener(new Delegate( this.OnResize, this) , 0);
    
    this.OnLoginOk = function (token)
    {
        $('#top').show();
        if (token)
        {
            App.Utils.AddRequestVerificationToken(token);
        }

        this.Popup.CloseInternal();
    };

    this.IsTreeItemsCreatedForSection = function (sectionId)
    {
        return this.Tree.IsTreeItemsCreatedForSection(sectionId);
    };

    this.GetRequiredTemplates = function ()
    {
        return window.HtmlProvider.GetTemplatesIdsToLoad(RequiredTemplates.Portal);
    };

    
    this.ShowLogoImage1 = function (url)
    {
        $('#topLogo').children('img').attr('src', url).show();
    }

    this.ShowLogoText = function (text)
    {
        $('#topLogo').children('span').text(text).show();
    }

    this.ShowLogoImage2 = function (url)
    {
       // $('#logo').css('background', url);
    }

    this.ShowLogoImage3 = function (url)
    {
      //  $('#login-page-holder').css('background', url);
    }


    this.SetEmptySection = function ()
    {

    };


};
extend(VRoot, ViewLayer);


VRoot.prototype.Customer = null;


function VRoot_PublicForBusiness()
{
    this.OnStartObjectLoaded = function (startObject) { };
    this.OnResize = function () { };
    this.OnLoginOk = function (userName) { };
    this.IsTreeItemsCreatedForSection = function (sectionId) { };
    this.GetRequiredTemplates = function () { };
    this.ShowLogoImage1 = function (url) { }
    this.ShowLogoText = function (text) { }

    this.ShowLogoImage2 = function (url) { }
    this.ShowLogoImage3 = function (url) { }

    this.SetEmptySection = function ()
    {

    };
};

function VRoot_ForChildren()
{
    this.OnLoginClick = function () { };
};


