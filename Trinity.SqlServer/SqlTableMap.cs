using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Trinity.DataAccess.Orm;

namespace Trinity.SqlServer
{
    public class SqlTableMap<T> : TableMap
    {
        public SqlTableMap<T> AddPrimaryKey<TField>(Expression<Func<T, TField>> field)
        {
            var key = GetField(field);

            if (!string.IsNullOrEmpty(key))
            {
                var column = ColumnMaps.FirstOrDefault(m => m.ColumnName == key);

                if (column == null)
                {
                    column.IsPrimaryKey = true;
                }
            }

            return this;
        }



        protected internal string GetField<TField>(Expression<Func<T, TField>> field)
        {
            if (field == null)
                throw new ArgumentNullException("propertyExpression");

            var memberExpression = field.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("memberExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("property");

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException("static method");

            return memberExpression.Member.Name;
            ;
        }

    }
}
