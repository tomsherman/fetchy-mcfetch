using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClasses;

namespace FetchPoints.FakeData
{
    public class DataSource
    {
        static List<PointsEntry> fakeData;

        public static void PopulateFakeData() {
            fakeData = new List<PointsEntry>
            {
                new PointsEntry() { Payer = "Tom", Points = 1000, Timestamp = DateTime.Now },
                new PointsEntry() { Payer = "Joe", Points = 200, Timestamp = DateTime.Now }
            };
        }

        public static IEnumerable<PointsEntry> GetEntries()
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
