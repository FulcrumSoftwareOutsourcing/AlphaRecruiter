'use strict';
function VWorkspacePnl()
{
    ViewLayer.call(this);
    this.PublicForParent = new VWorkspacePnl_ForParent();

    this.ItIsVWorkspacePnl = true;


    this.SelectedWorkspace = ko.observable();

    var isBinded = false;

    this.CreatePanel = function ()
    {
        this.SelectedWorkspace(Metadata.GetWorkspace(Metadata.APPLICATION$CURRENTWORKSPACEID));
        if (!isBinded)
        {
            ko.applyBindings(this, document.getElementById('WorkspacesPnlHolder'));
            isBinded = true;
        }
    };

    this.OnClick = function ()
    {
        this.BusinessLayer.ShowDialog();
    };
};

extend(VWorkspacePnl, ViewLayer);

function VWorkspacePnl_ForParent()
{
    this.CreatePanel = function () { };
};