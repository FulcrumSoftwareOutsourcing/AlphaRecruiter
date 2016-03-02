'use strict';
function BusinessLayer()
{
    Layer.call(this);
    
    this.OnUnauthorized = function (postData, token)
    {
        ShowLoginForm(this, postData, token);
    };


};

extend(BusinessLayer, Layer);

BusinessLayer.prototype.ConnectLayers = function ()
{
    this.ConnectLayersInternal();
    var scriptId = App.Utils.ShortGuid();
    $('body').append('<script id=' + scriptId + ' type="text/javascript" >' + LayerFasade.AllDynScripts + '</script>');
    $('#' + scriptId).remove();
    delete this.ConnectLayersInternal;
    delete this.ConnectLayers;
};



BusinessLayer.prototype.ConnectLayersInternal = function ()
{
    delete this.AddChild;

    var dataLayer = this.DataLayer;
    if (!dataLayer)
        dataLayer = new DataLayer();

    this.DataLayer = new LayerFasade(dataLayer, this.DataBorder, App.Utils.GetMethods(dataLayer.PublicForBusiness));
    dataLayer.BusinessLayer = new LayerFasade(this, this.DataBorder, App.Utils.GetMethods(this.PublicForData), MessageSource.Data);
    //this.DataLayer.PublicForParent = dataLayer.PublicForParent;
    //this.DataLayer.PublicForChildren = dataLayer.PublicForChildren;
    //this.DataLayer.Children = dataLayer.Children;
    delete dataLayer.PublicForBusiness;

    var viewLayer = this.ViewLayer;
    if (viewLayer)
    {
        this.ViewLayer = new LayerFasade(viewLayer, this.ViewBorder, App.Utils.GetMethods(viewLayer.PublicForBusiness));
        viewLayer.BusinessLayer = new LayerFasade(this, this.ViewBorder, App.Utils.GetMethods(this.PublicForView), MessageSource.View);
        //this.ViewLayer.PublicForParent = viewLayer.PublicForParent;
        //this.ViewLayer.PublicForChildren = viewLayer.PublicForChildren;
        //this.ViewLayer.Children = viewLayer.Children;
        delete viewLayer.PublicForBusiness;
    }

    delete this.PublicForData;
    delete this.PublicForView;
    delete this.ViewBorder;
    delete this.DataBorder;


  


    ConnectParentWithChild(this);
    ConnectParentWithChild(viewLayer);
    ConnectParentWithChild(dataLayer);

  
    if (this.Children)
    {
        for (var i = 0; i < this.Children.length; i++)
        {
            this.Children[i].layer.ConnectLayersInternal();
        }
    }

    ReplaceChild(this);
    ReplaceChild(viewLayer);
    ReplaceChild(dataLayer);

    //delete this.Children;
    delete this.PublicForParent;
    delete this.PublicForChildren;

    if (this.ViewLayer)
    {
        delete this.ViewLayer.PublicForParent;
        delete this.ViewLayer.PublicForChildren;
       // delete this.ViewLayer.Children;
    }
    if (this.DataLayer)
    {
        delete this.DataLayer.PublicForParent;
        delete this.DataLayer.PublicForChildren;
        //delete this.DataLayer.Children;
    }


    function ConnectParentWithChild  (parent)
    {
        if (!parent || !parent.Children)
            return;

        for (var i = 0; i < parent.Children.length; i++)
        {
            var child = parent.Children[i].layer;
            var propName = parent.Children[i].propName;
            parent[propName] = new LayerFasade(child, null, App.Utils.GetMethods(child.PublicForParent));
            child.ParentLayer = new LayerFasade(parent, null, App.Utils.GetMethods(parent.PublicForChildren));

          //  parent.Children[i] = parent[propName];
        }

        
        
    };

    function ReplaceChild(parent)
    {
        if (!parent || !parent.Children)
            return;

        for (var i = 0; i < parent.Children.length; i++)
        {
            var propName = parent.Children[i].propName;
            parent.Children[i] = parent[propName];
        }



    };

    
    
};


BusinessLayer.prototype.AddChild = function (childLayer, layerPropertyName)
{
    if (App.Utils.IsStringNullOrWhitespace(layerPropertyName))
        throw new Error("Parameter <layerPropertyName> is required.");

    this.Children.push({ layer: childLayer, propName: layerPropertyName });

    if (this.ViewLayer && childLayer.ViewLayer)
    {
        this.ViewLayer.Children.push({ layer: childLayer.ViewLayer, propName: layerPropertyName });
    }
    if (this.DataLayer && childLayer.DataLayer)
    {
        this.DataLayer.Children.push({ layer: childLayer.DataLayer, propName: layerPropertyName });
    }
};


BusinessLayer.prototype.ViewLayer = null;
BusinessLayer.prototype.DataLayer = null;

BusinessLayer.prototype.ViewBorder = null;
BusinessLayer.prototype.DataBorder = null;




BusinessLayer.prototype.PublicForView = null;
BusinessLayer.prototype.PublicForData = null;


