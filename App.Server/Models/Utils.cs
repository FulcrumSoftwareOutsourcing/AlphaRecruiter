using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Server.Models
{
    public class Utils
    {
        public static void AddError(IDictionary<string, List<string>> existingErrorList, string propertyName, string error)
        {
            if (existingErrorList.ContainsKey(propertyName) == false)
            {
                existingErrorList.Add(propertyName, new List<string> { error });
            }
            else
            {
                existingErrorList[propertyName].Add(error);
            }
        }
    }
}