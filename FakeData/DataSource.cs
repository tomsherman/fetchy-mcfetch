using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClass;

namespace FetchPoints.FakeData
{
    internal class DataSource
    {
        static List<PointsEntry> entries;

        internal static IEnumerable<PointsEntry> getEntries()
        {
            return entries;
        }

        internal static void commitDebits(IEnumerable<PointsEntry> debits)
        {
            entries.AddRange(debits);
        }

        internal static void addCredit(PointsEntry credit)
        {
            foreach (PointsEntry entry in entries)
            {
                if (entry.payer == credit.payer
                    && entry.points == entry.points
                    && entry.timestamp == entry.timestamp)
                {
                    // duplicate; don't add
                } else
                {
                    entries.Add(credit);
                }
            }
        }

        internal static void clearData()
        {
            entries = new List<PointsEntry>();
        }

        internal static void populateFakeData()
        {
            entries = new List<PointsEntry>
            {
                PointsEntry.CreateCredit("DANNON", 1000, DateTime.Parse("2020-11-02T14:00:00Z")),
                PointsEntry.CreateCredit("UNILEVER", 200, DateTime.Parse("2020-10-31T11:00:00Z")),
                PointsEntry.CreateDebit("DANNON", 200, DateTime.Parse("2020-10-31T15:00:00Z")),
                PointsEntry.CreateCredit("MILLER COORS", 10000, DateTime.Parse("2020-11-01T14:00:00Z")),
                PointsEntry.CreateCredit("DANNON", 300, DateTime.Parse("2020-10-31T10:00:00Z")),
            };
        }

    }
}
