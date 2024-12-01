using System.Collections.Generic;
using FetchPoints.Entity;

namespace FetchPoints.PersistedData
{
    internal class ValidatedTransactionsStore
    {
        // in-memory store of validated receipts
        static Dictionary<string, ValidatedTransaction> validatedTransactions = new Dictionary<string, ValidatedTransaction>();

        internal static IEnumerable<ValidatedTransaction> getAll()
        {
            return validatedTransactions.Values;
        }

        internal static IEnumerable<string> getAllIds()
        {
            return validatedTransactions.Keys;
        }

        internal static ValidatedTransaction get(string id) {
            if (validatedTransactions.ContainsKey(id)) {
                return validatedTransactions[id];
            } else {
                return null;
            }
        }

        internal static void add(ValidatedTransaction transaction) {
            validatedTransactions.Add(transaction.Id(), transaction);
        }

        internal static bool has(ValidatedTransaction transaction) {
            return validatedTransactions.ContainsKey(transaction.Id());
        }

        internal static void clearData()
        {
            validatedTransactions = new Dictionary<string, ValidatedTransaction>();
        }

    }
}
