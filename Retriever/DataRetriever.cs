using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.DataClass;
using FetchPoints.FakeData;

namespace FetchPoints.Retriever
{
    internal class DataRetriever
    {
        internal static IEnumerable<PointsEntry> getEntries()
        {
            return DataSource.getEntries();
        }

        internal static void commitDebits(IEnumerable<PointsEntry> debits)
        {
            DataSource.commitDebits(debits);
        }

        #region "Fake stuff"

        internal static void populateFakeData()
        {
            DataSource.populateFakeData();
        }

        #endregion

    }
}
