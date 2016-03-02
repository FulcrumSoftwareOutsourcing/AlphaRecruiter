'use strict';
function ViewLayer()
{
    Layer.call(this);
};

extend(ViewLayer, Layer);

ViewLayer.prototype.BusinessLayer = null;
ViewLayer.prototype.PublicForBusiness = null;