using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Sniffer.Common
{
	[Serializable]
	public class ReflectionCopy<T> : List<ReflectionCopyItem>
	{
		public static ReflectionCopy<T> Copy(T obj)
		{
			return Copy(obj, new List<Type>());
		}

		public static ReflectionCopy<T> Copy(T obj, List<Type> typesToExcludeFromCopy)
		{
			return copy<T>(obj, new List<object>(), typesToExcludeFromCopy, true);
		}

		public static ReflectionCopy<T> Copy(T obj, bool deepCopy)
		{
			return copy<T>(obj, new List<object>(), new List<Type>(), deepCopy);
		}

		private static ReflectionCopy<ObjectType> copy<ObjectType>(ObjectType obj, List<object> copiedObjects, List<Type> typesToExcludeFromCopy, bool deepCopy)
		{
			ReflectionCopy<ObjectType> reflectionCopy = new ReflectionCopy<ObjectType>();

			List<FieldInfo> fieldInfos = getFields(obj.GetType());

			if (fieldInfos.Count == 0 && obj is IEnumerable)
			{
				IEnumerator items = ((IEnumerable)obj).GetEnumerator();

				while (items.MoveNext())
				{
					object value = items.Current;

					ReflectionCopy<object> valueFields = copy(value, copiedObjects, typesToExcludeFromCopy, deepCopy);

					reflectionCopy.Add(new ReflectionCopyItem(null, value, valueFields));
				}
			}
			else
			{
				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					object value = fieldInfo.GetValue(obj);

					ReflectionCopy<object> valueFields = copy(value, copiedObjects, typesToExcludeFromCopy, deepCopy);

					reflectionCopy.Add(new ReflectionCopyItem(fieldInfo, value, valueFields));
				}
			}

			return reflectionCopy;
		}

		private static ReflectionCopy<object> copy(object obj, List<object> copiedObjects, List<Type> typesToExcludeFromCopy, bool deepCopy)
		{
			ReflectionCopy<object> valueFields = null;

			if (deepCopy && obj != null)
			{
				if (!obj.GetType().IsPrimitive
					&& !contains(typesToExcludeFromCopy, obj)
					&& !contains(copiedObjects, obj)
				)
				{
					copiedObjects.Add(obj);
					valueFields = copy<object>(obj, copiedObjects, typesToExcludeFromCopy, deepCopy);
				}
			}

			return valueFields;
		}

		public static ReflectionCopy<T> CopyFieldsAndProperties(T obj)
		{
			return copyFieldsAndProperties<T>(obj);
		}

		private static ReflectionCopy<ObjectType> copyFieldsAndProperties<ObjectType>(ObjectType obj)
		{
			ReflectionCopy<ObjectType> reflectionCopy = new ReflectionCopy<ObjectType>();
			List<MemberInfo> memberInfos = ReflectionHelper.GetAllFieldsAndProperties(obj.GetType(), true, true);
			foreach (MemberInfo memberInfo in memberInfos)
			{
				object value = ReflectionHelper.GetValue(memberInfo, obj);
				reflectionCopy.Add(new ReflectionCopyItem(memberInfo, value, null));
			}

			return reflectionCopy;
		}

		private static bool contains(List<Type> objs, object obj)
		{
			for (int i = 0; i < objs.Count; i++)
			{
				if (objs[i].IsInstanceOfType(obj))
				{
					return true;
				}
			}

			return false;
		}

		private static bool contains(List<object> objs, object obj)
		{
			for (int i = 0; i < objs.Count; i++)
			{
				try
				{
					if (object.Equals(obj, objs[i]))
					{
						return true;
					}
				}
				catch
				{
					if (obj == objs[i])
					{
						return true;
					}
				}
			}

			return false;
		}

		public T CreateNew()
		{
			T newInstance = (T)ReflectionHelper.CreateNewInstance(typeof(T));
			this.Paste(newInstance);
			return newInstance;
		}

		public void Paste(T obj)
		{
			ReflectionCopy<T>.paste<T>(obj, this);
		}

		private static void paste<ObjectType>(ObjectType obj, ReflectionCopy<ObjectType> objFields)
		{
			foreach (ReflectionCopyItem item in objFields)
			{
				if (item.ValueFields != null)
				{
					paste<object>(item.Value, item.ValueFields);
				}

				if (item.MemberInfo != null)
				{
					ReflectionHelper.SetValue(item.MemberInfo, obj, item.Value);
				}
			}
		}

		private static List<FieldInfo> getFields(Type type)
		{
			List<FieldInfo> fieldInfos = new List<FieldInfo>(type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));

			for (int i = 0; i < fieldInfos.Count; i++)
			{
				if (fieldInfos[i].IsLiteral || fieldInfos[i].ReflectedType.IsSealed)
				{
					fieldInfos.RemoveAt(i);
					i--;
				}
			}

			Type baseType = type.BaseType;
			if (baseType != null)
			{
				fieldInfos.AddRange(getFields(baseType));
			}

			return fieldInfos;
		}
	}

	[Serializable]
	public class ReflectionCopyItem
	{
		public MemberInfo MemberInfo;
		public object Value;
		public ReflectionCopy<object> ValueFields;

		public ReflectionCopyItem(MemberInfo memberInfo, object value, ReflectionCopy<object> valueFields)
		{
			MemberInfo = memberInfo;
			Value = value;
			ValueFields = valueFields;
		}
	}
}
