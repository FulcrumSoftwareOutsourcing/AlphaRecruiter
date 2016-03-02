'use strict';
function ObservableObject(businessObject, observableProps, validator, rowsources, metadata, attrs, itIsDataRow)
{
    var subscriptions = [];
    this.ObservablePropsNames = observableProps;
    this.HasChanges = false;
    this.ChangedFields = [];
    this.OnPropertyChangedDelegate = null;

    if (itIsDataRow)
    {
        this.Selected = ko.observable(false);
    }

    this.Validate = true;

    /// <field name='HasErrorsChangedDelegate' type='Delegate'></field>
    this.HasErrorsChangedDelegate = null;

    for (var i = 0; i < observableProps.length; i++)
    {
        var propName = observableProps[i];
        if (itIsDataRow)
        {
            var dataItem = businessObject[i];
            this[propName] = ko.observable(dataItem.Value);
            this[propName].Readonly = dataItem.Readonly;
            this[propName].Visible = dataItem.Visible;
            this[propName].DisabledCommandIds = dataItem.DisabledCommandIds;
            this[propName].Editing = ko.observable(false);
            this[propName].EditorVisible = ko.observable(false);
            this[propName].EditorFocused = ko.observable(false);
            this[propName].Index = i;

            var attr = attrs[propName];
            if (attr.RowSourceId != '')
            {
                var rs = rowsources.Filtered[attr.Id];
                if(!rs)
                    rs = rowsources.Unfiltered[attr.RowSourceId];
                if (!rs)
                    rs = rowsources.Other[attr.RowSourceId];
                if (!rs)
                    rs = Metadata.GetStaticRowSource(attr.RowSourceId);
                

                for (var k = 0; k < rs.RowSourceData.length; k++)
                {
                    var data = rs.RowSourceData[k];
                    if (data.Value == 'True')
                        data.Value = true;
                    if (data.Value == 'False')
                        data.Value = false;
                }


                this[propName].RsItems = ko.observableArray();
                this[propName].RsItems.pushAll(rs.RowSourceData);
                this[propName].SelectedRsItem = ko.observable();
                this[propName].RsOpen = ko.observable(false);

                var found = false;
                for (var k = 0; k < rs.RowSourceData.length; k++)
                {
                    if (dataItem.Value == rs.RowSourceData[k].Value)
                    {
                        this[propName].SelectedRsItem(rs.RowSourceData[k]);
                        found = true;
                    }
                }
                if (found == false)
                    this[propName].SelectedRsItem({ Text: '', Value: null, ImageId: null });


            }


        }
        else
        {
            if (App.Utils.IsObject(businessObject[propName]))
            {
                this[propName] = ko.observable(businessObject[propName].Value);
                this[propName].Readonly = ko.observable( businessObject[propName].Readonly );
                this[propName].Visible = ko.observable( businessObject[propName].Visible );
                this[propName].DisabledCommandIds = businessObject[propName].DisabledCommandIds;
            }
            else
            {
                this[propName] = ko.observable(businessObject[propName]);
            }
            if (attrs)
            {
                var dataItem = businessObject[propName];
                var attr = attrs[propName];
                if (attr.RowSourceId != '')
                {
                    var rs = rowsources.Filtered[attr.Id];
                    if (!rs)
                        rs = rowsources.Unfiltered[attr.RowSourceId];
                    if (!rs)
                        rs = rowsources.Other[attr.RowSourceId];
                    if (!rs)
                        rs = Metadata.GetStaticRowSource(attr.RowSourceId);


                    for (var k = 0; k < rs.RowSourceData.length; k++)
                    {
                        var data = rs.RowSourceData[k];
                        if (data.Value == 'True')
                            data.Value = true;
                        if (data.Value == 'False')
                            data.Value = false;
                    }



                    this[propName].RsItems = ko.observableArray();
                    this[propName].RsItems.pushAll(rs.RowSourceData);
                    this[propName].SelectedRsItem = ko.observable();
                    this[propName].RsOpen = ko.observable(false);

                    var found = false;
                    for (var k = 0; k < rs.RowSourceData.length; k++)
                    {
                        if (dataItem.Value == rs.RowSourceData[k].Value)
                        {
                            this[propName].SelectedRsItem(rs.RowSourceData[k]);
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        this[propName].SelectedRsItem({ Text: '', Value: null, ImageId: null });
                        dataItem.Value = null;
                    }
                    
                }

            }

        }
        this[propName].HasErrors = ko.observable(false);
        this[propName].ErrorMessages = ko.observableArray();
        this[propName].Validator = validator;
        this[propName].MyName = propName;
        this[propName].OwnedObj = this;
        this[propName].HasFocus = ko.observable(false);







        var subscr = this[propName].subscribe(function (newValue)
        {
            if (this.ItIsRevert)
                return;

            this.OwnedObj.HasChanges = true;
            this.OwnedObj.ChangedFields.push(this);

            if (!this.OwnedObj.Validate)
                return;


            var thisObservable = this;


            var propName = thisObservable.MyName;

            var validationMethod = 'Validate_' + propName;
            if (thisObservable.Validator &&
                thisObservable.Validator[validationMethod])
            {
                var validationResult = thisObservable.Validator[validationMethod](newValue);
                if (validationResult && validationResult.length && validationResult.length > 0)
                {
                    thisObservable.HasErrors(false);
                    thisObservable.ErrorMessages.removeAll();
                    thisObservable.HasErrors(true);
                    thisObservable.ErrorMessages.pushAll(validationResult);
                    if (thisObservable.OwnedObj.HasErrorsChangedDelegate)
                        thisObservable.OwnedObj.HasErrorsChangedDelegate.Invoke();
                }
                else
                {
                    thisObservable.HasErrors(false);
                    thisObservable.ErrorMessages.removeAll();
                    if (thisObservable.OwnedObj.HasErrorsChangedDelegate)
                        thisObservable.OwnedObj.HasErrorsChangedDelegate.Invoke();
                }
            }

            

            if (attrs)
            {
                var attr = attrs[propName];
                if (thisObservable.OwnedObj.OnPropertyChangedDelegate)
                {
                    thisObservable.OwnedObj.OnPropertyChangedDelegate.Invoke(attr, newValue);
                }
            }


            if (thisObservable.OwnedObj.OnPropertyChangedDelegate && thisObservable.OwnedObj.BFilter && !thisObservable.OwnedObj.ItsFilterReset)
            {
                thisObservable.OwnedObj.OnPropertyChangedDelegate.Invoke(thisObservable.OwnedObj.BFilter.Attr, newValue);
            }
            
            

        }, this[propName]);

        subscriptions.push(subscr);
    }


    function Dispose(businessObj)
    {
        for (var i = 0; i < subscriptions.length; i++)
        {
            subscriptions[i].dispose();
        }

        businessObj = null;

    }

    this.ValidateEntity = function ()
    {
        //this.ErrorMessages = [];
        //this.HasErrors(false);

        var errors = [];
        
        for (var i = 0; i < observableProps.length; i++)
        {
            var propName = observableProps[i];

            var val = this[propName]();
            if (App.Utils.IsObject(val))
            {
                val = val.Value;
            }
            

            validator.Validate(val, propName, errors)

        }

        return errors;
    };


    this.ValidateAll = function ()
    {
        //this.ErrorMessages = [];
        //this.HasErrors(false);

        var result = true;
        var values = [];
        for (var i = 0; i < observableProps.length; i++)
        {
            var propName = observableProps[i];
            values.push({ Name: propName, Value: this[propName](), Errors: null });
        }

        var validationResult = validator.ValidateAll(values);



        for (var i = 0; i < validationResult.length; i++)
        {
            var valResult = validationResult[i];
            var propName = validationResult[i].Name;
            if (valResult.Errors && valResult.Errors.length && valResult.Errors.length > 0)
            {
                result = false;
                this[propName].HasErrors(true);
                this[propName].ErrorMessages.removeAll();
                this[propName].ErrorMessages.pushAll(valResult.Errors);
            }
            else
            {
                this[propName].HasErrors(false);
                this[propName].ErrorMessages.removeAll();
            }

            if (this[propName].OwnedObj.HasErrorsChangedDelegate)
                this[propName].OwnedObj.HasErrorsChangedDelegate.Invoke();

        }



        return result;
    };

    this.ToBusinessObject = function ()
    {
        if (!businessObject)
            return;
        var values = [];
        for (var i = 0; i < observableProps.length; i++)
        {
            var propName = observableProps[i];
            if (itIsDataRow)
                businessObject[i].Value = this[propName]();
            else
            {
                var val = this[propName]();
                if (App.Utils.IsObject(val))
                {
                    businessObject[propName] = val.Value;
                }
                else
                {
                    businessObject[propName] = this[propName]();
                }

            }
        }
        return businessObject;
    };

    this.GetBusinessObject = function ()
    {
        return businessObject;
    };

    this.FocusFirstField = function ()
    {
        if (this.ObservablePropsNames.length > 0)
        {
            this[this.ObservablePropsNames[0]].HasFocus(true);
        }
    };

    this.FocusFirstInvalidField = function ()
    {
        for (var i = 0; i < this.ObservablePropsNames.length; i++)
        {
            var propName = this.ObservablePropsNames[i];
            if (this[propName].HasErrors())
            {
                this[propName].HasFocus(true);
                break;
            }
        }
    };


    this.ShowErrors = function (errors)
    {

        for (var i = 0; i < this.ObservablePropsNames.length; i++)
        {
            var propName = this.ObservablePropsNames[i];
            var errorMessages = errors[propName];
            this[propName].HasErrors(false);
            this[propName].ErrorMessages.removeAll();
            if (errorMessages)
            {
                this[propName].HasErrors(true);
                this[propName].ErrorMessages.pushAll(errorMessages);
                if (this[propName].OwnedObj.HasErrorsChangedDelegate)
                    this[propName].OwnedObj.HasErrorsChangedDelegate.Invoke();
            }
        }


    };

    this.RevertChanges = function ()
    {
        if (this.WaitingToUpload)
            this.WaitingToUpload = {};

        if (!this.HasChanges)
            return;


        for (var i = 0; i < this.ChangedFields.length; i++)
        {
            var field = this.ChangedFields[i];;
            var dataItem;
            if (itIsDataRow)
            {
                dataItem = businessObject[field.Index];
            }
            else
            {
                dataItem = businessObject[field.MyName];
            }


            field.ItIsRevert = true;

            field(dataItem.Value);
            field.Readonly = dataItem.Readonly;
            field.Visible = dataItem.Visible;
            field.DisabledCommandIds = dataItem.DisabledCommandIds;
            if (itIsDataRow)
            {
                field.Editing(false);
                field.EditorVisible(false);
                field.EditorFocused(false);
            }

            var attr = attrs[field.MyName];
            if (attr.RowSourceId != '')
            {
                var rs = Metadata.GetStaticRowSource(attr.RowSourceId);
                if (!rs)
                {
                    rs = rowsources[attr.RowSourceId];
                }
                field.RsOpen(false);
                
                var found = false;
                for (var k = 0; k < rs.RowSourceData.length; k++)
                {
                    if (dataItem.Value == rs.RowSourceData[k].Value)
                    {
                        field.SelectedRsItem(rs.RowSourceData[k]);
                        found = true;
                    }
                }

                if (found == false)
                    this[propName].SelectedRsItem({ Text: '', Value: null, ImageId: null });
            }

            field.HasErrors(false);
            field.ErrorMessages.removeAll();

            field.ItIsRevert = false;


        }


        this.HasChanges = false;


    };




};




