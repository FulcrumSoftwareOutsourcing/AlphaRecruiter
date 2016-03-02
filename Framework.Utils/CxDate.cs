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
using System.Globalization;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Highest milliseconds values for different platforms
  /// </summary>
  public enum NxMaxMilliseconds
  {
    DotNet    = 999,
    SqlServer = 997
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Utility methods to work with DateTime
  /// </summary>
  public static class CxDate
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current date/time in common format.
    /// </summary>
    /// <returns>current date/time in common format</returns>
    static public string NowAsString()
    {
      return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
    }
    //-------------------------------------------------------------------------
    public static DateTime GetNextBusinessDay(DateTime datetime)
    {
      if (datetime.DayOfWeek == DayOfWeek.Saturday)
        return datetime.AddDays(2);
      else if (datetime.DayOfWeek == DayOfWeek.Friday)
        return datetime.AddDays(3);
      else
        return datetime.AddDays(1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minimal from two dates.
    /// </summary>
    /// <param name="d1">first date</param>
    /// <param name="d2">second date</param>
    /// <returns>minimal from two dates</returns>
    static public DateTime Min(DateTime d1, DateTime d2)
    {
      return (d1 < d2 ? d1 : d2);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns maximal from two dates.
    /// </summary>
    /// <param name="d1">first date</param>
    /// <param name="d2">second date</param>
    /// <returns>maximal from two dates</returns>
    static public DateTime Max(DateTime d1, DateTime d2)
    {
      return (d1 > d2 ? d1 : d2);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Parses datetime value using standard format.
    /// </summary>
    /// <param name="o">value to parse</param>
    /// <returns>parsed datetime value</returns>
    static public bool Parse(object o, out DateTime d)
    {
      d = DateTime.MinValue;
      if (CxUtils.IsEmpty(o))
      {
        return false;
      }
      if (o is DateTime)
      {
        d = (DateTime)o;
        return true;
      }
      try
      {
        d = Convert.ToDateTime(o);
        return true;
      }
      catch
      {
        return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses datetime value using standard format.
    /// </summary>
    /// <param name="o">value to parse</param>
    /// <returns>parsed datetime value</returns>
    static public DateTime Parse(object o, DateTime defaultValue)
    {
      DateTime d;
      if (Parse(o, out d))
      {
        return d;
      }
      else
      {
        return defaultValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts datetime stored in the object to short date string.
    /// </summary>
    static public string ToShortDateString(object o)
    {
      DateTime date;
      if (Parse(o, out date))
      {
        return date.ToShortDateString();
      }
      return "";
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Converts datetime stored in the object to short date and time string.
    /// </summary>
    static public string ToShortDateTimeString(object o)
    {
      DateTime date;
      if (Parse(o, out date))
      {
        return date.ToShortDateString() + " " + date.ToShortTimeString();
      }
      return "";
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns date of first day of the month.
    /// </summary>
    /// <param name="date">date</param>
    /// <returns>date of first day of the month</returns>
    static public DateTime GetFirstMonthDay(DateTime date)
    {
      return new DateTime(date.Year, date.Month, 1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns date of last day of the month.
    /// </summary>
    /// <param name="date">date</param>
    /// <returns>date of last day of the month</returns>
    static public DateTime GetLastMonthDay(DateTime date)
    {
      return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is DateTime, returns date of first day of the month.
    /// </summary>
    /// <param name="dateObj">date object</param>
    /// <returns>date of first day of the month</returns>
    static public object GetFirstMonthDay(object dateObj)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetFirstMonthDay(date);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is DateTime, returns date of last day of the month.
    /// </summary>
    /// <param name="dateObj">date object</param>
    /// <returns>date of last day of the month</returns>
    static public object GetLastMonthDay(object dateObj)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetLastMonthDay(date);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns date of first day of the week representing by the given date.
    /// </summary>
    /// <param name="date">date to get first day of week for</param>
    /// <returns>date of first day of the week</returns>
    static public DateTime GetFirstWeekDay(DateTime date)
    {
      int firstWeekDay = Convert.ToInt32(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
      int dateWeekDay = Convert.ToInt16(date.DayOfWeek);
      int difference = dateWeekDay - firstWeekDay;
      if (difference < 0)
      {
        difference = 7 + difference;
      }
      return difference > 0 ? date.AddDays(-difference) : date;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns date of last day of the week representing by the given date.
    /// </summary>
    /// <param name="date">date to get last day of week for</param>
    /// <returns>date of last day of the week</returns>
    static public DateTime GetLastWeekDay(DateTime date)
    {
      DateTime firstWeekDayDate = GetFirstWeekDay(date);
      return firstWeekDayDate.AddDays(6);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is date, returns date of first day of the week representing by the given date.
    /// </summary>
    /// <param name="dateObj">date to get first day of week for</param>
    /// <returns>date of first day of the week</returns>
    static public object GetFirstWeekDay(object dateObj)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetFirstWeekDay(date);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is date, returns date of last day of the week representing by the given date.
    /// </summary>
    /// <param name="dateObj">date to get last day of week for</param>
    /// <returns>date of last day of the week</returns>
    static public object GetLastWeekDay(object dateObj)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetLastWeekDay(date);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns this date with lowest time, i.e. 01/01/2008 00:00:00.000 for 01/01/2008.
    /// </summary>
    /// <param name="date">date</param>
    /// <returns>date with lowest time</returns>
    static public DateTime GetDateWithLowestTime(DateTime date)
    {
      return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns this date with highest time, i.e. 01/01/2008 23:59:59.997 for 01/01/2008. 
    /// 997 is the highest number of milliseconds supported by SQL Server, 
    /// 998 and 999 are rounded to 000.
    /// </summary>
    /// <param name="date">date</param>
    /// <param name="milliseconds">milliseconds for highest time</param>
    /// <returns>date with highest time</returns>
    static public DateTime GetDateWithHighestTime(DateTime date, NxMaxMilliseconds milliseconds)
    {
      return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, Convert.ToInt32(milliseconds));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is DateTime returns this date with lowest time,
    /// i.e. 01/01/2008 00:00:00.000 for 01/01/2008.
    /// </summary>
    /// <param name="dateObj">date object</param>
    /// <returns>date with lowest time</returns>
    static public object GetDateWithLowestTime(object dateObj)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetDateWithLowestTime(date);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If given object is DateTime returns this date with highest time,
    /// i.e. 01/01/2008 23:59:59.997 for 01/01/2008. 
    /// 997 is the highest number of milliseconds supported by SQL Server,
    /// 998 and 999 are rounded to 000.
    /// </summary>
    /// <param name="dateObj">date object</param>
    /// <param name="milliseconds">milliseconds for highest time</param>
    /// <returns>date with highest time</returns>
    static public object GetDateWithHighestTime(object dateObj, NxMaxMilliseconds milliseconds)
    {
      DateTime date;
      if (Parse(dateObj, out date))
      {
        return GetDateWithHighestTime(date, milliseconds);
      }
      return dateObj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first day of the previous month.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>first day of the previous month</returns>
    static public DateTime GetFirstPrevMonthDay(DateTime date)
    {
      return GetFirstMonthDay(date).AddMonths(-1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns last day of the previous month.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>first day of the previous month</returns>
    static public DateTime GetLastPrevMonthDay(DateTime date)
    {
      return GetFirstMonthDay(date).AddDays(-1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first day of the year.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>first day of the year</returns>
    static public DateTime GetFirstYearDay(DateTime date)
    {
      return new DateTime(date.Year, 1, 1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns last day of the year.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>last day of the year</returns>
    static public DateTime GetLastYearDay(DateTime date)
    {
      return new DateTime(date.Year, 12, 31);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first day of the previous year.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>first day of the previous year</returns>
    static public DateTime GetFirstPrevYearDay(DateTime date)
    {
      return new DateTime(date.Year - 1, 1, 1);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns last day of the previous year.
    /// </summary>
    /// <param name="date">date to return result for</param>
    /// <returns>last day of the previous year</returns>
    static public DateTime GetLastPrevYearDay(DateTime date)
    {
      return new DateTime(date.Year - 1, 12, 31);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns 1 if database date more than precise date, 
    /// -1 if database date less than precise date,
    /// 0 if equal.
    /// </summary>
    /// <param name="dbDate">database date</param>
    /// <param name="date">precise date</param>
    static public int DbCompare(DateTime dbDate, DateTime date)
    {
      TimeSpan timeSpan = dbDate - date;
      if (timeSpan.TotalMilliseconds > 5) return 1;
      if (timeSpan.TotalMilliseconds < -5) return -1;
      return 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Formats the given time-span using the typical date-time formatting string.
    /// </summary>
    /// <returns>formatted string</returns>
    static public string FormatTimespan(TimeSpan timespan, string format)
    {
      var now = DateTime.Now;
      var date = new DateTime(now.Year, now.Month, now.Day).Add(timespan);
      return date.ToString(format);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Produces the string - amount of hours in the given timespan.
    /// </summary>
    static public string FormatTimespanInHours(TimeSpan timespan)
    {
      return timespan.TotalHours.ToString("0.##");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses the given hours string back to timespan.
    /// </summary>
    /// <returns>the time-span parsed, null if failed</returns>
    static public TimeSpan? ParseTimespanInHours(string str)
    {
      double hours;
      if (double.TryParse(str, out hours))
      {
        var hoursInt = Convert.ToInt32(Math.Floor(hours));
        var minutesInt = Convert.ToInt32((hours - Math.Floor(hours)) * 60);
        return new TimeSpan(hoursInt, minutesInt, 0);
      }
      return null;
    }
    //-------------------------------------------------------------------------
  }
}