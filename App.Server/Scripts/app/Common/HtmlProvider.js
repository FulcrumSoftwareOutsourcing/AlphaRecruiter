/// <reference path="Consts.js" />
function HtmlProvider() { };
HtmlProvider.prototype.Templates = {};


HtmlProvider.prototype.GetTemplatesIdsToLoad = function (requiredIds)
{
    var needFromServer = [];
    for (var i = 0; i < requiredIds.length; i++)
    {
        if (this[requiredIds[i]] == undefined)
            needFromServer.push(requiredIds[i]);

    }
    return needFromServer;
};

HtmlProvider.prototype.AddTemplate = function (templateId, template)
{
    if (this[templateId] == undefined)
    {
        this[templateId] = 1;
        $('body').append(template);
    }
    else
    {
        var tEl = document.getElementById(templateId);
        if (tEl)
        {
            document.body.removeChild(tEl);
        }

        $('body').append(template);
    }
};

HtmlProvider.prototype.AddTemplates = function (templates)
{
    if (templates != undefined)
    {
        for (var i = 0; i < templates.length; i++)
        {
            this.AddTemplate(templates[i].Id, templates[i].Template);
        }
    }
};

HtmlProvider.prototype.GetTemplateText = function (templateId)
{
    if (!this.Templates[templateId])
    {

        var el = document.getElementById(templateId)
        if (!el)
            throw new Error('There is no template with id "' + templateId + '"');

        this.Templates[templateId] = el.innerHTML;
    }
    return this.Templates[templateId];
};
