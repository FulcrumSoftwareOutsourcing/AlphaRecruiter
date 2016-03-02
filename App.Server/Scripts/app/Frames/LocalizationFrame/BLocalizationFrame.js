'use strict';
function BLocalizationFrame()
{
    BGridFrame.call(this);

    var selectedLangFilter;
    for (var i = 0; i < Metadata.Languages.length; i++)
    {
        var item = Metadata.Languages[i];
        if (item.IsSelected)
        {
            selectedLangFilter = item.LanguageCd;
        }

    }


    this.PublicForView = new BLocalizationFrame_for_View();

    this.base_EntityMetadataLoaded = this.EntityMetadataLoaded;
    this.EntityMetadataLoaded = function (data)
    {
        InsertLangFilterItem(this.FilterItems, selectedLangFilter);

        this.base_EntityMetadataLoaded(data);
    }
  
    this.OnLangFilterChanged = function (langCd)
    {
        selectedLangFilter = langCd;
        this.Refresh();
        
        //this.Find([{ Name: "LANGUAGECD", OperationAsString: 'Equal', Values: [langCd, undefined] }]);
    }
  
    this.base_Refresh = this.Refresh;
    this.Refresh = function ()
    {
        InsertLangFilterItem(this.FilterItems, selectedLangFilter);
        this.base_Refresh();
    }

    function InsertLangFilterItem(filters, val)
    {
        for(var i = 0; i < filters.length; i++)
        {
            var filter = filters[i];
            if (filter.Name == "LANGUAGECD")
            {
                filter.Values[0] = val;
                return;
            }
        }

        filters.push({ Name: "LANGUAGECD", OperationAsString: 'Equal', Operation: 'Equal', Values: [val, undefined] })
    }
}

extend(BLocalizationFrame, BGridFrame);

BLocalizationFrame.CreateInstance = function ()
{
    var bFrame = new BLocalizationFrame();
    bFrame.ViewLayer = new VLocalizationFrame();




    var bCommands = new BCommandBar();
    bCommands.ViewLayer = new VCommandBar();
    bCommands.ViewBorder = new CommandBarVBorder();
    bFrame.AddChild(bCommands, "Commands");


    var bPagingPnl = new BPagingPnl();
    bPagingPnl.ViewLayer = new VPagingPnl();
    bFrame.AddChild(bPagingPnl, 'PagingPnl');

    var bFilters = new BFilters();
    bFilters.ViewLayer = new VFilters();
    bFilters.ViewBorder = new FiltersViewBorder();
    bFrame.AddChild(bFilters, 'Filters');

    var bDataGrid = new BDataGrid();
    bDataGrid.ViewLayer = new VDataGrid();
    bDataGrid.ViewBorder = new DataGridViewBorder();
    bFrame.AddChild(bDataGrid, 'DataGrid');


    bFrame.ConnectLayers();
    return bFrame;
};

Metadata.AddClass("LocalizationFrame", BLocalizationFrame);


function BLocalizationFrame_for_View()
{
    BGridFrame_ForView.call(this);

    this.OnLangFilterChanged = function (langCd) { }
}