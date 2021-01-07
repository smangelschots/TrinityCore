using System.ComponentModel;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandPropertyChangedEventArgs : PropertyChangedEventArgs
       
    {
        public ModelCommandPropertyChangedEventArgs(string propertyName)
            : base(propertyName)
        {


        }

        public object Value { get; set; }

        public IDataCommand ModelCommand { get; set; }
    }
}