function LayoutBuilder(root)
{

    var m_rootFrame = root;



    this.GetBuilder = function (type)
    {
        switch (type)
        {
            case "frame":
                return new FrameBuilder(m_rootFrame);
            case "panel":
                return new CxPanelBuilder(m_rootFrame);
            case "tab_control":
                return new CxTabControlBuilder(m_rootFrame);
            case "tab":
                return new CxTabBuilder(m_rootFrame);
            case "hint":
                return new CxHintBuilder(m_rootFrame);

            default:
                throw new ExApplicationException("The Layout Builder for layout " +
                    "element '{0}' is not defined.", type);
        }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets type of suppoted Layout element.
    /// </summary>
    this.TargetType = ''
    //---------------------------------------------------------------------------
    /// <summary>
    /// Gets UI element for layuot.
    /// </summary>
    /// <param name="element">CxLayoutElement to create UI element.</param>
    /// <param name="entityUsage">CxEntityUsage from frame.</param>
    /// <param name="entity">CxBaseEntity from frame.</param>
    /// <param name="openMode">Frame open mode.</param>
    /// <returns>Greated UI element.</returns>
    this.GetUiElement = function (element, entityUsage, entity, openMode) { };

    //----------------------------------------------------------------------------
    /// <summary>
    /// Initializes for layout.
    /// </summary>
    /// <param name="grid">Grid that need to initialize.</param>
    /// <param name="element">CxLayoutElement to grid initialize.</param>
    this.InitLayoutGrid = function(grid, element)
    {
        var rowsCount = element.RowsCount > 0 ? element.RowsCount : 1;
        var columnsCount = element.ColumnsCount > 0 ? element.ColumnsCount : 1;
        var rowsHeightList = ParseGridLengthArray(element.RowsHeight);
        var columnsWidthList = ParseGridLengthArray(element.ColumnsWidth);

        for (var i = 0; i < rowsCount; i++)
        {
            var rowDefinition = {
                Height: 'auto'
            };

            //    if ((rowsHeightList.Count - 1) >= i && rowsHeightList[i] is GridUnitType)
            //    {
            //        rowDefinition.Height = new GridLength(1, (GridUnitType) rowsHeightList[i]);
            //    }
            //if ((rowsHeightList.Count - 1) >= i && rowsHeightList[i] is double)
            //{
            //    rowDefinition.Height = new GridLength((double) rowsHeightList[i], GridUnitType.Pixel);
            //}
            grid.RowDefinitions.push(rowDefinition);
        }
        for (var i = 0; i < columnsCount; i++)
        {
            var columnDefinition =
              {
                  Width: '100%',
              };

            //  if ((columnsWidthList.Count - 1) >= i && columnsWidthList[i] is GridUnitType)
            //{
            //    columnDefinition.Width = new GridLength(1, (GridUnitType) columnsWidthList[i]);
            //}
            //  if ((columnsWidthList.Count - 1) >= i && columnsWidthList[i] is double)
            //{
            //    columnDefinition.Width = new GridLength((double) columnsWidthList[i], GridUnitType.Pixel);
            //}
            grid.ColumnDefinitions.push(columnDefinition);
        }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and initializes grid for layout.
    /// </summary>
    /// <param name="element">CxLayoutElement to grid initialize.</param>
    /// <returns>Created grid.</returns>
    this.GetLayoutGrid = function (element)
    {
        var grid = {
            RowDefinitions: [],
            ColumnDefinitions: [],
        };

        this.InitLayoutGrid(grid, element);

        return grid;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parses grid length data from layuot markup.
    /// </summary>
    /// <param name="markup">layuot markup to parse.</param>
    /// <returns>List of standard WPF grid length objects. </returns>
    function ParseGridLengthArray(markup)
    {
        var valuesAsObj = [];

        if (App.Utils.IsStringNullOrWhitespace(markup))
        {
            return valuesAsObj;
        }

        var values = markup.split(',');

        for (var i = 0; i < values.length; i++)
        {
            var s = values[i];
            var lowerVal = s.replace(' ', string.Empty);
            lowerVal = lowerVal.toLowerCase();

            switch (lowerVal)
            {
                case "auto":
                    valuesAsObj.push('auto');
                    break;
                case "*":
                    valuesAsObj.push('100%');
                    break;
                default:
                    valuesAsObj.push(lowerVal + 'px');
                    break;
            }
        }


        return valuesAsObj;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets rows and columns count in given FrameworkElement.
    /// </summary>
    function SetRowAndColumn(
      uiElement,
      layoutElement)
    {
        uiElement.SetValue(Grid.RowProperty, layoutElement.Row);
        uiElement.SetValue(Grid.ColumnProperty, layoutElement.Column);

        uiElement.SetValue(Grid.RowSpanProperty, layoutElement.RowSpan);
        uiElement.SetValue(Grid.ColumnSpanProperty, layoutElement.ColumnSpan);
    }
    //-------------------------------------------------------------------------

}



