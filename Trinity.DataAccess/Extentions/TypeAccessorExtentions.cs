using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Logging;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Extentions
{
    public static class TypeAccessorExtentions
    {


        public static bool TrySetValue<T>(this TypeAccessor accessor, ICommandResult result, T newObject, IColumnMap column, object value, string name = "") where T : class
        {
            Type objectType = typeof(T);
            var properties = accessor.GetMembers().Select(m => m.Name).ToList();
            var propertyList = string.Join(", ", properties);
            PropertyInfo prop = null;
            string propertyName = column.PropertyName;

            if (!string.IsNullOrEmpty(name))
            {
                prop = objectType.GetProperty(name);
                propertyName = name;
            }
            else
            {
                if (column != null)
                {
                    if (string.IsNullOrEmpty(column.PropertyName))
                    {
                        prop = objectType.GetProperty(column.ColumnName);
                        propertyName = column.ColumnName;
                    }
                    else
                    {
                        prop = objectType.GetProperty(column.PropertyName);
                        propertyName = column.PropertyName;
                    }

                }
            }

            bool fallBackToSlowMethod = false;
            if (accessor.GetMembers().Any(m => m.Name == propertyName))
            {
                try
                {
                    accessor[newObject, propertyName] = value.ConvertValue(prop);
                }
                catch (Exception ex)
                {
                    fallBackToSlowMethod = true;
                    result.AddWaring(string.Format("{1} {0} ", name, objectType.Name), ex);
                }
            }
            if(fallBackToSlowMethod)
            {
                if (prop != null)
                {
                    if (value != DBNull.Value)
                    {
                        try
                        {
                            prop.SetValue(newObject, value.ConvertValue(prop), null);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            result.AddWaring($"{ex.Message}", ex);
                        }
                    }
                    else
                    {

                    }
                }
                result.AddMessage($"Property {nameof(propertyName)} not found on {objectType.Name}. Available properties: {propertyList}");
            }

            return false;
        }


    }
}
