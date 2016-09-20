using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HDapper.Translate
{
	public class SimpleKeyBuilder : ExpressionVisitor
	{
		public string Build(Expression exp)
		{
			this.m_builder = new StringBuilder();
			this.Visit(exp);
			return this.Key;
		}

		public string Key { get { return this.m_builder.ToString(); } }

		private StringBuilder m_builder;

		public override Expression Visit(Expression exp)
		{
			if (exp == null) return exp;

			this.Accept("$b$").Accept((int)exp.NodeType).Accept(exp.Type);
			var result = base.Visit(exp);
			this.Accept("$e$");

			return result;
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			this.Accept(b.IsLifted).Accept(b.IsLiftedToNull).Accept(b.Method);
			return base.VisitBinary(b);
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			this.Accept(c.Value);
			return base.VisitConstant(c);
		}

		protected virtual SimpleKeyBuilder Accept(int value)
		{
			this.m_builder.Append(value).Append("|");
			return this;
		}

		protected virtual SimpleKeyBuilder Accept(bool value)
		{
			this.m_builder.Append(value ? "1|" : "0|");
			return this;
		}

		protected virtual SimpleKeyBuilder Accept(Type type)
		{
			this.m_builder.Append(type == null ? "null" : type.FullName).Append("|");
			return this;
		}

		protected virtual SimpleKeyBuilder Accept(MemberInfo member)
		{
			if (member == null)
			{
				this.m_builder.Append("null|");
				return this;
			}

			return this.Accept(member.DeclaringType).Accept(member.Name);
		}

		protected virtual SimpleKeyBuilder Accept(object value)
		{
			this.m_builder.Append(value == null ? "null" : value.ToString()).Append("|");
			return this;
		}
	}
}
