using System;

namespace Framework.Db.LightWeight
{
  public class CxLwColumn
  {
    public string Name { get; set; }
    public Type DataType { get; set; }

    public override string ToString()
    {
      return Name ?? base.ToString();
    }
  }
}
