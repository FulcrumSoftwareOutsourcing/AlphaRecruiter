'use strict';
function VAutoLayoutFrame()
{
    VBaseFrame.call(this);
    this.PublicForBusiness = new VAutoLayoutFrame_ForBusiness();
    this.UsedTemplatesIds = RequiredTemplates.AutoLayoutFrame;
    this.Layout = ko.observableArray();
    this.OpenMode = null;
    
    this.ChildFrames = { All: [], WoTabs: [], ByTabs: {}, ByIds: {} };

    var layoutCreated = false;

    this.IsNewEntity = false;

    this.Type = 'auto-layout';

    this.CreateLayout = function (layout)
    {
        if (!layoutCreated)
        {
            if (this.CurrentEntity())
            {
                FindChildFramesRecursive(layout, this.ChildFrames);
                this.Layout.push(layout);
            }

            if (this.OpenMode == NxOpenMode.New || this.OpenMode == NxOpenMode.ChildNew)
                this.IsNewEntity = true;

            this.InitDateControls();

            if (this.ChildFrames.All.length > 0)
            {
                $('#' + this.TempId).find('[class="ChildFrameHolder"]').last().hide();
            }

            //var tabs = $('[c-type="tabsContent"]');
            //if (tabs.length == 1)
            //{
            //    var root = $('#layout_root_' + this.TempId)[0];
            //    var commandBars = $(root).find('[id^=commandsBar_]');
            //    $(commandBars).removeClass('CommandsBar_Child');
            //}

            layoutCreated = true;


        }   

        

      


        Resizer.ResizeRequest(new Delegate(this.Resize, this));

        for (var i = 0; i < this.ChildFrames.WoTabs.length; i++)
        {
            if (this.ChildFrames.WoTabs[i].LayoutElement != this.Layout()[0])
                RunChildFrame.call(this, this.ChildFrames.WoTabs[i]);
        }
        for (var i = 0; i < this.ChildFrames.All.length; i++)
        {
            if (this.ChildFrames.All[i].IsOpened)
            {
                var f = this.ChildFrames.All[i];

                //if (!f.FrameHolder)
                //{
                //    f = this.ChildFrames.ByIds[f.LayoutElement.Id];
                //}
                if (f != this.Layout()[0])
                    RunChildFrame.call(this, f);
                
            }

           
        }
    };


    function RunChildFrame(frameElement)
    {
        if (!frameElement.BFrame)
        {
            var frameInstance = Metadata.CreateClassInstance(frameElement.LayoutElement.FrameClassId);
            if (!frameInstance)
            {
                var layout = Metadata.GetFrame(frameElement.LayoutElement.FrameClassId);
                var frameInstance = BAutoLayoutFrame.CreateInstance();
                if (!layout)
                    frameInstance.Layout = Metadata.GetFrame("DefaultDetailsLayout".toUpperCase());
                else
                    frameInstance.Layout = layout;
            }
            var childOpenMode;
            if (this.OpenMode == NxOpenMode.ChildEdit || this.OpenMode == NxOpenMode.Edit)
                childOpenMode = NxOpenMode.ChildEdit;
            if (this.OpenMode == NxOpenMode.ChildView || this.OpenMode == NxOpenMode.View)
                childOpenMode = NxOpenMode.ChildView;
            if (this.OpenMode == NxOpenMode.ChildNew || this.OpenMode == NxOpenMode.New)
                childOpenMode = NxOpenMode.ChildNew;

            var frameData = {
                CurrentEntityUsageId: frameElement.LayoutElement.EntityUsageId,
                ParentEntity: this.BusinessLayer.GetCurrentEntity(),
                ParentEntityUsageId: this.EntityUsage.Id,
                EntityUsageId: frameElement.LayoutElement.EntityUsageId,
                ParentPks: this.BusinessLayer.GetCurrentEntityPks(),
                OpenMode: childOpenMode,
                FrameHolderId: frameElement.FrameHolder.id
            };
            frameInstance.InitFrame(frameData)

            if (this.ChildFrames.All.length == 1)
            {
                $('#child_frame_holder_' + this.TempId + frameInstance.EntityMetadataId).first().show();
                if(this.Layout()[0] && this.Layout()[0].Children[0] && this.Layout()[0].Children[0].Type ==	"tab_control")
                    $('#' + this.TempId).find('[class="ChildFrameHolder"]').last().hide();
                if (this.Layout()[0] && this.Layout()[0].Children.length > 2)
                    $('#' + this.TempId).find('[class="ChildFrameHolder"]').last().hide();
                
            }
            


            frameElement.BFrame = frameInstance;
        }

        if (frameElement.BFrame)
        {
            if (!frameElement.BFrame.DataLoaded)
                frameElement.BFrame.Run();
            else
                frameElement.BFrame.Refresh();
        }


    };

    function FindChildFramesRecursive(layout, foundContainer)
    {
        if (layout.Type == 'frame' && (!App.Utils.IsStringNullOrWhitespace(layout.FrameClassId) || (!App.Utils.IsStringNullOrWhitespace(layout.SlAutoLayoutFrameId))))
        {
            var f = { LayoutElement: layout, BFrame: undefined };
            foundContainer.All.push(f);
            foundContainer.ByIds[layout.Id] = f;
            if (foundContainer.LastTab)
            {
                if (foundContainer.ByTabs[foundContainer.LastTab.Id])
                    foundContainer.ByTabs[foundContainer.LastTab.Id].push(f);
                else
                {
                    foundContainer.ByTabs[foundContainer.LastTab.Id] = [];
                    foundContainer.ByTabs[foundContainer.LastTab.Id].push(f);
                }
            }
            else
            {
                foundContainer.WoTabs.push(f);
            }
        }

        if (layout.Type == 'tab')
        {
            foundContainer.LastTab = layout;
        }

        for (var i = 0; i < layout.Children.length; i++)
        {
            var child = layout.Children[i];
            FindChildFramesRecursive(child, foundContainer);
        }

    }

    this.base_FrameDataLoaded = this.FrameDataLoaded;
    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode)
    {
        this.base_FrameDataLoaded(entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode);
        this.OpenMode = openMode;
        if (vRows.length > 0)
        {
            vRows[0].OnPropertyChangedDelegate = new Delegate(this.OnPropertyChanged, this);
            this.CurrentEntity(vRows[0]);
            
        }
        this.Caption(this.GetFrameCaption());

        for (var i = 0; i < this.ChildFrames.WoTabs.length; i++)
        {
            RunChildFrame.call(this, this.ChildFrames.WoTabs[i]);
        }

        for (var i = 0; i < this.ChildFrames.All.length; i++)
        {
            if( this.ChildFrames.All[i].IsOpened)
                RunChildFrame.call(this, this.ChildFrames.All[i]);
        }

        var thumbs = $('[class="Thumb"]').children('img');
        for (var i = 0; i < thumbs.length; i++)
        {
            var href = $(thumbs[i]).attr('src');
            href = App.Utils.UpdateURLParameter(href, 'g', App.Utils.ShortGuid());
            $(thumbs[i]).attr('src', href);
        }

                
    };

    this.OnPropertyChanged = function (attr, value)
    {
        this.BusinessLayer.OnPropertyChanged(attr, value);
    }

    this.ExpressionValuesChanged = function (data)
    {
        if (this.CurrentEntity)
        {
            for (var i = 0; i < data.ExprResult.Attributes.length; i++)
            {
                var attr = data.ExprResult.Attributes[i];
                this.CurrentEntity()[attr.Id].Readonly(attr.ReadOnly);
                this.CurrentEntity()[attr.Id].Visible(attr.Visible);

                if (data.ExprResult.Values[attr.Id] && this.CurrentEntity()[attr.Id])
                {
                    this.CurrentEntity()[attr.Id](data.ExprResult.Values[attr.Id]);
                }
                
            }

            for (var i = 0; i < data.ExprResult.FilteredRowSources.length; i++)
            {
                var rs = data.ExprResult.FilteredRowSources[i];
                var prop = this.CurrentEntity()[rs.OwnerAttributeId];

                prop.RsItems.removeAll()
                prop.RsItems.pushAll(rs.RowSourceData);
                var found = false;
                for (var k = 0; k < rs.RowSourceData.length; k++)
                {
                    if (prop() == rs.RowSourceData[k].Value)
                    {
                        prop.SelectedRsItem(rs.RowSourceData[k]);
                        prop(rs.RowSourceData[k].Value);
                        found = true;
                    }
                }
                if (found == false)
                {
                    prop.SelectedRsItem({ Text: '', Value: null, ImageId: null });
                    prop(null);
                }
                
            }

           

        }
    };

    this.GetLayoutTableRows = function (lElement, parent)
    {
        var rows = [];

        for (var i = 0; i < lElement.RowsCount; i++)
        {
            var rowObj = {
                LayoutElement: lElement,
                Height: ConvertToHtmlSize(lElement.RowsHeight, i),
                Columns: [],
                Index: i,
            }

            for (var k = 0; k < lElement.ColumnsCount; k++)
            {
                var colObj = {
                    CssWidth: ConvertToHtmlSize(lElement.ColumnsHeight, k),
                    ColumnWidth: ConvertColumnWidth(lElement.ColumnsHeight, i),
                    Index: k,
                    LayoutElement: lElement,
                    RowObj: rowObj,
                }
                rowObj.Columns.push(colObj);

            }

            rows.push(rowObj);
        }
        return rows;
    };

    this.GetPanelTableRows = function (lElement, parent, openMode)
    {

        var rows = [];
        var rowsCount = lElement.RowsCount;
        if (!rowsCount || rowsCount == 0)
            rowsCount = 1;


        if (lElement.Children.length == 0)//panel with controls
        {
            var colsCount = lElement.ColumnsCount * 2;
            if (!colsCount || colsCount == 0)
                colsCount = 2;

            for (var i = 0; i < rowsCount; i++)//create rows and columns
            {
                var row = { Columns: [] };
                rows.push(row);
                for (var k = 0; k < colsCount; k++)
                {
                    row.Columns.push({ ColumnType: 'empty' });
                }
            }


            var pnlAttrs = GetAttributesForPanel(this.EntityUsage, lElement.Id);

            var usedRows = 0;
            var usedCols = 0

            for (var i = 0; i < pnlAttrs.length; i++)
            {


                var attr = pnlAttrs[i];

                if (CheckIsNeedNewRow(usedCols, colsCount, attr))
                {
                    usedRows++;
                    usedCols = 0;
                    AddRow(rows, colsCount);
                    rowsCount++;
                }

                var labelCol = { ColumnType: 'label', Text: attr.Caption, IsRequired: IsLabelRequired(attr, openMode) };
                rows[usedRows].Columns[usedCols] = labelCol;
                usedCols++;
                var freeColsInRow = colsCount - usedCols;

                if (attr.ControlWidth && attr.ControlWidth > 0)//control with defined Control Width in attribute metadata
                {
                    if (attr.ControlWidth > freeColsInRow)// 'ControlWidth' wider than we have in layput, just trim control to fit in layout
                    {
                        var controlCol = { ColumnType: 'control', Attr: attr, ColSpan: rows[usedRows].Columns.length - usedCols, ControlHeight: ConverToControlHeight(attr.ControlHeight) };
                        rows[usedRows].Columns[usedCols] = controlCol;
                        usedCols = colsCount;
                        continue;
                    }
                    else// 'ControlWidth' fits in current row
                    {
                        var controlCol = { ColumnType: 'control', Attr: attr, ColSpan: attr.ControlWidth, ControlHeight: ConverToControlHeight(attr.ControlHeight) };
                        rows[usedRows].Columns[usedCols] = controlCol;
                        usedCols = usedCols + attr.ControlWidth;
                        continue;
                    }
                }
                else
                {
                    var controlCol = { ColumnType: 'control', Attr: attr, ColSpan: 0, ControlHeight: ConverToControlHeight(attr.ControlHeight) };
                    rows[usedRows].Columns[usedCols] = controlCol;
                    usedCols++;
                    continue;
                }

            }
            SetColumnsWidth(rows);

            //return rows;
        }
        else// panel with child layout : tabs, frames, etc.
        {

            var colsCount = lElement.ColumnsCount;
            if (!colsCount || colsCount == 0)
                colsCount = 2;

            for (var i = 0; i < rowsCount; i++)//create rows and columns
            {
                var row = { Columns: [] };
                rows.push(row);
                for (var k = 0; k < colsCount; k++)
                {
                    row.Columns.push({ ColumnType: 'empty' });
                }
            }
            rows[0].Columns.push({ ColumnType: 'child_layout', Children: lElement.Children });





        }



        return rows;
    };

    function CheckIsNeedNewRow(usedCols, colsCount, attr)
    {
        if (!attr.ControlWidth && attr.ControlWidth == 0)
        {
            return colsCount - usedCols < 2;
        }
        else
        {
            return (colsCount - usedCols - 1) < attr.ControlWidth;
        }
    };

    function AddRow(rows, colsCount)
    {
        var row = { Columns: [] };
        rows.push(row);
        for (var k = 0; k < colsCount; k++)
        {
            row.Columns.push({ ColumnType: 'empty' });
        }
    };

    this.GetChildrenForColumn = function (lElement, rowIndex, colIndex)
    {
        var children = [];
        for (var i = 0; i < lElement.Children.length; i++)
        {
            var child = lElement.Children[i];
            if (child.Row == rowIndex && child.Column == colIndex)
                children.push(child);
        }
        return children;
    };

   

    var createdTabChilds = {};
    var tabsLevel = {};
    var tabsFlow = [];
    this.GetChildrenForTabControl = function (parent, forWhat)
    {
        if (parent.Type == "tab_control")
        {
            if (!tabsLevel[parent.Id])
                tabsLevel[parent.Id] = tabsFlow.push(parent);
        }

        var parentEl;
        if (parent.LayoutElement)
            parentEl = parent.LayoutElement;
        else
            parentEl = parent;


        if (createdTabChilds[parentEl.Id])
            return createdTabChilds[parentEl.Id];

        var tabs = [];
        parent.TabViews = [];
        for (var i = 0; i < parentEl.Children.length; i++)
        {
            var tab = parentEl.Children[i];
            var tabViewObj = {
                LayoutElement: tab,
                Selected: ko.observable(false),
                Type: tab.Type,
                RowsHeight: App.Utils.IsStringNullOrWhitespace(tab.RowsHeight) ? 'auto' : tab.RowsHeight + 'px',
                Parent: parent,
                TabLevel: tabsLevel[parent.Id]
            };
            tabs.push(tabViewObj);
            parent.TabViews.push(tabViewObj);
        }

        //TODO: get Selected Tab from settings

        var selectedFromSettings;// = Settings.GetSettings('al_frame_' + this.EntityUsage.Id, 'sel_tab');
        //if (!App.Utils.IsStringNullOrWhitespace(selectedFromSettings))
        //{
        //    selectedFromSettings = JSON.parse(selectedFromSettings);
        //}

        if (tabs.length > 0 && App.Utils.IsStringNullOrWhitespace(selectedFromSettings))
        {
            tabs[0].Selected(true);
            var byTab = this.ChildFrames.ByTabs[tabs[0].LayoutElement.Id]
            if (byTab)
            {
                for (var i = 0; i < byTab.length; i++)
                {
                    byTab[i].IsOpened = true;
                }
            }
                
        }

        if (tabs.length > 0 && !App.Utils.IsStringNullOrWhitespace(selectedFromSettings))
        {
            for (var i = 0; i < tabs.length; i++)
            {
                if (tabs[i].LayoutElement && tabs[i].LayoutElement.Id == selectedFromSettings)
                {
                    tabs[i].Selected(true);
                    var byTab = this.ChildFrames.ByTabs[tabs[i].LayoutElement.Id]
                    if (byTab)
                    {
                        for (var k = 0; k < byTab.length; k++)
                        {
                            byTab[k].IsOpened = true;
                        }
                    }
                }
            }

        }

        createdTabChilds[parentEl.Id] = tabs;
        
        return tabs;

    }

    function ConvertToHtmlSize(layoutSize, index)
    {

        if (!App.Utils.IsStringNullOrWhitespace(layoutSize))
        {
            var lowerVal = layoutSize.replace(' ', '');
            var splitted = lowerVal.split(',');
            var result = index ? splitted[index] : splitted[0];
            if (result == 'auto')
                return 'auto';
            if (result == '*')
                return '100%';
            return result + 'px';
        }
        return "auto";
    }

    function ConvertColumnWidth(layoutSize, index)
    {

        if (!App.Utils.IsStringNullOrWhitespace(layoutSize))
        {
            var lowerVal = layoutSize.replace(' ', '');
            var splitted = lowerVal.split(',');
            var result = index ? splitted[index] : splitted[0];
            if (result == 'auto')
                return '';
            if (result == '*')
                return '100%';
            return result;
        }
        return '';
    }

    function ConverToControlHeight(layoutSize)
    {

        if (layoutSize)
        {
            if (layoutSize == 0)
                return 'auto';
            return layoutSize + 'px';
        }
        return 'auto';
    }

    this.TabClick = function (item, context)
    {
        if (item.Selected())
            return;
        for (var i = 0; i < item.Parent.TabViews.length; i++)
        {
            item.Parent.TabViews[i].Selected(false);
        }
        item.Selected(true);

        //Settings.AddChangedSettings('al_frame_' + this.EntityUsage.Id, 'sel_tab', item.LayoutElement.Id);

        for (var i = 0; i < context.ChildFrames.All.length; i++)
        {
            context.ChildFrames.All[i].IsOpened = false;
        }

        var childFrames = context.ChildFrames.ByTabs[item.LayoutElement.Id];
        if (childFrames && childFrames.length > 0)
        {
            
            for (var i = 0; i < childFrames.length; i++)
            {
                childFrames[i].IsOpened = true;
                var f = childFrames[i];
                if (!f.FrameHolder)
                {
                    f = this.ChildFrames.ByIds[f.LayoutElement.Id];
                }

                RunChildFrame.call(this, f);
            }
        }



    };

    function GetAttributesForPanel(entityUsage, panelId)
    {
        var attributes = [];

        if (panelId.toUpperCase() == "PNDEFAULT")
        {
            for (var i = 0; i < entityUsage.EditableAttributes.length; i++)
            {
                var attr = entityUsage.Attributes[entityUsage.EditableAttributes[i]];
                if (App.Utils.IsStringNullOrWhitespace(attr.ControlPlacement) &&
                    entityUsage.EditableAttributesById[attr.Id])
                {
                    attributes.push(attr);
                }
            }
        }
        else
        {
            for (var i = 0; i < entityUsage.EditableAttributes.length; i++)
            {
                var attr = entityUsage.Attributes[entityUsage.EditableAttributes[i]];
                if (attr.ControlPlacement.toUpperCase() == panelId.toUpperCase() &&
                    entityUsage.EditableAttributesById[attr.Id])
                {
                    attributes.push(attr);
                }
            }
        }
        return attributes;
    };

    function IsLabelRequired(attributeMetadata, openMode)
    {
        if (!attributeMetadata.Nullable &&
                       (openMode == NxOpenMode.Edit ||
                           openMode == NxOpenMode.New ||
                           openMode == NxOpenMode.ChildEdit ||
                           openMode == NxOpenMode.ChildView))
        {
            return true;
        }
        return false;
    };

    function SetColumnsWidth(rows)
    {
        var maxControlsInRow = 1;
        for (var i = 0; i < rows.length; i++)
        {
            var row = rows[i];
            var controlsCount = 0;
            for (var k = 0; k < row.Columns.length; k++)
            {
                if (row.Columns[k].ColumnType == 'control')
                    controlsCount++;
            }
            if (maxControlsInRow < controlsCount)
                maxControlsInRow = controlsCount;
        }

        var widthPercent = (100 / maxControlsInRow) + '%';

        for (var i = 0; i < rows.length; i++)
        {
            var row = rows[i];

            for (var k = 0; k < row.Columns.length; k++)
            {
                if (row.Columns[k].ColumnType == 'control')
                    row.Columns[k].Width = widthPercent;
            }

        }

    };

    this.Resize = function ()
    {
        
       



        //var x = $('.FieldSet');
        //for (var i = 0; i < x.length; i++)
        //{
        //    $(x[i]).width();

        //    var maxContentHeight = 0;
        //    var tabContent = $(x[i]).find('.TabContent');
        //    for (var k = 0; k < tabContent.length; k++)
        //    {
        //        var he = $(tabContent[k]).height();
        //        if (maxContentHeight < he)
        //            maxContentHeight = he;
        //    }
        //    if (maxContentHeight > 0)
        //    {
        //        var oldH = $(x[i]).height();
        //        if (oldH < maxContentHeight)
        //        {
        //          //  $(x[i]).height(oldH + maxContentHeight);

        //        }

        //    }
        //}

        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1)
        {
            $('[id^=rs_arrow_]').css("margin-top", -12);
         
        }

        

    };

    this.GetControlTemplateName = function (attr, openMode, val)
    {
        var mode = 'edit_';
        if (openMode == NxOpenMode.ChildView || openMode == NxOpenMode.View ||
            ((openMode == NxOpenMode.Edit || openMode == NxOpenMode.ChildEdit) && attr.ReadOnlyForUpdate))
                var mode = 'view_';
        if (!App.Utils.IsStringNullOrWhitespace(attr.RowSourceId))
        {
            return 'form_' + mode + ControlsTemplatesIds.RowSourceComboBox;
        }
        else
        {
            var type = attr.Type;
            if (!App.Utils.IsStringNullOrWhitespace(attr.SlControl))
                type = attr.SlControl;

            var template = ControlsTemplatesIds[type];
            if (!template)
                template = ControlsTemplatesIds.string;


            return 'form_' + mode + template;

        }
    };

    this.SaveAndClose = function ()
    {
       // if (this.HasChanges())
       // {
            var errors = this.CurrentEntity().ValidateEntity();
            if (errors.length > 0)
            {
                ShowMessage(errors, 'Validation', [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning);
            }
            else
            {
                this.BusinessLayer.SaveCurentEntity(this.CurrentEntity());
            }
        //}

    };


    this.HasChanges = function ()
    {
        if (this.OpenMode == NxOpenMode.New || this.OpenMode == NxOpenMode.ChildNew)
            return true;

        if (this.CurrentEntity && this.CurrentEntity() && this.CurrentEntity().HasChanges)
        {
            return true;
        }
        return false;
    };

    this.RevertChanges = function ()
    {
        if (this.CurrentEntity && this.CurrentEntity() && this.CurrentEntity().HasChanges)
        {
            this.CurrentEntity().RevertChanges();
        }
    };

    this.SaveChanges = function (doneDelegate)
    {
        
        


        if (this.CurrentEntity && this.CurrentEntity() /*&& this.CurrentEntity().HasChanges*/)
        {

            var errors = this.CurrentEntity().ValidateEntity();
            if (errors.length > 0)
            {
                ShowMessage(errors, 'Validation', [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning);
                return;
            }

            this.BusinessLayer.SaveCurentEntity(this.CurrentEntity(), doneDelegate);
        }
        else
        {
            doneDelegate.Invoke();
        }

    };

    this.AllChangesSaved = function ()
    {
        this.CurrentEntity().HasChanges = false;
        this.IsNewEntity = false;
        if (this.OpenMode == NxOpenMode.New)
            this.OpenMode = NxOpenMode.Edit;
        if (this.OpenMode == NxOpenMode.ChildNew)
            this.OpenMode = NxOpenMode.ChildEdit;
    };

    this.AddChildFrame = function (frameLayoutElement, context)
    {

        context.ChildFrames.ByIds[frameLayoutElement.Id].FrameHolder = document.getElementById('child_frame_holder_' + context.TempId + frameLayoutElement.EntityUsageId);
        return App.Utils.ShortGuid();
    };

    this.GetDownloadlinkUrl = function (attrId, context)
    {

        return downloadUrl + '?entityUsageId=' +
           this.EntityUsage.Id + '&attributeId=' +
           attrId + '&pkVals=' + JSON.stringify(context.BusinessLayer.GetCurrentEntityPks()) + '&g=' + App.Utils.ShortGuid();
    }

    this.GetPhotoUrl = function (attrId, context, thumb)
    {

        return downloadPhotoUrl + '?entityUsageId=' +
           this.EntityUsage.Id + '&attributeId=' +
           attrId + '&pkVals=' + JSON.stringify(context.BusinessLayer.GetCurrentEntityPks() ) + '&thumb=' + thumb + '&g=' + App.Utils.ShortGuid();
    }

    this.OnThumbClick = function (attrId, context)
    {
        window.open(showImageUrl + "?imageUrl=" + encodeURIComponent( context.GetPhotoUrl(attrId, context, false)) , "_blank");
    }

    this.Base_Dispose = this.Dispose;
    this.Dispose = function ()
    {
        this.Base_Dispose();

        for (var i = 0; i < this.ChildFrames.All.length; i++)
        {
            var child = this.ChildFrames.All[i];

            if (child.FrameHolder)
                ko.cleanNode(child.FrameHolder);

            if (child.BFrame && child.BFrame.ViewLayer && child.BFrame.ViewLayer.Dispose)
                child.BFrame.ViewLayer.Dispose();

             

        }

    };

    this.Refresh = function ()
    {
        this.BusinessLayer.Refresh();
    };

    this.GetSelectedTabClassName = function(frame)
    {
        if (frame.OpenMode.indexOf('Child') == -1)
            return 'TabGripSelected';
        else
            return 'TabGripSelected_Child';
    }

    this.GetChildFrame = function (entityUsageId)
    {
        for (var i = 0; i < this.ChildFrames.All.length; i++)
        {
            if (this.ChildFrames.All[i].BFrame && this.ChildFrames.All[i].BFrame.EntityMetadataId == entityUsageId)
                return this.ChildFrames.All[i].BFrame;
        }

        
    };


    this.DymmicCommandClick = function (item, attr)
    {
        this.BusinessLayer.DymmicCommandClick(item, attr);
    }

};
extend(VAutoLayoutFrame, VBaseFrame);

function VAutoLayoutFrame_ForBusiness()
{
    VBaseFrame_ForBusiness.call(this);
    this.CreateLayout = function (layout) { };
    this.AllChangesSaved = function (layout) { };
    this.GetChildFrame = function (entityUsageId) { };
};

