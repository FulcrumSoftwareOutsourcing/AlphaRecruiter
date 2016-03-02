using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Metadata
{
  public class CxNonInheritablePropertyRegistry
  {
    //-------------------------------------------------------------------------
    private static Dictionary<Type, List<string>> m_TypeToPropertyMap;
    //-------------------------------------------------------------------------
    protected static Dictionary<Type, List<string>> TypeToPropertyMap
    {
      get { return m_TypeToPropertyMap; }
      set { m_TypeToPropertyMap = value; }
    }
    //-------------------------------------------------------------------------
    static CxNonInheritablePropertyRegistry()
    {
      TypeToPropertyMap = new Dictionary<Type, List<string>>();

    }
    //-------------------------------------------------------------------------
    public static void RegisterProperty(Type type, string property)
    {
      if (!IsPropertyRegistered(type, property))
      {
        if (!TypeToPropertyMap.ContainsKey(type))
          TypeToPropertyMap[type] = new List<string>();
        TypeToPropertyMap[type].Add(property);
      }
    }
    //-------------------------------------------------------------------------
    public static bool IsPropertyRegistered(Type type, string property)
    {
      if (TypeToPropertyMap.ContainsKey(type) && TypeToPropertyMap[type].Contains(property))
        return true;
      return false;
    }
    //-------------------------------------------------------------------------
    public static void UnregisterProperty(Type type, string property)
    {
      if (IsPropertyRegistered(type, property))
      {
        TypeToPropertyMap[type].Remove(property);
      }
    }
    //-------------------------------------------------------------------------
    public static string[] GetProperties(Type type)
    {
      if (TypeToPropertyMap.ContainsKey(type))
        return TypeToPropertyMap[type].ToArray();
      return new string[0];
    }
    //-------------------------------------------------------------------------
  }
}
