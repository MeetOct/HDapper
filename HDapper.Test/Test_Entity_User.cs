using HDapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Test
{
	[Table("[CJZK.Product].[dbo].[Auth_MemberInfo]")]
	public class Test_Entity_User
	{
		public string real_name { get; set; }
		public int id { get; set; }
		public DateTime create_date { get; set; }

	}
}
