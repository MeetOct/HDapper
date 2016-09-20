﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class TableAttribute : Attribute
	{
		private string _tableName = string.Empty;

		public TableAttribute(string tableName)
		{
			this._tableName = tableName;
		}

		public string TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}
	}
}
