/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;

namespace Framework.Entity
{
	/// <summary>
	/// Available filter operations.
	/// </summary>
	public enum NxFilterOperation
	{
    None,
    Equal,
    NotEqual,
    Less,
    Greater,
    LessEqual,
    GreaterEqual,
    Between,
    Like,
    NotLike,
    StartsWith,
    IsNull,
    IsNotNull,
    Today,
    ThisWeek,
    ThisMonth,
    ThisYear,
    Yesterday,
    PrevWeek,
    PrevMonth,
    PrevYear,
    InThePast,
    TodayOrLater,
    Tomorrow,
    NotExists,
    Myself,
    Custom
	}
}