using System;
using Trinity.DataAccess.Results;

namespace Trinity.DataAccess.Events
{
    public class AfterSaveEventArgs  : EventArgs
    {
        public ResultList Results { get; set; }

    }
}
