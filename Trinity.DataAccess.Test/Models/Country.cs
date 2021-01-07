using System;
using System.Collections.Generic;
using System.Text;
using Trinity.DataAccess.LobModels;
using Trinity.DataAccess.Orm;

namespace Trinity.Test.Models
{
    public class Country : ModelBase
    {
        private int _countryId;
        private string _name;

        public Country()
        {
            var config = new ModelConfiguration<Country>();
            config.SetModelConfiguration(this);
            config.SetRequired(m => m.Name, "Country name is required", ModelConfiguration.NotNullExpression);
            config.AfterModelPropertyValidate += (s, e) =>
            {

            };

            this.Configuration = config;
        }


        public int CountryId
        {
            get { return _countryId; }
            set
            {
                if (_countryId != value)
                {
                    _countryId = value;
                    SendPropertyChanged();
                }

            }
        }

        private string _code; 

        public string Code
        {
            get { return _code; }

            set
            {
                if (_code != value)
                {
                    _code = value;
                    SendPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    SendPropertyChanged();
                }
            }
        }

        public override List<string> GetProperties()
        {
            throw new NotImplementedException();
        }
    }

}
