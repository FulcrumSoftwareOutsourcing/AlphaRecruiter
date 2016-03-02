using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Linq;

namespace Framework.Remote
{
  public class ExAccessToConfigException : Exception
  {
    public ExAccessToConfigException()
    {
    }

    public ExAccessToConfigException(string message) : base(message)
    {
    }

    public ExAccessToConfigException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ExAccessToConfigException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    //---------------------------------------------------------------------------
    public XElement ModifiedSection { get; set; }
    //---------------------------------------------------------------------------
    public XDocument ConfigFile { get; set; }
    
  }


}
