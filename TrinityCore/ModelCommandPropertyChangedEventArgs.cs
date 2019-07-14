using System.ComponentModel;

namespace TrinityCore
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