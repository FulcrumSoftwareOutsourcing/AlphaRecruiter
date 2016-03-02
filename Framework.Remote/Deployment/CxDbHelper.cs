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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Framework.Db;
using System.Linq;

namespace Framework.Remote
{
  public  class CxDbHelper
  {    
    //Data Source=localhost;Initial Catalog=Tracker;user=sa; password=sa
    //Data Source=localhost;Initial Catalog=Tracker;integrated security=true
    private const string DATA_SOURCE_PART = "Data Source=";
    private const string INITIAL_CATALOG_PART = "Initial Catalog=";
    private const string INTEGRATED_SECURITY_PART = "Integrated Security=True";
    private const string USER_PART = "user=";
    private const string PASSWORD_PART = "password=";
    private const string PARAMS_DELIMITER = ";";

    //---------------------------------------------------------------------------
    public string DataSourceValue { get; private set; }

    //---------------------------------------------------------------------------
    public string InitialCatalogValue { get; private set; }

    //---------------------------------------------------------------------------
    public bool IntegratedSecurity { get; private set; }

    //---------------------------------------------------------------------------
    public string UserNameValue { get; private set; }

    //---------------------------------------------------------------------------
    public string PasswordValue { get; private set; }
    //---------------------------------------------------------------------------
    public bool HasChanges { get; private set; }


    ////---------------------------------------------------------------------------
    //public string InitialCatalogValue
    //{
    //  get { return m_InitialCatalogValue; }
    //  set { m_InitialCatalogValue = value; }
    //}

    //---------------------------------------------------------------------------
    public CxDbHelper(
      string serverName,
      bool integratedSecurity,
      string userName,
      string password,
      string databaseName)
    {
      InitValuesFromConfig(GetConnectionConfigElement());

      if (string.Compare(DataSourceValue, serverName, true) != 0)
      {
        HasChanges = true;
        DataSourceValue = serverName;
      }

      if (IntegratedSecurity != integratedSecurity)
      {
        HasChanges = true;
        IntegratedSecurity = integratedSecurity;  
      }

      if (string.Compare(UserNameValue, userName, true) != 0)
      {
        HasChanges = true;
        UserNameValue = userName;  
      }

      if (string.Compare(PasswordValue, password, false) != 0)
      {
        HasChanges = true;
        PasswordValue = password;  
      }

      if (string.Compare(InitialCatalogValue, databaseName, true) != 0)
      {
        HasChanges = true;
        InitialCatalogValue = databaseName;
      }
    }

    //---------------------------------------------------------------------------
    public CxDbHelper()
    {
      InitValuesFromConfig(GetConnectionConfigElement());
    }

    //---------------------------------------------------------------------------
    private XElement GetConnectionConfigElement()
    {
      XElement connElement = null;
      try
      {
        Configuration conf = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        using (StreamReader reader = new StreamReader(conf.FilePath))
        {
          XElement configXml = XElement.Load(reader);
          XElement dbConnectionsSection = configXml.Element("databaseConnections");
          if (dbConnectionsSection == null)
            throw new ApplicationException("<databaseConnections> section is not defined in Web.config file.");

          connElement = dbConnectionsSection.Element("connection");
          if (connElement == null)
            throw new ApplicationException(
              "<databaseConnections> section in Web.config file does not contains required section <connection>.");
        }
        
      }
      catch (ExAccessToConfigException ex)
      {
        HttpContext.Current.Session[CxDeploymentKeys.ACCESS_TO_CONFIG_ERROR_SESSION_KEY] = ex;
        HttpContext.Current.Server.Transfer("~/Deployment/AccessToConfigErrorPage.aspx");
      }
      return connElement;
    }
    //---------------------------------------------------------------------------
    private void InitValuesFromConfig(XElement confElement)
    {
      XAttribute connSttrAttr = confElement.Attribute("connectionString");
      if(connSttrAttr == null)
        throw new ApplicationException(
            "<connection> section in Web.config file does not contains required attribute 'connectionString'.");

      string connStrFromConfig = connSttrAttr.Value;

      DataSourceValue = GetParameterFromConnString(connStrFromConfig, "Data Source", "localhost");
      InitialCatalogValue = GetParameterFromConnString(connStrFromConfig, "Initial Catalog", string.Empty);

      if (connStrFromConfig.ToLower().Contains("integrated security"))
      {
        IntegratedSecurity = true;
        UserNameValue = string.Empty;
        PasswordValue = string.Empty;
      }
      else
      {
        IntegratedSecurity = false;
        UserNameValue = GetParameterFromConnString(connStrFromConfig, "user", "sa");
        PasswordValue = GetParameterFromConnString(connStrFromConfig, "password", "sa");
      }
    }

    //---------------------------------------------------------------------------
    private string GetParameterFromConnString(
      string connString,
      string paramName,
      string defaultValue)
    {
      string[] paramsPairs = connString.Split(';');
      string paramPair = paramsPairs.FirstOrDefault(pair =>
        pair.ToLower().Contains(paramName.ToLower()));
      if (paramPair == null)
      {
        return defaultValue;
      }
      string[] splittedParamPair = paramPair.Split('=');
      if (splittedParamPair.Count() != 2)
        return defaultValue;
      return splittedParamPair[1];
    }
    //---------------------------------------------------------------------------
    public void CheckDatabaseIsAvailable()
    {
      using (SqlConnection conn = new SqlConnection(CreateConnectionString(false)))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand(
          @"select top 1 [name] from DbUpdateLog", conn);
        cmd.ExecuteScalar();
        conn.Close();
      }
    }

    //---------------------------------------------------------------------------
    public void CheckDatabaseServerIsAvailable()
    {
      using (SqlConnection conn = new SqlConnection(CreateConnectionString(true)))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand(
          @"SELECT  SERVERPROPERTY('productversion')",
            conn);
        cmd.ExecuteScalar();
        
      }
    }

    //---------------------------------------------------------------------------
    public void CreateDatabase()
    {
      
      using (SqlConnection conn = new SqlConnection(CreateConnectionString(true)))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand(
          @"CREATE DATABASE [" + InitialCatalogValue + "]", conn);
        cmd.ExecuteScalar();
        
      }     
    }

    //---------------------------------------------------------------------------
    public void CreateUser(string dbName,  string userName)
    {
      using (SqlConnection conn = new SqlConnection(CreateConnectionString(true)))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand(
          @"USE [" + dbName + "] " +
          "CREATE USER [" + userName + "]", conn);
        cmd.ExecuteScalar();
          
      }
    }
    //---------------------------------------------------------------------------
    public void InitDatabase()
    {
      string path = HttpContext.Current.Server.MapPath(@"~\Deployment\Scripts\InitDatabase.sql");
      string script;
      using (StreamReader reader = new StreamReader(path))
      {
        script = reader.ReadToEnd();
      }
      script = script.Replace("FulcrumWebIssueTracker", InitialCatalogValue);
      using (SqlConnection conn = new SqlConnection(CreateConnectionString(true)))
      {
        CxSqlResolver rs = new CxSqlResolver();
        string result = rs.ExecuteDeploymentStatement(script, conn);
      }
    }
    //---------------------------------------------------------------------------
    private string CreateConnectionString(bool useMaster)
    {
      string catalog = useMaster ? "master" : InitialCatalogValue;
      string securityPart = IntegratedSecurity
                              ? INTEGRATED_SECURITY_PART
                              :
                                USER_PART + UserNameValue + PARAMS_DELIMITER + PASSWORD_PART + PasswordValue;
      return
        DATA_SOURCE_PART +
        DataSourceValue +
        PARAMS_DELIMITER +
        INITIAL_CATALOG_PART +
        catalog +
        PARAMS_DELIMITER +
        securityPart;
    }

    //---------------------------------------------------------------------------
    public void SaveConnectionString()
    {
      if(!HasChanges)
        return;

      XDocument configDoc = null;
      XElement dbConnectionsSection = null;
      try
      {
         Configuration conf = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
         
         using (StreamReader reader = new StreamReader(conf.FilePath))
         {
           configDoc = XDocument.Load(reader);
           dbConnectionsSection = configDoc.Element("configuration").Element("databaseConnections");
           if (dbConnectionsSection == null)
             throw new ApplicationException("<databaseConnections> section is not defined in Web.config file.");

           XElement connElement = dbConnectionsSection.Element("connection");
           if (connElement == null)
             throw new ApplicationException(
               "<databaseConnections> section in Web.config file does not contains required section <connection>.");

           XAttribute connSttrAttr = connElement.Attribute("connectionString");
           if (connSttrAttr == null)
             throw new ApplicationException(
                 "<connection> section in Web.config file does not contains required attribute 'connectionString'.");

           FileInfo fileInfo = new FileInfo(conf.FilePath);
           bool wasReadonly = false;
           if (fileInfo.IsReadOnly)
           {
             fileInfo.IsReadOnly = false;
             wasReadonly = true;
           }

           connSttrAttr.Value = CreateConnectionString(false);
           configDoc.Save(conf.FilePath, SaveOptions.None);

           if (wasReadonly)
             fileInfo.IsReadOnly = true;

         }

      }
      catch(Exception ex)
      {
        ExAccessToConfigException cEx = new ExAccessToConfigException("Error occured when system try to modify Web.config file: " +
          ex.Message);

        cEx.ModifiedSection = dbConnectionsSection;
        cEx.ConfigFile = configDoc;
        throw cEx;
      }
        
      

      
    }
  }
}
