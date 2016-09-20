using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HDapper.Translate
{
	public interface IExpressionCache<T> where T : class
	{
		T Get(Expression key, Func<Expression, T> creator);
	}

	public class SimpleKeyCache<T> : IExpressionCache<T> where T : class
	{
		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
		private Dictionary<string, T> m_storage = new Dictionary<string, T>();

		public T Get(Expression key, Func<Expression, T> creator)
		{
			T value;
			string cacheKey = new SimpleKeyBuilder().Build(key);
			this.m_rwLock.EnterReadLock();
			try
			{
				if (this.m_storage.TryGetValue(cacheKey, out value))
				{
					return value;
				}
			}
			finally
			{
				m_rwLock.ExitReadLock();
			}

			this.m_rwLock.EnterWriteLock();
			try
			{
				if (this.m_storage.TryGetValue(cacheKey, out value))
				{
					return value;
				}
				value = creator(key);
				this.m_storage.Add(cacheKey, value);
				return value;
			}
			finally
			{
				this.m_rwLock.ExitWriteLock();
			}
		}
	}
}
