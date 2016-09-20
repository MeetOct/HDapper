using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Query
{
	public class HQuery<T> : IQueryable<T>
	{
		HQueryProvider provider;
		Expression exp;

		public HQuery(HQueryProvider pr)
		{
			provider = pr;
			exp = Expression.Constant(pr);
		}

		public HQuery(HQueryProvider pr, Expression ex)
		{
			provider = pr;
			exp = ex;
		}

		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		Expression IQueryable.Expression
		{
			get
			{
				return exp;
			}
		}

		IQueryProvider IQueryable.Provider
		{
			get
			{
				return provider;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.provider.Execute(this.exp)).GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)this.provider.Execute(this.exp)).GetEnumerator();
		}
	}

	public class HOrderedQueryable<T> : HQuery<T>, IOrderedQueryable
	{
		HQueryProvider provider;
		Expression exp;
		public HOrderedQueryable(HQueryProvider pr) : base(pr)
		{
			provider = pr;
			exp = Expression.Constant(pr);
		}

		public HOrderedQueryable(HQueryProvider pr, Expression ex) : base(pr, ex)
		{
			provider = pr;
			exp = ex;
		}
	}
}
