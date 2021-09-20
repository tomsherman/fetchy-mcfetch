using System;
using System.Collections.Generic;
using FetchPoints.DataClass;

namespace FetchPoints.FakeData
{
    internal class DataSource
    {
        // in-memory store of entries
        static List<PointsEntry> entries = new List<PointsEntry>();

        /// <summary>
        /// Returns all entries, both debits and credits.
        /// </summary>
        internal static IEnumerable<PointsEntry> getEntries()
        {
            return entries;
        }

        /// <summary>
        /// Adds associated debits to the in-memory list.
        /// </summary>
        /// <remarks>this would be a database irl</remarks>
        /// <param name="debits"></param>
        internal static void commitDebits(IEnumerable<PointsEntry> debits)
        {
            entries.AddRange(debits);
        }

        /// <summary>
        /// Idempotent method to add a credit. Will not add the same credit twice.
        /// </summary>
        /// <param name="credit"></param>
        internal static void addCredit(PointsEntry credit)
        {
            var isNew = true;

            foreach (PointsEntry entry in entries)
            {
                if (entry.payer == credit.payer
                    && entry.points == entry.points
                    && entry.timestamp == entry.timestamp)
                {
                    isNew = false;
                    break;
                }
            }
            
            if (isNew) entries.Add(credit);
        }

        /// <summary>
        /// Clears in-memory data
        /// </summary>
        internal static void clearData()
        {
            entries = new List<PointsEntry>();
        }

        /// <summary>
        /// Creates fake data that matches the example
        /// </summary>
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
