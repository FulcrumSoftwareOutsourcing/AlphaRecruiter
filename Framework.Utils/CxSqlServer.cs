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
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// SQL Server authentication mode.
  /// </summary>
  public enum NxSqlAuthMode {Unknown, Sql, Win}
  //---------------------------------------------------------------------------
  /// <summary>
  /// Options - how to construct connection string.
  /// </summary>
  [Flags]
  public enum NxConnectStringOptions
  {
    None            = 0,
    ExcludeProvider = 1,
    PersistSecurity = 2,
    ExcludePassword = 4
  }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Database objects to recompile.
  /// </summary>
  [Flags]
  public enum NxDbObjectToRecompile
  {
    None      = 0,
    View      = 1,
    Function  = 2,
    Procedure = 4,
    Trigger   = 8,
    All       = View | Function | Procedure | Trigger
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
	/// <summary>
	/// Utility methods for work with SQL Server database and MSDE installation.
	/// </summary>
	public class CxSqlServer
	{
    //-------------------------------------------------------------------------
    protected const string SQL_SERVICE_NAME = "MSSQLSERVER";
    protected const string BACKUP_DIRECTORY_NAME = "BackupDirectory";
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if local SQL server is running.
    /// </summary>
    static protected bool GetIsLocalServerRunning()
    {
      try
      {
        ServiceController sc = new ServiceController(SQL_SERVICE_NAME);
        return sc != null && sc.Status == ServiceControllerStatus.Running;
      }
      catch
      {
        return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Starts local MS SQL service or raises an exception if failed to run.
    /// </summary>
    /// <param name="timeOut">timeout in seconds; 0 means unlimited timeout</param>
    static public void RunLocalServer(int timeOut)
    {
      CxProcess.StartService(SQL_SERVICE_NAME, timeOut, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns registry key where SQL Server settings are stored.
    /// </summary>
    static protected RegistryKey GetSqlServerRegistryKey()
    {
      return Registry.LocalMachine
        .CreateSubKey("Software")
        .CreateSubKey("Microsoft")
        .CreateSubKey("MSSQLServer")
        .CreateSubKey("MSSQLServer");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL Server backup directory.
    /// </summary>
    static protected string GetLocalBackupDirectory()
    {
      RegistryKey key = GetSqlServerRegistryKey();
      return key.GetValue(BACKUP_DIRECTORY_NAME, "").ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets SQL Server backup directory.
    /// If given directory parameter is empty, sets backup dir to 'C:\'
    /// </summary>
    static protected void SetLocalBackupDirectory(string directory)
    {
      if (CxUtils.NotEmpty(directory))
      {
        if (directory.EndsWith("\\"))
        {
          directory = directory.Substring(0, directory.Length - 1);
        }
      }
      else
      {
        directory = "C:\\";
      }
      RegistryKey key = GetSqlServerRegistryKey();
      key.SetValue(BACKUP_DIRECTORY_NAME, directory);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Installs MSDE on the local computer.
    /// </summary>
    /// <param name="setupFileName">MSDE setup file name</param>
    /// <param name="setupArguments">MSDE setup command line parameters</param>
    static public bool InstallMSDE(string setupFileName, string setupArguments)
    {
      string fullName = Path.GetFullPath(setupFileName);
      string fileName = Path.GetFileName(fullName);
      ProcessStartInfo psi = new ProcessStartInfo(fileName);
      psi.Arguments = setupArguments;
      psi.WorkingDirectory = Path.GetDirectoryName(fullName);
      //psi.WindowStyle = ProcessWindowStyle.Hidden;
      //psi.CreateNoWindow = true;
      //psi.UseShellExecute = false;
      string curDir = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(psi.WorkingDirectory);
      Process process;
      try
      {
        process = Process.Start(psi);
        while (!process.HasExited)
        {
          Application.DoEvents();
        }
      }
      finally
      {
        Directory.SetCurrentDirectory(curDir);
      }
      return process != null && process.ExitCode == 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if a given name is a name of the local server.
    /// </summary>
    static public bool IsLocalServerName(string serverName)
    {
      if (CxUtils.IsEmpty(serverName))
      {
        return true;
      }
      else
      {
        if (serverName.ToLower() == "localhost" ||
            serverName.ToLower() == CxUtils.Nvl(Environment.MachineName).ToLower())
        {
          return true;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns OLE DB or SQL provider connection string to connect to MS SQL Server.
    /// </summary>
    /// <param name="serverName">server name</param>
    /// <param name="userName">user name</param>
    /// <param name="password">password</param>
    /// <param name="databaseName">database name</param>
    /// <param name="authMode">authentication mode</param>
    /// <param name="options">connection string options</param>
    /// <returns>connection string</returns>
    static public string GetConnectionString(
      string serverName,
      string userName,
      string password,
      string databaseName,
      NxSqlAuthMode authMode,
      NxConnectStringOptions options)
    {
      return
        ((options & NxConnectStringOptions.ExcludeProvider) == 0 ? "Provider={SQLOLEDB.1};" : "") +
        string.Format("Data Source={0};", serverName) +
        (CxUtils.NotEmpty(databaseName) ? string.Format("Initial Catalog={0};", databaseName) : "") +
        (authMode == NxSqlAuthMode.Win ?
        "Integrated Security=SSPI" :
        (CxUtils.NotEmpty(userName) ? String.Format("User ID={0};", userName) : string.Empty) +
        ((options & NxConnectStringOptions.ExcludePassword) == 0 && CxUtils.NotEmpty(password) ? string.Format("Password={0};", password) : string.Empty)) +
        ((options & NxConnectStringOptions.PersistSecurity) != 0 ? "Persist Security Info=True;" : "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns OLE DB or SQL provider connection string to connect to MS SQL Server.
    /// </summary>
    /// <param name="serverName">server name</param>
    /// <param name="userName">user name</param>
    /// <param name="password">password</param>
    /// <param name="databaseName">database name</param>
    /// <param name="authMode">authentication mode</param>
    /// <returns>connection string</returns>
    static public string GetConnectionString(
      string serverName,
      string userName,
      string password,
      string databaseName,
      NxSqlAuthMode authMode)
    {
      return GetConnectionString(
        serverName, userName, password, databaseName, authMode, NxConnectStringOptions.None);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns text of the command which kills SQL Server sessions for
    /// the specified database.
    /// </summary>
    static protected string GetKillSessionsCommandText(string databaseName)
    {
      return
        "BEGIN " +
        "  DECLARE @spid VARCHAR(25) " +
        "  DECLARE @sql  VARCHAR(1000) " +
        "  DECLARE kill_cursor CURSOR FOR " +
        "    SELECT sp.spid " +
        "      FROM sysprocesses sp, sysdatabases sd " +
        "     WHERE sp.dbid = sd.dbid " +
        "       AND sd.name = '" + databaseName + "' " +
        "  OPEN kill_cursor " +
        "  FETCH NEXT FROM kill_cursor INTO @spid " +
        "  WHILE @@fetch_status = 0 " +
        "  BEGIN " +
        "    SET @sql = 'KILL ' + @spid " +
        "    EXEC (@sql) " +
        "    FETCH NEXT FROM kill_cursor INTO @spid " +
        "  END " +
        "  CLOSE kill_cursor " +
        "  DEALLOCATE kill_cursor " +
        "END";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Kills all SQL Server sessions which are using given database.
    /// </summary>
    /// <param name="connection">open connection to MASTER database</param>
    /// <param name="databaseName">name of the database</param>
    static public void KillSessions(
      IDbConnection connection,
      string databaseName)
    {
      if (connection != null && CxUtils.NotEmpty(databaseName))
      {
        using (IDbCommand cmd = connection.CreateCommand())
        {
          cmd.CommandText = GetKillSessionsCommandText(databaseName);
          cmd.ExecuteNonQuery();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given database exists.
    /// </summary>
    /// <param name="connection">connection to SQL server</param>
    /// <param name="databaseName">database to check</param>
    static public bool IsDatabaseExist(
      IDbConnection connection, 
      string databaseName)
    {
      if (CxUtils.NotEmpty(databaseName))
      {
        using (IDbCommand cmd = connection.CreateCommand())
        {
          cmd.CommandText = String.Format(@"SELECT COUNT(*) 
                                              FROM master..sysdatabases 
                                             WHERE name = '{0}'", databaseName);
          return CxInt.Parse(cmd.ExecuteScalar(), 0) > 0;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL server command to backup given database to given backup name.
    /// </summary>
    static protected string GetBackupDbCommandText(
      string dbName,
      string backupName)
    {
      return
        "DECLARE @str VARCHAR(4000) " +
        "EXECUTE xp_regread " +
          "'HKEY_LOCAL_MACHINE', " +
          "'Software\\Microsoft\\MSSQLServer\\MSSQLServer', " +
          "'" + BACKUP_DIRECTORY_NAME + "', @str OUTPUT " +
        "SET @str = @str + '\\" + backupName + ".bkp' " +
        "IF NOT EXISTS (SELECT * FROM sysdevices WHERE name = '" + backupName + "') " +
        "EXECUTE sp_addumpdevice 'disk', '" + backupName + "', @str " +
        "BACKUP DATABASE " + dbName + " TO " + backupName + " WITH INIT";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Backups given SQL server database with the given backup name.
    /// </summary>
    /// <param name="connection">open connection to MASTER database</param>
    /// <param name="dbName">database to backup</param>
    /// <param name="backupName">backup device name</param>
    static public void BackupDb(
      IDbConnection connection,
      string dbName, 
      string backupName)
    {
      using (IDbCommand cmd = connection.CreateCommand())
      {
        cmd.CommandText = GetBackupDbCommandText(dbName, backupName);
        cmd.ExecuteNonQuery();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL server command to restore given database from the given backup name.
    /// </summary>
    static protected string GetRestoreDbCommandText(
      string dbName,
      string backupName)
    {
      return
        "RESTORE DATABASE " + dbName + " FROM " + backupName + " WITH REPLACE, RESTART";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL server command to backup device with the given name.
    /// </summary>
    static protected string GetDropBackupCommandText(string backupName)
    {
      return
        "IF EXISTS (SELECT * FROM sysdevices WHERE name = '" + backupName + "') " +
        "EXECUTE sp_dropdevice '" + backupName + "', 'DELFILE'";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Restores given SQL server database from the given backup name.
    /// </summary>
    /// <param name="connection">open connection to MASTER database</param>
    /// <param name="dbName">database to restore</param>
    /// <param name="backupName">backup device name</param>
    static public void RestoreDb(
      IDbConnection connection,
      string dbName, 
      string backupName)
    {
      using (IDbCommand cmd = connection.CreateCommand())
      {
        cmd.CommandText = GetRestoreDbCommandText(dbName, backupName);
        cmd.ExecuteNonQuery();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Drops backup device with the given name.
    /// </summary>
    /// <param name="connection">open connection to MASTER database</param>
    /// <param name="backupName">backup device to drop</param>
    static public void DropBackupDevice(
      IDbConnection connection,
      string backupName)
    {
      using (IDbCommand cmd = connection.CreateCommand())
      {
        cmd.CommandText = GetDropBackupCommandText(backupName);
        cmd.ExecuteNonQuery();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns serverName, if it is not empty, and network local computer name,
    /// if it is empty.
    /// </summary>
    static public string NvlServerName(string serverName)
    {
      return CxUtils.Nvl(serverName, LocalServerName);
    }
		//-------------------------------------------------------------------------
    /// <summary>
    /// Returns name for the local server.
    /// </summary>
    static public string LocalServerName
    {
      get
      {
        return Environment.MachineName;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
		/// Gets server`s databases location path
		/// </summary>
		static public string GetServerDBPath(IDbConnection conn)
		{
			string getDBPathSQL = "select substring(filename, 1, len(filename) - len(name) - 4) " 
				+ "from sysdatabases where name = 'master'";
      using (IDbCommand cmd = conn.CreateCommand())
      {
        cmd.CommandText = getDBPathSQL;
        object o = cmd.ExecuteScalar();
        if (o == null || o as string == null)
        {
          throw new ApplicationException("Can`t obtain server`s DBs path.");
        }
        return o as string;
      }
		}
		//-------------------------------------------------------------------------
		/// <summary>
		/// Drops a database
		/// </summary>
    static public void DropDatabase(IDbConnection conn, string DatabaseName)
		{
      using (IDbCommand cmd = conn.CreateCommand())
      {
        cmd.CommandText = "DROP DATABASE " + DatabaseName;
        cmd.ExecuteNonQuery();
      }
		}
		//-------------------------------------------------------------------------
		/// <summary>
		/// Attaches the database
		/// </summary>
		static public void AttachDatabase(IDbConnection conn, string DatabasePath, string DatabaseName)
		{
      using (IDbCommand cmd = conn.CreateCommand())
      {
        cmd.CommandText = "exec sp_attach_db N'" + DatabaseName
                          + "', N'" + DatabasePath + DatabaseName + ".mdf'";
        cmd.ExecuteNonQuery();
      }
		}
		//-------------------------------------------------------------------------
    /// <summary>
    /// Returns database list from the specified SQL server connection.
    /// </summary>
    static public IList GetDatabaseList(IDbConnection connection)
    {
      IDbCommand cmd = connection.CreateCommand();
      try
      {
        cmd.CommandText = "SELECT name FROM master..sysdatabases ORDER BY name";
        if (connection.State != ConnectionState.Open)
        {
          connection.Open();
        }
        IDataReader reader = cmd.ExecuteReader();
        ArrayList result = new ArrayList();
        while (reader.Read())
        {
          result.Add(reader.GetString(0));
        }
        reader.Close();
        return result;
      }
      finally
      {
        cmd.Dispose();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of the databases present on the SQL Server.
    /// </summary>
    static public IList GetDatabaseList(
      string serverName,
      string userName,
      string password,
      NxSqlAuthMode authMode)
    {
      string connectionString = 
        GetConnectionString(serverName, userName, password, "", authMode, NxConnectStringOptions.ExcludeProvider);
      using (SqlConnection c = new SqlConnection(connectionString))
      {
        return GetDatabaseList(c);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Recompiles SQL Server DB objects by executing ALTER object statement.
    /// </summary>
    static public void RecompileDbObjects(
      SqlConnection connection,
      NxDbObjectToRecompile objectsToRecompile,
      out int compiledCount,
      out string errorText)
    {
      compiledCount = 0;
      errorText = "";

      ArrayList objectTypeList = new ArrayList();
      if ((NxDbObjectToRecompile.View & objectsToRecompile) > 0)
      {
        objectTypeList.Add("V");
      }
      if ((NxDbObjectToRecompile.Function & objectsToRecompile) > 0)
      {
        objectTypeList.Add("IF");
        objectTypeList.Add("TF");
      }
      if ((NxDbObjectToRecompile.Procedure & objectsToRecompile) > 0)
      {
        objectTypeList.Add("P");
      }
      if ((NxDbObjectToRecompile.Trigger & objectsToRecompile) > 0)
      {
        objectTypeList.Add("TR");
      }

      string objectTypeInClause = "";
      foreach (string s in objectTypeList)
      {
        objectTypeInClause += (objectTypeInClause.Length > 0 ? "," : "") + "'" + s + "'";
      }
      if (objectTypeInClause.Length == 0)
      {
        return;
      }

      string selectSql = @"select c.id,
                                  c.colid,
                                  o.name,
                                  c.text
                             from syscomments c,
                                  sysobjects o
                            where c.id = o.id
                              and o.xtype in (" + objectTypeInClause + @")
                              and o.name not like 'sys%'";

      DataTable dt = new DataTable();
      using (SqlDataAdapter da = new SqlDataAdapter(selectSql, connection))
      {
        da.Fill(dt);
      }
      DataRow[] rows = dt.Select("", "id, colid", DataViewRowState.CurrentRows);

      string recompileSql = "";
      string prevObjId = "";
      string currObjId = "";
      string prevObjName = "";
      for (int i = 0; i <= rows.Length; i++)
      {
        currObjId = i < rows.Length ? rows[i]["id"].ToString() : "";
        if (prevObjId != currObjId)
        {
          if (recompileSql.Length > 0)
          {
            // Prepare recompile SQL, replace CREATE with ALTER
            Regex r = new Regex("^CREATE\\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (r.IsMatch(recompileSql))
            {
              recompileSql = r.Replace(recompileSql, "ALTER ", 1);
            }
            else
            {
              r = new Regex("\\s+CREATE\\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
              if (r.IsMatch(recompileSql))
              {
                recompileSql = r.Replace(recompileSql, " ALTER ", 1);
              }
            }
            // Perform recompile SQL
            using (SqlCommand command = new SqlCommand(recompileSql, connection))
            {
              try
              {
                command.ExecuteNonQuery();
              }
              catch (Exception e)
              {
                errorText += "Error compiling '" + prevObjName + "': \r\n" + e.Message + "\r\n\r\n";
              }
            }
            compiledCount++;
            recompileSql = "";
          }
        }
        if (i < rows.Length)
        {
          recompileSql += rows[i]["text"].ToString();
          prevObjName = rows[i]["name"].ToString();
        }
        prevObjId = currObjId;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns string constant ready to put into SQL statement.
    /// </summary>
    static public string GetStringConst(string text)
    {
      if (CxUtils.IsEmpty(text))
      {
        return "null";
      }
      else 
      {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
          if (text[i] < 32)
          {
            if (i > 0)
            {
              if (text[i - 1] >= 32)
              {
                sb.Append('\'');
              }
              sb.Append(" + ");
            }
            sb.Append("Char(" + Convert.ToInt16(text[i]).ToString() + ")");
          }
          else 
          {
            if (i == 0)
            {
              sb.Append('\'');
            }
            else if (i > 0 && text[i - 1] < 32)
            {
              sb.Append(" + '");
            }

            if (text[i] == '\'')
            {
              sb.Append("''");
            }
            else
            {
              sb.Append(text[i]);
            }
          }
        }
        if (text[text.Length - 1] >= 32)
        {
          sb.Append('\'');
        }
        return sb.ToString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if string is in SQL representation.
    /// </summary>
    static public bool IsSqlString(string s)
    {
      return CxUtils.NotEmpty(s) && s.Length > 2 && 
             ((s.StartsWith("\"") && s.EndsWith("\"")) || (s.StartsWith("'") && s.EndsWith("'")));
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Parses SQL string representation to the normal string value.
    /// </summary>
    static public string ParseSqlString(string s)
    {
      if (IsSqlString(s))
      {
        s = s.Substring(1).Substring(0, s.Length - 2);
        s = CxText.Replace(s, new string[]{"\"\"", "''"}, new string[]{"\"", "'"});
      }
      return s;
    }
    //--------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if local SQL server is running.
    /// </summary>
    static public bool IsLocalServerRunning
    { get {return GetIsLocalServerRunning();} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets SQL Server backup directory.
    /// </summary>
    static public string LocalBackupDirectory
    { get {return GetLocalBackupDirectory();} set {SetLocalBackupDirectory(value);}}
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}