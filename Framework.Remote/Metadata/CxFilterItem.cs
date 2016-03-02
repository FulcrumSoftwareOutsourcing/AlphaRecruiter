/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections;
using System.Runtime.Serialization;
using Framework.Entity;

namespace Framework.Remote
{
    [DataContract]
    public class CxFilterItem : IxFilterElement
    {
        [DataMember]
        public string Name{get; set;}

        [DataMember]
        public string OperationAsString{get; set;}

        [DataMember]
        public IList Values{get; set; }

        #region Implementation of IxFilterElement for server side

        public NxFilterOperation Operation{get; set;}

        public void SetValue(int index, object value)
        {
            Values[index] = value;
        }

        #endregion
    }
}

