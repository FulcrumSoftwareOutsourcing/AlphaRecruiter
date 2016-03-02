'use strict';
function VDataGrid()
{
    ViewLayer.call(this);
    this.PublicForBusiness = new VDataGrid_ForBusiness();
    this.PublicForParent = new VDataGrid_ForParent();
    var tableHolderElement;
   // var tableElement;
    var tempId;
    var allRows = { List: [], ById: {} };
    var head;
    var selectedItem;
    var colSizes = {};

    var gridColMenuEl = null;
    var gridMenuPopupEl = null;

    var miMultiselectEl = null;
    var miSelectAllEl = null;
    var miSelectNoneEl = null;
    var miToSingleSelEl = null;
   
    var selectMode = 's';

    var me = this;

    var allowMultiselect;
    var metaId;

    var loadTimes = 0;

    var tableElement;


    this.LoadData = function (rows, headRow, sortDescs, multiselect, entityUsageId, openMode)
    {
        loadTimes++;
        metaId = entityUsageId;

        allowMultiselect = multiselect;

        selAll = false;

    //    if (tableElement)
      //      $(tableElement).off('keydown', OnDataTableKyeDown);

        for (var i = 0; i < allRows.List.length; i++)
        {
            $(allRows.List[i].HtmlElement()).off('click', OnRowClicked);
        }
        allRows.List = rows;


        if (head)
        {
            var h = head.Headers();
            for (var i = 0; i < h.length; i++)
            {
                if (h[i].RemoveClick)
                {
                    h[i].RemoveClick(OnHeaderClicked);
                }

            }
        }



        $(document).on('mouseup', ColResizerMouseUp);

        tempId = this.ParentLayer.GetTempId();
        tableHolderElement = document.getElementById(tempId + '_dataGridRoot');


      
        
       

        var tableHtml = $('#DataGridRootTemplate').html().replace('<!--no_data_text-->', GetTxt('There is no data in the grid'));

        var gridContentHtml = { text: '' };

        head = headRow;
        headRow.Render(gridContentHtml);



        for (var i = 0; i < rows.length; i++)
        {
            rows[i].Render(gridContentHtml);
        }

        var tableId = App.Utils.ShortGuid();
        tableHtml = tableHtml.replace('%id%', tableId).replace('<!--content-->', gridContentHtml.text);

        //$(tableHolderElement).empty();

        //$(tableHolderElement).append($(tableHtml));


        $(tableHolderElement).html(tableHtml);


        tableElement = $('#' + tableId)[0];
        var noDataRow = $(tableElement).find('[c-type="noDataRow"]');
        if (rows.length == 0)
        {
            $(noDataRow).children().attr('colspan', headRow.Headers().length);
            $(noDataRow).show();
        }
        else
            $(noDataRow).hide();


    //    $(tableElement).on('keydown', OnDataTableKyeDown);



        for (var i = 0; i < rows.length; i++)
        {
            allRows.ById[rows[i].Id()] = rows[i];
            $(rows[i].HtmlElement()).on('click', OnRowClicked);

        }

        var sizesFromSettings = Settings.GetSettings('grid_' + metaId, 'col_sizes');
        if (!App.Utils.IsStringNullOrWhitespace(sizesFromSettings))
            colSizes = JSON.parse(sizesFromSettings);

        var coolHeads = headRow.Headers();
        for (var i = 0; i < coolHeads.length; i++)
        {
            var header = coolHeads[i];

            if (header.SortDescr)
            {
                header.SortDescr.call(coolHeads[i], sortDescs);
            }
            if (header.AddClick)
            {
                header.AddClick(OnHeaderClicked);
            }
            if (header.AddResizerMouseDown)///////////////////////////
            {
                header.AddResizerMouseDown(ColResizerMouseDown);
            }
            //if (coolHeads[i].AddResizerMouseUp)///////////////////////////
            //{
            //    coolHeads[i].AddResizerMouseUp(ColResizerMouseUp);
            //}

            if (header.DomEl)
            {
                var columnEl = header.DomEl();
                $(columnEl).attr('attr-id', header.Attr.Id);

                if (!colSizes[header.Attr.Id])
                {
                    var clmW = $(columnEl).width();
                    $(columnEl).attr('width', clmW);
                    colSizes[header.Attr.Id] = clmW;
                }
                else
                {
                    $(columnEl).attr('width', colSizes[header.Attr.Id]);
                }

            }
        }


        for (var i = 0; i < coolHeads.length; i++)
        {
            var header = coolHeads[i];
            if (header.DomEl)
            {
                var columnEl = header.DomEl();
                var clmW = $(columnEl).width();
                $(columnEl).children('[class="ColResizeGrip"]').css('left', clmW);
            }
        }

        //if (rows.length > 0)
        //{
        //    rows[0].Selected(true);
        //    this.BusinessLayer.SelectedItemChanged([rows[0].DataItem()]);
        //}
        //else
        //    this.BusinessLayer.SelectedItemChanged([]);



        if (gridColMenuEl)
            $(gridColMenuEl).off('click', GridMenuClick);
        gridColMenuEl = $(tableHolderElement).find('[c-type="headerCursor"]');
        $(gridColMenuEl).on('click', GridMenuClick);

        //if (miMultiselectEl)
        //    $(miMultiselectEl).off('click', MultipleSelectionClick);
        //miMultiselectEl = $(tableHolderElement).find('[c-type="giMulti"]');
        //$(miMultiselectEl).text(GetTxt('Multiple Selection'));
        //$(miMultiselectEl).on('click', MultipleSelectionClick);

        //if (miSelectAllEl)
        //    $(miSelectAllEl).off('click', SelectAllClick);
        //miSelectAllEl = $(tableHolderElement).find('[c-type="giSelAll"]');
        //$(miSelectAllEl).text(GetTxt('Select All'));
        //$(miSelectAllEl).on('click', SelectAllClick);

        //if (miSelectNoneEl)
        //    $(miSelectNoneEl).off('click', SelectNoneClick);
        //miSelectNoneEl = $(tableHolderElement).find('[c-type="giSelNone"]');
        //$(miSelectNoneEl).text(GetTxt('Select None'));
        //$(miSelectNoneEl).on('click', SelectNoneClick);

        //if (miToSingleSelEl)
        //    $(miToSingleSelEl).off('click', ToSingleSelectionClick);
        //miToSingleSelEl = $(tableHolderElement).find('[c-type="giSingle"]');
        //$(miToSingleSelEl).text(GetTxt('Switch to Single Selection'));
        //$(miToSingleSelEl).on('click', ToSingleSelectionClick);

        //$(tableHolderElement).find('[c-type="giCancel"]').text(GetTxt('Cancel'));

        //gridMenuPopupEl = $(tableHolderElement).find('[class="GridColMenu"]')[0];
        

        //$('[c-type="lbl-no-file"]').text(GetTxt('Download...'));



   

        //if (!App.Utils.IsStringNullOrWhitespace(openMode) && openMode.indexOf('Child') != -1 && loadTimes == 1)
        //{
           

        //    var h = $('#' + tempId + '_dataGridRoot').height();
        //    if (h > 200)
        //    {
        //        var wh = $(window).height();
        //        var percent = wh * 50 / 100;

        //        if (openMode.indexOf('Edit') != -1)
        //        {
        //            $('#' + tempId + '_dataGridRoot').height(h - percent);
        //        }
        //        if (openMode.indexOf('View') != -1)
        //        {
        //            $('#' + tempId + '_dataGridRoot').height(h - percent);
        //        }
        //    }
        //}
       

    };

    var selAll = false;
    function GridMenuClick(e)
    {
        if (allowMultiselect)
        {
            var newSelected = [];
            selAll = !selAll;
            for (var i = 0; i < allRows.List.length; i++)
            {
                if (allRows.List[i].Selected)
                {
                    allRows.List[i].Selected(selAll)
                    if (selAll)
                    {
                        newSelected.push(allRows.List[i]);
                    }
                }
            }


           
            me.BusinessLayer.SelectedItemChanged(newSelected);
        }


        //e.stopPropagation();
        //$(gridMenuPopupEl).css('display') == 'none' ? $(gridMenuPopupEl).css('display', 'block') : $(gridMenuPopupEl).css('display', 'none');
        //$('#settingsMenuHolder').hide();
        //$('#logoutMenuHolder').hide();
        //$('#mmHolder').hide();
        
    }

    function MultipleSelectionClick(e)
    {
        me.SetSelectionMode('m');
    }

    function SelectAllClick(e)
    {
        me.SetSelectionMode('m');
    }

    function SelectNoneClick(e)
    {

    }
    
    function ToSingleSelectionClick(e)
    {
        me.SetSelectionMode('s');
    }


    function OnDataTableKyeDown(e)
    {
        if (PressedKey == 'up')
        {

        }
        if (PressedKey == 'down')
        {

        }
    };

    function OnHeaderClicked()
    {
        if (capturedColGrip.afterResize)
        {
            capturedColGrip.afterResize = false;
            return;
        }

        var coolHeads = head.Headers();

        if (PressedKey != 'control')
        {
            for (var i = 0; i < coolHeads.length; i++)
            {
                var h = coolHeads[i];
                if (h.Id() != this.id && h.ResetDescr)
                    h.ResetDescr();
            }
        }

        for (var i = 0; i < coolHeads.length; i++)
        {
            var h = coolHeads[i];
            if (h.Id() == this.id && h.ChangeDescr)
            {
                h.ChangeDescr();
            }
        }

        var sorts = [];
        for (var i = 0; i < coolHeads.length; i++)
        {
            if (coolHeads[i].SortDescr)
            {
                var sort = coolHeads[i].SortDescr();
                if (sort.Direction != NxListSortDirection.None)
                    sorts.push(sort);
            }
        }

        me.BusinessLayer.SortingChanged(sorts);
    }

    function OnRowClicked(event)
    {
        

        var row = allRows.ById[this.id];

        var fromCursor = false;
        var cType = $(event.target).attr('c-type');
        if ($(event.target).hasClass('GridCursor') || cType == 'row-sel-img' || cType == 'round-img' || cType == 'checked-hover-img')
            fromCursor = true;

        


        if (PressedKey == 'other' || (PressedKey == null && !fromCursor) || !allowMultiselect)//no keys pressed, only row clicked
        {
            for (var i = 0; i < allRows.List.length; i++)
            {
                allRows.List[i].Selected(false);
            }

            row.Selected(true);
            me.BusinessLayer.SelectedItemChanged([row]);
            ProcessOtherOnClick(row, event);
            return;
        }

        if (PressedKey == 'control' || fromCursor)
        {
            var newSelected = [];
            if (row.Selected())
            {
                row.Selected(false);
                for (var i = 0; i < allRows.List.length; i++)
                {
                    if (allRows.List[i].Selected())
                        newSelected.push(allRows.List[i]);
                }
                me.BusinessLayer.SelectedItemChanged(newSelected);
                ProcessOtherOnClick(row, event);
                return;
            }
            else
            {
                row.Selected(true);
                for (var i = 0; i < allRows.List.length; i++)
                {
                    if (allRows.List[i].Selected())
                        newSelected.push(allRows.List[i]);
                }
                me.BusinessLayer.SelectedItemChanged(newSelected);
                ProcessOtherOnClick(row, event);
                return;
            }
        }

        if (PressedKey == 'shift')
        {
            var nowSelected = [];
            for (var i = 0; i < allRows.List.length; i++)
            {
                if (allRows.List[i].Selected())
                    nowSelected.push(allRows.List[i]);
            }

            if (row.Index() == nowSelected[0].Index())
            {
                ProcessOtherOnClick(row, event);
                return;
            }



            if (row.Index() < nowSelected[0].Index())
            {
                var newSelected = [];
                for (var i = row.Index() ; i < nowSelected[0].Index() ; i++)
                {
                    allRows.List[i].Selected(true);
                    newSelected.push(allRows.List[i]);
                }
                me.BusinessLayer.SelectedItemChanged(newSelected);
                ProcessOtherOnClick(row, event);
                return;
            }

            if (row.Index() > nowSelected[0].Index())
            {
                var newSelected = [];


                for (var i = nowSelected[0].Index() ; i <= row.Index() ; i++)
                {
                    var r = allRows.List[i];
                    r.Selected(true);

                }
                for (var i = 0; i < allRows.List.length; i++)
                {
                    if (allRows.List[i].Selected())
                        newSelected.push(allRows.List[i]);
                }

                me.BusinessLayer.SelectedItemChanged(newSelected);
                ProcessOtherOnClick(row, event);
                return;
            }



        }


        ProcessOtherOnClick(row, event);

    };

    function ProcessOtherOnClick(row, event)
    {
        Download(row, event);
    };

    function Download(row, event)
    {
        if ($(event.target).attr('c-type') == 'd-link')
        {
            var attrId = $(event.target).attr('attr-id');
            me.BusinessLayer.Download(attrId, row);
        }
        if ($(event.target).attr('c-type') == 'cmd-link')
        {
            var attrId = $(event.target).attr('attr-id');
            me.BusinessLayer.DymmicCommandClick(attrId, row.DataItem());
        }
    };

    var capturedColGrip = { column: null, oldX: 0, afterResize: false };
    function ColResizerMouseDown(e)
    {
        e.stopPropagation();
        document.body.style.cursor = 'col-resize';
        capturedColGrip.column = $(e.currentTarget).parent()[0];
        capturedColGrip.oldX = e.clientX;

    };

    function ColResizerMouseUp(e)
    {
        if (!capturedColGrip.column)
            return

        $(tableHolderElement).css('max-width', $(tableHolderElement).css('width'));

        document.body.style.cursor = 'default';
        var currentColW = $(capturedColGrip.column).width();
        var newW = currentColW + (e.clientX - capturedColGrip.oldX);
        $(capturedColGrip.column).attr('width', newW);

        var attrId = $(capturedColGrip.column).attr('attr-id');
        colSizes[attrId] = newW;

        Settings.AddChangedSettings('grid_' + metaId, 'col_sizes', colSizes);

        var coolHeads = head.Headers();
        for (var i = 0; i < coolHeads.length; i++)
        {
            var header = coolHeads[i];
            if (header.DomEl)
            {
                var columnEl = header.DomEl();
                var clmW = $(columnEl).width();
                $(columnEl).children('[class="ColResizeGrip"]').css('left', clmW);
            }
        }


       // $(tableHolderElement).css('max-width', 'none');

        capturedColGrip.column = null;
        capturedColGrip.afterResize = true;

        //var clmW = $(capturedColGrip.column).width();
        //$(capturedColGrip.column).children('[class="ColResizeGrip"]').css('left', clmW);


        //capturedColGrip.column = null;
    };

    this.Dispose = function ()
    {
        for (var i = 0; i < allRows.List.length; i++)
        {
            $(allRows.List[i].HtmlElement()).off('click', OnRowClicked);
        }

        var coolHeads = head.Headers();
        for (var i = 0; i < coolHeads.length; i++)
        {
            if (coolHeads[i].RemoveClick)
                coolHeads[i].RemoveClick(OnHeaderClicked);
            if (coolHeads[i].RemoveResizerMouseDown)///////////////////////////
            {
                coolHeads[i].RemoveResizerMouseDown(ColResizerMouseDown);
            }
            //if (coolHeads[i].RemoveResizerMouseUp)///////////////////////////
            //{
            //    coolHeads[i].RemoveResizerMouseUp(ColResizerMouseUp);
            //}
        }

        $(document).off('mouseup', ColResizerMouseUp);
    //    $(tableElement).off('keydown', OnDataTableKyeDown);
      //  tableElement = undefined;
        tableHolderElement = undefined;
    };

    this.SetSelectionMode = function (mode)
    {
        
        for (var i = 0; i < allRows.List.length; i++)
        {
            allRows.List[i].SetSelectionMode(mode);
            
        }
        head.SetSelectionMode(mode);
    };

    this.CreateTotalCalcFunction = function (name, source)
    {
        $('body').append('<script type="text/javascript" > function ' + name + ' ( attr, data, metadata ) { ' + source + ' } </script>');
    }

    this.ShowTotals = function (totals)
    {
        var totalsByIndexes = {};
        for (var i = 0; i < totals.length; i++)
        {
            var total = totals[i];
            totalsByIndexes[total.IndexInSet] = total;
        }

      

        var tableId = tableElement.id;
        var child = $('#' + tableId + ' tr:first').children();

        var rowsHtml = '';
        for (var i = 0; i < child.length; i++)
        {
            var content = '';
            if ( totalsByIndexes[i - 1] )
            {
                var total = totalsByIndexes[i - 1];
                var lbl = '';
                //if (!App.Utils.IsStringNullOrWhitespace(total.Attr.TotalText))
                //{
                //    lbl = '<span>' + total.Attr.TotalText + '</span>'
                //}
                content = '<div class="TotalCell">' + total.Total  + lbl + '</div>';
            }

            rowsHtml += '<td width="' + $(child[i]).width() + '">' + content + '</td>';
        }

      //  var rowsCount = $('#' + tableId).find('tr');
     //   rowsCount = rowsCount / 2 - 1;

        var lastRow = $('#' + tableId + ' tr:last');
        var className = 'TotalRow';
     //   if (!(rowsCount % 2))
       //     className += ' AlterRow';

        $(lastRow).after('<tr class="' + className + '" >' + rowsHtml + '</tr>');

        


        //holder.html('<table class="DataGrid"><tr>' + rowsHtml + '</tr></table>');
    }
};

extend(VDataGrid, ViewLayer);

function VDataGrid_ForBusiness()
{
    this.LoadData = function (data) { };
    this.SetSelectionMode = function (mode) { };
    this.CreateTotalCalcFunction = function (name, source) { }
    this.ShowTotals = function (totals) { }
}

function VDataGrid_ForParent()
{
    this.Dispose = function () { };
}

//===================================================================================================
//===================================================================================================
//===================================================================================================
//===================================================================================================
//===================================================================================================
//===================================================================================================

function VCursorColumnHeader(templateText, multiselect)
{
    var template = templateText;
    template = template.replace('%selectAllVisible%', multiselect ? 'block' : 'none');
    this.Render = function (container)
    {
        container.text += template;
    }

    this.Id = function () { return ''; }

    this.SetSelectionMode = function (mode)
    {
        
        if (mode == 'm')
        {
            var x = 8;
        }
        if (mode == 's')
        {

        }
    };
};

function VColumnHeader(attr, templateText)
{
    var id = App.Utils.ShortGuid();
    this.Attr = attr;
    var template = templateText;

    var sortDescr = { AttributeId: attr.Id, Direction: NxListSortDirection.None };

    var html = template.replace('<!--text-->', this.Attr.Caption).replace('%id%', id).replace('%width%', App.Utils.ConvertDataGridColumnWidth(attr.GridWidth));

    var element;

    var ascArrowEl;
    var desArrowEl;

    this.Id = function () { return id; }

    var resizeGripEl;

    this.Render = function (container)
    {
        container.text += html;
        template = undefined;
    }

    function DomElement()
    {
        if (!element)7
            element = $('#' + id)[0];
        return element;
    }

    this.DomEl = function ()
    {
        return DomElement();
    }

    function ResizeGripElement()
    {
        if (!resizeGripEl)
            resizeGripEl = $('#' + id).children('[class="ColResizeGrip"]')[0];
        return resizeGripEl;
    }

    function AscArrowEl()
    {
        if (!ascArrowEl)
            ascArrowEl = $('#' + id).children('[c-type="up-arr"]')[0];
        return ascArrowEl;
    }

    function DesArrowEl()
    {
        if (!desArrowEl)
            desArrowEl = $('#' + id).children('[c-type="down-arr"]')[0];
        return desArrowEl;
    }

    this.SortDescr = function (descrs)
    {
        if (arguments.length == 0)
        {
            return sortDescr;
        }

        if (!descrs[this.Attr.Id])
        {
            $(AscArrowEl()).hide();
            $(DesArrowEl()).hide();
            return;
        }


        var descr = descrs[this.Attr.Id];

        if (arguments.length == 0)
            return sortDescr;
        else
        {
            sortDescr = descr;
            Hide_ShowArrows();
        }
    }

    function Hide_ShowArrows()
    {
        if (sortDescr.Direction == NxListSortDirection.Ascending)
        {
            $(AscArrowEl()).show();
            $(DesArrowEl()).hide();
        }
        else if (sortDescr.Direction == NxListSortDirection.Descending)
        {
            $(AscArrowEl()).hide();
            $(DesArrowEl()).show();
        }
        else
        {
            $(AscArrowEl()).hide();
            $(DesArrowEl()).hide();
        }
    }

    this.ChangeDescr = function ()
    {
        if (sortDescr.Direction == NxListSortDirection.None)
        {
            sortDescr.Direction = NxListSortDirection.Ascending;
            Hide_ShowArrows();
            return;
        }
        if (sortDescr.Direction == NxListSortDirection.Ascending)
        {
            sortDescr.Direction = NxListSortDirection.Descending;
            Hide_ShowArrows();
            return;
        }
        if (sortDescr.Direction == NxListSortDirection.Descending)
        {
            sortDescr.Direction = NxListSortDirection.Ascending;
            Hide_ShowArrows();
            return;
        }

    }

    this.ResetDescr = function ()
    {
        sortDescr.Direction = NxListSortDirection.None;
        Hide_ShowArrows();
    }

    this.AddClick = function (handler)
    {
        $(DomElement()).on('click', handler);
    }

    this.RemoveClick = function (handler)
    {
        $(DomElement()).off('click', handler);
    }

    this.AddResizerMouseDown = function (handler)
    {
        $(ResizeGripElement()).on("mousedown", handler);
        var clmW = $(DomElement()).width();
        $(ResizeGripElement()).css('left', clmW);
    }

    this.RemoveResizerMouseDown = function (handler)
    {
        $(ResizeGripElement()).off("mousedown", handler);
    }

    this.AddResizerMouseUp = function (handler)
    {
        $(ResizeGripElement()).on('mouseup', handler);
    }

    this.RemoveResizerMouseUp = function (handler)
    {
        $(ResizeGripElement()).off('mouseup', handler);
    }


};

function VRowCursor(templateText, isVisible)
{

    var htmlElement;
    var template = templateText;
    var id = App.Utils.ShortGuid();
    var html = template.replace('%id%', id).replace('%cursor_disp%', isVisible ? 'block' : 'none');
    var _mode = 's';
    var selected = false;

    this.Render = function (container)
    {
        container.text += html;
        template = undefined;
    }

    function HtmlElement()
    {
        if (!htmlElement)
            htmlElement = $(document.getElementById(id)).parent()[0];

        return htmlElement;
    }

    this.Selected = function (value)
    {
        if (arguments.length == 0)
            return selected;

        selected = value;

        if (selected)
        {
            $(HtmlElement()).find('img[c-type="row-sel-img"]').show();
        }
        else
        {
            $(HtmlElement()).find('img[c-type="row-sel-img"]').hide();
        }

        //if (_mode == 's')
        //{
        //    if(value)
        //        $(HtmlElement()).find('[c-type="s-cursor"]').show();
        //    else
        //        $(HtmlElement()).find('[c-type="s-cursor"]').hide();
        //}
        //if (_mode == 'm')
        //{
        //    if (value)
        //    {
        //        $(HtmlElement()).find('[c-type="m-cursor-img"]').show();
        //        $(HtmlElement()).find('[c-type="m-cursor"]').show();
        //    }

        //    else
        //    {
        //        $(HtmlElement()).find('[c-type="m-cursor-img"]').hide();
        //        $(HtmlElement()).find('[c-type="m-cursor"]').hide();
        //    }
        //}

        
    }


    this.SetSelectionMode = function (mode)
    {
        //_mode = mode;

        //if (selected)
        //{
        //    if (mode == 'm')
        //    {
        //        $(HtmlElement()).find('[c-type="m-cursor-img"]').show();
        //        $(HtmlElement()).find('[c-type="m-cursor"]').show();
        //        $(HtmlElement()).find('[c-type="s-cursor"]').hide();
        //    }
        //    if (mode == 's')
        //    {
        //        $(HtmlElement()).find('[c-type="m-cursor-img"]').hide();
        //        $(HtmlElement()).find('[c-type="m-cursor"]').hide();
        //        $(HtmlElement()).find('[c-type="s-cursor"]').show();
        //    }
        //}
        //else
        //{
        //    $(HtmlElement()).find('[c-type="m-cursor"]').hide();
        //    $(HtmlElement()).find('[c-type="m-cursor-img"]').hide();
        //    $(HtmlElement()).find('[c-type="s-cursor"]').hide();
        //}

        //if(!selected && mode == 'm')
        //    $(HtmlElement()).find('[c-type="m-cursor"]').show();






     



       
    };
};

function VGridHeaderRow(templateText, colHeaders, cursorTemplate, multiselect)
{
    this.Id = App.Utils.ShortGuid();

    var template = templateText.replace('%id%', this.Id);
    var headers = colHeaders;



    this.Render = function (container)
    {
        var cellsHtml = { text: '' };

        for (var i = 0; i < headers.length; i++)
        {
            headers[i].Render(cellsHtml);
        }
        container.text = template.replace('<!--content-->', cellsHtml.text).replace();
        template = undefined;
    }

    this.Headers = function ()
    {
        return headers;
    }

    this.SetSelectionMode = function (mode)
    {
        if(headers.length > 0 && headers[0].SetSelectionMode)
            headers[0].SetSelectionMode(mode);
        
    };

};

function VGridRow(attrs, templateText, rowIndex, bRow, cursorTemplate, isSelected)
{
    var id = App.Utils.ShortGuid();
    var attrsList = attrs;
    var index = rowIndex;
    var template = templateText.replace('%id%', id);
    var htmlElement = undefined;
    var selected = isSelected;
    var dataItem = bRow;
    var cursor = new VRowCursor(cursorTemplate, isSelected);


    if (index % 2)
    {
        if (selected)
            template = template.replace('%class%', 'AlterRow SelectedRow');
        else
            template = template.replace('%class%', 'AlterRow');
    }
    else
    {
        if (selected)
            template = template.replace('%class%', 'SelectedRow');
        else
            template = template.replace('%class%', '');

    }



    var cells = [];

    this.AddCell = function (cell)
    {
        cells.push(cell);
    };

    this.Render = function (container)
    {
        var cellsHtml = { text: '' };
        cursor.Render(cellsHtml);
        for (var i = 0; i < cells.length; i++)
        {
            cells[i].Render(cellsHtml);
        }

        container.text += template.replace('<!--content-->', cellsHtml.text);;

        template = undefined;

    }

    this.HtmlElement = function ()
    {
        if (!htmlElement)
            htmlElement = document.getElementById(id);

        return htmlElement;
    }

    

    this.Id = function ()
    {
        return id;
    }

    this.Selected = function (value)
    {
        if (arguments.length == 0)
            return selected;

        selected = value;

        if (value)
        {
            $(this.HtmlElement()).addClass('SelectedRow');
            cursor.Selected(true);
        }
        else
        {
            $(this.HtmlElement()).removeClass('SelectedRow');
            cursor.Selected(false);
        }
    }

    this.DataItem = function ()
    {
        return dataItem;
    };

    this.Index = function ()
    {
        return index;
    }

    this.SetSelectionMode = function (mode)
    {
        cursor.SetSelectionMode (mode);
    };

};

function VGridCell(attr, templateText, dataItem, rowsources, wordwrapRowdata, fileState, meta)
{
    this.Id = App.Utils.ShortGuid();
    this.Attr = attr;
    var template = templateText.replace('%id%', this.Id);
    template = template.replace('%ctrl_width%', wordwrapRowdata ? 'auto' : App.Utils.ConvertDataGridControlWidth(attr.GridWidth));
    template = template.replace('%wordWrap%', wordwrapRowdata ? 'white-space: normal !important;' : '');
    var data = dataItem;
    var me = this;

    this.Render = function (container)
    {
        container.text += template.replace('<!--text-->', GetValueText());
        template = undefined;
    }

    function GetValueText()
    {
        if (data.Value == null && me.Attr.Type != 'photo')
            return '';

        if (attr.RowSourceId != '')
        {
            var rsTemplate = HtmlProvider.GetTemplateText('RowSourceComboBox');
            

            var rs = rowsources.Filtered[attr.Id];
            if (!rs)
                rs = rowsources.Unfiltered[attr.RowSourceId];
            if (!rs)
                rs = rowsources.Other[attr.RowSourceId];
            if (!rs)
                rs = Metadata.GetStaticRowSource(attr.RowSourceId);

            for (var z = 0; z < rs.RowSourceData.length; z++)
            {
                var r_data = rs.RowSourceData[z];
                if (r_data.Value == 'True')
                    r_data.Value = true;
                if (r_data.Value == 'False')
                    r_data.Value = false;
            }


            var selected;
            var imgUrl = undefined;



            var hasImages = false;
            for (var k = 0; k < rs.RowSourceData.length; k++)
            {
                if (!App.Utils.IsStringNullOrWhitespace(rs.RowSourceData[k].ImageId))
                    hasImages = true;

                if (dataItem.Value == rs.RowSourceData[k].Value)
                {
                    var selectedRsItem = rs.RowSourceData[k];
                    selected = selectedRsItem.Text;
                    if (selectedRsItem.ImageId)
                    {
                        var imageMeta = Metadata.GetImage(selectedRsItem.ImageId);
                        if (imageMeta)
                            imgUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;
                    }
                    //break;
                }
            }

            if (selected)
            {
                
                rsTemplate = rsTemplate.replace('<!--text-->', selected);
                if (imgUrl)
                {
                    rsTemplate = rsTemplate.replace('%rs_img%', imgUrl);
                    rsTemplate = rsTemplate.replace('%img_disp%', 'block');
                    rsTemplate = rsTemplate.replace('%img_sp_disp%', 'inline-block');
                    
                    // rsTemplate = rsTemplate.replace('%img_div_disp%', 'none');
                }
                else
                {
                    rsTemplate = rsTemplate.replace('%rs_img%', '');
                    rsTemplate = rsTemplate.replace('%img_disp%', 'none');
                    if (!hasImages)
                    {
                        rsTemplate = rsTemplate.replace('%img_sp_disp%', 'none');
                    }
                    else
                    {
                        rsTemplate = rsTemplate.replace('%img_sp_disp%', 'inline-block');
                    }
                    
                    
                    // rsTemplate = rsTemplate.replace('%img_div_disp%', 'block');
                }

               


            }
            else
            {
                rsTemplate = rsTemplate.replace('%rs_img%', '');
                rsTemplate = rsTemplate.replace('%img_disp%', 'none');
            }

            return rsTemplate;
        }

        if (me.Attr.Type == 'file' || me.Attr.Type == 'photo')
        {
            var linkHtml = '';
            if (fileState != BlobStateAttrValues.BLOB_PRESENT_IN_DB)
                linkHtml = HtmlProvider.GetTemplateText('GridFileContentTemplateNoFile');
            else
                linkHtml = HtmlProvider.GetTemplateText('GridFileContentTemplate');

            linkHtml = linkHtml.replace('<!--text-->', GetTxt('Download...')).replace('"%attr-id%"', me.Attr.Id);
            return linkHtml;
        }

        //if (me.Attr.Type == 'photo')
        //{
        //    var linkHtml = '';
        //    if (fileState != BlobStateAttrValues.BLOB_PRESENT_IN_DB)
        //        linkHtml = HtmlProvider.GetTemplateText('type="photo"');
        //    else
        //        linkHtml = HtmlProvider.GetTemplateText('GridPhotoContentTemplate');

        //    linkHtml = linkHtml.replace('<!--text-->', GetTxt('Download...')).replace('"%attr-id%"', me.Attr.Id);
        //    linkHtml = linkHtml.replace('thumb_w', me.Attr.ThumbnailWidth + 'px').replace('thumb_h', me.Attr.ThumbnailHeight);
        //    linkHtml = linkHtml.replace('thumb_url', downloadPhotoUrl + '?entityUsageId=' + meta.Id + '&attributeId=' + me.Attr.Id + '&pkVals=');
            
        //    return linkHtml;
        //}

        if (me.Attr.Type == 'hyperlink')
        {
            var linkHtml = HtmlProvider.GetTemplateText('CommandHyperlinkTemplate');

            linkHtml = linkHtml.replace('<!--text-->', data.Value).replace('"%attr-id%"', me.Attr.Id);
            return linkHtml;
        }


        return data.Value;
    }

  

};

