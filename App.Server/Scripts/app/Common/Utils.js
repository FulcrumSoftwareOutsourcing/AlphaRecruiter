

function extend(Child, Parent)
{
    var F = function () { };
    F.prototype = Parent.prototype;
    Child.prototype = new F();
    Child.prototype.constructor = Child;
    Child.superclass = Parent.prototype;
};

function App() { };
App.Utils = function () { };

App.Utils.GlobaIdCounter = 0;
App.Utils.ShortGuid = function ()
{
   // App.Utils.GlobaIdCounter ++;
   // return '_' + App.Utils.GlobaIdCounter;
    var S4 = function ()
    {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return '_' + (S4() + S4() /*+ S4()*/);
};

App.Utils.Guid = function ()
{
    
    var S4 = function ()
    {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return '_' + (S4() + S4() + S4());
};

App.Utils.IsStringNullOrWhitespace = function (str)
{
    if (str == null)
        return true;

    var hasSymbols = false;
    for (var i = 0; i < str.length; i++)
    {
        if (str[i] != ' ')
            hasSymbols = true;
    }

    if (str != undefined && str != null && str != '' && hasSymbols)
        return false;

    return true;
};

App.Utils.GetMethods = function (obj)
{
    if (!obj)
        return [];

    var methods = [];
    for (var m in obj)
    {
        if (typeof obj[m] == 'function')
        {
            methods.push(m);
        }
    }
    return methods;
};

App.Utils.GetRequestVerificationToken = function ()
{
    var tokenDescr = { name: null, value: null };
    var token = $("input[name^='__RequestVerificationToken']").val();
    var tokenValMame = $("input[name^='__RequestVerificationToken']").attr('name');
    tokenDescr.name = tokenValMame;
    tokenDescr.value = token;
    return tokenDescr;
};

App.Utils.AddRequestVerificationToken = function (token)
{
    $("input[name^='__RequestVerificationToken']").remove();
    $('body').append(token);
};

App.Utils.IsIE = function ()
{
    return ((navigator.appName == 'Microsoft Internet Explorer') || ((navigator.appName == 'Netscape') && (new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})").exec(navigator.userAgent) != null)));
}

App.Utils.IsMobile = function()
{
    var isMobile = ('DeviceOrientationEvent' in window || 'orientation' in window);
    // But with my Chrome on windows, DeviceOrientationEvent == fct()
    if (/Windows NT|Macintosh|Mac OS X|Linux/i.test(navigator.userAgent))
        isMobile = false;
    // My android have "linux" too
    if (/Mobile/i.test(navigator.userAgent))
        isMobile = true;

    return isMobile;
}

App.Utils.CheckFileLenght = function (file)
{
    var maxUploadSize = 2147483647;

    if( !App.Utils.IsStringNullOrWhitespace ( Metadata.APPLICATION$MAXUPLOADSIZE) )
        maxUploadSize = new Number( Metadata.APPLICATION$MAXUPLOADSIZE );



    if (file.size > maxUploadSize)
    {
        ShowMessage(GetTxt('File') + ' ' + file.name + ' ' + GetTxt('has length more than Server settings permit to upload.'), 'Too long file', [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Error);
        return false;
    }
    return true;
}


if (!Array.prototype.forEach)
{
    Array.prototype.forEach = function (func)
    {
        for (var i = 0; i < this.length; i++)
        {
            func(this[i], i, this);
        }
    }
};

function PostDataObject()
{
    this.PostData = {};
    this.PostData.RequiredTemplates = [];
};

PostDataObject.prototype.WebRequestUrl = null;
PostDataObject.prototype.WebRequestCallback = null;
PostDataObject.prototype.PostData = null;
PostDataObject.prototype.WaitingId = null;

function Delegate(handler, context, handlerParams)
{
    this.Handler = handler;
    var context = context;
    var params = handlerParams;

    this.Invoke = function ()
    {
        if (params)
            this.Handler.apply(context, params);
        else
            this.Handler.apply(context, arguments);
    };

    this.IsTheSameContext = function (val)
    {
        return context == val;
    };
};




(function ()
{
    var attachEvent = document.attachEvent;
    var isIE = navigator.userAgent.match(/Trident/);
   
    var requestFrame = (function ()
    {
        var raf = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame ||
            function (fn) { return window.setTimeout(fn, 20); };
        return function (fn) { return raf(fn); };
    })();

    var cancelFrame = (function ()
    {
        var cancel = window.cancelAnimationFrame || window.mozCancelAnimationFrame || window.webkitCancelAnimationFrame ||
               window.clearTimeout;
        return function (id) { return cancel(id); };
    })();

    function resizeListener(e)
    {
        var win = e.target || e.srcElement;
        if (win.__resizeRAF__) cancelFrame(win.__resizeRAF__);
        win.__resizeRAF__ = requestFrame(function ()
        {
            var trigger = win.__resizeTrigger__;
            trigger.__resizeListeners__.forEach(function (fn)
            {
                fn.call(trigger, e);
            });
        });
    }

    function objectLoad(e)
    {
        this.contentDocument.defaultView.__resizeTrigger__ = this.__resizeElement__;
        this.contentDocument.defaultView.addEventListener('resize', resizeListener);
    }

    window.addResizeListener = function (element, fn)
    {
        if (!element.__resizeListeners__)
        {
            element.__resizeListeners__ = [];
            if (attachEvent)
            {
                element.__resizeTrigger__ = element;
                element.attachEvent('onresize', resizeListener);
            }
            else
            {
                if (getComputedStyle(element).position == 'static') element.style.position = 'relative';
                var obj = element.__resizeTrigger__ = document.createElement('object');
                obj.setAttribute('style', 'display: block; position: absolute; top: 0; left: 0; height: 100%; width: 100%; overflow: hidden; pointer-events: none; z-index: -1;');
                obj.__resizeElement__ = element;
                obj.onload = objectLoad;
                obj.type = 'text/html';
                if (isIE) element.appendChild(obj);
                obj.data = 'about:blank';
                if (!isIE) element.appendChild(obj);
            }
        }
        element.__resizeListeners__.push(fn);
    };

    window.removeResizeListener = function (element, fn)
    {
        element.__resizeListeners__.splice(element.__resizeListeners__.indexOf(fn), 1);
        if (!element.__resizeListeners__.length)
        {
            if (attachEvent) element.detachEvent('onresize', resizeListener);
            else
            {
                element.__resizeTrigger__.contentDocument.defaultView.removeEventListener('resize', resizeListener);
                element.__resizeTrigger__ = !element.removeChild(element.__resizeTrigger__);
            }
        }
    }
})();


ko.observableArray.fn.pushAll = function (valuesToPush)
{
    var underlyingArray = this();
    this.valueWillMutate();
    ko.utils.arrayPushAll(underlyingArray, valuesToPush);
    this.valueHasMutated();
    return this;
};

function GetClientHeight()
{
    if (navigator.userAgent.search("Opera") >= 0)
    {
        return window.innerHeight;
    }

    var v = 0, d = document, w = window;
    if ((!d.compatMode || d.compatMode == 'CSS1Compat') && !w.opera && d.documentElement && d.documentElement.clientHeight)
    { v = d.documentElement.clientHeight; }
    else if (d.body && d.body.clientHeight)
    { v = d.body.clientHeight; }
    else if (xDef(w.innerWidth, w.innerHeight, d.height))
    {
        v = w.innerHeight;
        if (d.height > w.innerHeight) v -= 16;
    }
    return v;
};

function GetClientWidth()
{
    if (document.offsetWidth)
        return document.offsetWidth;

    if (navigator.userAgent.search("Opera") >= 0)
    {
        return window.innerWidth;
    }

    var v = 0, d = document, w = window;
    if ((!d.compatMode || d.compatMode == 'CSS1Compat') && !w.opera && d.documentElement && d.documentElement.clientWidth)
    { v = d.documentElement.clientWidth; }
    else if (d.body && d.body.clientWidth)
    { v = d.body.clientWidth; }
    else if (xDef(w.innerWidth, w.innerHeight, d.height))
    {
        v = w.innerWidth;
        if (d.width > w.innerWidth) v -= 16;
    }
    return v;
};

function AppSizes()
{
    this.ClientWidth = 0;
    this.ClientHeight = 0;
    this.TopHeight = 0;
    this.SectionsHeight = 0;
    this.SectionsWidth = 0;
};




function ResizeDispatcher()
{
    this.OnClientResized = function ()
    {
        rtime = new Date();
        if (timeout === false)
        {
            timeout = true;
            setTimeout(ResizeEnd, delta);
        }
    }

    if (typeof window.addEventListener !== 'undefined')
        window.addEventListener('resize', this.OnClientResized);
    else if (typeof window.attachEvent !== 'undefined')
        window.attachEvent('onresize', this.OnClientResized);

    var appSizes = new AppSizes();
    var listeners = [];

    var rtime = new Date(1, 1, 2000, 12, 0, 0);
    var timeout = false;
    var delta = 200;

    this.AddResizeListener = function(delegate, callOrder)
    {
        //0 - root;
        //5 - popup;
        //10 - sections
        //15 - Tree
        listeners[callOrder] = delegate;
    };

   

    function ResizeEnd()
    {
        if (new Date() - rtime < delta)
        {
            setTimeout(ResizeEnd, delta);
        } else
        {
            timeout = false;

            //appSizes.ClientHeight = GetClientHeight();
            //appSizes.ClientWidth = GetClientWidth();

            //for (var i = 0; i < listeners.length; i++)
            //{
            //    if (listeners[i])
            //    {
            //        listeners[i].Invoke(appSizes);
            //    }

            //}

        }
    };

    this.ResizeNow = function ()
    {
        ResizeEnd();
    };


};

function BusyIndicator(id, diameter, color)
{
    var loader = new CanvasLoader(id);
    loader.setDiameter(diameter);
    loader.setDensity(109);
    loader.setRange(0.8);
    loader.setSpeed(4);
    loader.setFPS(30);
    loader.setColor(color);

    this.Show = function ()
    {
        loader.show();
    };

    this.Hide = function ()
    {
        loader.hide();
    };
};

Array.prototype.sortBy = function (p)
{
    return this.slice(0).sort(function (a, b)
    {
        return (a[p] > b[p]) ? 1 : (a[p] < b[p]) ? -1 : 0;
    });
};

App.Utils.ConvertDateTime = function (jsonDate )
{
    if (!jsonDate || jsonDate == '')
        return '';

    var re = /-?\d+/;
    var m = re.exec(jsonDate);
    var date = Date(parseInt(m[0]));

}

Array.prototype.Add = function (key, value)
{
    this.push(value);
    this[key] = value;
};

Array.prototype.ContainsKey = function (key)
{
    return (this[key] != undefined)
};

Array.prototype.Remove = function (key)
{
    if ((this[key] != undefined))
    {

        this._remove(this.indexOf(this[key]))
        delete this[key];
    }
};

Array.prototype._remove = function (from, to)
{
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};

App.Utils.GetRandomInt = function (min, max)
{
    return Math.floor(Math.random() * (max - min + 1)) + min;
};

App.Utils.IsObject = function(obj)
{
    return obj === Object(obj);
}

App.Utils.IsNormalInteger = function(str)
{
    return /^\+?(0|[1-9]\d*)$/.test(str);
}

App.Utils.IsNormalFloat = function (str)
{
    return !isNaN(parseFloat(n)) && isFinite(n);
}

App.Utils.ConvertDataGridControlWidth = function(layoutSize)
{
    if (layoutSize <= 0)
        return '';
    else
        return layoutSize + 'px';
}

App.Utils.ConvertDataGridColumnWidth = function(layoutSize)
{
    if (layoutSize <= 0)
        return '';
    else
        return layoutSize;
}

App.Utils.HumanFileSize = function(bytes, si)
{
    var thresh = si ? 1000 : 1024;
    if (Math.abs(bytes) < thresh)
    {
        return bytes + ' B';
    }
    var units = si
        ? ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
        : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
    var u = -1;
    do
    {
        bytes /= thresh;
        ++u;
    } while (Math.abs(bytes) >= thresh && u < units.length - 1);
    return bytes.toFixed(1) + ' ' + units[u];
}

App.Utils.UpdateURLParameter = function (url, param, paramVal)
{
    var newAdditionalURL = "";
    var tempArray = url.split("?");
    var baseURL = tempArray[0];
    var additionalURL = tempArray[1];
    var temp = "";
    if (additionalURL)
    {
        tempArray = additionalURL.split("&");
        for (i = 0; i < tempArray.length; i++)
        {
            if (tempArray[i].split('=')[0] != param)
            {
                newAdditionalURL += temp + tempArray[i];
                temp = "&";
            }
        }
    }

    var rows_txt = temp + "" + param + "=" + paramVal;
    return baseURL + "?" + newAdditionalURL + rows_txt;
}