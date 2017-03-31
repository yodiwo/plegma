using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string Name_Portable(this Type type)
        {
#if NETFX
            if (type.IsGenericType)
                return type.FullName.Split('`')[0] + "`" + type.GetGenericArguments().Count() + "[" + string.Join(", ", type.GetGenericArguments().Select(x => "[" + Name_Portable(x) + "]").ToArray()) + "]";
#elif UNIVERSAL
            if (type.GetTypeInfo().IsGenericType)
                return type.FullName.Split('`')[0] + "`" + type.GenericTypeArguments.Length + "[" + string.Join(", ", type.GenericTypeArguments.Select(x => "[" + Name_Portable(x) + "]").ToArray()) + "]";
#endif
            else
                return type.FullName;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string AssemblyQualifiedName_Portable(this Type type)
        {
#if NETFX
            if (type.IsGenericType)
                return type.FullName.Split('`')[0] + "`" + type.GetGenericArguments().Count() + "[" + string.Join(", ", type.GetGenericArguments().Select(x => "[" + AssemblyQualifiedName_Portable(x) + "]").ToArray()) + "] , " + type.Assembly.GetName().Name;
#elif UNIVERSAL
            if (type.GetTypeInfo().IsGenericType)
                return type.FullName.Split('`')[0] + "`" + type.GenericTypeArguments.Length + "[" + string.Join(", ", type.GenericTypeArguments.Select(x => "[" + AssemblyQualifiedName_Portable(x) + "]").ToArray()) + "] , " + type.GetTypeInfo().Assembly.GetName().Name;
#endif
            else
#if NETFX
                return type.FullName + ", " + type.Assembly.GetName().Name;
#elif UNIVERSAL
                return type.FullName + ", " + type.GetTypeInfo().Assembly.GetName().Name;
#endif
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string GetFriendlyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(uint))
                return "uint";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";
#if NETFX
            else if (type.IsGenericType)
                return (type.FullName.LeftOf('`') + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x))) + ">").Replace("+", ".");
#elif UNIVERSAL
            else if (type.GetTypeInfo().IsGenericType)
                return (type.FullName.LeftOf('`') + "<" + string.Join(", ", type.GetTypeInfo().GenericTypeArguments.Select(x => GetFriendlyName(x))) + ">").Replace("+", ".");
#endif
            else
                return type.FullName.Replace("+", ".");
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static string DeFriendlifyName(string type)
        {
            if (type == "int")
                return typeof(int).FullName;
            else if (type == "short")
                return typeof(short).FullName;
            else if (type == "byte")
                return typeof(byte).FullName;
            else if (type == "bool")
                return typeof(bool).FullName;
            else if (type == "long")
                return typeof(long).FullName;
            else if (type == "float")
                return typeof(float).FullName;
            else if (type == "double")
                return typeof(double).FullName;
            else if (type == "decimal")
                return typeof(decimal).FullName;
            else if (type == "string")
                return typeof(string).FullName;
            else
            {
                string retType = "";

                //processs generics
                if (type.Contains("<"))
                {
                    var lefovers = type.RightOf("<");
                    var generics = new List<string>();
                    int i = 0;
                    int start = 0;
                    for (int n = 0; n < lefovers.Length; n++)
                    {
                        var c = lefovers[n];
                        if (c == '<')
                            i++;
                        else if (c == '>')
                            i--;
                        else if (i == 0 && c == ',')
                        {
                            generics.Add(lefovers.Substring(start, n - start));
                            start = n + 1;
                        }
                    }
                    if (i == -1)
                        generics.Add(lefovers.Substring(start, lefovers.Length - start - 1));
                    //process found generics recursively
                    retType = type.LeftOf("<") + "`" + generics.Count + "[" + string.Join(", ", generics.Select(t => "[" + DeFriendlifyName(t))) + "]" + "]";
                }
                else
                    retType = type;

                //try to find it retType is valid
                if (TypeCache.GetType(retType, DeFriendlify: false) != null)
                    return retType;

                //break type appart and find what are nested classes
                var elem = retType.Split('.');
                string finalType = elem[0];
                for (int n = 1; n < elem.Length; n++)
                    finalType += (TypeCache.GetType(finalType) != null ? "+" : ".") + elem[n];
                return finalType;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------


        public static bool IsList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type) ||
#if NETFX
                    type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
#elif UNIVERSAL
                    type.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
#else
#error unkown platform
#endif
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool IsDictionary(this Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type) ||
#if NETFX
                   type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
#elif UNIVERSAL
                   type.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
#else
#error unkown platform
#endif
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool IsCollection(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type) ||
#if NETFX
                   type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
#elif UNIVERSAL
                   type.GetInterfaces().Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
#else
#error unkown platform
#endif
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool IsInteger(this Type type)
        {
            return type == typeof(sbyte) || type == typeof(byte) ||
                   type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) ||
                   type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64);
        }

        public static bool IsDecimal(this Type type)
        {
            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);

        }
        public static bool IsNumber(this Type type)
        {
            return type.IsInteger() || type.IsDecimal();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
