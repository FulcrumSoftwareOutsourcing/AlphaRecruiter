'use strict';
function WaitDispatcher()
{
    this.IndividualWaiters = {};
    this.FullScreenWaiters = { Count: 0 };
};

WaitDispatcher.prototype.IndividualWaiters = null;
WaitDispatcher.prototype.FullScreenWaiters = null;
WaitDispatcher.prototype.FullScreenWaiterInfo = null;

WaitDispatcher.prototype.ShowWaiter = function (id)
{
    if (typeof this.IndividualWaiters[id] !== 'undefined')
    {
        var waiter = this.IndividualWaiters[id];
        waiter.ShowWaiter.call(waiter.Context);
    }
    else
    {
        this.FullScreenWaiters[id] = id;
        this.FullScreenWaiters.Count++;
        var me = this;
        setTimeout(
            function ()
            {
                if (me.FullScreenWaiters.Count > 0)
                {
                    me.FullScreenWaiterInfo.ShowWaiter.call(me.FullScreenWaiterInfo.Context);
                }
            },
            100);
    }
};



WaitDispatcher.prototype.HideWaiter = function (id)
{
    if (typeof this.IndividualWaiters[id] !== 'undefined')
    {
        var waiter = this.IndividualWaiters[id];
        waiter.HideWaiter.call(waiter.Context);
    }
    else
    {
        delete this.FullScreenWaiters[id];
        if (this.FullScreenWaiters.Count > 0)
            this.FullScreenWaiters.Count--;
        if (this.FullScreenWaiters.Count == 0)
            this.FullScreenWaiterInfo.HideWaiter.call(this.FullScreenWaiterInfo.Context);
    }
};

WaitDispatcher.prototype.AddIndividualWaiter = function (id, ShowMethod, HideMethod, context)
{
    this.IndividualWaiters[id] = { ShowWaiter: ShowMethod, HideWaiter: HideMethod, Context: context };
};

WaitDispatcher.prototype.AddFullScreenWaiter = function (ShowMethod, HideMethod, context)
{
    this.FullScreenWaiterInfo = { ShowWaiter: ShowMethod, HideWaiter: HideMethod, Context: context };
};

