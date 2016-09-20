using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Extensions
{
	public class PageExtension
	{
		public int PageSize { get; set; }

		public int PageIndex { get; set; }

		public string OrderBy { get; set; }
	}
}
