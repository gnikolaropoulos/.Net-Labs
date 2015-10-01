//-------------------------------------------------------------------------------------------------
// Code from Typed Reflector http://clarius.codeplex.com/releases/view/9495
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace System.SoftBytes.Reflection
{
    /// <summary>
    /// Provides strong-typed reflection of the <typeparamref name="TTarget"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">Type to reflect.</typeparam>
    public static class Reflector<TTarget>
    {
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        public static MethodInfo GetMethod(Expression<Action<TTarget>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda 
        /// expression or it does not represent a method invocation.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        public static MethodInfo GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> method)
        {
            return GetMethodInfo(method);
        }

        private static MethodInfo GetMethodInfo(Expression method)
        {
            if (method == null) { throw new ArgumentNullException("method"); }

            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null) { throw new ArgumentException("Not a lambda expression", "method"); }
            if (lambda.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException("Not a method call", "method");
            }

            return ((MethodCallExpression)lambda.Body).Method;
        }

        /// <summary>
        /// Gets the property represented by the lambda expression.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is 
        /// not a lambda expression or it does not represent a property access.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        public static PropertyInfo GetProperty(Expression<Func<TTarget, Object>> property)
        {
            PropertyInfo info = GetMemberInfo(property) as PropertyInfo;
            if (info == null) { throw new ArgumentException("Member is not a property"); }

            return info;
        }

        /// <summary>
        /// Gets the field represented by the lambda expression.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="method"/> is not a
        /// lambda expression or it does not represent a field access.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = " ")]
        public static FieldInfo GetField(Expression<Func<TTarget, Object>> field)
        {
            FieldInfo info = GetMemberInfo(field) as FieldInfo;
            if (info == null) { throw new ArgumentException("Member is not a field"); }

            return info;
        }

        /// <summary>
        /// Gets the member info.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfo(Expression member)
        {
            return Reflector.GetMemberInfo(member);
        }
    }

    public static class Reflector
    {
        /// <summary>
        /// Gets the member info.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo(Expression member)
        {
            if (member == null) { throw new ArgumentNullException("member"); }

            LambdaExpression lambda = member as LambdaExpression;
            if (lambda == null) { throw new ArgumentException("Not a lambda expression", "member"); }

            MemberExpression memberExpr = null;

            // The Func<TTarget, object> we use returns an object, so first statement can be either 
            // a cast (if the field/property does not return an object) or the direct member access.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual member access expression.
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null) { throw new ArgumentException("Not a member access", "member"); }

            return memberExpr.Member;
        }
    }
}