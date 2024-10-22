namespace Trinity.DataAccess.Interfaces
{
    public interface IColumnMap<T> : IColumnMap
    {
        T DbType { get; set; }
    }

    public interface IColumnMap
    {
        string ColumnName { get; set; }
        bool IsPrimaryKey { get; set; }
        bool IsForeignKey { get; set; }
        int Size { get; set; }
        int OrdinalPosition { get; set; }
        string Default { get; set; }
        bool IsNullable { get; set; }
        bool IsMapped { get; set; }
        int Id { get; set; }
        bool IsIdentity { get; set; }
        bool IsComputed { get; set; }
        string PropertyName { get; set; }
        
    }
    
}