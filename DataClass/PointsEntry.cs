using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.Input;

namespace FetchPoints.DataClass
{
    public class PointsEntry : IComparable<PointsEntry>
    {

        #region "Properties"

        public string payer { get; }

        public DateTime timestamp { get; }

        public int points { get; }

        internal bool isCredit
        {
            get
            {
                return (points > 0);
            }
        }

        internal bool isDebit
        {
            get
            {
                return (points < 0);
            }
        }

        #endregion

        #region "Constructors"

        private PointsEntry(string payer, int points, DateTime timestamp) 
        {
            this.payer = payer;
            this.points = points;
            this.timestamp = timestamp;
        }

        #endregion

        #region "Factory methods"

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

        public static PointsEntry CreateCreditFromRequest(CreditRequest request)
        {
            return CreateCredit(request.payer, request.points, request.timestamp);
        }

        #endregion

        #region "Comparers"

        public int CompareTo([AllowNull] PointsEntry other)
        {
            if (timestamp > other.timestamp)
            {
                return 1;
            } else if (timestamp < other.timestamp)
            {
                return -1;
            } else
            {
                return 0;
            }
        }

        #endregion

    }
}
