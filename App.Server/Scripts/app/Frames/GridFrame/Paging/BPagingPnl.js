'use strict';
function BPagingPnl()
{
    BusinessLayer.apply(this);
    this.PublicForParent = new BPagingPnl_ForParent();
    this.PublicForView = new BPagingPnl_ForView();

    var m_TotalRecordAmount = 0;
    var m_CurrentPageIndex;
    var m_PageSize;
    var TotalPageAmount;

    

    this.Run = function (metaId, data)
    {
        var meta = Metadata.GetEntityUsage(metaId);
        
        if (m_TotalRecordAmount != data.EntityList.TotalDataRecordAmount || data.EntityList.TotalDataRecordAmount == 0 || data.EntityList.TotalDataRecordAmount == -1)
        {
            m_CurrentPageIndex = 1;
        }

        if (data.EntityList.TotalDataRecordAmount == -1)
            m_TotalRecordAmount = 1;
        else
            m_TotalRecordAmount = data.EntityList.TotalDataRecordAmount;

        m_PageSize = data.RecordsAmount;

        

        if (m_PageSize != 25 &&
            m_PageSize != 50 &&
            m_PageSize != 100 &&
            m_PageSize != 200 &&
            m_PageSize != 500 )
        {
            m_PageSize = 50;
            Settings.AddChangedSettings('grid_' + metaId, 'page_size', m_PageSize);
            
        }

        if (App.Utils.IsIE() && m_PageSize == 500)
        {
            m_PageSize = 200;
            Settings.AddChangedSettings('grid_' + metaId, 'page_size', m_PageSize);
        }

        TotalPageAmount = Math.ceil(((m_TotalRecordAmount - 1) / m_PageSize));
        if (TotalPageAmount == 0)
            TotalPageAmount = 1;


        this.ViewLayer.Create(metaId, m_TotalRecordAmount, m_CurrentPageIndex, m_PageSize, m_CurrentPageIndex, TotalPageAmount);


        
    };

    this.RowsPerPageChanged = function (rpp)
    {
        this.ParentLayer.RowsPerPageChanged(rpp)
        m_CurrentPageIndex = 1;
    };

    this.PageChanged = function (op)
    {
        if (op == 'first')
        {
            this.ChangePageIndex(1);
        }
        if (op == 'back')
        {
            this.ChangePageIndex(Math.max(m_CurrentPageIndex - 1, 1));
        }
        if (op == 'next')
        {
            this.ChangePageIndex(Math.min(m_CurrentPageIndex + 1, TotalPageAmount));
        }
        if (op == 'last')
        {
            this.ChangePageIndex(TotalPageAmount);
        }
    };

    this.ChangePageIndex = function (pageIndexNew)
    {
       if (m_CurrentPageIndex != pageIndexNew)
       {
           m_CurrentPageIndex = pageIndexNew;
           this.ParentLayer.PageIndexChanged((m_CurrentPageIndex - 1) * m_PageSize);
       }
    }

    this.CanFirst = function ()
    {
        return m_CurrentPageIndex != 1;
    };

    this.CanBack = function ()
    {
        return m_CurrentPageIndex != 1;
    };

    this.CanNext = function ()

    {
        return m_CurrentPageIndex != TotalPageAmount;
    };
    this.CanLast = function ()
    {
        return m_CurrentPageIndex != TotalPageAmount;
    };

    this.ExportToCsv = function ()
    {
        this.ParentLayer.ExportToCsv();
    };

    this.Reset = function ()
    {
        m_CurrentPageIndex = 0;
    };
};

extend(BPagingPnl, BusinessLayer);

function BPagingPnl_ForParent()
{
    this.Run = function (metaId, data) { };
    this.Reset = function (){ };
};

function BPagingPnl_ForView()
{
    this.RowsPerPageChanged = function (rowsPerPage) { };
    this.PageChanged = function (op) { };

    this.CanFirst = function () { };
    this.CanBack = function () { };
    this.CanNext = function () { };
    this.CanLast = function () { };
    this.ExportToCsv = function (){};
};