using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Extensions
{
	public static class AttributeExtension
	{
		public static string GetTableName(this Type type)
		{
			var attr = type.GetCustomAttribute(typeof(Attributes.TableAttribute), true) as Attributes.TableAttribute;
			return attr != null ? attr.TableName : type.Name;
		}

		public static string GetColumnName(this Type type,string name)
		{
			var property= type.GetProperty(name);
			if (property == null)
			{
				return name;
			}
			var attr = property.GetCustomAttribute(typeof(Attributes.ColumnAttribute), true) as Attributes.ColumnAttribute;
			return attr != null ? attr.ColumnName: name;
		}
	}
}
