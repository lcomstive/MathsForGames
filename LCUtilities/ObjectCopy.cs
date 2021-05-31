using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LCUtils
{
	public static class ObjectCopy
	{
		/// <summary>
		/// Copies one object's field and writable properties to another.
		/// </summary>
		/// <param name="args">Arguments of the object's constructor</param>
		public static T Copy<T>(this T original, params object[] args)
		{
			Type t = typeof(T);
			T instance = (T)Activator.CreateInstance(t, args);

			if (original == null)
				throw new Exception("NULL ORIGINAL"); // return instance;

			// Copy properties
			foreach (PropertyInfo property in t.GetProperties())
				if (property.CanWrite)
					property.SetValue(instance, property.GetValue(original, null), null);

			// Copy fields
			foreach (FieldInfo field in t.GetFields())
				field.SetValue(instance, field.GetValue(original));

			return instance;
		}
	}
}
