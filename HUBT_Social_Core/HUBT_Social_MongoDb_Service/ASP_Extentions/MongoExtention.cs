using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_MongoDb_Service.ASP_Extentions
{
    public static class MongoExtention
    {
        public static async Task<T?> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.FirstOrDefault();
        }

        public static async Task<T?> LastOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.LastOrDefault();
        }

        public static async Task<T?> SingleOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.SingleOrDefault();
        }

        public static async Task<bool> AnyAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.Any();
        }

        public static async Task<int> CountAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.Count();
        }

        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.ToList();
        }

        public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            var body = Expression.AndAlso(left!, right!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter = oldParameter;
            private readonly ParameterExpression _newParameter = newParameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : node;
            }
        }
    }
}
