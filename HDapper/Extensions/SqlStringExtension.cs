using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Extensions
{
	public class SqlStringExtension
	{
		private StringBuilder sbSql = new StringBuilder();
		private StringBuilder sbOrderBy = new StringBuilder();
		private bool SqlOn = true;

		public void AppendSql(string value)
		{
			if (SqlOn)
			{
				sbSql.Append(value);
			}
			else
			{
				sbOrderBy.Append(value);
			}
		}

		public void AppendFormat(string format, object arg0)
		{
			if (SqlOn)
			{
				sbSql.AppendFormat(format, arg0);
			}
			else
			{
				sbOrderBy.AppendFormat(format, arg0);
			}
		}

		public void AppendFormat(string format, object arg0, object arg1)
		{
			if (SqlOn)
			{
				sbSql.AppendFormat(format, arg0, arg1);
			}
			else
			{
				sbOrderBy.AppendFormat(format, arg0, arg1);
			}
		}

		public void AppendFormat(string format, params object[] args)
		{
			if (SqlOn)
			{
				sbSql.AppendFormat(format, args);
			}
			else
			{
				sbOrderBy.AppendFormat(format, args);
			}
		}

		public void SwitchToOrderBy()
		{
			SqlOn = false;
		}

		public override string ToString()
		{
			return sbSql.ToString();
		}
	}
}
