using System;
using System.Reflection;

namespace Foundation.Architecture
{
    public static class ReflectionExt
    {
        public static FieldInfo[] GetFields(this Type type, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetFields(bindingAttr);
        }

        public static PropertyInfo[] GetProperties(this Type type, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetProperties(bindingAttr);
        }

        public static MethodInfo[] GetMethods(this Type type, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetMethods(bindingAttr);
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GetGenericArguments();
        }
        public static EventInfo GetEvent(this Type type, string name)
        {
            return type.GetTypeInfo().GetEvent(name);
        }
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }
        public static MethodInfo CreateDelegate(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }
    }
}