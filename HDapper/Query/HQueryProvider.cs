using Dapper;
using HDapper.Translate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Query
{
	public class HQueryProvider : IQueryProvider
	{
		private IDbConnection _conn = new SqlConnection("Data Source=192.168.1.2;Initial Catalog=CJZK.Product;User Id=sa;Password=sa;");
		public IQueryable CreateQuery(Expression expression)
		{
			Type et = expression.Type;

			return (IQueryable)Activator.CreateInstance(typeof(HQuery<>).MakeGenericType(et), new object[] { this, expression });
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new HQuery<TElement>(this, expression);
		}

		/// <summary>
		/// 执行入口，解析表达式生成SQL语句，并返回SQL数据源数据
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public object Execute(Expression expression)
		{
			var translator = new SqlTranslate();
			var trans = translator.Translate(expression);
			var elist= SqlMapper.Query(_conn, trans.Item1, trans.Item2);
			return elist;
		}

		/// <summary>
		/// 执行入口，解析表达式生成SQL语句，并返回SQL数据源一行数据
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public TResult Execute<TResult>(Expression expression)
		{
			var translator = new SqlTranslate();
			var trans = translator.Translate(expression);

			return SqlMapper.Query<TResult>(_conn, trans.Item1, trans.Item2).FirstOrDefault();

			//return default(TResult);
			//return (TResult)this.Execute(expression);
		}
	}
}
