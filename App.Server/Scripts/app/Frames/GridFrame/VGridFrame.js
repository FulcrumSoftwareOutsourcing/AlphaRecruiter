'use strict';
function VGridFrame()
{
    /// <field name='BusinessLayer' type='BGridFrame_ForView'/>

    VBaseFrame.call(this);
    this.UsedTemplatesIds = RequiredTemplates.GridFrame  ;
    this.PublicForChildren = new VGridFrame_ForChild();

    this.GridOrderedAttrs = null;

    this.Rows = ko.observableArray();
    
    this.PropInEdit = null;
    
    var isBinded = false;

    this.SelectedRow = null;

    
    this.Type = 'grid';
    

    this.base_FrameDataLoaded = this.FrameDataLoaded;
    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows,  templates, openMode)
    {
        this.EntityUsage = Metadata.GetEntityUsage(entityUsageId);
        
        
        this.AttrsInSet = attrsInSet;
        this.AttrsByIds = attrsByIds;
        this.GridOrderedAttrs = this.EntityUsage.GridOrderedAttributes;
        this.Rows.removeAll();
        this.Rows.pushAll(vRows);

        //if (vRows.length > 0)
        //{
        //    vRows[0].Selected = true;
        //    this.SelectedRowChanged(vRows[0])
        //}
        //else
        //{
        //    this.SelectedRowChanged([])
        //}
            

        
        this.base_FrameDataLoaded(entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode);
        
        if (this.EntityUsage.DisplayAllRecordsWithoutFooter)
        {
            $('#padingBar_' + this.TempId).hide();
            $('#padingBar_' + this.TempId).height(0);
        }

        this.Caption(this.GetFrameCaption());
        
        this.InitDateControls();

      
       
      
       
        
        

    };

    this.Refresh = function ()
    {
        this.BusinessLayer.Refresh();
        
    };

    this.GetGridControlTemplateName = function (row, attr)
    {
        var attr = this.AttrsByIds[attr];
        var valObj = row[attr];
        if (!App.Utils.IsStringNullOrWhitespace(attr.RowSourceId))
        {
            return ControlsTemplatesIds.RowSourceComboBox;
        }
        else
        {
            var template = ControlsTemplatesIds[attr.Type];
            if (!template)
                template = ControlsTemplatesIds.string;
            return template;
            
        }
    };


    this.Resize = function ()
    {
        var lp = $('#left').css('position');
        var lw = 0;
        if (lp != 'absolute')
           lw = $('#left').width();
        
        var sw = $('#splitter').width();

        var fw = GetClientWidth() - lw - sw - 0;
        if (fw < 700)
            fw = 700;
        $('#FrameHolder').width(fw);

        $('#filter_table' + this.TempId).width(fw - 30);
        $('#filter_table_adv' + this.TempId).width(fw - 30);

       
        

        if (App.Utils.IsStringNullOrWhitespace( this.OpenMode ) || this.OpenMode.indexOf('Child') == -1)
        {

            var h = $('#FrameHolder').height();
            var cb = $('#commandsBar_' + this.TempId).height();
            if (!cb || (this.EntityUsage.Commands && this.EntityUsage.Commands.length == 0))
                cb = 0;

            var fb = $('#filters_panel' + this.TempId).height();
            if (!fb || !this.EntityUsage.IsFilterEnabled)
                fb = 0;

            var pb = $('#padingBar_' + this.TempId).height();
            if (!pb || !this.EntityUsage.IsPagingEnabled)
                pb = 0;

            var hint = $('#grid_hint' + this.TempId)[0];
            var hh = 0;
            if (hint)
                hh = hint.clientHeight;

            //var tb = $('#totalBar_' + this.TempId).height();
            //if (!tb )
            //    tb = 0;

            $('#' + this.TempId + '_dataGridRoot').height(h - cb - fb - pb - hh  - 90);
        }
      
   
        
     
        


        



        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1)
        {
            $('[id^=rs_arrow_]').css("margin-top", -12);

        }
    


       

    }
    

    this.OnResize = function (appSizes)
    {
       
    };


    this.RowClick = function (row, propObj)
    {
        if (PressedKey != 'shift')
        {
            for (var i = 0; i < this.Rows().length; i++)
            {
                if (this.Rows()[i] != row)
                    this.Rows()[i].Selected(false);
            }
            this.SelectedRowChanged();
            if (row.Selected() && propObj.Editing())
                return;
            if (!row.Selected())
            {
                if (this.PropInEdit)
                    this.PropInEdit.Editing(false);

              

                row.Selected(true);
                this.SelectedRowChanged();
                return;
            }

            if (row.Selected())
            {
                if (this.PropInEdit)
                    this.PropInEdit.Editing(false);

                propObj.Editing(true);
                this.PropInEdit = propObj;
            }
        }
        else
        {
            if (!row.Selected())
            {
                row.Selected(true);
                this.SelectedRowChanged();
                return;
            }
            if (row.Selected())
            {
                if (this.PropInEdit)
                    this.PropInEdit.Editing(false);

                propObj.Editing(true);
                this.PropInEdit = propObj;
            }
        }
    };

    this.SelectedRowChanged = function (rows)
    {
        //var selected = [];
        //for (var i = 0; i < this.Rows().length; i++)
        //{
        //    if (this.Rows()[i].Selected)
        //        selected.push(this.Rows()[i]);
        //}

        this.BusinessLayer.SelectedItemChanged(rows);

        
    };

    this.HasChanges = function ()
    {
        var changes = false;
        if (this.Rows && this.Rows())
        {
            for (var i = 0; i < this.Rows().length; i++)
            {
                if (this.Rows()[i].HasChanges)
                {
                    changes = true;
                    break;
                }
            }
        }

        return changes;

    };


    this.RevertChanges = function ()
    {
        if (this.Rows && this.Rows())
        {
            for (var i = 0; i < this.Rows().length; i++)
            {
                this.Rows()[i].RevertChanges();
                
            }
        }

    };

    this.GetFrameCaption = function ()
    {
        if (!this.EntityUsage)
            return '';

        if (App.Utils.IsStringNullOrWhitespace(this.OpenMode))
            return this.EntityUsage.PluralCaption;

        if (this.OpenMode == NxOpenMode.Edit || this.OpenMode == NxOpenMode.ChildEdit)
            return GetTxt("Edit") + ' ' + this.EntityUsage.PluralCaption;
        if (this.OpenMode == NxOpenMode.View || this.OpenMode == NxOpenMode.ChildView)
            return GetTxt("View") + ' ' + this.EntityUsage.PluralCaption;
        if (this.OpenMode == NxOpenMode.New || this.OpenMode == NxOpenMode.ChildNew)
            return GetTxt("New") + ' ' + this.EntityUsage.PluralCaption;
    };
   
    this.DownloadLinkClicked = function (e)
    {
        
    };


    this.Dispose_base = this.Dispose;
    this.Dispose = function ()
    {
        this.Dispose_base();

        this.DataGrid.Dispose();
        this.PagingPnl.Dispose();
    };

};

extend(VGridFrame, VBaseFrame);


function VGridFrame_ForChild()
{
    VBaseFrame_ForChild.call(this);
    this.SelectedRowChanged = function (rs) { }

};


