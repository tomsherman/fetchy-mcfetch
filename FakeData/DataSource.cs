using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClasses;

namespace FetchPoints.FakeData
{
    internal class DataSource
    {
        static List<PointsEntry> fakeData;

    #region "Methods called by controller"
        internal static void PopulateFakeData() {
            fakeData = new List<PointsEntry>
            {
                PointsEntry.CreateCredit("DANNON", 1000, DateTime.Parse("2020-11-02T14:00:00Z")),
                PointsEntry.CreateCredit("UNILEVER", 200, DateTime.Parse("2020-10-31T11:00:00Z")),
                PointsEntry.CreateDebit("DANNON", 200, DateTime.Parse("2020-10-31T15:00:00Z")),
                PointsEntry.CreateCredit("MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z")),
                PointsEntry.CreateCredit("DANNON", 300, DateTime.Parse("2020-10-31T10:00:00Z")),
            };
        }

        internal static IEnumerable<PointsEntry> GetEntries()
        {
            return fakeData;
        }

        public static int GetBalance(string payer)
        {
            var balance = 0;

            foreach(PointsEntry entry in GetEntries())
            {
                if (entry.Payer == payer)
                {
                    balance += entry.Points;
                }
            }

            return balance;
        }

        #endregion

        public static int Spend(string payer, int points)
        {
            var spentPoints = 0;
            var balance = GetBalance(payer);

            // spend no more than is available
            var spendablePoints = Math.Min(points, balance);

            if (spendablePoints > 0) 
            {
                fakeData.Add(new PointsEntry { Payer = payer, Points = -1 * spendablePoints, Timestamp = DateTime.Now });
                spentPoints = spendablePoints;
            }

            return spentPoints;
        }

    }
}
