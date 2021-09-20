using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FetchPoints.Input
{
    /// <summary>
    /// Represents a request, submitted via the API, to add points.
    /// </summary>
    public class CreditRequest
    {
        public int points { get; set; }
        public string payer { get; set; }
        public DateTime timestamp { get; set; }
    }
}
