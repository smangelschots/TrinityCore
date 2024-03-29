using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{
    public class ColumnConfiguration<T>  : ITableConfigurationAttribute, IColumnAttribute, IPrimaryKeyAttribute where T : class
    {
        public string Name { get; set; }

        public string SequenceName { get; set; }

        public bool IsAutoIncrement { get; set; }

        public string TableName { get; set; }

        public string EntityName { get; set; }

        public bool Ignore { get; set; }

        public bool PrimaryKey { get; set; }

        public bool ForeignKey { get; set; }

        public ILookupConfiguration LookupConfiguration { get; set; }
    

        public ColumnConfiguration<T> IsPrimaryKey()
        {
            this.PrimaryKey = true;
            return this;
        }

        public ColumnConfiguration<T> IsForeignKey()
        {
            ForeignKey = true;
            return this;
        }


        public LookupConfiguration<TClass> AddLookupConfiguration<TClass>()
        {

            this.LookupConfiguration = new LookupConfiguration<TClass>();
            return this.LookupConfiguration as LookupConfiguration<TClass>;
        }

       


        public ColumnConfiguration<T> AutoIncrement()
        {
            this.IsAutoIncrement = true;
            this.PrimaryKey = true;
            return this;
        }


        public ColumnConfiguration<T> IsIgnore()
        {
            this.Ignore = true;
            return this;
        }

        public ColumnConfiguration<T> Table(string tableName)
        {
            TableName = tableName;
            return this;
        }

        //TODO Move to entity support project
        //public ColumnConfiguration<T> Entity<TClass>()
        //{
        //    this.EnityName = typeof(TClass).Name;
        //    var modelType = typeof(TClass);
        //    var tableAttribute = modelType.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), false);
        //    if (tableAttribute.Any())
        //    {
        //        var table = tableAttribute[0] as System.ComponentModel.DataAnnotations.Schema.TableAttribute;
        //        if (table != null)
        //        {
        //            TableName = table.Name;
        //        }
        //    }
        //    return this;
        //}

    }
}