'use strict';
function VPagingPnl()
{
    
    ViewLayer.apply(this);
    /// <field name='BusinessLayer' type='BPagingPnl_ForView'/>
    this.PublicForBusiness = new VPagingPnl_ForBusiness();
    this.PublicForParent = new VPagingPnl_ForParent();
    this.RowsPerPage = 25;
    this.CurrentPageNum = 0;;
    this.TotalPages = 0;
    this.TotalRows = 0;
    
    var isBinded = false;
    var tempId;

    var firstBtnEl;
    var backBtnEl;
    var pagesInfoPnlEl;
    var nextBtnEl;
    var lastBtnEl;
    var pageDropDownEl;
    var pagePopupEl;
    var totalRecordsInfoEl;

    var exportBtnEl;


    var firstBtnDisabled = false;
    var backBtnDisabled = false;
    var nextBtnDisabled = false;
    var lastBtnDisabled = false;
    

    var created = false;
    var me = this;

    var pages_info;

    var _metaId;

    this.Create = function (metaId, totalRows, startIndex, rowsPerPage, currentPageNum, totalPages )
    {
        if (!created)
        {
            this.RowsPerPage = rowsPerPage;

            _metaId = metaId;
            var meta = Metadata.GetEntityUsage(metaId);

            tempId = this.ParentLayer.GetTempId();
            var template = HtmlProvider.GetTemplateText(HtmlTemplateIds.PagingPnlTemplate);

            var id = tempId + 'pagingPnlLeft';
            var idRightPart = tempId + 'pagingPnlRight';
            var exportBtnId = tempId + 'exportBtn';
            var html = template.replace('%id%', id).replace('%id_rp%', idRightPart).replace('%exp_id%', exportBtnId);

            $('#padingBar_' + tempId).html(html);
            var panel = $('#' + id);
            var rightPanel = $('#' + idRightPart);

            if (meta.IsPagingEnabled)
                $(panel).show();


            //if()
            firstBtnEl = $(panel).find('[ctrl-type="first"]')[0];
            $(firstBtnEl).children('span').text(GetTxt('First'));
            $(firstBtnEl).on('click', FirstClick);
            
            

            backBtnEl = $(panel).find('[ctrl-type="back"]')[0];
            $(backBtnEl).children('span').text(GetTxt('Back'));
            $(backBtnEl).on('click', BackClick);

            nextBtnEl = $(panel).find('[ctrl-type="next"]')[0];
            $(nextBtnEl).children('span').text(GetTxt('Next'));
            $(nextBtnEl).on('click', NextClick);

            lastBtnEl = $(panel).find('[ctrl-type="last"]')[0];
            $(lastBtnEl).children('span').text(GetTxt('Last'));
            $(lastBtnEl).on('click', LastClick);

            pages_info = $(panel).find('[ctrl-type="pages_info"]')[0];
            $(pages_info).text(GetTxt('Page') + ' ' + currentPageNum + ' ' + 'of' + ' ' + totalPages);

            $(panel).find('[ctrl-type="rows_pp_lbl"]').text(GetTxt('Rows per Page:'));

            pageDropDownEl = $(panel).find('[ctrl-type="rs-paging"]')[0];
            $(pageDropDownEl).children('[ctrl-type="p-sel-text"]').text(this.RowsPerPage);
            $(pageDropDownEl).on('click', OnPagesDropDownClick);

            pagePopupEl = $(pageDropDownEl).find('[ctrl-type="paging-popup"]')[0];
            $(pagePopupEl).on('click', OpenPagingDropDown);
            if (App.Utils.IsIE())
            {
                $(pagePopupEl).find('[c-type="item_500"]').hide();
                $(pagePopupEl).css('top', '-140px');
            }
                


            totalRecordsInfoEl = $(rightPanel).find('[ctrl-type="total_rec_lbl"]')[0];

        

            exportBtnEl = $('#' + exportBtnId)[0];
            var exportTitle = GetTxt('Export');
            $(exportBtnEl).attr('title', GetTxt('Export'));
            $(exportBtnEl).find('img').attr('title', GetTxt('Export'));
            $(exportBtnEl).on("click", ExportClick);
           
            

            $(document).on("click", this.OnBodyClick);
            $(document).on('keydown', OnBodyKeyDown);

            

            created = true;
        }
       
     
       
        $(pages_info).text(GetTxt('Page') + ' ' + currentPageNum + ' ' + 'of' + ' ' + totalPages);
        $(totalRecordsInfoEl).text(GetTxt('Total rows:') + ' ' + totalRows);

        if (this.BusinessLayer.CanFirst())
            $(firstBtnEl).removeClass('DisabledCmd');
        else
            $(firstBtnEl).addClass('DisabledCmd');
        
        if (this.BusinessLayer.CanBack())
            $(backBtnEl).removeClass('DisabledCmd');
        else
            $(backBtnEl).addClass('DisabledCmd');

        if (this.BusinessLayer.CanNext())
            $(nextBtnEl).removeClass('DisabledCmd');
        else
            $(nextBtnEl).addClass('DisabledCmd');

        if (this.BusinessLayer.CanLast())
            $(lastBtnEl).removeClass('DisabledCmd');
        else
            $(lastBtnEl).addClass('DisabledCmd');

       


    }

    function OnPagesDropDownClick(e)
    {
        if (pagePopupEl.style.display == 'block')
        {
            pagePopupEl.style.display = 'none';
        }
        else
        {
            pagePopupEl.style.display = 'block';

            var items =  $(pagePopupEl).find('[ctrl-type="p-item-text"]');
            for (var i = 0; i < items.length; i++)
            {
                $(items[i]).parent().attr('class', 'RsPagingItem');
                if ($(items[i]).text() == (me.RowsPerPage + ''))
                {
                    $(items[i]).parent().attr('class', 'RsPagingItem selected');
                }

            }
            
        }

        

        return false;
    }

    function OnPagesItemClick()
    {
        var items = $(pagePopupEl).find('[ctrl-type="p-item-text"]');
        for (var i = 0; i < items.length; i++)
        {
            $(items[i]).parent().attr('class', 'RsPagingItem');
            if ($(items[i]).text() == (me.RowsPerPage + ''))
            {
                $(items[i]).parent().attr('class', 'RsPagingItem selected');
            }

        }
    }

    function OpenPagingDropDown(e)
    {
        var clicked = $(e.target).text();
        if (clicked != (me.RowsPerPage + ''))
        {
            me.RowsPerPage = new Number(clicked);
            $(pageDropDownEl).children('[ctrl-type="p-sel-text"]').text(clicked);
            Settings.AddChangedSettings('grid_' + _metaId, 'page_size', me.RowsPerPage);
            me.BusinessLayer.RowsPerPageChanged(me.RowsPerPage);
        }

    }

    var FirstClick = function ()
    {
        if (me.BusinessLayer.CanFirst())
            me.BusinessLayer.PageChanged('first');
    };

    var BackClick = function ()
    {
        if (me.BusinessLayer.CanBack())
            me.BusinessLayer.PageChanged('back');
    };

    var NextClick = function ()
    {
        if (me.BusinessLayer.CanNext())
            me.BusinessLayer.PageChanged('next');
    };

    var LastClick = function ()
    {
        if (me.BusinessLayer.CanLast())
            me.BusinessLayer.PageChanged('last');
    };

    var ExportClick = function ()
    {
        me.BusinessLayer.ExportToCsv();
    };

    var CustomizationClick = function ()
    {
    };

    this.PagingChanged = function (isPagingEnabled, rowsPerPage, currentPageNum, totalPages, totalRows)
    {
        if (!isBinded)
        {
            tempId = this.ParentLayer.GetTempId();

            isBinded = true;
        }
    };

    this.OnBodyClick = function ()
    {
        $(pagePopupEl).hide();
    };

    function OnBodyKeyDown(e)
    {

        if (e.keyCode != 40 && e.keyCode != 38 && e.keyCode != 13)
        {
            $(me.pagePopupEl).hide();
        }

    };

    this.Dispose = function ()
    {
        $(document).off("click", this.OnBodyClick);
        $(document).off('keydown', OnBodyKeyDown);

        $(backBtnEl).off('click', BackClick);
        $(firstBtnEl).off('click', FirstClick);
        $(nextBtnEl).off('click', NextClick);
        $(lastBtnEl).off('click', LastClick);
        $(pageDropDownEl).off('click', OnPagesDropDownClick);
        $(pagePopupEl).off('click', OpenPagingDropDown);
        $(exportBtnEl).off('click', ExportClick);

    }
};

extend(VPagingPnl, ViewLayer);

function VPagingPnl_ForBusiness()
{
    this.Create = function (rowsPerPage, currentPageNum, totalPages, totalRows){}
}

function VPagingPnl_ForParent()
{
    this.Dispose = function () { }
}

