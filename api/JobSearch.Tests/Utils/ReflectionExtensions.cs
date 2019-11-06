namespace JobSearch.Tests.Utils
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static PropertyInfo AsPropertyInfo<T>(this Expression<Func<T, object>> property)
        {
            MemberExpression memberExpression = GetMemberExpression(property);
            return (PropertyInfo)memberExpression.Member;
        }

        public static bool Has<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes<TAttribute>(false).Any();
        }

        public static bool Has<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttributes<TAttribute>().Any();
        }

        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member", nameof(expression));
            }

            return memberExpression;
        }
    }
}
