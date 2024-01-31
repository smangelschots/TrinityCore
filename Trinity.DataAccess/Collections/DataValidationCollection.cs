using System;
using System.Collections;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Collections
{
    public class DataValidationCollection : CollectionBase
    {

        public void Add(DataValidation validation)
        {
            this.List.Add(validation);
        }

        public void Remove(DataValidation validation)
        {
            this.List.Remove(validation);
        }

        public DataValidation this[int index]
        {
            get
            {
                return (DataValidation)this.List[index];
            }
        }

        public bool ContainsColumnName(string columnname)
        {
            foreach (DataValidation item in this)
            {
                if (item.Columname.Equals(columnname))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
