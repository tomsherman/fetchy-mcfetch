using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FetchPoints.DataClass
{
    internal class PayerLedger : IComparable<PayerLedger>
    {
        private List<PointsEntry> adjustedCredits { get; }

        internal string payer { get; }

        private PointsEntry oldestCredit
        {
            get
            {
                return adjustedCredits.Count() > 0 ? adjustedCredits.First() : null;
            }
        }

        #region "Private constructor"

        private PayerLedger(string payer, List<PointsEntry> adjustedCredits)
        {
            this.payer = payer;
            this.adjustedCredits = adjustedCredits;
        }

        #endregion

        #region "Factory methods"

        internal static PayerLedger create(string payer, List<PointsEntry> entries)
        {
            var adjustedCredits = getAdjustedCredits(entries);
            return new PayerLedger(payer, adjustedCredits);
        }

        private static List<PointsEntry> getAdjustedCredits(List<PointsEntry> entries)
        {
            var adjustedCredits = new List<PointsEntry>();
            var credits = entries.Where(entry => entry.isCredit);
            var runningTotal = entries.Where(entry => entry.isDebit).Sum(entry => entry.points);
            var inTheBlack = false;

            foreach(PointsEntry credit in credits)
            {
                if (inTheBlack)
                {
                    adjustedCredits.Add(credit);
                } else
                {
                    runningTotal += credit.points;
                    if (runningTotal > 0) {
                        inTheBlack = true;
                        adjustedCredits.Add(PointsEntry.CreateCredit(credit.payer, runningTotal, credit.timestamp));
                    }
                }
            }

            return adjustedCredits;
        }

        #endregion

        internal PointsEntry spendOldestAvailablePoints(int points)
        {
            if (hasAvailableCredit())
            {
                var oldestCredit = adjustedCredits.First();
                if (points >= oldestCredit.points)
                {
                    // completely spent
                    adjustedCredits.RemoveAt(0);
                    return oldestCredit;
                } else
                {
                    // partially spent
                    // we are done with this particular point-spending operation
                    return PointsEntry.CreateCredit(oldestCredit.payer, points, oldestCredit.timestamp);
                }


            } else
            {
                throw new Exception("No points to spend.");
            }
        }

        internal int getBalance()
        {
            var balance = 0;
            foreach (PointsEntry entry in adjustedCredits)
            {
                balance += entry.points;
            }
            return balance;
        }

        internal void addEntry(PointsEntry entry)
        {
            adjustedCredits.Add(entry);
        }

        internal bool hasAvailableCredit()
        {
            return oldestCredit != null;
        }

        internal int oldestAvailablePoints()
        {
            if (!hasAvailableCredit())
            {
                return 0;
            }
            else
            {
                return oldestCredit.points;
            }
        }

        public int CompareTo([AllowNull] PayerLedger other)
        {
            if (oldestCredit == null && other.oldestCredit == null) return 0;
            if (oldestCredit == null && other.oldestCredit != null) return 1;
            if (oldestCredit != null && other.oldestCredit == null) return -1;
            if (oldestCredit.timestamp > other.oldestCredit.timestamp) return 1;
            if (oldestCredit.timestamp < other.oldestCredit.timestamp) return -1;
            return 0;
        }
    }
}
