﻿using System;
using System.Linq.Expressions;

namespace Trinity.DataAccess.Orm
{
    //TODO create parrameters expressions
    public class ExpressionToSql
    {
        /// <summary>
        /// Converts a Func expression to a best guess SQL string
        /// </summary>
        public static string Convert<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression)
                return Convert(expression, (MemberExpression)expression.Body);
            if (expression.Body is MethodCallExpression)
                return Convert((MethodCallExpression)expression.Body);
            if (expression.Body is UnaryExpression)
                return Convert(expression, (UnaryExpression)expression.Body);
            if (expression.Body is ConstantExpression)
                return Convert((ConstantExpression)expression.Body);


            throw new InvalidOperationException("Unable to convert expression to SQL");
        }


        /// <summary>
        /// Converts a boolean Func expression to a best guess SQL string
        /// </summary>
        public static string Convert<T>(Expression<Func<T, bool>> expression)
        {
            if (expression.Body is BinaryExpression)
                return Convert<T>((BinaryExpression)expression.Body);
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null && memberExpression.Type == typeof(bool))
                return Convert(CreateExpression<T>(memberExpression)) + " = " + Convert(true);
            var unaryExpression = expression.Body as UnaryExpression;
            if (unaryExpression != null && unaryExpression.Type == typeof(bool) && unaryExpression.NodeType == ExpressionType.Not)
                return Convert(CreateExpression<T>(unaryExpression.Operand)) + " = " + Convert(false);


            throw new InvalidOperationException("Unable to convert expression to SQL");
        }


        private static string Convert<T>(Expression<Func<T, object>> expression, MemberExpression body)
        {
            // TODO: should really do something about conventions and overridden names here
            var member = body.Member;


            if (member.DeclaringType.IsAssignableFrom(typeof(T)))
                return member.Name;

            // try get value of lambda, hoping it's just a direct value return or local reference
            var compiledExpression = expression.Compile();
            var value = compiledExpression(default(T)); // give it null/default because a value will not need anything


            return Convert(value);
        }


        /// <summary>
        /// Gets the value of a method call.
        /// </summary>
        /// <param name="body">Method call expression</param>
        private static string Convert(MethodCallExpression body)
        {
            return Convert(body.Method.Invoke(body.Object, null));
        }


        private static string Convert<T>(Expression<Func<T, object>> expression, UnaryExpression body)
        {
            var constant = body.Operand as ConstantExpression;


            if (constant != null)
                return Convert(constant);


            var member = body.Operand as MemberExpression;


            if (member != null)
                return Convert(expression, member);


            var unaryExpression = body.Operand as UnaryExpression;


            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                return Convert(expression, unaryExpression);


            throw new InvalidOperationException("Unable to convert expression to SQL");
        }


        private static string Convert(ConstantExpression expression)
        {
            if (expression.Type.IsEnum)
                return Convert((int)expression.Value);


            return Convert(expression.Value);
        }


        private static Expression<Func<T, object>> CreateExpression<T>(Expression body)
        {
            var expression = body;
            var parameter = Expression.Parameter(typeof(T), "x");


            if (expression.Type.IsValueType)
                expression = Expression.Convert(expression, typeof(object));


            return (Expression<Func<T, object>>)Expression.Lambda(typeof(Func<T, object>), expression, parameter);
        }


        private static string Convert<T>(BinaryExpression expression)
        {

            //TODO make paramters values.
            var left = Convert(CreateExpression<T>(expression.Left));
            var right = Convert(CreateExpression<T>(expression.Right));
            string op;


            switch (expression.NodeType)
            {
                default:
                case ExpressionType.Equal:
                    op = "=";
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                case ExpressionType.NotEqual:
                    op = "!=";
                    break;
            }


            return left + " " + op + " " + Convert(right);
        }


        private static string Convert(object value)
        {
            if (value == null) return string.Empty;

            if (value is string)
            {
                if (value.ToString().EndsWith("'") == false)
                    return "'" + value + "'";

                return value.ToString();
            }

            if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }

            return value.ToString();
        }
    }

}
