using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDapper.Query;
using HDapper.Extensions;
using System;

namespace HDapper.Test
{
	[TestClass]
	public class Test_SqlTranslate
	{
		[TestMethod]
		public void TestMethod1()
		{
			var user = new Test_Entity_User() { real_name = "hance", id = 24 };

			var result = HDBContext.User
									.Where(u => u.real_name == "321" && u.create_date > DateTime.Now.AddMonths(-1))
									//.OrderBy(u => u.id)
									//.ThenByDesc(u => u.name)
									.Paged(new PageExtension() { PageIndex = 1, PageSize = 2, OrderBy = "ORDER BY id DESC" });
			//.QueryFirst();

		}
	}
}
