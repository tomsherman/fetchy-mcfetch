using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchPoints.Retriever;
using FetchPoints.ApiException;

namespace FetchPoints.DataClass
{
    internal class UserPoints
    {
        private Dictionary<string, PayerLedger> _ledgerDictionary = new Dictionary<string, PayerLedger>();

        private Dictionary<string, PayerLedger> ledgerDictionary {
            get
            {
                return _ledgerDictionary;
            }
        }

        #region "Factory methods"

        internal static UserPoints create()
        {
            var entries = DataRetriever.getEntries().ToList();
            entries.Sort(); // oldest to newest
            var userPoints = new UserPoints();


            var tempDict = new Dictionary<string, List<PointsEntry>>();

            foreach (PointsEntry entry in entries)
            {
                if (!tempDict.ContainsKey(entry.payer)) tempDict.Add(entry.payer, new List<PointsEntry>());
                tempDict[entry.payer].Add(entry);
            }

            foreach(KeyValuePair<string, List<PointsEntry>> kvp in tempDict)
            {
                userPoints.ledgerDictionary.Add(kvp.Key, PayerLedger.create(kvp.Key, kvp.Value));
            }

            return userPoints;
        }

        #endregion

        #region "Instance methods"

        internal int getBalance()
        {
            var balance = 0;
            foreach(PayerLedger ledger in ledgerDictionary.Values)
            {
                balance += ledger.getBalance();
            }
            return balance;
        }

        internal Dictionary<string, int> getPayerBalances()
        {
            var payerBalances = new Dictionary<string, int>();
            foreach(PayerLedger ledger in ledgerDictionary.Values)
            {
                var balance = ledger.getBalance();
                if (balance > 0) payerBalances.Add(ledger.payer, balance);
            }
            return payerBalances;
        }


        public List<PointsEntry> spend(int points)
        {
            var ledgers = ledgerDictionary.Values.ToList();
            var spentCredits = new List<PointsEntry>();
            var newDebits = new List<PointsEntry>();

            var overallBalance = ledgers.Sum(ledger => ledger.getBalance());
            if (points > overallBalance) throw new InsufficientPointsException();

            var pointsToSpend = points;
            while(pointsToSpend > 0)
            {
                ledgers = ledgers.Where(ledger => ledger.hasAvailableCredit()).ToList();
                if (ledgers.Count() == 0) break; // should never happen due to overall balance check above

                // sort oldest to newest, by credits
                ledgers.Sort();

                var ledgerToSpendFrom = ledgers.First();
                var spentCredit = ledgerToSpendFrom.spendOldestAvailablePoints(pointsToSpend);
                pointsToSpend -= spentCredit.points;
                spentCredits.Add(spentCredit);
                newDebits.Add(PointsEntry.CreateDebit(ledgerToSpendFrom.payer, spentCredit.points, DateTime.Now));
            }

            // commit to "database"
            DataRetriever.commitDebits(newDebits);

            return spentCredits;
        }

        #endregion

    }
}
