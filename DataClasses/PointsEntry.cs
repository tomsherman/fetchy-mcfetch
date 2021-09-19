using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FetchPoints.DataClasses
{
    public class PointsEntry
    {
        public string Payer { get; set; }

        public int Points { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
