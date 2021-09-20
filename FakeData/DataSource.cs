using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClass;

namespace FetchPoints.FakeData
{
    internal class DataSource
    {
        static List<PointsEntry> fakeData;

        internal static IEnumerable<PointsEntry> getEntries()
        {
            return fakeData;
        }

        internal static void commitDebits(IEnumerable<PointsEntry> debits)
        {
            fakeData.AddRange(debits);
        }

        internal static void populateFakeData()
        {
            fakeData = new List<PointsEntry>
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
