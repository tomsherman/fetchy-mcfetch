using System;
using System.Collections.Generic;
using FetchPoints.DataClass;
using FetchPoints.FakeData;

namespace FetchPoints.Retriever
{
    /// <summary>
    /// This class mimics a retriever that would typically talk to a data store. But here we're faking it.
    /// </summary>
    internal class DataRetriever
    {
        #region "Methods"

        internal static IEnumerable<PointsEntry> getEntries()
        {
            return DataSource.getEntries();
        }

        internal static void addCredit(PointsEntry credit)
        {
            DataSource.addCredit(credit);
        }

        internal static void commitDebits(IEnumerable<PointsEntry> debits)
        {
            DataSource.commitDebits(debits);
        }

        internal static void clearData()
        {
            DataSource.clearData();
        }

        #endregion

        #region "Fake stuff"

        internal static void populateFakeData()
        {
            DataSource.populateFakeData();
        }

        #endregion

    }
}
