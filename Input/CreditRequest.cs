using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FetchPoints.Input
{
    public class CreditRequest
    {
        public int points { get; set; }
        public string payer { get; set; }
        public DateTime timestamp { get; set; }
    }
}
