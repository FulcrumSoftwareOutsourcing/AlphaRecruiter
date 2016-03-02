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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Type related utility methods.
  /// </summary>
  public class CxType
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Type classification lists
    /// </summary>
    static protected Type[] m_NumericTypes = 
    {
      typeof(Byte), typeof(SByte),
      typeof(Int16), typeof(Int32), typeof(Int64),
      typeof(UInt16), typeof(UInt32), typeof(UInt64),
      typeof(Single), typeof(Double), typeof(Decimal), typeof(float),
      typeof(Byte?), typeof(SByte?),
      typeof(Int16?), typeof(Int32?), typeof(Int64?),
      typeof(UInt16?), typeof(UInt32?), typeof(UInt64?),
      typeof(Single?), typeof(Double?), typeof(Decimal?), typeof(float?),
    };
    //-------------------------------------------------------------------------
    static protected Type[] m_IntegerTypes = 
    {
      typeof(Byte), typeof(SByte),
      typeof(Int16), typeof(Int32), typeof(Int64),
      typeof(UInt16), typeof(UInt32), typeof(UInt64),
      typeof(Byte?), typeof(SByte?),
      typeof(Int16?), typeof(Int32?), typeof(Int64?),
      typeof(UInt16?), typeof(UInt32?), typeof(UInt64?)
    };
    //-------------------------------------------------------------------------
    static protected Type[] m_StringTypes = 
    {
      typeof(String), typeof(Char), typeof(Char?)
    };
    //-------------------------------------------------------------------------
    static protected Type[] m_BooleanTypes = 
    {
      typeof(Boolean), typeof(Boolean?)
    };
    //-------------------------------------------------------------------------
    static protected Type[] m_DateTimeTypes = 
    {
      typeof(DateTime), typeof(DateTime?)
    };
    //-------------------------------------------------------------------------
    static protected Type[] m_GuidTypes = 
    {
      typeof(Guid), typeof(Guid?)
    };
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is one of numeric types.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is one of numeric types</returns>
    public static bool IsNumber(Type type)
    {
      return Array.IndexOf(m_NumericTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is one of integer types.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is one of integer types</returns>
    public static bool IsInteger(Type type)
    {
      return Array.IndexOf(m_IntegerTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is one of string types (string or char).
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is one of string types</returns>
    public static bool IsString(Type type)
    {
      return Array.IndexOf(m_StringTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is the datetime type.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is the datetime type</returns>
    public static bool IsDateTime(Type type)
    {
      return Array.IndexOf(m_DateTimeTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is the boolean type.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is the boolean type</returns>
    public static bool IsBoolean(Type type)
    {
      return Array.IndexOf(m_BooleanTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is the GUID type.
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is the GUID type</returns>
    public static bool IsGuid(Type type)
    {
      return Array.IndexOf(m_GuidTypes, type) >= 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given type is the binary type (byte array).
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true if the given type is the binary type</returns>
    public static bool IsBinary(Type type)
    {
      return type == typeof(Byte[]);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns type of the class using its name.
    /// </summary>
    /// <param name="assembly">assembly containing class</param>
    /// <param name="className">name of the class</param>
    /// <returns>type of the class or null if type is not found</returns>
    static public Type GetTypeByName(Assembly assembly, string className)
    {
      if (assembly != null && CxUtils.NotEmpty(className))
      {
        try
        {
          return assembly.GetType(className);
        }
        catch
        {
          return null;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns type of the class using its name.
    /// Searches all loaded assemblies.
    /// </summary>
    /// <param name="className">name of class</param>
    /// <returns>type of the class or null if type is not found</returns>
    static public Type GetTypeByName(string className)
    {
      Type type = GetTypeByName(Assembly.GetCallingAssembly(), className);
      if (type == null)
      {
        type = GetTypeByName(Assembly.GetEntryAssembly(), className);
      }
      if (type == null)
      {
        type = GetTypeByName(Assembly.GetExecutingAssembly(), className);
      }
      if (type == null)
      {
        // Check AppDomain.GetAssemblies()
        Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < arr.Length; i++)
        {
          type = GetTypeByName(arr[i], className);
          if (type != null)
          {
            break;
          }
        }
      }
      return type;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns type reference for className from the specified assembly.
    /// </summary>
    /// <param name="assemblyFolder">folder where assembly located</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="className">name of the class</param>
    /// <returns>type reference for className from the specified assembly</returns>
    static public Type GetTypeByName(string assemblyFolder, string assemblyName, string className)
    {
      Assembly assembly = GetAssembly(assemblyFolder, assemblyName);
      return GetTypeByName(assembly, className);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// </summary>
    /// <param name="className">name of the class to create</param>
    /// <param name="types">constructor parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(string className, Type[] types, object[] parameters)
    {
      Type type = GetTypeByName(className);
      if (type == null)
      {
        throw new ExClassNotFoundException(className);
      }
      return CreateInstance(type, types, parameters);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// </summary>
    /// <param name="assemblyFolder">folder where assembly located</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="className">name of the class to create</param>
    /// <param name="types">list of parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <param name="raiseException">if true exception will be raised when class could not be created, 
    /// if false null will be returned</param>
    /// <returns>created object or null</returns>
    static public Object CreateInstance(string assemblyFolder,
      string assemblyName,
      string className,
      Type[] types,
      object[] parameters,
      bool raiseException)
    {
      Assembly assembly = GetAssembly(assemblyFolder, assemblyName, raiseException);
      if (assembly != null)
      {
        Type type = assembly.GetType(className);
        if (type == null)
        {
          if (raiseException)
          {
            throw new ExClassNotFoundException(className);
          }
          else
          {
            return null;
          }
        }
        return CreateInstance(type, types, parameters, raiseException);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="assemblyFolder">folder where assembly located</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="className">name of the class to create</param>
    /// <param name="types">list of parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(string assemblyFolder,
      string assemblyName,
      string className,
      Type[] types,
      object[] parameters)
    {
      return CreateInstance(assemblyFolder, assemblyName, className, types, parameters, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="className">name of the class to create</param>
    /// <param name="types">list of parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(string assemblyName,
      string className,
      Type[] types,
      object[] parameters)
    {
      Assembly assembly = GetAssembly(assemblyName);
      Type type = assembly.GetType(className);
      if (type == null)
      {
        throw new ExClassNotFoundException(className);
      }
      return CreateInstance(type, types, parameters);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// </summary>
    /// <param name="type">type of instance to create</param>
    /// <param name="types">list of parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <param name="raiseException">if true exception will be raised when class could not be created. 
    /// <returns>created object or null</returns>
    static public Object CreateInstance(Type type,
      Type[] types,
      object[] parameters,
      bool raiseException)
    {
      ConstructorInfo constructor = type.GetConstructor(types);
      if (constructor == null)
      {
        if (raiseException)
        {
          throw new ExConstructorNotFoundException(type, types);
        }
        else
        {
          return null;
        }
      }
      Object obj = constructor.Invoke(parameters);
      return obj;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="type">type of instance to create</param>
    /// <param name="types">list of parameter types</param>
    /// <param name="parameters">constructor parameters</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(Type type, Type[] types, object[] parameters)
    {
      return CreateInstance(type, types, parameters, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type using constructor that matches with parameters.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="className">name of the class to create</param>
    /// <param name="parameters">constructor parameters</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(string className, params object[] parameters)
    {
      Type[] types = new Type[parameters.Length];
      for (int i = 0; i < types.Length; i++)
      {
        if (parameters[i] == null)
        {
          throw new ArgumentNullException("Create Instance Parameter");
        }
        types[i] = parameters[i].GetType();
      }
      return CreateInstance(className, types, parameters);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="className">class name to create instance of</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(string className)
    {
      Type type = GetTypeByName(className);
      if (type == null)
      {
        throw new ExClassNotFoundException(className);
      }
      return CreateInstance(type);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type.
    /// </summary>
    /// <param name="assemblyFolder">folder where assembly located</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="className">name of the class to create</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(
      string assemblyFolder,
      string assemblyName,
      string className)
    {
      Assembly assembly = GetAssembly(assemblyFolder, assemblyName);
      Type type = assembly.GetType(className);
      if (type == null)
      {
        throw new ExClassNotFoundException(className);
      }
      return CreateInstance(type);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the given type.
    /// If it is impossible to create such instance raises an exception.
    /// </summary>
    /// <param name="type">type to create instance of</param>
    /// <returns>created object</returns>
    static public Object CreateInstance(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException("Create Instance Type Parameter");
      }
      return type.Assembly.CreateInstance(type.FullName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly from file.
    /// </summary>
    /// <param name="assemblyName">name of the file with assembly</param>
    /// <returns>loaded assembly</returns>
    static public Assembly GetAssembly(string assemblyName)
    {
      return GetAssembly(null, assemblyName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly from file.
    /// </summary>
    /// <param name="subFolderName">name of the folder with assembly</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="raiseException">if true exception will be raised when assembly could not be loaded.
    /// If false returns null.</param>
    /// <returns>loaded assembly</returns>
    static public Assembly GetAssembly(string subFolderName, string assemblyName, bool raiseException)
    {
      return GetAssembly(
        CxPath.GetApplicationBinaryFolder(),
        subFolderName,
        assemblyName,
        raiseException);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly from file.
    /// </summary>
    /// <param name="rootFolderName">name of the root folder with the assembly</param>
    /// <param name="subFolderName">name of the sub-folder with assembly</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <param name="raiseException">if true exception will be raised when assembly could not be loaded.
    /// If false returns null.</param>
    /// <returns>loaded assembly</returns>
    static public Assembly GetAssembly(string rootFolderName, string subFolderName, string assemblyName, bool raiseException)
    {
      string path = Path.Combine(rootFolderName, CxUtils.Nvl(subFolderName));
      string fileName = Path.Combine(path, CxUtils.Nvl(assemblyName));
      Assembly assembly = Assembly.LoadFrom(fileName);
      if (assembly == null && raiseException)
      {
        throw new ExAssemblyNotFoundException(fileName);
      }
      return assembly;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly from file.
    /// </summary>
    /// <param name="subFolderName">name of the folder with assembly</param>
    /// <param name="assemblyName">name of the assembly</param>
    /// <returns>loaded assembly</returns>
    static public Assembly GetAssembly(string subFolderName, string assemblyName)
    {
      return GetAssembly(subFolderName, assemblyName, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds object method with specified name and parameter types.
    /// </summary>
    /// <param name="obj">obejct to find method</param>param>
    /// <param name="methodName">name of the method to find</param>
    /// <param name="paramTypes">array of parameter types</param>
    /// <returns>object method with specified name and parameter types
    /// or null if not found</returns>
    static public MethodInfo FindMethod(
      object obj,
      string methodName,
      Type[] paramTypes)
    {
      Type type = obj.GetType();
      MethodInfo method = null;
      while (method == null && type != null)
      {
        method = type.GetMethod(
          methodName,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          paramTypes,
          null);
        type = type.BaseType;
      }
      return method;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls method of the object using reflection.
    /// </summary>
    /// <param name="obj">object to call method of</param>
    /// <param name="methodName">method name</param>
    /// <param name="types">array with parameter types</param>
    /// <param name="parameters">array with parameter values</param>
    /// <returns>value returned by method</returns>
    static public object CallMethod(
      object obj,
      string methodName,
      Type[] types,
      object[] parameters)
    {
      if (obj == null)
      {
        throw new ArgumentNullException("CallMethod obj parameter");
      }
      MethodInfo methodInfo = FindMethod(obj, methodName, types);
      if (methodInfo == null)
      {
        throw new ExMethodNotFoundException(obj, methodName);
      }
      object result = methodInfo.Invoke(obj, parameters);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds field in the object and returns its value.
    /// </summary>
    /// <param name="obj">object to find field in</param>
    /// <returns>field value</returns>
    static public object GetFieldValue(object obj, string fieldName)
    {
      FieldInfo fieldInfo = null;
      Type type = obj.GetType();
      while (fieldInfo == null && type != null)
      {
        fieldInfo = type.GetField(
          fieldName, 
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        type = type.BaseType;
      }
      if (fieldInfo == null)
      {
        throw new ExFieldNotFoundException(obj, fieldName);
      }
      object result = fieldInfo.GetValue(obj);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if descendant is inherited from the ancestor or the types are equal.
    /// </summary>
    /// <param name="descendant">descendant to check</param>
    /// <param name="ancestor">ancestor to check inheritance from</param>
    /// <returns>true if descendant is inherited from the ancestor or the types are equal</returns>
    static public bool IsInheritedFrom(Type descendant, Type ancestor)
    {
      if (descendant != null && ancestor != null)
      {
        return ancestor.IsAssignableFrom(descendant);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ancestor of the given type where field with the given name is
    /// actually defined.
    /// </summary>
    /// <param name="type">current type that has given field (inherited or declared)</param>
    /// <param name="fieldName">field name to find</param>
    /// <returns>ancestor of the given type where field with the given name is defined</returns>
    static public Type GetTypeWhereFieldDefined(Type type, string fieldName)
    {
      if (type != null && !String.IsNullOrEmpty(fieldName))
      {
        Type parentType = type;
        FieldInfo fieldInfo = null;
        while (parentType != null && fieldInfo == null)
        {
          fieldInfo = parentType.GetField(fieldName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          if (fieldInfo == null)
          {
            parentType = parentType.BaseType;
          }
        }
        return parentType;
      }
      return null;
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Class not found exception.
  /// </summary>
  public class ExClassNotFoundException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="className">name of the non-found class</param>
    public ExClassNotFoundException(string className) : 
      base(String.Format("Class {0} not found.", className))
    {
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Assembly not found exception.
  /// </summary>
  public class ExAssemblyNotFoundException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assemblyName">name of the non-found assembly</param>
    public ExAssemblyNotFoundException(string assemblyName) :
      base(String.Format("Assembly {0} not found.", assemblyName))
    {
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Constructor not found exception.
  /// </summary>
  public class ExConstructorNotFoundException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">type with non-found constructor</param>
    /// <param name="paramTypes">constructor parameter types</param>
    public ExConstructorNotFoundException(Type type, Type[] paramTypes) :
      base(ComposeMessage(type, paramTypes))
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes error message.
    /// </summary>
    /// <param name="type">type with non-found constructor</param>
    /// <param name="paramTypes">constructor parameter types</param>
    /// <returns>exception error message</returns>
    static protected string ComposeMessage(Type type, Type[] paramTypes)
    {
      string typeName = type != null ? type.Name : "";
      string paramList = "";
      if (paramTypes != null)
      {
        for (int i = 0; i < paramTypes.Length; i++)
        {
          if (paramTypes[i] != null)
          {
            paramList += (i == 0 ? "" : ", ") + paramTypes[i].Name;
          }
        }
      }
      return String.Format("Constructor '{0}({1})' not found.", typeName, paramList);
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Method not found exception.
  /// </summary>
  public class ExMethodNotFoundException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="obj">object instance</param>
    /// <param name="methodName">name of the non-found method</param>
    public ExMethodNotFoundException(object obj, string methodName) :
      base(String.Format("Method <{0}> not found for the type <{1}>.",
            methodName,
            obj != null ? obj.GetType().Name : ""))
    {
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Field not found exception.
  /// </summary>
  public class ExFieldNotFoundException : ApplicationException
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="obj">object instance</param>
    /// <param name="fieldName">name of the non-found field</param>
    public ExFieldNotFoundException(object obj, string fieldName)
      :
      base(String.Format("Field <{0}> not found for the type <{1}>.",
            fieldName,
            obj != null ? obj.GetType().Name : ""))
    {
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}