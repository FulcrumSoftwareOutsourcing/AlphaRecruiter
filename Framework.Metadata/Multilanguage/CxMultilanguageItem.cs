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
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Class describing multilanguage item.
	/// </summary>
	public class CxMultilanguageItem : IComparable
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="applicationCd"></param>
    /// <param name="objectTypeCd"></param>
    /// <param name="propertyCd"></param>
    /// <param name="objectName"></param>
    /// <param name="defaultValue"></param>
    /// <param name="isNotUsed"></param>
    public CxMultilanguageItem(
      string applicationCd,
      string objectTypeCd,
      string propertyCd,
      string objectName,
      string defaultValue,
      bool isNotUsed)
		{
      ApplicationCd = applicationCd;
      ObjectTypeCd = objectTypeCd;
      PropertyCd = propertyCd;
      ObjectName = objectName;
      DefaultValue = defaultValue;
      IsNotUsed = isNotUsed;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="objectTypeCd"></param>
    /// <param name="propertyCd"></param>
    /// <param name="objectName"></param>
    /// <param name="defaultValue"></param>
    public CxMultilanguageItem(
      string objectTypeCd,
      string propertyCd,
      string objectName,
      string defaultValue) : this (null, objectTypeCd, propertyCd, objectName, defaultValue, false)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares object to another object.
    /// </summary>
    public int CompareTo(object obj)
    {
      if (obj is CxMultilanguageItem)
      {
        return UniqueKey.CompareTo(((CxMultilanguageItem)obj).UniqueKey);
      }
      throw new ExException("Invalid argument for the CxMultilanguageItem.CompareTo method.");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Application code
    /// </summary>
    public string ApplicationCd;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Object type code
    /// </summary>
    public string ObjectTypeCd;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Property code
    /// </summary>
    public string PropertyCd;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Object unique name
    /// </summary>
    public string ObjectName;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default property value (value to translate)
    /// </summary>
    public string DefaultValue;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the item is used.
    /// </summary>
    public bool IsNotUsed;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique key of the object.
    /// </summary>
    public string UniqueKey
    {
      get
      {
        return CxUtils.Nvl(ApplicationCd).ToUpper() + "." +
               CxUtils.Nvl(ObjectTypeCd).ToUpper() + "." +
               CxUtils.Nvl(PropertyCd).ToUpper() + "." +
               CxUtils.Nvl(ObjectName).ToUpper();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns insert SQL.
    /// </summary>
    public string InsertSql
    {
      get
      {
        return "insert into Framework_LocalizationItems\r\n" +
                "(ApplicationCd, ObjectTypeCd, PropertyCd, ObjectName, DefaultValue)\r\n" +
                "values\r\n" +
                "(" + CxSqlServer.GetStringConst(ApplicationCd) + ", " +
                CxSqlServer.GetStringConst(ObjectTypeCd) + ", " +
                CxSqlServer.GetStringConst(PropertyCd) + ", " +
                CxSqlServer.GetStringConst(ObjectName) + ", " +
                CxSqlServer.GetStringConst(DefaultValue) + ")\r\n";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns update SQL.
    /// </summary>
    public string UpdateSql
    {
      get
      {
        return "update Framework_LocalizationItems\r\n" +
               "   set DefaultValue = " + CxSqlServer.GetStringConst(DefaultValue) + ",\r\n" +
               "       IsNotUsed = 0\r\n" +
               " where ApplicationCd = " + CxSqlServer.GetStringConst(ApplicationCd) + "\r\n" +
               "   and ObjectTypeCd = " + CxSqlServer.GetStringConst(ObjectTypeCd) + "\r\n" +
               "   and PropertyCd = " + CxSqlServer.GetStringConst(PropertyCd) + "\r\n" +
               "   and ObjectName = " + CxSqlServer.GetStringConst(ObjectName) + "\r\n";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns disable item SQL.
    /// </summary>
    public string DisableSql
    {
      get
      {
        return "update Framework_LocalizationItems\r\n" +
               "   set IsNotUsed = 1\r\n" +
               " where ApplicationCd = " + CxSqlServer.GetStringConst(ApplicationCd) + "\r\n" +
               "   and ObjectTypeCd = " + CxSqlServer.GetStringConst(ObjectTypeCd) + "\r\n" +
               "   and PropertyCd = " + CxSqlServer.GetStringConst(PropertyCd) + "\r\n" +
               "   and ObjectName = " + CxSqlServer.GetStringConst(ObjectName) + "\r\n";
      }
    }
    //-------------------------------------------------------------------------
  }
}