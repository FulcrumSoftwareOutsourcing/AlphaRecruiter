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
using System.Collections.Generic;
using System.Text;

namespace Framework.Entity
{
  //------------------------------------------------------------------------------
  /// <summary>
  /// Activity attribute ID constants
  /// </summary>
  public class CxActivityAttr
  {
    //----------------------------------------------------------------------------
    // Appointment attributes
    public const string APPOINTMENT_ID = "EntityId";
    public const string START_DATE = "StartDate";
    public const string END_DATE = "EndDate";
    public const string IS_ALL_DAY = "IsAllDay";
    public const string LOCATION = "Location";
    public const string RECURRENCY_TYPE_CODE = "RecurrencyTypeCode";
    public const string TIME_STATUS_ID = "TimeStatusId";
    public const string LABEL_ID = "LabelId";
    public const string REMINDER_TIME = "ReminderTimeBeforeStartSeconds";
    public const string RESOURCE_ID = "ResourceId";
    public const string RESOURCE_ID_LIST = "ResourceIdList";
    public const string ICON_ID = "ActivityIconId";
    public const string CONTACT_NAME = "Parent1Name";
    public const string MAIN_PROFILE_NAME = "Parent2Name";
    // Appointment calculated attributes
    public const string CALC_TYPE_CODE = "_CalcTypeCode";
    public const string CALC_STATUS_ID = "_CalcStatusId";
    public const string CALC_LABEL_ID = "_CalcLabelId";
    public const string CALC_RECURRENCE = "_CalcRecurrence";
    public const string CALC_REMINDER = "_CalcReminder";
    public const string CALC_RESOURCES = "_CalcResources";
    // Recurrency attributes
    public const string PERIOD_CODE = "PeriodCode";
    public const string PERIODICITY = "Periodicity";
    public const string WEEK_DAYS = "WeekDays";
    public const string WEEK_OF_MONTH = "WeekOfMonth";
    public const string DAY_NUMBER = "DayNumber";
    public const string MONTH_NUMBER = "MonthNumber";
    public const string LIMIT_TYPE_CODE = "LimitTypeCode";
    public const string RANGE_START_DATE = "RangeStartDate";
    public const string RANGE_END_DATE = "RangeEndDate";
    public const string RANGE_OCCURRENCE_COUNT = "RangeOccurrenceCount";
    // Internal attributes
    public const string TAG_APPOINTMENT = "_TagAppointment";
    public const string TAG_SCHEDULER = "_TagScheduler";
    public const string TAG_IS_RECURRENT = "_TagIsRecurrent";
    //----------------------------------------------------------------------------
  }
  //------------------------------------------------------------------------------
  /// <summary>
  /// Activity status attribute ID constants.
  /// </summary>
  public class CxActivityStatusAttr
  {
    //----------------------------------------------------------------------------
    // Activity status attributes
    public const string STATUS_ID = "ActivityStatusId";
    public const string COLOR = "ColorRGB";
    public const string PREDEFINED_TYPE = "PredefinedTypeCode";
    //-------------------------------------------------------------------------
  }
  //------------------------------------------------------------------------------
  /// <summary>
  /// Utility methods to work with activity.
  /// </summary>
  public class CxActivityUtils
  {
  }
  //------------------------------------------------------------------------------
}