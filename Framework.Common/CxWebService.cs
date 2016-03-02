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
using System.Runtime.Remoting.Contexts;
using System.Web.Services.Protocols;
using System.Xml;

namespace Framework.Utils
{
	/// <summary>
	/// Utility methods to work with web service.
	/// </summary>
	public class CxWebService
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates detail SOAP exception with the information about original exception.
    /// </summary>
    static public SoapException CreateServerException(Exception e, string uri)
    {
      Exception actualException = CxUtils.GetOriginalException(e);
      string stackTrace = CxCommon.GetExceptionFullStackTrace(e);

      XmlDocument doc = new XmlDocument();

      XmlNode detailNode = doc.CreateNode(
        XmlNodeType.Element, 
        SoapException.DetailElementName.Name, 
        SoapException.DetailElementName.Namespace);

      XmlElement errorElement = doc.CreateElement("Exception");
      errorElement.SetAttribute("type", actualException.GetType().FullName);

      XmlElement messageElement = doc.CreateElement("Message");
      messageElement.InnerText = actualException.Message;
      errorElement.AppendChild(messageElement);

      XmlElement stackElement = doc.CreateElement("StackTrace");
      stackElement.InnerText = stackTrace;
      errorElement.AppendChild(stackElement);

      detailNode.AppendChild(errorElement);

      SoapException soapError = new SoapException(
        e.Message, 
        SoapException.ClientFaultCode, 
        uri,
        detailNode);

      return soapError;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates web service client exception with the information about
    /// web service original error.
    /// </summary>
    static public ExWebServiceException CreateClientException(SoapException e)
    {
      string message = e.Message;
      string typeName = null;
      Exception webServiceException = null;
      string webServiceStackTrace = null;
      if (e.Detail != null)
      {
        XmlNode errorNode = e.Detail.SelectSingleNode("Exception");
        if (errorNode is XmlElement)
        {
          typeName = CxXml.GetAttr((XmlElement)errorNode, "type");

          XmlNode messageNode = errorNode.SelectSingleNode("Message");
          if (messageNode != null)
          {
            message = messageNode.InnerText;
          }

          XmlNode stackNode = errorNode.SelectSingleNode("StackTrace");
          if (stackNode != null)
          {
            webServiceStackTrace = stackNode.InnerText;
          }
        }
      }
      // Try to create exception object of the web service exception type.
      if (CxUtils.NotEmpty(typeName))
      {
        try
        {
          webServiceException = (Exception) CxType.CreateInstance(typeName, message);
        }
        catch 
        {
          try
          {
            webServiceException = (Exception) CxType.CreateInstance(typeName);
          }
          catch
          {
            webServiceException = null;
          }
        }
      }
      return new ExWebServiceException(message, webServiceException, webServiceStackTrace, e);
    }
    //-------------------------------------------------------------------------
  }
}