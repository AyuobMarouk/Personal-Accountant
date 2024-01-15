using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Accountant.Schemas
{
    public class TransSchema
    {
        public string Type { get; set; }
        public double Amount { get; set; }
        public string AccountType { get; set; }
    }
}
