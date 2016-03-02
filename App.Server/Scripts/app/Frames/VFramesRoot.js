'use strict';
function VFramesRoot()
{
    ViewLayer.call(this);
    this.PublicForBusiness = new VFramesRoot_ForBusiness();

    this.Caption = ko.observable('');
    var currentFrame = null;
    
    this.SetCurrentFrame = function (frame)
    {
        
        Navigation.AddFrame(frame);
        if (currentFrame)
            this.RemoveCurrentFrame();
        currentFrame = frame;
    };

    this.RemoveCurrentFrame = function ()
    {
        if (currentFrame )
        {
            Resizer.RemoveResizable(currentFrame);
            if (currentFrame.Dispose)
            {
                currentFrame.Dispose();
            }
        }


        ko.cleanNode(document.getElementById('FrameHolder'));
        $('#FrameHolder').html('');
    };




};

extend(VFramesRoot, ViewLayer);

function VFramesRoot_ForBusiness()
{
    this.SetCurrentFrame = function (frame) { };
    this.RemoveCurrentFrame = function () { };
};