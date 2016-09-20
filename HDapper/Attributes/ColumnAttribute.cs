using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class ColumnAttribute: Attribute
	{
		private string _columnName = string.Empty;

		public ColumnAttribute(string columnName)
		{
			this._columnName = columnName;
		}

		public string ColumnName
		{
			get { return _columnName; }
			set { _columnName = value; }
		}
	}
}
