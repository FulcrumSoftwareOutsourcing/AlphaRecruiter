'use strict';
function LayerFasade(layer, borderLine, methods, messageSource)
{
   
    var me = this;

    if (layer instanceof DataLayer)
    {
        if (!methods)
        {
            methods = [];
        }
        methods.push("Request");
    }

    if (layer instanceof BusinessLayer)
    {
        if (!methods)
        {
            methods = [];
        }
        methods.push("OnUnauthorized");
    }

    //if (layer instanceof ViewLayer)
    //{
    //    if (!methods)
    //    {
    //        methods = [];
    //    }
    //    methods.push("OnResize");
    //}

    var tempId = App.Utils.ShortGuid();
    this.TempId = tempId;
    window[tempId] = this;

    var realLayerId = App.Utils.ShortGuid();
    this.RealLayerId = realLayerId;
    window[realLayerId] = layer;

    var bLineId = App.Utils.ShortGuid();
    this.BLineId = bLineId;
    if(borderLine)
        window[bLineId] = borderLine;

    

    for (var i = 0; i < methods.length; i++)
    {
        var methodName = methods[i];

        var converterPresent = false;
        if (borderLine && borderLine[methodName])
            converterPresent = true;

        var callMethodScript = 'return realLayer.' + methodName + '.apply(realLayer, finalParams);';
        if (messageSource == MessageSource.View || messageSource == MessageSource.Data )
        {
            var goAheadLayer = '';
            if(messageSource == MessageSource.View)
                goAheadLayer = 'DataLayer'
            if (messageSource == MessageSource.Data)
                goAheadLayer = 'ViewLayer'

         callMethodScript = 'if(!realLayer.' + methodName + ')' +
            '{' +
            'if(realLayer.' + goAheadLayer + '.' + methodName + ')' +
                'return realLayer.' + goAheadLayer + '.' + methodName + '.apply(' + goAheadLayer + ', finalParams);' +
        '}' +
            'else ' +
        ' return realLayer.' + methodName + '.apply(realLayer, finalParams);';

        }

        var fasadeMethodScript;
        if (converterPresent)
        {
            fasadeMethodScript = 
                  'window.' + tempId + '.' + methodName + '=function(){' +
                            'var bLine=window.' + bLineId + ';' +
                         'var converted=bLine.' + methodName + '.apply(bLine, arguments);' +

                          'var finalParams=arguments;' +
                          'if (converted)' +
                          '{' +
                                'if ((converted instanceof Array)==false)' +
                                    'finalParams=[converted]; ' +
                                'else ' +
                                    'finalParams=converted; ' +
                          '}' +

                          'var realLayer=window.' + realLayerId + ';' +
                            callMethodScript +

                  '};';

        }
        else
        {
            fasadeMethodScript = 
                   ' window.' + tempId + '.' + methodName + '=function(){' +
                           'var finalParams=arguments;' +
                           'var realLayer=window.' + realLayerId + ';' +
                             callMethodScript +

                   '};';
        }


      
        LayerFasade.AllDynScripts += fasadeMethodScript

      

    
    }



    //this.Dispose = function ()
    //{
    //    if (window[this.RealLayerId].Dispose)
    //    {
    //        window[this.RealLayerId].Dispose();
    //    }

        
    //    delete window[this.BLineId];
    //   // delete window[this.RealLayerId];
    //    delete window[this.TempId];
    //};


};

LayerFasade.AllDynScripts = '';

function MessageSource() { };
MessageSource.View = 0;
MessageSource.Data = 1;