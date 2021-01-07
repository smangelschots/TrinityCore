using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.DataAccess.Results
{
    public class ModelCommandResult<T> : CommandResult where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public string ToJson()
        {
            return null;
        }


    }
}
