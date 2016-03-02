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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Xml;

using Framework.Common;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  public class CxMailTemplateUtilsBase
  {
    //-------------------------------------------------------------------------
    // Available parameter types.
    private const string TYPE_MODULE_EX = "{0} could be used in the Web Application only!";
    public const string TYPE_STRING = "STRING";
    public const string TYPE_HTML = "HTML";
    public const string TYPE_DATE = "DATE";
    public const string TYPE_DATETIME = "DATETIME";
    public const string TYPE_INT = "INT";
    public const string TYPE_FLOAT = "FLOAT";
    public const string TYPE_BOOLEAN = "BOOLEAN";
    public const string TYPE_IMAGE = "IMAGE";
    public const string TYPE_MODULE = "MODULE";
    //-------------------------------------------------------------------------
    public const string DT_EMAIL = "EMAIL";
    public const string DT_LETTER = "LETTER";
    //-------------------------------------------------------------------------
    public const string TEMPLATE_ENTITY_ID = "MailTemplate";
    public const string TEMPLATE_PARAM_ENTITY_ID = "MailTemplateParameter";
    public const string TEMPLATE_INSTANCE_ENTITY_ID = "MailTemplateInstance";
    public const string PREDEFINED_PARAM_ENTITY_ID = "MailTemplatePredefinedParameter";
    public const string TEMPLATE_SENT_ITEM_ENTITY_ID = "MailTemplateSentItem";
    public const string ENTITY_ATTRIBUTE_EMAIL_TO = "EmailTo";
    public const string ENTITY_ATTRIBUTE_EMAIL_FROM = "EmailFrom";
    public const string ENTITY_ATTRIBUTE_SUBJECT = "Subject";
    public const string ENTITY_ATTRIBUTE_MAIL_CONTENT = "MailContent";
    //-------------------------------------------------------------------------
    protected const string FIELD_PREFIX = "=PARENT.";
    protected const string PREDEFINED_PREFIX = "=PREDEFINED.";
    //-------------------------------------------------------------------------


    //-------------------------------------------------------------------------
    /// <summary>
    /// SMTP Address
    /// </summary>
    protected static string _smtpMailServer = null;
    static public string SmtpMailServer
    {
      get
      {
        lock (typeof(CxMailTemplateUtilsBase))
        {
          return _smtpMailServer ?? CxConfigurationHelper.SmtpMailServer;
        }
      }
      set
      {
        lock (typeof(CxMailTemplateUtilsBase))
        {
          _smtpMailServer = value;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates combo item from entity attribute.
    /// </summary>
    /// <param name="entityMetadata"></param>
    /// <param name="attr"></param>
    /// <returns></returns>
    static protected CxComboItem CreateComboItemFromAttribute(
      CxEntityMetadata entityMetadata,
      CxAttributeMetadata attr)
    {
      string expr = FIELD_PREFIX + entityMetadata.Id + "." + attr.Id;
      string text = entityMetadata.SingleCaption + "." + attr.Caption + " (" + attr.Id + ")";
      CxComboItem item = new CxComboItem(expr, text);
      return item;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of items for drop-down list for mail template 
    /// parameter value (default value) field.
    /// </summary>
    /// <param name="templateEntity">template entity</param>
    /// <param name="paramTypeId">parameter type</param>
    /// <returns>list of CxComboItem objects</returns>
    static public IList<CxComboItem> GetParamValueRowSourceItems(
      CxBaseEntity templateEntity,
      string paramTypeId,
      CxDbConnection connection)
    {
      List<CxComboItem> list = new List<CxComboItem>();
      CxEntityUsageMetadata sourceEntityUsageMetadata =
        templateEntity.Metadata.Holder.EntityUsages.Find(CxUtils.ToString(templateEntity["EntityId"]));
      CxEntityMetadata sourceEntityMetadata =
        templateEntity.Metadata.Holder.Entities.Find(CxUtils.ToString(templateEntity["EntityId"]));
      if (sourceEntityMetadata != null || sourceEntityUsageMetadata != null)
      {
        CxEntityMetadata attrSource =
          sourceEntityUsageMetadata ?? sourceEntityMetadata;
        foreach (CxAttributeMetadata attr in attrSource.Attributes)
        {
          if (GetIsAttrTypeCompatible(attr, paramTypeId))
          {
            CxComboItem item = CreateComboItemFromAttribute(attrSource, attr);
            list.Add(item);
          }
        }
        IList<CxParentEntityMetadata> parentEntities = sourceEntityUsageMetadata != null ?
          sourceEntityUsageMetadata.Entity.ParentEntities : sourceEntityMetadata.ParentEntities;
        foreach (CxParentEntityMetadata parentMetadata in parentEntities)
        {
          CxEntityMetadata parentEntityMetadata =
            templateEntity.Metadata.Holder.Entities.Find(parentMetadata.Id);
          if (parentEntityMetadata != null)
          {
            foreach (CxAttributeMetadata attr in parentEntityMetadata.Attributes)
            {
              if (GetIsAttrTypeCompatible(attr, paramTypeId))
              {
                CxComboItem item = CreateComboItemFromAttribute(parentEntityMetadata, attr);
                list.Add(item);
              }
            }
          }
        }
        list.Sort();
      }
      CxEntityUsageMetadata predefParamEntityUsage =
        templateEntity.Metadata.Holder.EntityUsages[PREDEFINED_PARAM_ENTITY_ID];
      DataTable dt = new DataTable();
      //using (CxDbConnection connection = CxDbConnections.CreateEntityConnection(predefParamEntityUsage))
      //{
      predefParamEntityUsage.ReadData(connection, dt);
      //}
      DataRow[] rows = dt.Select("", predefParamEntityUsage.OrderByClause);
      foreach (DataRow row in rows)
      {
        CxBaseEntity predefParamEntity =
          CxBaseEntity.CreateByDataRow(predefParamEntityUsage, row);
        if (GetIsPredefParamTypeCompatible(predefParamEntity, paramTypeId) &&
            (GetIsPredefParamEntityCompatible(predefParamEntity, sourceEntityUsageMetadata) ||
             GetIsPredefParamEntityCompatible(predefParamEntity, sourceEntityMetadata)))
        {
          string expr = PREDEFINED_PREFIX + CxUtils.ToString(predefParamEntity["Code"]);
          string text = "." + CxUtils.ToString(predefParamEntity["Name"]);
          CxComboItem item = new CxComboItem(expr, text);
          list.Add(item);
        }
      }
      list.Insert(0, new CxComboItem("", ""));
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if parameter type is compatible with the attribute type.
    /// </summary>
    static protected bool GetIsAttrTypeCompatible(
      CxAttributeMetadata attr,
      string typeId)
    {
      switch (typeId)
      {
        case TYPE_STRING:
        case TYPE_HTML:
          return attr.Type != CxAttributeMetadata.TYPE_IMAGE &&
            attr.Type != CxAttributeMetadata.TYPE_FILE;
        case TYPE_DATE:
        case TYPE_DATETIME:
          return attr.Type == CxAttributeMetadata.TYPE_DATE ||
            attr.Type == CxAttributeMetadata.TYPE_DATETIME;
        case TYPE_INT:
        case TYPE_FLOAT:
          return attr.Type == CxAttributeMetadata.TYPE_INT ||
            attr.Type == CxAttributeMetadata.TYPE_FLOAT;
        case TYPE_BOOLEAN:
          return attr.Type == CxAttributeMetadata.TYPE_BOOLEAN;
        case TYPE_IMAGE:
          return false;
        default:
          return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if parameter type is compatible with the predefined parameter type.
    /// </summary>
    static protected bool GetIsPredefParamTypeCompatible(
      CxBaseEntity predefParamEntity,
      string typeId)
    {
      string predefTypeId = CxUtils.ToString(predefParamEntity["TypeId"]);
      switch (typeId)
      {
        case TYPE_STRING:
        case TYPE_HTML:
          return predefTypeId != TYPE_IMAGE && predefTypeId != TYPE_MODULE;
        case TYPE_DATE:
        case TYPE_DATETIME:
          return predefTypeId == TYPE_DATE || predefTypeId == TYPE_DATETIME;
        case TYPE_INT:
        case TYPE_FLOAT:
          return predefTypeId == TYPE_INT || predefTypeId == TYPE_FLOAT;
        case TYPE_BOOLEAN:
          return predefTypeId == TYPE_BOOLEAN;
        case TYPE_IMAGE:
          return false;        
        default:
          return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if parameter type is compatible with the predefined parameter type.
    /// </summary>
    static protected bool GetIsPredefParamEntityCompatible(
      CxBaseEntity predefParamEntity,
      CxEntityMetadata sourceEntityMetadata)
    {
      if (CxUtils.NotEmpty(predefParamEntity["CompatibleEntities"]))
      {
        if (sourceEntityMetadata == null)
        {
          return false;
        }
        string entityStr = CxUtils.ToString(predefParamEntity["CompatibleEntities"]);
        IList<string> entityIdList = CxText.DecomposeWithSeparator(entityStr, ",");
        foreach (string entityId in entityIdList)
        {
          if (CxText.ToUpper(entityId) == sourceEntityMetadata.Id)
          {
            return true;
          }
        }
        return false;
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value of the given template parameter.
    /// </summary>
    static protected string GetParameterValue(
      CxBaseEntity paramEntity,
      CxBaseEntity paramValueEntity,
      CxBaseEntity sourceEntity)
    {
      string paramName = CxUtils.ToString(paramEntity["Name"]);
      string typeId = CxUtils.ToString(paramEntity["TypeId"]);
      string paramValue =
        ConvertParameterValueToString(paramValueEntity[paramName], typeId);
      if (CxUtils.NotEmpty(paramValue))
      {
        switch (typeId)
        {
          case TYPE_MODULE:
            //string controlPath = CxWebUtils.GetAbsUrl(paramValue);
            //Control ctrl = CxBasePortalForm.CurrentPortalForm.LoadControl(controlPath);
            //if (ctrl is IxPrintable)
            //{
            //  ((IxPrintable)ctrl).IsInPrintMode = true;
            //  ((IxPrintable)ctrl).EntityToPrint = sourceEntity;
            //}
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //ctrl.DataBind();
            //ctrl.RenderControl(htw);
            //return sw.ToString();
            throw new ExException(string.Format(TYPE_MODULE_EX, typeId));
          case TYPE_IMAGE:
            return "<IMG SRC='" + paramValue + "' />";
          default:
            return paramValue;
        }
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts parameter value to string.
    /// </summary>
    /// <param name="paramValue"></param>
    /// <param name="paramTypeId"></param>
    /// <returns></returns>
    static protected string ConvertParameterValueToString(
      object paramValue,
      string paramTypeId)
    {
      if (CxUtils.IsEmpty(paramValue))
      {
        return "";
      }
      switch (paramTypeId)
      {
        case TYPE_DATE:
          return Convert.ToDateTime(paramValue).ToString(
            DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
        case TYPE_DATETIME:
          return Convert.ToDateTime(paramValue).ToString(
            DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " + DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
        case TYPE_INT:
          return CxInt.Parse(paramValue, 0).ToString();
        case TYPE_FLOAT:
          return CxFloat.ParseFloat(paramValue, 0).ToString("#0.00");
        case TYPE_BOOLEAN:
          return CxBool.Parse(paramValue) ? "Yes" : "No";
        default:
          return CxUtils.ToString(paramValue);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns hashtable of predefined parameters.
    /// </summary>
    static public Hashtable GetPredefinedParameterMap(CxEntityUsageMetadata predefEntityUsage, CxDbConnection connection)
    {
      DataTable predefTable = new DataTable();
      
      predefEntityUsage.ReadData(connection, predefTable);

      Hashtable predefMap = new Hashtable();
      foreach (DataRow row in predefTable.Rows)
      {
        CxBaseEntity predefEntity = CxBaseEntity.CreateByDataRow(predefEntityUsage, row);
        predefMap[CxUtils.ToString(predefEntity["Code"])] = predefEntity;
      }
      return predefMap;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts parameter type to attribute metadata type.
    /// </summary>
    /// <param name="typeId"></param>
    /// <returns></returns>
    static protected string GetAttrType(string typeId)
    {
      switch (typeId)
      {
        case TYPE_STRING:
        case TYPE_HTML:
        case TYPE_IMAGE:
        case TYPE_MODULE:
          return CxAttributeMetadata.TYPE_STRING;
        case TYPE_DATE:
          return CxAttributeMetadata.TYPE_DATE;
        case TYPE_DATETIME:
          return CxAttributeMetadata.TYPE_DATETIME;
        case TYPE_INT:
          return CxAttributeMetadata.TYPE_INT;
        case TYPE_FLOAT:
          return CxAttributeMetadata.TYPE_FLOAT;
        case TYPE_BOOLEAN:
          return CxAttributeMetadata.TYPE_BOOLEAN;
        default:
          return "";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts parameter type to attribute web control type.
    /// </summary>
    /// <param name="typeId"></param>
    /// <returns></returns>
    static protected string GetAttrWebControl(string typeId)
    {
      return typeId == TYPE_HTML ? CxAttributeMetadata.WEB_CONTROL_HTML : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns control width for the parameter attribute.
    /// </summary>
    /// <param name="typeId"></param>
    /// <returns></returns>
    static protected string GetAttrControlWidth(string typeId)
    {
      return typeId == TYPE_STRING || typeId == TYPE_HTML ? "2" : "1";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns actual parameter expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="predefMap"></param>
    /// <returns></returns>
    static protected string GetParameterExpression(
      string expression,
      Hashtable predefMap)
    {
      string result = expression;
      if (CxUtils.NotEmpty(result) && result.StartsWith(PREDEFINED_PREFIX))
      {
        string predefCode = result.Substring(PREDEFINED_PREFIX.Length);
        CxBaseEntity predefEntity = (CxBaseEntity)predefMap[predefCode];
        result = predefEntity != null ? CxUtils.ToString(predefEntity["Expression"]) : "";
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all template parameter entities.
    /// </summary>
    /// <param name="templateEntity">template entity</param>
    static public IList<CxBaseEntity> GetParameterList(CxBaseEntity templateEntity, CxDbConnection connection)
    {
      CxEntityUsageMetadata paramEntityUsage =
        templateEntity.Metadata.Holder.EntityUsages[TEMPLATE_PARAM_ENTITY_ID];

      DataTable paramTable = new DataTable();

      paramEntityUsage.ReadData(
        connection,
        paramTable,
        "TemplateId = :TemplateId",
        new object[] { templateEntity["TemplateId"] });

      List<CxBaseEntity> paramList = new List<CxBaseEntity>();
      foreach (DataRow row in paramTable.Rows)
      {
        CxBaseEntity paramEntity = CxBaseEntity.CreateByDataRow(paramEntityUsage, row);
        paramList.Add(paramEntity);
      }
      return paramList;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces placeholders in the given template content.
    /// </summary>
    /// <param name="paramValuesEntity">entity with parameter values</param>
    /// <param name="paramList">list of template parameters</param>
    /// <param name="templateContent">html template content</param>
    /// <returns>html template content with replaced parameters</returns>
    static public string ReplaceTemplatePlaceholders(
      CxBaseEntity paramValuesEntity,
      IList<CxBaseEntity> paramList,
      string templateContent,
      CxBaseEntity sourceEntity)
    {
      string replacedContent = templateContent;
      foreach (CxBaseEntity paramEntity in paramList)
      {
        string paramName = CxUtils.ToString(paramEntity["Name"]);
        string paramValue = GetParameterValue(paramEntity, paramValuesEntity, sourceEntity);
        replacedContent = replacedContent.Replace(paramName, paramValue);
      }
      return replacedContent;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates metadata for a template instance.
    /// </summary>
    /// <param name="templateEntity"></param>
    /// <param name="paramList"></param>
    /// <param name="predefMap"></param>
    static public CxEntityUsageMetadata CreateTemplateInstanceMetadata(
      CxBaseEntity templateEntity,
      IList<CxBaseEntity> paramList,
      Hashtable predefMap, 
      bool hasWebControls)
    {
      CxEntityUsageMetadata baseEntityUsage =
        templateEntity.Metadata.Holder.EntityUsages[TEMPLATE_INSTANCE_ENTITY_ID];
      CxEntityUsageMetadata resultEntityUsage =
        new CxEntityUsageMetadata(baseEntityUsage.Holder, baseEntityUsage);
      resultEntityUsage.Id = TEMPLATE_INSTANCE_ENTITY_ID + "_" + CxUtils.ToString(templateEntity["Code"]);

      XmlDocument emailFromDoc = new XmlDocument();
      XmlElement emailFromElement = emailFromDoc.CreateElement("attribute_usage");
      emailFromDoc.AppendChild(emailFromElement);
      emailFromElement.SetAttribute("id", "EmailFrom");
      emailFromElement.SetAttribute(
        "default",
        GetParameterExpression(CxUtils.ToString(templateEntity["EmailFrom"]), predefMap));
      CxAttributeUsageMetadata emailFromAttr =
        new CxAttributeUsageMetadata(emailFromElement, resultEntityUsage);
      resultEntityUsage.AddAttribute(emailFromAttr);

      XmlDocument emailToDoc = new XmlDocument();
      XmlElement emailToElement = emailToDoc.CreateElement("attribute_usage");
      emailToDoc.AppendChild(emailToElement);
      emailToElement.SetAttribute("id", "EmailTo");
      emailToElement.SetAttribute(
        "default",
        GetParameterExpression(CxUtils.ToString(templateEntity["EmailTo"]), predefMap));
      CxAttributeUsageMetadata emailToAttr =
        new CxAttributeUsageMetadata(emailToElement, resultEntityUsage);
      resultEntityUsage.AddAttribute(emailToAttr);

      XmlDocument addrDoc = new XmlDocument();
      XmlElement addrElement = addrDoc.CreateElement("attribute_usage");
      addrDoc.AppendChild(addrElement);
      addrElement.SetAttribute("id", "PostAddress");
      addrElement.SetAttribute(
        "default",
        GetParameterExpression(CxUtils.ToString(templateEntity["PostAddress"]), predefMap));
      CxAttributeUsageMetadata addrAttr =
        new CxAttributeUsageMetadata(addrElement, resultEntityUsage);
      resultEntityUsage.AddAttribute(addrAttr);

      foreach (CxBaseEntity paramEntity in paramList)
      {
        string name = CxUtils.ToString(paramEntity["Name"]);
        string typeId = CxUtils.ToString(paramEntity["TypeId"]);
        XmlDocument doc = new XmlDocument();
        XmlElement attrElement = doc.CreateElement("attribute");
        doc.AppendChild(attrElement);

        attrElement.SetAttribute("id", name);
        attrElement.SetAttribute("caption", name.Substring(1, name.Length - 2));
        attrElement.SetAttribute("type", GetAttrType(typeId));

        if (hasWebControls)
        {
          string webControl = GetAttrWebControl(typeId);
          if (CxUtils.NotEmpty(webControl))
          {
            attrElement.SetAttribute("web_control", webControl);
          }
          attrElement.SetAttribute("visible", (typeId != TYPE_MODULE && typeId != TYPE_IMAGE).ToString().ToLower());
          attrElement.SetAttribute("control_width", GetAttrControlWidth(typeId));
        }
        string expression =
          GetParameterExpression(CxUtils.ToString(paramEntity["DefaultValue"]), predefMap);
        if (CxUtils.NotEmpty(expression) &&
            typeId != TYPE_MODULE && typeId != TYPE_IMAGE)
        {
          attrElement.SetAttribute("default", expression);
        }
        CxAttributeMetadata attr = new CxAttributeMetadata(attrElement, resultEntityUsage);
        resultEntityUsage.AddAttribute(attr);
      }
      return resultEntityUsage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity instance for mail template.
    /// </summary>
    /// <param name="targetEntityUsage"></param>
    /// <param name="paramList"></param>
    /// <param name="predefMap"></param>
    /// <returns></returns>
    static public CxBaseEntity CreateTemplateInstanceEntity(
      CxBaseEntity templateEntity,
      CxEntityUsageMetadata targetEntityUsage,
      IList<CxBaseEntity> paramList,
      Hashtable predefMap,
      DxGetParentValue getParentValue,
      DxGetSequenceValue getSequenceValue,
      CxDbConnection connection)
    {
      CxBaseEntity targetEntity = CxBaseEntity.CreateWithDefaults(
        targetEntityUsage,
        getParentValue,
        getSequenceValue,
        connection);

      targetEntity.IsNew = true;

      targetEntity["TemplateCode"] = templateEntity["Code"];
      targetEntity["Subject"] = templateEntity["Subject"];

      foreach (CxBaseEntity paramEntity in paramList)
      {
        string name = CxUtils.ToString(paramEntity["Name"]);
        string typeId = CxUtils.ToString(paramEntity["TypeId"]);
        string expression =
          GetParameterExpression(CxUtils.ToString(paramEntity["DefaultValue"]), predefMap);
        if (CxUtils.NotEmpty(expression) &&
            (typeId == TYPE_IMAGE || typeId == TYPE_MODULE))
        {
          targetEntity[name] = expression;
        }
      }
      return targetEntity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns row source items for the mail template selection drop down.
    /// </summary>
    /// <param name="categoryCode"></param>
    /// <returns></returns>
    static public IList GetTemplateSelectRowSourceItems(string categoryCode,
      //CxBaseEntity templateEntity,
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection)
    {
      ArrayList list = new ArrayList();
      //CxEntityUsageMetadata entityUsage = templateEntity.Metadata.EntityUsages[TEMPLATE_ENTITY_ID];
      DataTable dt = new DataTable();

      entityUsage.ReadData(
        connection, dt, "CategoryCode = :CategoryCode", new object[] { categoryCode });

      DataRow[] rows = dt.Select("", entityUsage.OrderByClause);
      foreach (DataRow row in rows)
      {
        CxBaseEntity entity = CxBaseEntity.CreateByDataRow(entityUsage, row);
        CxComboItem item = new CxComboItem(entity["Code"], CxUtils.ToString(entity["Name"]));
        list.Add(item);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns sent item entity to write into sent items DB log.
    /// </summary>
    static public CxBaseEntity CreateSentItemEntity(
      CxBaseEntity templateEntity,
      CxBaseEntity paramValuesEntity,
      IList<CxBaseEntity> paramList,
      CxBaseEntity sourceEntity,
      DxGetParentValue getParentValue,
      DxGetSequenceValue getSequenceValue,
      CxDbConnection connection)
    {
      CxEntityUsageMetadata entityUsage =
        templateEntity.Metadata.Holder.EntityUsages[TEMPLATE_SENT_ITEM_ENTITY_ID];

      CxBaseEntity entity = CxBaseEntity.CreateWithDefaults(
        entityUsage,
        getParentValue,
        getSequenceValue,
        connection);

      entity.IsNew = true;

      entity["TemplateId"] = templateEntity["TemplateId"];
      string subjectContent = ReplaceTemplatePlaceholders(
        paramValuesEntity,
        paramList,
        CxUtils.ToString(paramValuesEntity["Subject"]),
        sourceEntity);

      entity["Subject"] = subjectContent;

      entity["EmailFrom"] = paramValuesEntity["EmailFrom"];
      entity["EmailTo"] = paramValuesEntity["EmailTo"];
      entity["PostAddress"] = paramValuesEntity["PostAddress"];


      string templateContent = CxUtils.ToString(templateEntity["TemplateHtml"]);
      string mailContent = ReplaceTemplatePlaceholders(
        paramValuesEntity,
        paramList,
        templateContent,
        sourceEntity);
      entity["MailContent"] = mailContent;

      if (CxUtils.NotEmpty(paramValuesEntity["DeliveryTypeId"]))
      {
        entity["DeliveryTypeId"] = paramValuesEntity["DeliveryTypeId"];
      }
      entity["PostedDateTime"] = DateTime.Now;

      foreach (string name in entity.AllProperties)
      {
        string AttrId = name.ToUpper();
        if (entity[AttrId] == null && paramValuesEntity[AttrId] != null)
        {
          entity[AttrId] = paramValuesEntity[AttrId];
        }
      }

      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SentItem entity by Template Code and Source Entity
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    static public CxBaseEntity CreateSentItemEntity(
      string templateCode,
      CxBaseEntity sourceEntity,
      DxGetParentValue getParentValue,
      DxGetSequenceValue getSequenceValue,
      CxDbConnection connection,
      bool hasWebControls)
    {
      CxBaseEntity templateEntity = LoadTemplateEntity(templateCode, sourceEntity, connection);
      if (templateEntity == null)
      {
        throw new ExException(
          "Mail template entity with code = '" + templateCode + "' " +
          "is not found in the database.");
      }

      IList<CxBaseEntity> paramList = GetParameterList(templateEntity, connection);
      CxEntityUsageMetadata predefEntityUsage = templateEntity.Metadata.Holder.EntityUsages[PREDEFINED_PARAM_ENTITY_ID];

      Hashtable predefMap = GetPredefinedParameterMap(predefEntityUsage, connection);

      CxEntityUsageMetadata entityUsage = templateEntity.Metadata.EntityUsages[TEMPLATE_ENTITY_ID];

      CxEntityUsageMetadata instanceEntityUsage =
        CreateTemplateInstanceMetadata(templateEntity, paramList, predefMap, hasWebControls);

      CxBaseEntity instanceEntity = CreateTemplateInstanceEntity(
        templateEntity,
        instanceEntityUsage,
        paramList,
        predefMap,
        getParentValue,
        getSequenceValue,
        connection);

      CxBaseEntity sendItemEntity = CreateSentItemEntity(
        templateEntity,
        instanceEntity,
        paramList,
        sourceEntity,
        getParentValue,
        getSequenceValue,
        connection);

      return sendItemEntity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends item by mail.
    /// </summary>
    /// <param name="sendItemEntity"></param>
    static public void SendByEmail(CxBaseEntity sendItemEntity, CxDbConnection connection)
    {
      SendByEmail(sendItemEntity, connection, SmtpDeliveryMethod.Network);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends item by mail.
    /// </summary>
    /// <param name="sendItemEntity"></param>
    static public void SendByEmail(CxBaseEntity sendItemEntity, CxDbConnection connection, SmtpDeliveryMethod smtpDeliveryMethod)
    {
      MailMessage msg = new MailMessage();
      msg.From = new MailAddress(CxUtils.ToString(sendItemEntity["EmailFrom"]));
      msg.To.Add(new MailAddress(CxUtils.ToString(sendItemEntity["EmailTo"])));
      msg.Subject = CxUtils.ToString(sendItemEntity["Subject"]);
      msg.BodyEncoding = Encoding.UTF8;
      msg.IsBodyHtml = true;
      msg.Body = CxUtils.ToString(sendItemEntity["MailContent"]);

      SendEmail(msg, smtpDeliveryMethod);
      sendItemEntity["SentDateTime"] = DateTime.Now;

      WriteLog(sendItemEntity, connection);
      /*
      MailMessage msg = new MailMessage();
      msg.From = CxUtils.ToString(sendItemEntity["EmailFrom"]);
      msg.To = CxUtils.ToString(sendItemEntity["EmailTo"]);
      msg.Subject = CxUtils.ToString(sendItemEntity["Subject"]);
      msg.BodyEncoding = Encoding.UTF8;
      msg.BodyFormat = MailFormat.Html;
      msg.Body = CxUtils.ToString(sendItemEntity["MailContent"]);

      SendEmail(msg);
      sendItemEntity["SentDateTime"] = DateTime.Now;

      WriteLog(sendItemEntity, connection);
      */
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads template entity by the given template code.
    /// </summary>
    static public CxBaseEntity LoadTemplateEntity(
      string templateCode,
      CxBaseEntity sourceEntity,
      CxDbConnection connection)
    {
      CxEntityUsageMetadata templateEntityUsage = sourceEntity.Metadata.EntityUsages[TEMPLATE_ENTITY_ID];

      CxHashtable valueProvider = new CxHashtable();
      valueProvider["Code"] = templateCode;
      CxBaseEntity templateEntity = CxBaseEntity.CreateAndReadFromDb(
        templateEntityUsage,
        connection,
        "Code = :Code",
        valueProvider);
      return templateEntity;

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends template email.
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    static public void SendTemplateEmail(
      string templateCode, 
      CxBaseEntity sourceEntity,
      CxDbConnection connection,
      bool hasWebControls)
    {
      SendTemplateEmail(templateCode, sourceEntity, connection, hasWebControls, SmtpDeliveryMethod.Network);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends template email.
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    static public void SendTemplateEmail(
      string templateCode,
      CxBaseEntity sourceEntity,
      CxDbConnection connection,
      bool hasWebControls,
      SmtpDeliveryMethod smtpDeliveryMethod)
    {
      CxBaseEntity sendItemEntity = CreateSentItemEntity(
        templateCode,
        sourceEntity,
        delegate(string expr) { return sourceEntity.GetParentEntityValue(connection, expr, true); },
        new DxGetSequenceValue(sourceEntity.GetSequenceValue),
        connection,
        hasWebControls);

      SendByEmail(sendItemEntity, connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends template email.
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    static public void SendTemplateEmail(
      string templateCode,
      CxBaseEntity sourceEntity,
      bool reloadEntityFromDatabase,
      CxDbConnection connection,
      bool hasWebControls)
    {
      SendTemplateEmail(templateCode, sourceEntity, reloadEntityFromDatabase, connection, hasWebControls, SmtpDeliveryMethod.Network);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends template email.
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    static public void SendTemplateEmail(
      string templateCode,
      CxBaseEntity sourceEntity,
      bool reloadEntityFromDatabase,
      CxDbConnection connection,
      bool hasWebControls,
      SmtpDeliveryMethod smtpDeliveryMethod
      )
    {
      // Reload entity from database.
      if (reloadEntityFromDatabase)
      {
        sourceEntity.ReadFromDb(connection);
      }

      // Send template email.
      SendTemplateEmail(templateCode, sourceEntity, connection, hasWebControls, smtpDeliveryMethod);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes Log after Sending E-mail or Letter Printing
    /// </summary>
    /// <param name="sentItemEntity"></param>
    static public void WriteLog(CxBaseEntity sentItemEntity, CxDbConnection connection)
    {
      int ItemId = CxInt.Parse(sentItemEntity["ItemId"], -1);
      if (!CxBool.Parse(connection.ExecuteScalar(@"select 1 
                                                      from MailTemplateSentItems
                                                     where ItemId = :ItemId", new object[] { ItemId }), false))
      {
        try
        {
          if (!connection.InTransaction)
            connection.BeginTransaction();
          sentItemEntity.WriteChangesToDb(connection);

          CxDbParameter[] param = new CxDbParameter[1];
          param[0] = connection.CreateParameter("ItemId", ItemId);

          connection.ExecuteCommandSP("p_MailTemplate_SendFinish", param);

          connection.Commit();
        }
        catch (Exception)
        {
          connection.Rollback();
          throw;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends email using SMTP server settings from Web.config file.
    /// .NET 2.0 mail classes are used.
    /// Uses Network as SmtpDeliveryMethod
    /// </summary>
    /// <param name="message">mail message object to send</param>
    static public void SendEmail(MailMessage message)
    {
      SendEmail(message, SmtpDeliveryMethod.Network);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sends email using SMTP server settings from Web.config file.
    /// .NET 2.0 mail classes are used.
    /// </summary>
    /// <param name="message">mail message object to send</param>
    /// <param name="smtpDeliveryMethod">delivery method for sending email</param>
    static public void SendEmail(MailMessage message, SmtpDeliveryMethod smtpDeliveryMethod)
    {
      string debugEmailRedirectAddress = CxConfigurationHelper.DebugEmailRedirectAddress;
      if (CxUtils.NotEmpty(message.To) &&
          CxUtils.NotEmpty(debugEmailRedirectAddress))
      {
        message.Sender = new MailAddress(debugEmailRedirectAddress);
      }

      string defaultEmailFrom = CxConfigurationHelper.DefaultEmailFromAddress;
      if ((message.From == null || String.IsNullOrEmpty(message.From.Address)) &&
          CxUtils.NotEmpty(defaultEmailFrom))
      {
        message.From = new MailAddress(defaultEmailFrom);
      }

      string smtpServer = SmtpMailServer;
      SmtpClient smtpClient = new SmtpClient(smtpServer);
      smtpClient.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
      smtpClient.DeliveryMethod = smtpDeliveryMethod;
      smtpClient.Send(message);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets Email body template email.
    /// </summary>
    /// <param name="templateCode">template code</param>
    /// <param name="sourceEntity">entity to get values from</param>
    /// <returns>Email body as string</returns>
    public static string GetEmailBody(
      string templateCode,
      CxBaseEntity sourceEntity,
      CxDbConnection connection,
      bool hasWebControls)
    {
      CxBaseEntity sendItemEntity = CreateSentItemEntity(
        templateCode,
        sourceEntity,
        delegate(string expr) { return sourceEntity.GetParentEntityValue(connection, expr, true); },
        new DxGetSequenceValue(sourceEntity.GetSequenceValue),
        connection,
        hasWebControls);

      return CxUtils.ToString(sendItemEntity["MailContent"]);
    }
    //-------------------------------------------------------------------------
  }
}