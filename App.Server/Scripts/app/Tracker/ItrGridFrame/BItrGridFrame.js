'use strict';
function BItrGridFrame()
{
    BGridFrame.call(this);

    this.base_ITR_Run = this.Run;
    this.Run = function (treeItem)
    {
        this.base_ITR_Run(treeItem);
    }
};

extend(BItrGridFrame, BGridFrame);













BItrGridFrame.base_CreateInstance = BGridFrame.CreateInstance;
BItrGridFrame.CreateInstance = function ()
{
    return this.base_CreateInstance();
    //var bItrGridFrame = new BItrGridFrame();
    //bItrGridFrame.ViewLayer = new VItrGridFrame();

    //var bCommands = new BCommandBar();
    //bCommands.ViewLayer = new VCommandBar();
    //bCommands.ViewBorder = new CommandBarVBorder();
    //bItrGridFrame.AddChild(bCommands, "Commands");


    //var bPagingPnl = new BPagingPnl();
    //bPagingPnl.ViewLayer = VPagingPnl();
    //bItrGridFrame.AddChild(bPagingPnl, 'PagingPnl');

    



    //bItrGridFrame.ConnectLayers();
    //return bItrGridFrame;
};

Metadata.AddClass("Itr_GridFrame", BItrGridFrame);