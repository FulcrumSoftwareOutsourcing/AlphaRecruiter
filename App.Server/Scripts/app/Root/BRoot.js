

function BRoot()
{
    
    /// <field name='DataLayer' type='DRoot_ForBusiness'></field>
    /// <field name='ViewLayer' type='VRoot_PublicForBusiness'></field>
    /// <field name='LoginPanel' type='BLoginPanel_PublicForParent'/>
    /// <field name='Sections' type='BSections_ForParent'/>
    /// <field name='Tree' type='BTree_ForParent'/>
    /// <field name='Frame' type='BFrame_ForParent'/>

    BusinessLayer.call(this);
    
    
    this.PublicForData = new BRoot_PublicForData();
    this.PublicForChildren = new BRoot_PublicForChildren();
    this.PublicForView = new BRoot_PublicForView();
    var selectedSection = null;
    var me = this;
    
    this.Run = function ()
    {
        var postData = new PostDataObject();
        postData.WebRequestCallback = "OnStartObjectLoaded";
        postData.PostData.RequiredTemplates = this.ViewLayer.GetRequiredTemplates();
        postData.PostData.workspaceId = Metadata.APPLICATION$CURRENTWORKSPACEID;
        this.DataLayer.GetStartObject(postData);
       
    };

    this.ShowWaiter = function (){}
    this.HideWaiter = function (){}
    window.Waiting.AddIndividualWaiter('Ping', this.ShowWaiter, this.HideWaiter, this);

    this.OnStartObjectLoaded = function (startObj)
    {
        

        Metadata.JsAppProperties = startObj.PortalMetadata.JsAppProperties;
        Metadata.Images = startObj.PortalMetadata.Images;
        Metadata.Sections = startObj.PortalMetadata.Sections;
        Metadata.Languages = startObj.PortalMetadata.Languages;
        Metadata.InitMultilanguage(startObj.PortalMetadata.MultilanguageItems);
       

        var logoImg1 = Metadata.JsAppProperties['logo_image1_id'];
        if (!App.Utils.IsStringNullOrWhitespace(logoImg1))
        {
            var imageMeta = Metadata.GetImage(logoImg1);
            if (imageMeta && imageMeta.Folder && imageMeta.FileName)
            {
                this.ViewLayer.ShowLogoImage1(imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName);
            }
        }

        if (!App.Utils.IsStringNullOrWhitespace(Metadata.JsAppProperties["app_logo_text"]))
        {
            this.ViewLayer.ShowLogoText(GetTxt(Metadata.JsAppProperties["app_logo_text"]));
        }

        //var logoImg2 = Metadata.JsAppProperties['logo_image2_id'];
        //if (!App.Utils.IsStringNullOrWhitespace(logoImg2))
        //{
        //    var imageMeta = Metadata.GetImage(logoImg2);
        //    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        //    {
        //        this.ViewLayer.ShowLogoImage2(imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName);
        //    }
        //}
        
        //var logoImg3 = Metadata.JsAppProperties['logo_image2_id'];
        //if (!App.Utils.IsStringNullOrWhitespace(logoImg3))
        //{
        //    var imageMeta = Metadata.GetImage(logoImg3);
        //    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        //    {
        //        this.ViewLayer.ShowLogoImage2(imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName);
        //    }
        //}
        
        Metadata.APPLICATION$WORKSPACEAVAILABLE = startObj.PortalMetadata.ApplicationValues.APPLICATION$WORKSPACEAVAILABLE.sortBy('DefaultOrder');
        Metadata.APPLICATION$CURRENTWORKSPACEID = startObj.PortalMetadata.ApplicationValues.APPLICATION$CURRENTWORKSPACEID;
        Metadata.APPLICATION$CLIENTDATEFORMAT = startObj.PortalMetadata.ApplicationValues.APPLICATION$CLIENTDATEFORMAT;
        Metadata.APPLICATION$CLIENTDATETIMEFORMAT = startObj.PortalMetadata.ApplicationValues.APPLICATION$CLIENTDATETIMEFORMAT;
        Metadata.APPLICATION$LANGUAGECODE = startObj.PortalMetadata.ApplicationValues.APPLICATION$LANGUAGECODE;
        Metadata.APPLICATION$MAXUPLOADSIZE = startObj.PortalMetadata.ApplicationValues.APPLICATION$MAXUPLOADSIZE;
        Metadata.APPLICATION$UploadPacketSize = startObj.PortalMetadata.ApplicationValues.APPLICATION$UploadPacketSize;

        for (var i = 0; i < Metadata.APPLICATION$WORKSPACEAVAILABLE.length; i++)
        {
            Metadata.APPLICATION$WORKSPACEAVAILABLE[i].Selected = false;
        }
        Metadata.GetWorkspace(Metadata.APPLICATION$CURRENTWORKSPACEID).Selected = true;
        Metadata.AddStaticRowsources(startObj.PortalMetadata.StaticRowsources);
        Metadata.AddFrames(startObj.PortalMetadata.Frames);
    
        
        
        

        this.Tree.ClearTree();
        this.ViewLayer.OnStartObjectLoaded(startObj);
        this.OnLoginOk(startObj);
        this.Sections.OnSectionsLoaded(Metadata.Sections);

        

        window.pingTimerId = setInterval(function ()
        {
            var postData = new PostDataObject();
            postData.WebRequestCallback = "PingResponse";
            postData.WebRequestUrl = pingUrl;
            postData.WaitingId = 'Ping';
            me.DataLayer.Request(postData);

        }, 300000);

       

        window.onbeforeunload = function (e)
        {

            var postData = new PostDataObject();
            postData.WebRequestCallback = "PingResponse";
            postData.WebRequestUrl = saveSettingsUrl;
            postData.WaitingId = 'Ping';
            me.DataLayer.Request(postData);
            //if( !window.IsDownload )
            //{
            //    return GetTxt('Warning: All unsaved data will be lost.');
            //}
            //window.IsDownload = false;
        };


    };

    this.PingResponse = function(data){}

    this.OnLoginOk = function (args)
    {
        this.LoginPanel.OnLoginOk(args.UserName);
        this.ViewLayer.OnLoginOk(args.Token);
        
    };

    this.SelectedSectionChanged = function (section)
    {
        selectedSection = section;
        if (this.ViewLayer.IsTreeItemsCreatedForSection(section.Id) == false)
        {
            this.Tree.CreateTreeItems(section.TreeItems, section.Id);
        }
        this.Tree.ShowTreeFor(section.Id);
        
    };

    this.WorkspaceChanged = function ()
    {
        var postData = new PostDataObject();
        postData.WebRequestCallback = "OnStartObjectLoaded";
        postData.PostData.workspaceId = Metadata.APPLICATION$CURRENTWORKSPACEID;
        this.DataLayer.GetStartObject(postData);
    };


    this.LanguageChanged = function (langCode)
    {
        ShowMessage([GetTxt('To change the application language reload is required.'), GetTxt('All unsaved data will be lost.'), GetTxt('Do you want to reload the application right now?')], GetTxt('Reload Request'), [DialogButtons.Yes, DialogButtons.No], NxMessageBoxIcon.Question,
            new Delegate(function (dlgResult)
            {
                if (dlgResult == 'yes')
                {
                    Settings.AddChangedSettings('app', 'UserLang', langCode);

                    var postData = new PostDataObject();
                    postData.WebRequestCallback = "SaveUserLangResponse";
                    postData.WebRequestUrl = saveSettingsUrl;
                    me.DataLayer.Request(postData);
                }
              
            }, this));
    };

    this.SaveUserLangResponse = function (data)
    {
        if (data == 'ok')
        {
            location.reload(true);
        }
    }

    this.OnSelectedTreeItemChanged = function (treeItem)
    {
        this.Frame.Run(treeItem);
    };

    this.SetEmptySection = function ()
    {

    };
};

extend(BRoot, BusinessLayer);







function BRoot_PublicForData()
{
    this.OnStartObjectLoaded = function (startObject) { };
    this.PingResponse = function (data) { }
    this.SaveUserLangResponse = function (data){}
};

function BRoot_PublicForChildren()
{
    this.OnLoginOk = function (userName) { };
    this.SelectedSectionChanged = function (section) { };
    this.WorkspaceChanged = function () { };
    
    this.OnSelectedTreeItemChanged = function () { };
    this.SetEmptySection = function () { };
};

function BRoot_PublicForView()
{
    this.WorkspaceChanged = function () { }
    this.LanguageChanged = function (langCode) { };
};



