'use strict';
function BFramesRoot()
{
    BusinessLayer.call(this);
    this.PublicForParent = new BFrame_ForParent();
    var currentFrame = null;
    


    this.Run = function (treeItem)
    {
        if (App.Utils.IsStringNullOrWhitespace(treeItem.EntityMetadataId) && App.Utils.IsStringNullOrWhitespace(treeItem.FrameClassId))
        {
            treeItem.FrameClassId = 'UnderConstructionFrame';
        }

        if (!App.Utils.IsStringNullOrWhitespace(treeItem.DashboardId) || treeItem.FrameClassId == 'LinkedFrame')//TODO: implement dashboard
        {
            treeItem.FrameClassId = 'UnderConstructionFrame';

           // Navigation.CloseCurrentFrame();
            
           // Resizer.ResizeRequest();
           // return;
        }

        var frameClassId = treeItem.FrameClassId;
        
        if (App.Utils.IsStringNullOrWhitespace(frameClassId))
        {
            frameClassId = 'CxGridFrame'
        }
        
            //frameClass = 
        var classInstance = Metadata.CreateClassInstance(frameClassId);
            currentFrame = null;

            

            if (classInstance)
            {
                currentFrame = classInstance;
                currentFrame.Run(treeItem);
                Resizer.ResizeRequest();
            }
            else
            {
                this.ViewLayer.RemoveCurrentFrame();
            }
            
            
    };

    this.OpenFrame = function (frame)
    {
        currentFrame = frame;
        currentFrame.Run();
    };



};

extend(BFramesRoot, BusinessLayer);

function BFrame_ForParent()
{
    this.Run = function (treeItem) { };
};