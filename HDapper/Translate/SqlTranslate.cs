using Dapper;
using HDapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Translate
{
	public class SqlTranslate: ExpressionVisitor
	{
		StringBuilder sbSql = null;
		DynamicParameters param =null;
		int count = 0;
		bool hasWhere = false;
		bool isleft = true;

		public Tuple<string, DynamicParameters> Translate(Expression expr)
		{
			sbSql = new StringBuilder();
			param = new DynamicParameters();
			this.Visit(expr);
			return new Tuple<string, DynamicParameters>(sbSql.ToString(),param);
		}

		/// <summary>
		/// 访问方法
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.Name == "Where")
			{
				this.Visit(node.Arguments[0]);
				if (!hasWhere)
				{
					sbSql.Append(" WHERE 1=1 ");
					hasWhere = true;
				}
				LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
				this.Visit(lambda.Body);
				return node;
			}

			if (node.Method.Name == "OrderBy")
			{
				this.Visit(node.Arguments[0]);
				sbSql.Append(" ORDER BY ");
				LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
				this.Visit(lambda.Body);
				sbSql.Append(" ASC ");
				return node;
			}
			if (node.Method.Name == "ThenBy")
			{
				this.Visit(node.Arguments[0]);
				sbSql.Append(" , ");
				LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
				this.Visit(lambda.Body);
				sbSql.Append(" ASC ");
				return node;
			}
			if (node.Method.Name == "OrderByDesc")
			{
				this.Visit(node.Arguments[0]);
				LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
				sbSql.Append(" ORDER BY ");
				this.Visit(lambda.Body);
				sbSql.Append(" DESC ");
				return node;
			}
			if (node.Method.Name == "ThenByDesc")
			{
				this.Visit(node.Arguments[0]);
				LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
				sbSql.Append(" , ");
				this.Visit(lambda.Body);
				sbSql.Append(" DESC ");
				return node;
			}

			if (node.Method.Name == "QueryFirst")
			{
				sbSql.Append(" SELECT TOP 1 * FROM ");
				this.Visit(node.Arguments[0]);
				if (node.Arguments.Count() > 1)
				{
					LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
					this.Visit(lambda.Body);
				}
				return node;
			}

			if (node.Method.Name == "QueryList")
			{
				sbSql.Append(" SELECT  * FROM ");
				this.Visit(node.Arguments[0]);
				if (node.Arguments.Count() > 1)
				{
					LambdaExpression lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
					this.Visit(lambda.Body);
				}
				return node;
			}
			if (node.Method.Name == "Paged")
			{
				sbSql.Append(" SELECT  {0},* FROM ");
				this.Visit(node.Arguments[0]);
				var pageInfo = (node.Arguments[1] as ConstantExpression).Value as PageExtension;

				var pageSql = string.Format(@"TOP {0} * FROM ( SELECT ROW_NUMBER() OVER ({1}) AS RowNum ", pageInfo.PageSize,pageInfo.OrderBy);

				sbSql = new StringBuilder(string.Format(sbSql + ") AS T WHERE RowNum BETWEEN ({1}-1)*{2}+1 AND {1}*{2} ORDER BY RowNum", pageSql, pageInfo.PageIndex, pageInfo.PageSize));

				return node;
			}

			LambdaExpression lam = Expression.Lambda(node);
			var fn = lam.Compile();
			this.Visit(Expression.Constant(fn.DynamicInvoke(null), node.Type));
			return node;
		}

		/// <summary>
		/// 访问二元操作符
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		protected override Expression VisitBinary(BinaryExpression b)
		{
			isleft = true;
			sbSql.Append(" AND (");
			this.VisitBinaryWithParent(b, b.Left);

			switch (b.NodeType)
			{
				case ExpressionType.AndAlso:
					sbSql.Append(" AND ");
					break;
				case ExpressionType.OrElse:
					sbSql.Append(" OR ");
					break;
				case ExpressionType.GreaterThan:
					sbSql.Append(" > ");
					break;
				case ExpressionType.LessThan:
					sbSql.Append(" < ");
					break;
				case ExpressionType.Equal:
					sbSql.Append(" = ");
					break;
				case ExpressionType.LessThanOrEqual:
					sbSql.Append(" <= ");
					break;
				case ExpressionType.GreaterThanOrEqual:
					sbSql.Append(" >= ");
					break;
				case ExpressionType.NotEqual:
					sbSql.Append(" <> ");
					break;
			}

			isleft = false;
			this.VisitBinaryWithParent(b, b.Right);
			sbSql.Append(")");
			return b;
		}

		/// <summary>
		/// 访问成员
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected override Expression VisitMember(MemberExpression m)
		{
			if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter && isleft)
			{
				sbSql.AppendFormat("T_{0}", m.Member.DeclaringType.Name);
				sbSql.Append(".");
				sbSql.Append(m.Member.DeclaringType.GetColumnName(m.Member.Name));
				return m;
			}
			LambdaExpression lambda = Expression.Lambda(m);
			var fn = lambda.Compile();
			this.Visit(Expression.Constant(fn.DynamicInvoke(null), m.Type));
			return m;
		}

		/// <summary>
		/// 访问常量
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		protected override Expression VisitConstant(ConstantExpression c)
		{
			IQueryable q = c.Value as IQueryable;
			if (q != null)
			{
				sbSql.AppendFormat(" {0} AS T_{1}", q.ElementType.GetTableName(), q.ElementType.Name);
			}
			else if (c.Value == null)
			{
				sbSql.Append("NULL");
			}
			else
			{
				switch (Type.GetTypeCode(c.Value.GetType()))
				{
					case TypeCode.Boolean:
						param.Add(string.Format("param"+count), ((bool)c.Value) ? 1 : 0);
						break;
					case TypeCode.String:
						param.Add(string.Format("param" + count), c.Value);
						break;
					case TypeCode.DateTime:
						param.Add(string.Format("param" + count), c.Value);
						break;
					case TypeCode.Object:
						throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
					default:
						param.Add(string.Format("param" + count), c.Value);
						break;
				}
				sbSql.Append(string.Format("@param" + count));
				count++;
			}
			return c;
		}

		private void VisitBinaryWithParent(Expression parent, Expression node)
		{
			var left = isleft;
			if (IsDiffNoteType(parent, node))
			{
				sbSql.Append("(");
			}

			var bnode = node as BinaryExpression;

			if (bnode != null && bnode.Left is BinaryExpression)
			{
				this.VisitBinaryWithParent(node, bnode.Left);
			}
			else
			{
				isleft = true;
				if (bnode != null)
				{
					this.Visit(bnode.Left);
				}
				else
				{
					this.Visit(node);
				}
				isleft = left;
			}

			switch (node.NodeType)
			{
				case ExpressionType.AndAlso:
					sbSql.Append(" AND ");
					break;
				case ExpressionType.OrElse:
					sbSql.Append(" OR ");
					break;
				case ExpressionType.GreaterThan:
					sbSql.Append(" > ");
					break;
				case ExpressionType.LessThan:
					sbSql.Append(" < ");
					break;
				case ExpressionType.Equal:
					sbSql.Append(" = ");
					break;
				case ExpressionType.LessThanOrEqual:
					sbSql.Append(" <= ");
					break;
				case ExpressionType.GreaterThanOrEqual:
					sbSql.Append(" >= ");
					break;
				case ExpressionType.NotEqual:
					sbSql.Append(" <> ");
					break;
				default:
					return;
			}

			if (bnode != null && bnode.Right is BinaryExpression)
			{
				this.VisitBinaryWithParent(node, bnode.Right);
			}
			else
			{
				isleft = false;
				if (bnode != null)
				{
					this.Visit(bnode.Right);
				}
				else
				{
					this.Visit(node);
				}
				isleft = left;
			}

			if (IsDiffNoteType(parent, node))
			{
				sbSql.Append(")");
			}
		}

		private void GetParentMemberName(Expression node)
		{
			if (node.NodeType == ExpressionType.MemberAccess)
			{
				var memberEx = node as MemberExpression;
				GetParentMemberName(memberEx.Expression);
				sbSql.Append(memberEx.Member.Name);
				sbSql.Append(".");
			}
		}

		private bool IsDiffNoteType(Expression parent, Expression children)
		{
			if (IsOpertaorType(parent) && IsOpertaorType(children))
			{
				if (parent.NodeType == children.NodeType)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		bool IsOpertaorType(Expression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse:
					return true;
				default:
					return false;
			}
		}

		internal Expression[] VisitArguments(IArgumentProvider nodes)
		{
			Expression[] array = null;
			int i = 0;
			int argumentCount = nodes.ArgumentCount;
			while (i < argumentCount)
			{
				Expression argument = nodes.GetArgument(i);

				Expression expression = this.Visit(argument);
				if (array != null)
				{
					array[i] = expression;
				}
				else if (expression != argument)
				{
					array = new Expression[argumentCount];
					for (int j = 0; j < i; j++)
					{
						array[j] = nodes.GetArgument(j);
					}
					array[i] = expression;
				}
				i++;
			}
			return array;
		}

		private static Expression StripQuotes(Expression e)
		{
			while (e.NodeType == ExpressionType.Quote)
			{
				e = ((UnaryExpression)e).Operand;
			}
			return e;
		}
	}
}
