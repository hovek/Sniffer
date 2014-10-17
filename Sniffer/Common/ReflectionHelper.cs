using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sniffer.Common
{
	public class ReflectionHelper
	{
		public static List<PropertyInfo> GetAllProperties(Type type, bool inherit)
		{
			List<PropertyInfo> propertyInfos = null;

			if (inherit && type.BaseType != null)
			{
				propertyInfos = GetAllProperties(type.BaseType, inherit);
			}
			else
			{
				propertyInfos = new List<PropertyInfo>();
			}

			propertyInfos.AddRange(type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

			return propertyInfos;
		}

		public static List<FieldInfo> GetAllFields(Type type, bool inherit)
		{
			List<FieldInfo> fieldInfos = null;

			if (inherit && type.BaseType != null)
			{
				fieldInfos = GetAllFields(type.BaseType, inherit);
			}
			else
			{
				fieldInfos = new List<FieldInfo>();
			}

			fieldInfos.AddRange(type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

			return fieldInfos;
		}

		public static List<MemberInfo> GetAllFieldsAndProperties(Type type, bool inherit)
		{
			return GetAllFieldsAndProperties(type, inherit, false);
		}

		public static List<MemberInfo> GetAllFieldsAndProperties(Type type, bool inherit, bool onlyReadWriteProperties)
		{
			List<MemberInfo> memberInfos = null;

			if (inherit && type.BaseType != null)
			{
				memberInfos = GetAllFieldsAndProperties(type.BaseType, inherit, onlyReadWriteProperties);
			}
			else
			{
				memberInfos = new List<MemberInfo>();
			}

			memberInfos.AddRange(type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
			PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (onlyReadWriteProperties)
			{
				foreach (PropertyInfo pi in propertyInfos)
				{
					if (pi.CanRead && pi.CanWrite)
					{
						memberInfos.Add(pi);
					}
				}
			}
			else
			{
				memberInfos.AddRange(propertyInfos);
			}

			return memberInfos;
		}

		public static void SetValue(MemberInfo memberInfo, object obj, object value)
		{
			if (memberInfo.MemberType == MemberTypes.Property)
			{
				((PropertyInfo)memberInfo).SetValue(obj, value, null);
			}
			else if (memberInfo.MemberType == MemberTypes.Field)
			{
				((FieldInfo)memberInfo).SetValue(obj, value);
			}
			else
			{
				throw new NotImplementedException("MemberInfo not implemented.");
			}
		}

		public static object GetValue(MemberInfo memberInfo, object obj)
		{
			if (memberInfo.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)memberInfo).GetValue(obj, null);
			}
			else if (memberInfo.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)memberInfo).GetValue(obj);
			}

			throw new NotImplementedException("MemberInfo not implemented.");
		}

		public static Type GetType(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)memberInfo).PropertyType;
			}
			else if (memberInfo.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)memberInfo).FieldType;
			}

			throw new NotImplementedException("MemberInfo not implemented.");
		}

		public static object CreateNewInstance(Type type, params object[] parametersForConstructor)
		{
			Type[] types = new Type[parametersForConstructor.Length];
			for (int i = 0; i < parametersForConstructor.Length; i++)
			{
				types[i] = parametersForConstructor[i].GetType();
			}
			return CreateNewInstance(type, types, parametersForConstructor);
		}

		public static object CreateNewInstance(Type type, Type[] parametersTypes, object[] parametersForConstructor)
		{
			return type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, parametersTypes, null).Invoke(parametersForConstructor);
		}

		public static List<AttributeType> GetAttributes<AttributeType>(Type objectType, bool inherit) where AttributeType : Attribute
		{
			List<AttributeType> atributes = new List<AttributeType>();
			atributes.AddRange((AttributeType[])objectType.GetCustomAttributes(typeof(AttributeType), false));
			if (inherit && objectType.BaseType != null)
			{
				atributes.AddRange(GetAttributes<AttributeType>(objectType.BaseType, inherit));
			}

			return atributes;
		}
	}
}
