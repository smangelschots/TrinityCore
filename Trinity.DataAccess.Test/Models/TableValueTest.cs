using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.Test.Models
{
    public class TableValueTest
    {
        //IDs
        public Guid Id { get; set; }

        public int SecondId { get; set; }


        //Text
        public string Text { get; set; }

        public string LargeText { get; set; }

        public string JsonText { get; set; }

        public string XmlText { get; set; }




        //Numbers
        public int Number { get; set; }


        public long LongNumber { get; set; }

        public double DoubleNumber { get; set; }


        public decimal DecimalNumber { get; set; }


        public float FloatNumber { get; set; }


        public float Currency { get; set; }






        //Date Time
        public DateTime DateAndTime { get; set; }


        public DateTime Date { get; set; }


        public DateTime Time { get; set; }


        




    }
}
