function FrameBuilder(rootFrame)
{

    LayoutBuilder.call(this, rootFrame);

    var m_RootFrame = rootFrame;

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets type of suppoted Layout element.
    /// </summary>
    this.TargetType = 'frame';

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets UI element for layuot.
    /// </summary>
    /// <param name="element">CxLayoutElement to create UI element.</param>
    /// <param name="entityUsage">CxEntityUsage from frame.</param>
    /// <param name="entity">CxBaseEntity from frame.</param>
    /// <param name="openMode">Frame open mode.</param>
    /// <returns>Greated UI element.</returns>
    this.GetUiElement = function (
        element,
      entityUsage,
      entity,
      openMode)
    {
        if (!App.Utils.IsStringNullOrWhitespace(element.FrameClassId)) // frame with specified 'frame_class_id' 
        {
            var frame = Metadata.CreateClassInstance(element.FrameClassId);
            frame.OpenMode = openMode;
            frame.ParentFrame = m_RootFrame;
            SetRowAndColumn(frame, element);

            var data =
            {
                EntityUsageId: element.EntityUsageId,
                ParentEntity: entity,
                ParentEntityUsageId: entityUsage.EntityMetadata.Id,
                ParentPks: []
            };

            for (var i; i < entityUsage.AttributesList.length; i++)
            {
                var attribute = entityUsage.AttributesList[i];
                if (attribute.PrimaryKey)
                {
                    data.ParentPks.push(attribute.Id, entity[attribute.Id]);
                }
            }




            if (openMode == NxOpenMode.Edit || openMode == NxOpenMode.ChildEdit)
                data.OpenMode = NxOpenMode.ChildEdit;

            if (openMode == NxOpenMode.View || openMode == NxOpenMode.ChildView)
                data.OpenMode = NxOpenMode.ChildView;

            if (openMode == NxOpenMode.New || openMode == NxOpenMode.ChildNew)
                data.OpenMode = NxOpenMode.ChildNew;

            frame.InitFrame(data);
            m_rootFrame.ChildFrames.Add(frame);
            m_rootFrame.LayoutControlCreated(frame, entityUsage.EntityMetadata.Id, null);
            return frame;

        }

        var grid = this.GetLayoutGrid(element);

        //grid.Background = (Brush)CxAppContext.Instance.Resources["DetailsViewBackBrush"];

        if (element.Children)
        {
            if (element.Children.Count == 0) // frame element as reference on other frame
            {

                if (!Metadata.GetFrame(element.Id))
                {
                    throw new Error("The Frame with Id " + element.Id + " is not defined.");
                }

                var builder = GetBuilder(element.Type);
                var referencedElement = Metadata.GetFrame(element.Id);
                var childFrame = builder.GetUiElement(
                  referencedElement,
                  entityUsage,
                  entity,
                  openMode);

                SetRowAndColumn(childFrame, element);

                if (childFrame != null)
                {
                    m_rootFrame.ChildFrames.Add(frame);
                    m_rootFrame.LayoutControlCreated(frame, entityUsage.EntityMetadata.Id, null);
                }

                return childFrame;
            }

            for (var i = 0; i < element.Children.length; i++)
            {
                var childLayuot = element.Children[i];
                var builder = this.GetBuilder(childLayuot.Type);
                var childElement = builder.GetUiElement(childLayuot, entityUsage, entity, openMode);


                if (childElement != null)
                {
                    m_rootFrame.ChildFrames.Add(frame);
                    m_rootFrame.LayoutControlCreated(frame, entityUsage.EntityMetadata.Id, null);
                }

                grid.Children.push(childElement);
            }


        }
        return grid;
    }
}

extend(FrameBuilder, LayoutBuilder);