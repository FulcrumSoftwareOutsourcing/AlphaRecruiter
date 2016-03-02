'use strict';
function BUnderConstrustionFrame()
{
    

    BBaseFrame.call(this);

    this.Run = function (treeItem)
    {
        this.ViewLayer.FrameDataLoaded(treeItem);

    };

    
};

extend(BUnderConstrustionFrame, BBaseFrame);

//====================================================================
//====================================================================
//====================================================================




BUnderConstrustionFrame.CreateInstance = function ()
{
    var bFrame = new BUnderConstrustionFrame();
    bFrame.ViewLayer = new VUnderConstrustionFrame();
    bFrame.ViewBorder = function () { };
    bFrame.ConnectLayers();
    return bFrame;
};

Metadata.AddClass("UnderConstructionFrame", BUnderConstrustionFrame);


