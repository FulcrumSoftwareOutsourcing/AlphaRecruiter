function Metadata()
{
    this.Sections = null;
    this.Images = null;
    this.APPLICATION$WORKSPACEAVAILABLE = null;
    this.APPLICATION$CURRENTWORKSPACEID = null;
    this.APPLICATION$CLIENTDATEFORMAT = null;
    this.APPLICATION$CLIENTDATETIMEFORMAT = null;
    this.APPLICATION$LANGUAGECODE = null;

    this.JsAppProperties = {};

    var imagesByIds = null;
    var classesByIds = {};
    var entityUsagesByIds = {};
    var staticRowsources = {};
    var frames = {};

    this.RsImgsUrls = {};

    this.GetImage = function (imageId)
    {
        if (!this.Images)
            return null;
        if (!imagesByIds)
        {
            imagesByIds = {};
            for (var i = 0; i < this.Images.length; i++)
            {
                imagesByIds[this.Images[i].Id] = this.Images[i];
            }
        }
        return imagesByIds[imageId.toUpperCase()];
    };

    this.GetSection = function (sectionId)
    {
        if (!this.Sections)
            return null;
        for (var i = 0; i < this.Sections.length; i++)
        {
            if (this.Sections[i].Id == sectionId)
                return this.Sections[i];
        }

        return null;

    };

    this.GetWorkspace = function (workspaceId)
    {
        if (!this.APPLICATION$WORKSPACEAVAILABLE)
            return null;
        for (var i = 0; i < this.APPLICATION$WORKSPACEAVAILABLE.length; i++)
        {
            if (this.APPLICATION$WORKSPACEAVAILABLE[i].WorkspaceId == workspaceId)
                return this.APPLICATION$WORKSPACEAVAILABLE[i];
        }

        return null;

    };

    this.AddClass = function (classId, classDeclObj)
    {
        classesByIds[classId] = classDeclObj;
    }

    this.CreateClassInstance = function (classId)
    {
        if (!classesByIds[classId])
            return null;

        var classDecl = classesByIds[classId];

        if (classDecl.CreateInstance)
            return classesByIds[classId].CreateInstance();

        return new classDecl();

    };

    this.AddEntityUsage = function (entityUsage)
    {
        if (entityUsage)
        {
            entityUsagesByIds[entityUsage.Id.toUpperCase()] = entityUsage;
            entityUsage.EditableAttributesById = {};
            for (var i = 0; i < entityUsage.EditableAttributes.length; i++)
            {
                entityUsage.EditableAttributesById[entityUsage.EditableAttributes[i]] = entityUsage.Attributes[entityUsage.EditableAttributes[i]];
            }

            
        }
    }

    this.GetEntityUsage = function (entityUsageId)
    {
        if (entityUsagesByIds[entityUsageId.toUpperCase()])
            return entityUsagesByIds[entityUsageId.toUpperCase()];

        return null;



    };

    this.GetEntityUsagesIdsToLoad = function (requiredIds)
    {
        var needToLoad = [];
        for (var i = 0; i < requiredIds.length; i++)
        {
            if (!entityUsagesByIds[requiredIds[i].toUpperCase()])
            {
                needToLoad.push(requiredIds[i]);
            }
        }
    };

    this.AddStaticRowsources = function (rowsources)
    {
        for (var i = 0; i < rowsources.length; i++)
        {
            staticRowsources[rowsources[i].RowSourceId.toUpperCase()] = rowsources[i];
        }
    };

    this.GetStaticRowSource = function (rowsourceId)
    {
        return staticRowsources[rowsourceId.toUpperCase()];
    };

    this.AddFrames = function (framesMetadata)
    {
        for (var i = 0; i < framesMetadata.length; i++)
        {
            frames[framesMetadata[i].Id] = framesMetadata[i];
        }
    };

    this.GetFrame = function (frameId)
    {
        var f = frames[frameId];
        if (!f)
        {
            for (var n in frames)
            {

                if (!App.Utils.IsStringNullOrWhitespace(frames[n]["EntityUsageId"]) && (frames[n]["EntityUsageId"].toUpperCase() == frameId))
                    f = frames[n];
            }
        }
        return f;
    };



    this.GetDetailsFrame = function (frameClassId)
    {
        if (App.Utils.IsStringNullOrWhitespace(frameClassId))
        {
            var frame = BAutoLayoutFrame.CreateInstance();
            frame.Layout = Metadata.GetFrame("DefaultDetailsLayout".toUpperCase());
            return frame;
        }
        var customLayout = Metadata.GetFrame(frameClassId);

        if (!customLayout)
        {
            throw new Error("The frame with Id " + frameClassId + " is not defined in metadata.");
        }

        if (App.Utils.IsStringNullOrWhitespace(customLayout.FrameClassId))
        {
            var frame = BAutoLayoutFrame.CreateInstance();
            frame.Layout = customLayout;
            return frame;
        }
        var classInCustom = Metadata.CreateClassInstance(customLayout.FrameClassId.toUpperCase())
        if (!classesByIds)
        {
            throw new Error("The class with Id " + customLayout.FrameClassId + " is not defined in metadata.");
        }


        var customFrame = Metadata.CreateClassInstance(customLayout.FrameClassId.toUpperCase());
        if (customLayout.Children.Count == 0)
        {
            customFrame.Layout = Metadata.GetFrame("DefaultDetailsLayout".toUpperCase());
        }
        else
        {
            //CxLayoutElement clonedCustomLayout = CloneLayoutElement(customLayout);
            //clonedCustomLayout.FrameClassId = string.Empty;
            //customFrame.Layout = clonedCustomLayout;
        }

        return customFrame;
    }

    //this.AttributeUtils = new AttributeUtils();


    var multilang = { ByObjectName: {} };

    this.InitMultilanguage = function (mlFromServer)
    {
        for (var i = 0; i < mlFromServer.length; i++)
        {
            var item = mlFromServer[i];
            multilang.ByObjectName[item.ObjectName] = item;
        }
    }

    this.GetTxt = function (text)
    {
        if(multilang.ByObjectName[text])
        {
            var li = multilang.ByObjectName[text];
            if (!App.Utils.IsStringNullOrWhitespace(li.LocalizedValue))
                return li.LocalizedValue;
            if (!App.Utils.IsStringNullOrWhitespace(li.DefaultValue))
                return li.DefaultValue;
        }
        return text;
    }

};





window.Metadata = new Metadata();

