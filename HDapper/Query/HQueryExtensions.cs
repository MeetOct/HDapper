using HDapper.Extensions;
using HDapper.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Query
{
	public static class IQueryableExtensions
	{
		private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
		{
			return f.Method;
		}

		private static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
		{
			return f.Method;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source,
			Expression<Func<TSource, bool>> predicate)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(predicate) });
			return new HQueryProvider().CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 排序（顺序）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <returns></returns>
		public static IQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) });

			return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 排序（顺序）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <returns></returns>
		public static IQueryable<TSource> ThenBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) });

			return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 排序（倒序）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <returns></returns>
		public static IQueryable<TSource> OrderByDesc<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) });

			return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 排序（倒序）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <returns></returns>
		public static IQueryable<TSource> ThenByDesc<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) });

			return (IQueryable<TSource>)source.Provider.CreateQuery<TSource>(expression);
		}

		///// <summary>
		///// 待处理
		///// </summary>
		///// <typeparam name="TSource"></typeparam>
		///// <param name="source"></param>
		///// <param name="predicate"></param>
		///// <returns></returns>
		//public static TSource QueryFirst<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		//{
		//	var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(predicate) });

		//	return source.Provider.Execute<TSource>(expression);
		//}

		/// <summary>
		/// 执行查询（TOP 1）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static TSource QueryFirst<TSource>(this IQueryable<TSource> source)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression });

			return source.Provider.Execute<TSource>(expression);
		}

		/// <summary>
		/// 执行查询（列表）
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<TSource> QueryList<TSource>(this IQueryable<TSource> source)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression });

			return (IEnumerable<TSource>)source.Provider.Execute(expression);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IQueryable<TSource> Skip<TSource>(this IQueryable<TSource> source,int count)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(count) });
			return new HQueryProvider().CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IQueryable<TSource> Take<TSource>(this IQueryable<TSource> source, int count)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(count) });
			return new HQueryProvider().CreateQuery<TSource>(expression);
		}

		/// <summary>
		/// 分页
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<TSource> Paged<TSource>(this IQueryable<TSource> source, PageExtension page)
		{
			var expression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(page) });
			return (IEnumerable<TSource>)(new HQueryProvider().CreateQuery<TSource>(expression)).GetEnumerator();
		}
	}
}
