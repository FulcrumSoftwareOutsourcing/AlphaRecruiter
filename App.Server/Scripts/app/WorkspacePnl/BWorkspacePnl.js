'use strict';
function BWorkspacePnl()
{
    
    BusinessLayer.call(this);
    this.PublicForView = new BWorkspacePnl_ForView();
    this.PublicForChildren = new BWorkspacePnl_ForChild();

    this.ItIsBWorkspacePnl = true;
    this.ShowDialog = function ()
    {
        this.WorkspacesDlg.ShowDialog();
    };

    this.WorkspaceChanged = function ()
    {
        this.ParentLayer.WorkspaceChanged();
    };
};

extend(BWorkspacePnl, BusinessLayer);

function BWorkspacePnl_ForView()
{
    this.ShowDialog = function () { };
};

function BWorkspacePnl_ForChild()
{
    this.WorkspaceChanged = function () { };
};