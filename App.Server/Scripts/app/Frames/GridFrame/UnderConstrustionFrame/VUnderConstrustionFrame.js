'use strict';
function VUnderConstrustionFrame()
{

    this.PublicForBusiness = new VUnderConstrustionFrame_forBus();

    VBaseFrame.call(this);

    var isBinded = false;

    this.UsedTemplatesIds = ['UnderConstrustionFrameTemplate'];

    

    this.FrameDataLoaded = function (treeItem)
    {


        if (!isBinded)
        {
            
            var holder = document.getElementById('FrameHolder');
                ko.cleanNode(holder);
                $(holder).html('');
                $(holder).attr('data-bind', 'template:{name:"' + this.UsedTemplatesIds[0] + '"}');
                ko.applyBindings(this, holder);

            

            isBinded = true;

        }


    };


};

function VUnderConstrustionFrame_forBus()
{
    VBaseFrame_ForBusiness.call(this);
    this.Run = function (treeItem){    };

}


extend(VUnderConstrustionFrame, VBaseFrame);