'use strict';
function VSections()
{
    /// <field name='BusinessLayer' type='BSections_ForView'/>
    ViewLayer.call(this);
    this.PublicForBusiness = new VSections_ForBusiness();

    //this.Sections = ko.observableArray();

    var _setions = { All: [], ById: {} };

    var isBinded = false;


    var popupHolderEl = null;
    var mainMenuBtnEl = null;
    var menuContentEl = null;
    var me = this;


    this.CreateSections = function (sections)
    {
        
    //    this.Sections.removeAll();
      //  this.Sections.pushAll(sections);
        var selected;
        if (!isBinded)
        {
            var colors = [
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
                 '#186fcb',
                 '#f18e2a',
                 '#6d54c1',
                 '#1f8bab',
                 '#3ea048',
                 '#305899',
            ];


            popupHolderEl = document.getElementById('mmHolder');
            mainMenuBtnEl = document.getElementById('mainMenuBtn');
            menuContentEl = $(popupHolderEl).children().first();

            $(mainMenuBtnEl).click(MainMenuClick);
            $(window).click(function ()
            {
                $(popupHolderEl).hide();
                $('#logoutMenuHolder').hide();
                $('[class="GridColMenu"]').hide();
            })

            var itemTemplate = HtmlProvider.GetTemplateText('MSectionItem');
           
            

            _setions.All = sections;
            for (var i = 0; i < sections.length; i++)
            {
                var s = sections[i];
                _setions.ById[s.Id] = s;

                var imgUrl = '';
                var imageMeta = Metadata.GetImage(s.ImageId);
                if (imageMeta)
                    imgUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;
                else
                    imgUrl = '';

                var itemHtml = itemTemplate.replace('item-id', s.Id).replace('<!--item text-->', s.Text).replace('item_img', imgUrl).replace('bc-color', colors[i]);
                s.BgColor = colors[i];

                $(menuContentEl).append(itemHtml);
               
            }

            $(menuContentEl).children().click(ItemClick);
            

            Resizer.AddResizable('sections', new Delegate(Resize, this));



            //ko.applyBindings(this, document.getElementById('SectionsHolder'));
            isBinded = true;
        }


        var selectedId = window.Settings.GetSettings('app', 'sel_section_id');

        if (selectedId)
        {
            for (var i = 0; i < sections.length; i++)
            {
                if (sections[i].Id.toUpperCase() == selectedId.toUpperCase())
                {
                    selected = sections[i];
                    break;
                }
            }
        }
        else
        {
            for (var i = 0; i < sections.length; i++)
            {
                if (sections[i].IsDefault)
                    selected = sections[i];
            }
        }

        if (!selected && sections.length > 0)
        {
            selected = sections[0];
        }
        

        if (selected)
            this.SelectedSectionChanged(selected);

        

    //    window.ResizeDispatcher.OnClientResized();//////////////
        

    };

    function MainMenuClick(event)
    {
        event.stopPropagation();

        $('#settingsMenuHolder').hide();
        $('#logoutMenuHolder').hide();
        $('[class="GridColMenu"]').hide();
        


        if ($(popupHolderEl).css('display') == 'none')
        {
           
            $(popupHolderEl).show();
           
            
            
            Resize();
        }
        else
        {
            $(popupHolderEl).hide();
            
        }


        if ($('#left').css('position') == 'absolute')
        {
            $('#left').fadeOut(150);
        }
    }

    function ItemClick(event)
    {
        var iid = $(this).attr('i-id');
        var sect = _setions.ById[iid];
        
        Settings.AddChangedSettings('app', 'sel_section_id', iid);

        me.SelectedSectionChanged(sect);
        
    }

    function Resize()
    {
        var w = $(document).width();
        var childW = 0;
        var sects = $(menuContentEl).children();
        for (var i = 0; i < sects.length; i++)
        {
            childW += $(sects[i]).width() 
        }

        

        if (w > 940)
        {
            $(menuContentEl).children().removeClass('MmSectionSmall');
            $(menuContentEl).children().addClass('MmSection');
            //$(menuContentEl).width(850);
            return;
        }
        if (w <= 940 && w > 580)
        {
            $(menuContentEl).children().removeClass('MmSectionSmall');
            $(menuContentEl).children().addClass('MmSection');
            //$(menuContentEl).width(500);
            return;
        }
        if (w <= 580 && w > 460)
        {

            $(menuContentEl).children().removeClass('MmSection');
            $(menuContentEl).children().addClass('MmSectionSmall');

            //$(menuContentEl).width(350);
            return;
        }
        if (w <= 460)
        {

            $(menuContentEl).children().removeClass('MmSection');
            $(menuContentEl).children().addClass('MmSectionSmall');
            //$(menuContentEl).width(350);
            return;
        }

      //$(menuContentEl).width(225);
       

    }

    this.SelectedSectionChanged = function (section)
    {
        
        $('#sectionCaption').text(section.Text);
      //  $('#sectionCaption').css('background-color', section.BgColor);
        $('#mainMenuBtn').css('background-color', section.BgColor);
        
        this.BusinessLayer.SelectedSectionChanged(section);


    };

    var waitingSection ;
    this.OnSectionClick = function (section)
    {
        if (section.Selected())
            return;


        if (Navigation.CheckCurrentHasChanges())
        {
            waitingSection = section;
            ShowMessage([GetTxt('Frame has changes.'), GetTxt('Do you want to save changes?')], GetTxt('Question'), [DialogButtons.Yes, DialogButtons.No, DialogButtons.Cancel], NxMessageBoxIcon.Question, new Delegate(ContinueAfterSaveDialog, this));
        }
        else
        {
            SectionClicked(section);
        }

    };

    function SectionClicked(section)
    {
        for (var i = 0; i < section.Parent.Sections().length; i++)
        {
            section.Parent.Sections()[i].Selected(false);
        }
        section.Selected(true);

        section.Parent.SelectedSectionChanged.call(section.Parent, section)
    }

    function ContinueAfterSaveDialog(dlgResult)
    {
        if (dlgResult == 'yes')
        {
            Navigation.GetCurrentFrame().SaveChanges(new Delegate(ContinueAfterSaveDone, this));
        }
        if (dlgResult == 'no')
        {
            SectionClicked(waitingSection);
            waitingSection = null;
        }
        if (dlgResult == 'cancel')
        {
            waitingSection = null;
        }
    }

    function ContinueAfterSaveDone()
    {
        SectionClicked(waitingSection);
        waitingSection = null;
    }

    //this.OnResize = function (appSizes)
    //{
    //    ///<param name='appSizes' type='AppSizes' />
    //    appSizes.SectionsHeight = $('#SectionsHolder').height();
    //    appSizes.SectionsWidth = $('#SectionsHolder').width();

    //};
    //window.ResizeDispatcher.AddResizeListener(new Delegate(this.OnResize, this), 10);
    
};

extend(VSections, ViewLayer);

function VSections_ForBusiness()
{
    this.CreateSections = function (sections) { };
};