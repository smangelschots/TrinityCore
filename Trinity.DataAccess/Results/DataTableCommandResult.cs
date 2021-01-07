using System.Data;

namespace Trinity.DataAccess.Results
{
    public class DataTableCommandResult : CommandResult
    {
        public DataTable Data { get; set; }
    }
}