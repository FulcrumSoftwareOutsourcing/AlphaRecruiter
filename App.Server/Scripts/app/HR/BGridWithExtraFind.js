'use strict';
function BGridWithExtraFind()
{
    BGridFrame.call(this);

    this.PublicForView = new BGridWithExtraFind_for_View();

    var wasAdtSearch = false;

    this.AdtFindChanged = function (text)
    {
        this.Find([{ Name: "LASTNAME", OperationAsString: 'Like', Operation: 'Like', Values: [text, undefined] }]);
    }

   

}

extend(BGridWithExtraFind, BGridFrame);

BGridWithExtraFind.CreateInstance = function ()
{
    var bFrame = new BGridWithExtraFind();
    bFrame.ViewLayer = new VGridWithExtraFind();
   



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

Metadata.AddClass("GridWithExtraFind", BGridWithExtraFind);


function BGridWithExtraFind_for_View()
{
    BGridFrame_ForView.call(this);

    this.AdtFindChanged = function (text){}
}