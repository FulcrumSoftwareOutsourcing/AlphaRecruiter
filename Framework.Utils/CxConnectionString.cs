using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utils
{
  public static class CxConnectionString
  {
    public static string GetValue(string connectionString, string key)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(key))
        return string.Empty;

      var parts = connectionString.Split(';');
      var part = parts.FirstOrDefault(x => x.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase));
      if (part != null)
      {
        var subParts = part.Split('=');
        if (subParts.Length > 1)
          return subParts[1];
      }
      return string.Empty;
    }

    public static string SetValue(string connectionString, string key, string value)
    {
      connectionString = connectionString.Trim();
      var key1 = key + "=";
      var keyIndex = connectionString.IndexOf(key1, StringComparison.OrdinalIgnoreCase);

      // We have to remove
      if (string.IsNullOrEmpty(value))
      {
        if (keyIndex > -1)
        {
          var colonIndex = connectionString.IndexOf(';', keyIndex);
          if (colonIndex > -1)
            connectionString = connectionString.Remove(keyIndex, colonIndex - keyIndex + 1);
          else
            connectionString = connectionString.Remove(keyIndex);
          return connectionString;
        }
      }
      else
      {
        // We have to add
        if (keyIndex == -1)
        {
          if (!string.IsNullOrEmpty(connectionString) && !connectionString.EndsWith(";"))
            connectionString += ";";
          return connectionString + key1 + value;
        }

        // We have to modify
        var colonIndex = connectionString.IndexOf(';', keyIndex);
        if (colonIndex > -1)
        {
          connectionString = connectionString.Remove(keyIndex + key1.Length, colonIndex - (keyIndex + key1.Length));
          connectionString = connectionString.Insert(keyIndex + key1.Length, value);
        }
        else
        {
          connectionString = connectionString.Remove(keyIndex + key1.Length);
          connectionString = connectionString + value;
        }
        return connectionString;
      }
      return connectionString;
    }
  }
}
