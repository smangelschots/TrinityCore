namespace Trinity.DataAccess.Interfaces
{
    public interface IColumnAttribute
    {
        string Name { get; set; }

        string TableName { get; set; }
    }
}