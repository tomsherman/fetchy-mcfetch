using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FetchPoints.DataClasses
{
    public class PointsEntry : IComparable<PointsEntry>
    {
        public string Payer { get; }

        public int Points { get; }

        public DateTime Timestamp { get; }

    // private constructor
        private PointsEntry(string payer, int points, DateTime timestamp) 
        {
            Payer = payer;
            Points = points;
            Timestamp = timestamp;
        }

    // factory methods
        public static PointsEntry CreateCredit(string payer, int points, DateTime timestamp)
        {
            if (points <= 0) throw new ArgumentOutOfRangeException("Points specified must be positive");
            return new PointsEntry(payer, points, timestamp);
        }

        public static PointsEntry CreateDebit(string payer, int points, DateTime timestamp)
        {
            if (points <= 0) throw new ArgumentOutOfRangeException("Points specified must be positive");
            return new PointsEntry(payer, -1 * points, timestamp);
        }

        public int CompareTo([AllowNull] PointsEntry other)
        {

            // todo switch statement?
            if (Timestamp > other.Timestamp)
            {
                return 1;
            } else if (Timestamp < other.Timestamp)
            {
                return -1;
            } else
            {
                return 0;
            }
        }
    }
}
