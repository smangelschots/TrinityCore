﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DataAccess.Attributes;

namespace Trinity.DataAccess.Models
{

    [TableConfiguration("Z_Application")]
    public class ApplicationModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }




    }
}
