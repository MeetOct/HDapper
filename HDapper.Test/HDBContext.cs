using HDapper.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Test
{
	public static class HDBContext
	{
		public static IQueryable<Test_Entity_User> User
		{
			get { return new List<Test_Entity_User>().AsQueryable(); }
		}
	}
}
